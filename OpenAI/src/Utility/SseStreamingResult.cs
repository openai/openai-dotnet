using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal static class SseStreamingResult
{
    private static ReadOnlySpan<byte> TerminalData => "[DONE]"u8;

    public static AsyncStreamingResult<SseItem<T>> Create<T>(
        AsyncStreamingResult<SseItem<BinaryData>> source,
        Func<JsonElement, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
        CancellationToken cancellationToken,
        List<Action>? additionalDisposalActions = null,
        SseLifecycle<T>? lifecycle = null)
    {
        Argument.AssertNotNull(jsonSingleDeserializerFunc, nameof(jsonSingleDeserializerFunc));

        return Create(
            source,
            (element, _, options) => jsonSingleDeserializerFunc(element, options),
            cancellationToken,
            additionalDisposalActions,
            lifecycle);
    }

    public static AsyncStreamingResult<SseItem<T>> Create<T>(
        AsyncStreamingResult<SseItem<BinaryData>> source,
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
        CancellationToken cancellationToken,
        List<Action>? additionalDisposalActions = null,
        SseLifecycle<T>? lifecycle = null)
    {
        Argument.AssertNotNull(source, nameof(source));
        Argument.AssertNotNull(jsonSingleDeserializerFunc, nameof(jsonSingleDeserializerFunc));

        return Create(
            source,
            item =>
            {
                using JsonDocument document = JsonDocument.Parse(item.Data);
                return [jsonSingleDeserializerFunc(
                    document.RootElement,
                    item.Data,
                    ModelSerializationExtensions.WireOptions)];
            },
            cancellationToken,
            additionalDisposalActions,
            lifecycle);
    }

    public static AsyncStreamingResult<SseItem<T>> Create<T>(
        AsyncStreamingResult<SseItem<BinaryData>> source,
        Func<SseItem<BinaryData>, IEnumerable<T>> eventDeserializerFunc,
        CancellationToken cancellationToken,
        List<Action>? additionalDisposalActions = null,
        SseLifecycle<T>? lifecycle = null)
    {
        Argument.AssertNotNull(source, nameof(source));
        Argument.AssertNotNull(eventDeserializerFunc, nameof(eventDeserializerFunc));

        var response = new SourceStreamingResponse(
            source,
            exception => CompleteSource(
                exception,
                additionalDisposalActions,
                lifecycle,
                SseCompletionKind.Disposed));
        return AsyncStreamingResult.Create<SseItem<T>>(
            response,
            (_, producerCancellationToken) => EnumerateSourceAsync(
                source,
                eventDeserializerFunc,
                additionalDisposalActions,
                lifecycle,
                response,
                producerCancellationToken),
            cancellationToken);
    }

    private sealed class LifecycleEnumerator<T>(
        IEnumerator<T> inner,
        SseLifecycle<T>? lifecycle) : IEnumerator<T>
    {
        public T Current => inner.Current;

        object System.Collections.IEnumerator.Current => Current!;

        public bool MoveNext() => inner.MoveNext();

        public void Reset() => inner.Reset();

        public void Dispose()
        {
            try
            {
                inner.Dispose();
            }
            catch (Exception exception)
            {
                lifecycle?.OnException(exception);
                throw;
            }
        }
    }

    private static async IAsyncEnumerable<SseItem<T>> EnumerateSourceAsync<T>(
        AsyncStreamingResult<SseItem<BinaryData>> source,
        Func<SseItem<BinaryData>, IEnumerable<T>> eventDeserializerFunc,
        List<Action>? additionalDisposalActions,
        SseLifecycle<T>? lifecycle,
        SourceStreamingResponse response,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        SseCompletionKind completionKind = SseCompletionKind.Disposed;
        response.Detach();
        lifecycle?.OnTypedResponse();

        try
        {
            await using IAsyncEnumerator<SseItem<BinaryData>> events = ((IAsyncEnumerable<SseItem<BinaryData>>)source)
                .GetAsyncEnumerator(cancellationToken);

            while (true)
            {
                SseItem<BinaryData> item;
                try
                {
                    using (lifecycle?.Enter())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (!await events.MoveNextAsync().ConfigureAwait(false))
                        {
                            completionKind = SseCompletionKind.EndOfStream;
                            yield break;
                        }

                        item = events.Current;
                        lifecycle?.OnEvent();
                    }
                }
                catch (Exception exception)
                {
                    lifecycle?.OnException(exception);
                    throw;
                }

                if (item.Data.ToMemory().Span.SequenceEqual(TerminalData))
                {
                    completionKind = SseCompletionKind.EndOfStream;
                    yield break;
                }

                IEnumerator<T> CreateUpdates()
                {
                    try
                    {
                        using (lifecycle?.Enter())
                        {
                            return new LifecycleEnumerator<T>(
                                eventDeserializerFunc(item).GetEnumerator(),
                                lifecycle);
                        }
                    }
                    catch (Exception exception)
                    {
                        lifecycle?.OnException(exception);
                        throw;
                    }
                }

                using IEnumerator<T> updates = CreateUpdates();
                while (true)
                {
                    T update;
                    try
                    {
                        using (lifecycle?.Enter())
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            if (!updates.MoveNext())
                            {
                                break;
                            }

                            update = updates.Current;
                            lifecycle?.OnUpdate(update);
                        }
                    }
                    catch (Exception exception)
                    {
                        lifecycle?.OnException(exception);
                        throw;
                    }

                    yield return new SseItem<T>(update, item.EventType)
                    {
                        EventId = item.EventId,
                        ReconnectionInterval = item.ReconnectionInterval,
                    };
                }
            }
        }
        finally
        {
            using (lifecycle?.Enter())
            {
                CompleteSource(null, additionalDisposalActions, lifecycle, completionKind);
            }
        }
    }

    private sealed class SourceStreamingResponse(
        AsyncStreamingResult<SseItem<BinaryData>> source,
        Action<Exception?> complete) : PipelineResponse
    {
        private static readonly BinaryData s_emptyContent = BinaryData.FromBytes([]);
        private Action<Exception?>? _complete = complete;

        public override int Status => source.Status;

        public override string ReasonPhrase => source.ReasonPhrase;

        public override Stream? ContentStream
        {
            get => Stream.Null;
            set => throw new NotSupportedException();
        }

        public override BinaryData Content => s_emptyContent;

        protected override PipelineResponseHeaders HeadersCore => source.Headers;

        public override BinaryData BufferContent(CancellationToken cancellationToken = default)
            => s_emptyContent;

        public override ValueTask<BinaryData> BufferContentAsync(CancellationToken cancellationToken = default)
            => new(s_emptyContent);

        public void Detach() => Interlocked.Exchange(ref _complete, null);

        public override void Dispose()
        {
            try
            {
                source.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                Interlocked.Exchange(ref _complete, null)?.Invoke(exception);
                throw;
            }
            finally
            {
                Interlocked.Exchange(ref _complete, null)?.Invoke(null);
            }
        }
    }

    private static void CompleteSource<T>(
        Exception? exception,
        List<Action>? additionalDisposalActions,
        SseLifecycle<T>? lifecycle,
        SseCompletionKind completionKind)
    {
        try
        {
            if (exception is not null)
            {
                lifecycle?.OnException(exception);
            }

            foreach (Action additionalDisposalAction in additionalDisposalActions ?? [])
            {
                additionalDisposalAction.Invoke();
            }
        }
        catch (Exception disposalException)
        {
            lifecycle?.OnException(disposalException);
            throw;
        }
        finally
        {
            additionalDisposalActions?.Clear();
            lifecycle?.Complete(completionKind);
        }
    }
}

internal sealed class SseStreamingResult<T>
{
    private readonly Func<Task<ClientResult>> _sendRequestAsync;
    private readonly Func<JsonElement, BinaryData, ModelReaderWriterOptions, T> _jsonSingleDeserializerFunc;
    private readonly CancellationToken _cancellationToken;

    internal Func<SseLifecycle<T>?>? LifecycleFactory { get; set; }

    public List<Action> AdditionalDisposalActions { get; } = [];

    public SseStreamingResult(
        Func<Task<ClientResult>> sendRequestAsync,
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
        CancellationToken cancellationToken)
    {
        Argument.AssertNotNull(sendRequestAsync, nameof(sendRequestAsync));
        Argument.AssertNotNull(jsonSingleDeserializerFunc, nameof(jsonSingleDeserializerFunc));

        _sendRequestAsync = sendRequestAsync;
        _jsonSingleDeserializerFunc = jsonSingleDeserializerFunc;
        _cancellationToken = cancellationToken;
    }

    public async Task<AsyncStreamingResult<SseItem<T>>> CreateAsync()
    {
        SseLifecycle<T>? lifecycle = LifecycleFactory?.Invoke();
        ClientResult result;

        try
        {
            using (lifecycle?.Enter())
            {
                result = await _sendRequestAsync().ConfigureAwait(false);
                PipelineResponse response = result.GetRawResponse();
                lifecycle?.OnResponse(response);
            }
        }
        catch (Exception exception)
        {
            lifecycle?.OnException(exception);
            throw;
        }

        AsyncStreamingResult<SseItem<BinaryData>> source = AsyncStreamingResult.CreateSse(
            result.GetRawResponse(),
            operationCancellationToken: _cancellationToken);
        return SseStreamingResult.Create(
            source,
            _jsonSingleDeserializerFunc,
            _cancellationToken,
            AdditionalDisposalActions,
            lifecycle);
    }
}