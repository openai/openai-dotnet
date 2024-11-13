using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Net.ServerSentEvents;
using System.Text.Json;
using System.Threading;

#nullable enable

namespace OpenAI.Chat;

/// <summary>
/// Implementation of collection abstraction over streaming chat updates.
/// </summary>
internal class InternalStreamingChatCompletionUpdateCollection : CollectionResult<StreamingChatCompletionUpdate>
{
    private readonly Func<ClientResult> _sendRequest;
    private readonly CancellationToken _cancellationToken;

    public InternalStreamingChatCompletionUpdateCollection(
        Func<ClientResult> sendRequest,
        CancellationToken cancellationToken)
    {
        Argument.AssertNotNull(sendRequest, nameof(sendRequest));

        _sendRequest = sendRequest;
        _cancellationToken = cancellationToken;
    }

    public override ContinuationToken? GetContinuationToken(ClientResult page)
        // Continuation is not supported for SSE streams.
        => null;

    public override IEnumerable<ClientResult> GetRawPages()
    {
        // We don't currently support resuming a dropped connection from the
        // last received event, so the response collection has a single element.
        yield return _sendRequest();
    }

    protected override IEnumerable<StreamingChatCompletionUpdate> GetValuesFromPage(ClientResult page)
    {
        using IEnumerator<StreamingChatCompletionUpdate> enumerator = new StreamingChatUpdateEnumerator(page, _cancellationToken);
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    private sealed class StreamingChatUpdateEnumerator : IEnumerator<StreamingChatCompletionUpdate>
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
        private IEnumerator<SseItem<byte[]>>? _events;
        private IEnumerator<StreamingChatCompletionUpdate>? _updates;

        private StreamingChatCompletionUpdate? _current;
        private bool _started;

        public StreamingChatUpdateEnumerator(ClientResult page, CancellationToken cancellationToken)
        {
            Argument.AssertNotNull(page, nameof(page));

            _response = page.GetRawResponse();
            _cancellationToken = cancellationToken;
        }

        StreamingChatCompletionUpdate IEnumerator<StreamingChatCompletionUpdate>.Current
            => _current!;

        object IEnumerator.Current => _current!;

        public bool MoveNext()
        {
            if (_events is null && _started)
            {
                throw new ObjectDisposedException(nameof(StreamingChatUpdateEnumerator));
            }

            _cancellationToken.ThrowIfCancellationRequested();
            _events ??= CreateEventEnumerator();
            _started = true;

            if (_updates is not null && _updates.MoveNext())
            {
                _current = _updates.Current;
                return true;
            }

            if (_events.MoveNext())
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

        private IEnumerator<SseItem<byte[]>> CreateEventEnumerator()
        {
            if (_response.ContentStream is null)
            {
                throw new InvalidOperationException("Unable to create result from response with null ContentStream");
            }

            IEnumerable<SseItem<byte[]>> enumerable = SseParser.Create(_response.ContentStream, (_, bytes) => bytes.ToArray()).Enumerate();
            return enumerable.GetEnumerator();
        }

        public void Reset()
        {
            throw new NotSupportedException("Cannot seek back in an SSE stream.");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && _events is not null)
            {
                _events.Dispose();
                _events = null;

                // Dispose the response so we don't leave the network connection open.
                _response?.Dispose();
            }
        }
    }
}
