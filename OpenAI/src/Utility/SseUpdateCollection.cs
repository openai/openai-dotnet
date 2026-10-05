using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
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
    private ConditionalWeakTable<ClientResult, SseLifecycle<T>>? _lifecycles;

    internal Func<SseLifecycle<T>?>? LifecycleFactory { get; set; }

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
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, IEnumerable<T>> jsonMultiDeserializerFunc,
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
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
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

    // Continuation is not supported for SSE streams.
    public override ContinuationToken? GetContinuationToken(ClientResult page) => null;

    public override IEnumerable<ClientResult> GetRawPages()
    {
        // We don't currently support resuming a dropped connection from the
        // last received event, so the response collection has a single element.
        var lifecycle = LifecycleFactory?.Invoke();
        ClientResult? page = null;
        try
        {
            using (lifecycle?.Enter())
            {
                try
                {
                    page = _sendRequestFunc();
                    lifecycle?.OnResponse(page.GetRawResponse());
                }
                catch (Exception exception)
                {
                    lifecycle?.OnException(exception);
                    throw;
                }
            }
            if (lifecycle is not null)
            {
                LazyInitializer.EnsureInitialized(ref _lifecycles)!.Add(page, lifecycle);
            }
            yield return page;
        }
        finally
        {
            lifecycle?.Complete(SseCompletionKind.RawResponse);
            if (page is not null)
            {
                _lifecycles?.Remove(page);
            }
        }
    }

    protected override IEnumerable<T> GetValuesFromPage(ClientResult page)
    {
        SseLifecycle<T>? lifecycle = null;
        _lifecycles?.TryGetValue(page, out lifecycle);
        lifecycle?.OnTypedResponse();
        using IEnumerator<T> enumerator = new SseUpdateEnumerator<T>(_eventDeserializerFunc, page, _cancellationToken, AdditionalDisposalActions, lifecycle);
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
        private bool _disposed;
        private readonly SseLifecycle<U>? _lifecycle;

        public SseUpdateEnumerator(
            Func<SseItem<byte[]>, IEnumerable<U>> eventDeserializerFunc,
            ClientResult page,
            CancellationToken cancellationToken,
            List<Action> additionalDisposalActions,
            SseLifecycle<U>? lifecycle)
        {
            Argument.AssertNotNull(eventDeserializerFunc, nameof(eventDeserializerFunc));
            Argument.AssertNotNull(page, nameof(page));

            _eventDeserializerFunc = eventDeserializerFunc;
            _response = page.GetRawResponse();
            _cancellationToken = cancellationToken;
            _additionalDisposalActions = additionalDisposalActions;
            _lifecycle = lifecycle;
        }

        U IEnumerator<U>.Current => _current!;

        object IEnumerator.Current => _current!;

        public bool MoveNext()
        {
            using var activation = _lifecycle?.Enter();
            try
            {
                var hasNext = MoveNextCore();
                if (hasNext)
                {
                    _lifecycle?.OnUpdate(_current!);
                }
                else
                {
                    _lifecycle?.Complete(SseCompletionKind.EndOfStream);
                }
                return hasNext;
            }
            catch (Exception exception)
            {
                _lifecycle?.OnException(exception);
                throw;
            }
        }

        private bool MoveNextCore()
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

            // Keep advancing the outer event enumerator instead of stopping at the first
            // event. An event can legitimately deserialize to zero updates, for example an
            // unmodeled event that the deserializer maps to an empty sequence. Such an event
            // must be skipped so that later events still surface, otherwise a single
            // unrecognized event in the middle of a stream would end the whole stream and
            // look like a clean, early completion.
            while (true)
            {
                // Cancellation is observed once per event rather than only on entry, because
                // a run of events that yield no updates is consumed inside a single call.
                // The check sits ahead of the read so that a canceled token does not have to
                // wait for the server to send another event before it is noticed.
                _cancellationToken.ThrowIfCancellationRequested();

                if (!_events.MoveNext())
                {
                    break;
                }

                _lifecycle?.OnEvent();
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
            using var activation = _lifecycle?.Enter();
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception exception)
            {
                _lifecycle?.OnException(exception);
                throw;
            }
            finally
            {
                _lifecycle?.Complete(SseCompletionKind.Disposed);
            }
        }

        private void Dispose(bool disposing)
        {
            if ((!disposing) || (_disposed))
            {
                return;
            }
            _disposed = true;
            try
            {
                _updates?.Dispose();
                _events?.Dispose();
            }
            finally
            {
                _events = null;
                // Cancellation can occur before the first parser read, but the connection is already open.
                _response.Dispose();
            }

            foreach (Action additionalDisposalAction in _additionalDisposalActions ?? [])
            {
                additionalDisposalAction?.Invoke();
            }
            _additionalDisposalActions?.Clear();
        }
    }
}
