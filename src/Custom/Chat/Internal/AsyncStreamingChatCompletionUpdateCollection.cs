using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.ServerSentEvents;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Chat;

/// <summary>
/// Implementation of collection abstraction over streaming chat updates.
/// </summary>
internal class AsyncStreamingChatCompletionUpdateCollection : AsyncCollectionResult<StreamingChatCompletionUpdate>
{
    private readonly Func<Task<ClientResult>> _getResultAsync;

    public AsyncStreamingChatCompletionUpdateCollection(Func<Task<ClientResult>> getResultAsync) : base()
    {
        Argument.AssertNotNull(getResultAsync, nameof(getResultAsync));

        _getResultAsync = getResultAsync;
    }

    public override IAsyncEnumerator<StreamingChatCompletionUpdate> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new AsyncStreamingChatUpdateEnumerator(_getResultAsync, this, cancellationToken);
    }

    private sealed class AsyncStreamingChatUpdateEnumerator : IAsyncEnumerator<StreamingChatCompletionUpdate>
    {
        private static ReadOnlySpan<byte> TerminalData => "[DONE]"u8;

        private readonly Func<Task<ClientResult>> _getResultAsync;
        private readonly AsyncStreamingChatCompletionUpdateCollection _enumerable;
        private readonly CancellationToken _cancellationToken;

        // These enumerators represent what is effectively a doubly-nested
        // loop over the outer event collection and the inner update collection,
        // i.e.:
        //   foreach (var sse in _events) {
        //       // get _updates from sse event
        //       foreach (var update in _updates) { ... }
        //   }
        private IAsyncEnumerator<SseItem<byte[]>>? _events;
        private IEnumerator<StreamingChatCompletionUpdate>? _updates;

        private StreamingChatCompletionUpdate? _current;
        private bool _started;

        public AsyncStreamingChatUpdateEnumerator(Func<Task<ClientResult>> getResultAsync,
            AsyncStreamingChatCompletionUpdateCollection enumerable,
            CancellationToken cancellationToken)
        {
            Debug.Assert(getResultAsync is not null);
            Debug.Assert(enumerable is not null);

            _getResultAsync = getResultAsync!;
            _enumerable = enumerable!;
            _cancellationToken = cancellationToken;
        }

        StreamingChatCompletionUpdate IAsyncEnumerator<StreamingChatCompletionUpdate>.Current
            => _current!;

        async ValueTask<bool> IAsyncEnumerator<StreamingChatCompletionUpdate>.MoveNextAsync()
        {
            if (_events is null && _started)
            {
                throw new ObjectDisposedException(nameof(AsyncStreamingChatUpdateEnumerator));
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

                using JsonDocument doc = JsonDocument.Parse(_events.Current.Data);
                var updates = StreamingChatCompletionUpdate.DeserializeStreamingChatCompletionUpdates(doc.RootElement);
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
