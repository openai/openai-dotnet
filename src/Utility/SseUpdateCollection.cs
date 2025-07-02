using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Net.ServerSentEvents;
using System.Text.Json;
using System.Threading;

#nullable enable

namespace OpenAI;

/// <summary>
/// Implementation of collection abstraction over streaming updates.
/// </summary>
internal class SseUpdateCollection<T> : CollectionResult<T>
{
    private readonly Func<ClientResult> _sendRequestFunc;
    private readonly Func<SseItem<byte[]>, IEnumerable<T>> _eventDeserializerFunc;
    private readonly CancellationToken _cancellationToken;

    public List<Action> AdditionalDisposalActions { get; } = [];

    public SseUpdateCollection(
        Func<ClientResult> sendRequestFunc,
        Func<JsonElement, ModelReaderWriterOptions, IEnumerable<T>> jsonMultiDeserializerFunc,
        CancellationToken cancellationToken)
            : this(
                  sendRequestFunc,
                  AsyncSseUpdateCollection<T>.DeserializeSseToMultipleViaJson(jsonMultiDeserializerFunc),
                  cancellationToken)

    {
        Argument.AssertNotNull(jsonMultiDeserializerFunc, nameof(jsonMultiDeserializerFunc));
    }

    public SseUpdateCollection(
        Func<ClientResult> sendRequestFunc,
        Func<JsonElement, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
        CancellationToken cancellationToken)
            : this(
                  sendRequestFunc,
                  AsyncSseUpdateCollection<T>.DeserializeSseToSingleViaJson(jsonSingleDeserializerFunc),
                  cancellationToken)
    {
        Argument.AssertNotNull(jsonSingleDeserializerFunc, nameof(jsonSingleDeserializerFunc));
    }

    public SseUpdateCollection(
        Func<ClientResult> sendRequestFunc,
        Func<SseItem<byte[]>, IEnumerable<T>> eventDeserializerFunc,
        CancellationToken cancellationToken)
    {
        Argument.AssertNotNull(sendRequestFunc, nameof(sendRequestFunc));
        Argument.AssertNotNull(eventDeserializerFunc, nameof(eventDeserializerFunc));

        _sendRequestFunc = sendRequestFunc;
        _eventDeserializerFunc = eventDeserializerFunc;
        _cancellationToken = cancellationToken;
    }

    public override ContinuationToken? GetContinuationToken(ClientResult page)
        // Continuation is not supported for SSE streams.
        => null;

    public override IEnumerable<ClientResult> GetRawPages()
    {
        // We don't currently support resuming a dropped connection from the
        // last received event, so the response collection has a single element.
        yield return _sendRequestFunc();
    }

    protected override IEnumerable<T> GetValuesFromPage(ClientResult page)
    {
        using IEnumerator<T> enumerator = new SseUpdateEnumerator<T>(_eventDeserializerFunc, page, _cancellationToken, AdditionalDisposalActions);
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    private sealed class SseUpdateEnumerator<U> : IEnumerator<U>
    {
        private static ReadOnlySpan<byte> TerminalData => "[DONE]"u8;

        private List<Action> _additionalDisposalActions;

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
        private IEnumerator<U>? _updates;
        private readonly Func<SseItem<byte[]>, IEnumerable<U>> _eventDeserializerFunc;

        private U? _current;
        private bool _started;

        public SseUpdateEnumerator(
            Func<SseItem<byte[]>, IEnumerable<U>> eventDeserializerFunc,
            ClientResult page,
            CancellationToken cancellationToken,
            List<Action> additionalDisposalActions)
        {
            Argument.AssertNotNull(eventDeserializerFunc, nameof(eventDeserializerFunc));
            Argument.AssertNotNull(page, nameof(page));

            _eventDeserializerFunc = eventDeserializerFunc;
            _response = page.GetRawResponse();
            _cancellationToken = cancellationToken;
            _additionalDisposalActions = additionalDisposalActions;
        }

        U IEnumerator<U>.Current => _current!;

        object IEnumerator.Current => _current!;

        public bool MoveNext()
        {
            if (_events is null && _started)
            {
                throw new ObjectDisposedException(typeof(U).Name);
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

                _updates = _eventDeserializerFunc.Invoke(_events.Current).GetEnumerator();

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

            foreach (Action additionalDisposalAction in _additionalDisposalActions ?? [])
            {
                additionalDisposalAction?.Invoke();
            }
            _additionalDisposalActions?.Clear();
        }
    }
}
