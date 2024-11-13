using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Net.ServerSentEvents;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Chat;

/// <summary>
/// Implementation of collection abstraction over streaming chat updates.
/// </summary>
internal class InternalAsyncStreamingChatCompletionUpdateCollection : AsyncCollectionResult<StreamingChatCompletionUpdate>
{
    private readonly Func<Task<ClientResult>> _sendRequestAsync;
    private readonly CancellationToken _cancellationToken;

    public InternalAsyncStreamingChatCompletionUpdateCollection(
        Func<Task<ClientResult>> sendRequestAsync,
        CancellationToken cancellationToken)
    {
        Argument.AssertNotNull(sendRequestAsync, nameof(sendRequestAsync));

        _sendRequestAsync = sendRequestAsync;
        _cancellationToken = cancellationToken;
    }

    public override ContinuationToken? GetContinuationToken(ClientResult page)

        // Continuation is not supported for SSE streams.
        => null;

    public async override IAsyncEnumerable<ClientResult> GetRawPagesAsync()
    {
        // We don't currently support resuming a dropped connection from the
        // last received event, so the response collection has a single element.
        yield return await _sendRequestAsync();
    }

    protected async override IAsyncEnumerable<StreamingChatCompletionUpdate> GetValuesFromPageAsync(ClientResult page)
    {
        await using IAsyncEnumerator<StreamingChatCompletionUpdate> enumerator = new AsyncStreamingChatUpdateEnumerator(page, _cancellationToken);
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            yield return enumerator.Current;
        }
    }

    private sealed class AsyncStreamingChatUpdateEnumerator : IAsyncEnumerator<StreamingChatCompletionUpdate>
    {
        private static ReadOnlySpan<byte> TerminalData => "[DONE]"u8;

        private readonly CancellationToken _cancellationToken;
        private readonly PipelineResponse _response;

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

        public AsyncStreamingChatUpdateEnumerator(ClientResult page, CancellationToken cancellationToken)
        {
            Argument.AssertNotNull(page, nameof(page));

            _response = page.GetRawResponse();
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
            _events ??= CreateEventEnumeratorAsync();
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
                List<StreamingChatCompletionUpdate> updates = [StreamingChatCompletionUpdate.DeserializeStreamingChatCompletionUpdate(doc.RootElement)];
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

        private IAsyncEnumerator<SseItem<byte[]>> CreateEventEnumeratorAsync()
        {
            if (_response.ContentStream is null)
            {
                throw new InvalidOperationException("Unable to create result from response with null ContentStream");
            }

            IAsyncEnumerable<SseItem<byte[]>> enumerable = SseParser.Create(_response.ContentStream, (_, bytes) => bytes.ToArray()).EnumerateAsync();
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

                // Dispose the response so we don't leave the network connection open.
                _response?.Dispose();
            }
        }
    }
}
