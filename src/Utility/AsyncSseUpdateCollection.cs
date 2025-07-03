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
        Func<SseItem<byte[]>, IEnumerable<T>> eventDeserializerFunc,
        CancellationToken cancellationToken)
    {
        Argument.AssertNotNull(sendRequestAsync, nameof(sendRequestAsync));
        Argument.AssertNotNull(eventDeserializerFunc, nameof(eventDeserializerFunc));

        _sendRequestAsync = sendRequestAsync;
        _eventDeserializerFunc = eventDeserializerFunc;
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

    protected async override IAsyncEnumerable<T> GetValuesFromPageAsync(ClientResult page)
    {
        await using IAsyncEnumerator<T> enumerator = new AsyncSseUpdateEnumerator<T>(_eventDeserializerFunc, page, _cancellationToken, AdditionalDisposalActions);
        
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

        public AsyncSseUpdateEnumerator(
            Func<SseItem<byte[]>, IEnumerable<U>> deserializerFunc,
            ClientResult page,
            CancellationToken cancellationToken,
            List<Action> additionalDisposalActions)
        {
            Argument.AssertNotNull(page, nameof(page));

            _deserializerFunc = deserializerFunc;
            _response = page.GetRawResponse();
            _cancellationToken = cancellationToken;
            _additionalDisposalActions = additionalDisposalActions;
        }

        U IAsyncEnumerator<U>.Current => _current!;

        async ValueTask<bool> IAsyncEnumerator<U>.MoveNextAsync()
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

            if (await _events.MoveNextAsync().ConfigureAwait(false))
            {
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

            foreach (Action additionalDisposalAction in _additionalDisposalActions ?? [])
            {
                additionalDisposalAction.Invoke();
            }
            _additionalDisposalActions?.Clear();
        }
    }
}
