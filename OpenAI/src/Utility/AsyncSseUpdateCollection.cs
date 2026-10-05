using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

/// <summary>
/// Implementation of collection abstraction over streaming chat updates.
/// </summary>
internal class AsyncSseUpdateCollection<T> : AsyncCollectionResult<T>
{
    private readonly Func<Task<ClientResult>> _sendRequestAsync;
    private readonly Func<SseItem<byte[]>, IEnumerable<T>> _eventDeserializerFunc;
    private readonly CancellationToken _cancellationToken;
    private ConditionalWeakTable<ClientResult, SseLifecycle<T>>? _lifecycles;

    internal Func<SseLifecycle<T>?>? LifecycleFactory { get; set; }

    public List<Action> AdditionalDisposalActions { get; } = [];

    public AsyncSseUpdateCollection(
        Func<Task<ClientResult>> sendRequestAsync,
        Func<JsonElement, ModelReaderWriterOptions, IEnumerable<T>> jsonMultiDeserializerFunc,
        CancellationToken cancellationToken)
            : this(
                  sendRequestAsync,
                  DeserializeSseToMultipleViaJson(jsonMultiDeserializerFunc),
                  cancellationToken)
    {
        Argument.AssertNotNull(jsonMultiDeserializerFunc, nameof(jsonMultiDeserializerFunc));
    }

    public AsyncSseUpdateCollection(
        Func<Task<ClientResult>> sendRequestAsync,
        Func<JsonElement, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
        CancellationToken cancellationToken)
            : this(
                  sendRequestAsync,
                  DeserializeSseToSingleViaJson(jsonSingleDeserializerFunc),
                  cancellationToken)
    {
        Argument.AssertNotNull(jsonSingleDeserializerFunc, nameof(jsonSingleDeserializerFunc));
    }

    public AsyncSseUpdateCollection(
        Func<Task<ClientResult>> sendRequestAsync,
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, IEnumerable<T>> jsonMultiDeserializerFunc,
        CancellationToken cancellationToken)
            : this(
                  sendRequestAsync,
                  DeserializeSseToMultipleViaJson(jsonMultiDeserializerFunc),
                  cancellationToken)
    {
        Argument.AssertNotNull(jsonMultiDeserializerFunc, nameof(jsonMultiDeserializerFunc));
    }

    public AsyncSseUpdateCollection(
        Func<Task<ClientResult>> sendRequestAsync,
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
        CancellationToken cancellationToken)
            : this(
                  sendRequestAsync,
                  DeserializeSseToSingleViaJson(jsonSingleDeserializerFunc),
                  cancellationToken)
    {
        Argument.AssertNotNull(jsonSingleDeserializerFunc, nameof(jsonSingleDeserializerFunc));
    }

    public AsyncSseUpdateCollection(
        Func<Task<ClientResult>> sendRequestAsync,
        Func<SseItem<byte[]>, IEnumerable<T>> eventDeserializerFunc,
        CancellationToken cancellationToken)
    {
        Argument.AssertNotNull(sendRequestAsync, nameof(sendRequestAsync));
        Argument.AssertNotNull(eventDeserializerFunc, nameof(eventDeserializerFunc));

        _sendRequestAsync = sendRequestAsync;
        _eventDeserializerFunc = eventDeserializerFunc;
        _cancellationToken = cancellationToken;
    }

    // Continuation is not supported for SSE streams.
    public override ContinuationToken? GetContinuationToken(ClientResult page) => null;

    public async override IAsyncEnumerable<ClientResult> GetRawPagesAsync()
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
                    page = await _sendRequestAsync().ConfigureAwait(false);
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

    protected async override IAsyncEnumerable<T> GetValuesFromPageAsync(ClientResult page)
    {
        SseLifecycle<T>? lifecycle = null;
        _lifecycles?.TryGetValue(page, out lifecycle);
        lifecycle?.OnTypedResponse();
        await using IAsyncEnumerator<T> enumerator = new AsyncSseUpdateEnumerator<T>(_eventDeserializerFunc, page, _cancellationToken, AdditionalDisposalActions, lifecycle);

        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            yield return enumerator.Current;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Func<SseItem<byte[]>, IEnumerable<U>> DeserializeSseToMultipleViaJson<U>(
    Func<JsonElement, ModelReaderWriterOptions, IEnumerable<U>> jsonDeserializationFunc)
    {
        return (item) =>
        {
            using JsonDocument document = JsonDocument.Parse(item.Data);
            return jsonDeserializationFunc.Invoke(document.RootElement, ModelSerializationExtensions.WireOptions);
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Func<SseItem<byte[]>, IEnumerable<U>> DeserializeSseToSingleViaJson<U>(
        Func<JsonElement, ModelReaderWriterOptions, U> jsonSingleDeserializationFunc)
            => DeserializeSseToMultipleViaJson<U>((e, o) => [jsonSingleDeserializationFunc.Invoke(e, o)]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Func<SseItem<byte[]>, IEnumerable<U>> DeserializeSseToMultipleViaJson<U>(
    Func<JsonElement, BinaryData, ModelReaderWriterOptions, IEnumerable<U>> jsonDeserializationFunc)
    {
        return (item) =>
        {
            using JsonDocument document = JsonDocument.Parse(item.Data);
            return jsonDeserializationFunc.Invoke(document.RootElement, BinaryData.FromBytes(item.Data), ModelSerializationExtensions.WireOptions);
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Func<SseItem<byte[]>, IEnumerable<U>> DeserializeSseToSingleViaJson<U>(
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, U> jsonSingleDeserializationFunc)
            => DeserializeSseToMultipleViaJson<U>((e, d, o) => [jsonSingleDeserializationFunc.Invoke(e, d, o)]);

    private sealed class AsyncSseUpdateEnumerator<U> : IAsyncEnumerator<U>
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
        private IAsyncEnumerator<SseItem<byte[]>>? _events;
        private IEnumerator<U>? _updates;
        private readonly Func<SseItem<byte[]>, IEnumerable<U>> _deserializerFunc;

        private U? _current;
        private bool _started;
        private bool _disposed;
        private readonly SseLifecycle<U>? _lifecycle;

        public AsyncSseUpdateEnumerator(
            Func<SseItem<byte[]>, IEnumerable<U>> deserializerFunc,
            ClientResult page,
            CancellationToken cancellationToken,
            List<Action> additionalDisposalActions,
            SseLifecycle<U>? lifecycle)
        {
            Argument.AssertNotNull(page, nameof(page));

            _deserializerFunc = deserializerFunc;
            _response = page.GetRawResponse();
            _cancellationToken = cancellationToken;
            _additionalDisposalActions = additionalDisposalActions;
            _lifecycle = lifecycle;
        }

        U IAsyncEnumerator<U>.Current => _current!;

        async ValueTask<bool> IAsyncEnumerator<U>.MoveNextAsync()
        {
            using var activation = _lifecycle?.Enter();
            try
            {
                var hasNext = await MoveNextCoreAsync().ConfigureAwait(false);
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

        private async ValueTask<bool> MoveNextCoreAsync()
        {
            if (_events is null && _started)
            {
                throw new ObjectDisposedException(nameof(AsyncSseUpdateEnumerator<U>));
            }

            _cancellationToken.ThrowIfCancellationRequested();
            _events ??= CreateEventEnumeratorAsync();
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
                // Passing the token to the event enumerator is not enough on its own, since
                // events the parser has already buffered are returned without a read.
                _cancellationToken.ThrowIfCancellationRequested();

                if (!await _events.MoveNextAsync().ConfigureAwait(false))
                {
                    break;
                }

                _lifecycle?.OnEvent();
                if (_events.Current.Data.AsSpan().SequenceEqual(TerminalData))
                {
                    _current = default;
                    return false;
                }

                _updates = _deserializerFunc
                    .Invoke(_events.Current)
                    .GetEnumerator();

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
            using var activation = _lifecycle?.Enter();
            try
            {
                await DisposeAsyncCore().ConfigureAwait(false);
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

        private async ValueTask DisposeAsyncCore()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            try
            {
                _updates?.Dispose();
                if (_events is not null)
                {
                    await _events.DisposeAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _events = null;
                // Cancellation can occur before the first parser read, but the connection is already open.
                _response.Dispose();
            }

            foreach (Action additionalDisposalAction in _additionalDisposalActions ?? [])
            {
                additionalDisposalAction.Invoke();
            }
            _additionalDisposalActions?.Clear();
        }
    }
}
