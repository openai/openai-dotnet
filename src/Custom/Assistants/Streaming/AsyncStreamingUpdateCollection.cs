using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.ServerSentEvents;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

/// <summary>
/// Implementation of collection abstraction over streaming assistant updates.
/// </summary>
internal class AsyncStreamingUpdateCollection : AsyncCollectionResult<StreamingUpdate>
{
    private readonly Func<Task<ClientResult>> _getResultAsync;

    public AsyncStreamingUpdateCollection(Func<Task<ClientResult>> getResultAsync) : base()
    {
        Argument.AssertNotNull(getResultAsync, nameof(getResultAsync));

        _getResultAsync = getResultAsync;
    }

    public override IAsyncEnumerator<StreamingUpdate> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new AsyncStreamingUpdateEnumerator(_getResultAsync, this, cancellationToken);
    }

    private sealed class AsyncStreamingUpdateEnumerator : IAsyncEnumerator<StreamingUpdate>
    {
        private static ReadOnlySpan<byte> TerminalData => "[DONE]"u8;

        private readonly Func<Task<ClientResult>> _getResultAsync;
        private readonly AsyncStreamingUpdateCollection _enumerable;
        private readonly CancellationToken _cancellationToken;

        // These enumerators represent what is effectively a doubly-nested
        // loop over the outer event collection and the inner update collection,
        // i.e.:
        //   foreach (var sse in _events) {
        //       // get _updates from sse event
        //       foreach (var update in _updates) { ... }
        //   }
        private IAsyncEnumerator<SseItem<byte[]>>? _events;
        private IEnumerator<StreamingUpdate>? _updates;

        private StreamingUpdate? _current;
        private bool _started;

        public AsyncStreamingUpdateEnumerator(Func<Task<ClientResult>> getResultAsync,
            AsyncStreamingUpdateCollection enumerable,
            CancellationToken cancellationToken)
        {
            Debug.Assert(getResultAsync is not null);
            Debug.Assert(enumerable is not null);

            _getResultAsync = getResultAsync!;
            _enumerable = enumerable!;
            _cancellationToken = cancellationToken;
        }

        StreamingUpdate IAsyncEnumerator<StreamingUpdate>.Current
            => _current!;

        async ValueTask<bool> IAsyncEnumerator<StreamingUpdate>.MoveNextAsync()
        {
            if (_events is null && _started)
            {
                throw new ObjectDisposedException(nameof(AsyncStreamingUpdateEnumerator));
            }

            _cancellationToken.ThrowIfCancellationRequested();
            _events ??= await CreateEventEnumeratorAsync().ConfigureAwait(false);
            _started = true;

            if (_updates is not null && _updates.MoveNext())
            {
                _current = _updates.Current;
                return true;
            }

            if (await _events.MoveNextAsync().ConfigureAwait(false))
            {
                if (_events.Current.Data.AsSpan().SequenceEqual(TerminalData))
                {
                    _current = default;
                    return false;
                }

                var updates = StreamingUpdate.FromEvent(_events.Current);
                _updates = updates.GetEnumerator();

                if (_updates.MoveNext())
                {
                    _current = _updates.Current;
                    return true;
                }
            }

            _current = default;
            return false;
        }

        private async Task<IAsyncEnumerator<SseItem<byte[]>>> CreateEventEnumeratorAsync()
        {
            ClientResult result = await _getResultAsync().ConfigureAwait(false);
            PipelineResponse response = result.GetRawResponse();
            _enumerable.SetRawResponse(response);

            if (response.ContentStream is null)
            {
                throw new InvalidOperationException("Unable to create result from response with null ContentStream");
            }

            IAsyncEnumerable<SseItem<byte[]>> enumerable = SseParser.Create(response.ContentStream, (_, bytes) => bytes.ToArray()).EnumerateAsync();
            return enumerable.GetAsyncEnumerator(_cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        private async ValueTask DisposeAsyncCore()
        {
            if (_events is not null)
            {
                await _events.DisposeAsync().ConfigureAwait(false);
                _events = null;

                // Dispose the response so we don't leave the unbuffered
                // network stream open.
                PipelineResponse response = _enumerable.GetRawResponse();
                response.Dispose();
            }
        }
    }
}
