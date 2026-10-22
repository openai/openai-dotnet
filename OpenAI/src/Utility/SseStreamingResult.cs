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
using OpenAI.Telemetry;

#nullable enable

namespace OpenAI;

internal static class SseStreamingResult
{
    private static ReadOnlySpan<byte> TerminalData => "[DONE]"u8;

    public static AsyncStreamingResult<SseItem<T>> Create<T>(
        PipelineResponse response,
        Func<JsonElement, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
        CancellationToken cancellationToken,
        List<Action>? additionalDisposalActions = null,
        SseLifecycle<T>? lifecycle = null)
    {
        Argument.AssertNotNull(jsonSingleDeserializerFunc, nameof(jsonSingleDeserializerFunc));

        return Create<T>(
            response,
            (Func<SseItem<byte[]>, IEnumerable<T>>)(item =>
            {
                using JsonDocument document = JsonDocument.Parse(item.Data);
                return [jsonSingleDeserializerFunc(document.RootElement, ModelSerializationExtensions.WireOptions)];
            }),
            cancellationToken,
            additionalDisposalActions,
            lifecycle);
    }

    public static AsyncStreamingResult<SseItem<T>> Create<T>(
        PipelineResponse response,
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
        CancellationToken cancellationToken,
        List<Action>? additionalDisposalActions = null,
        SseLifecycle<T>? lifecycle = null)
    {
        Argument.AssertNotNull(jsonSingleDeserializerFunc, nameof(jsonSingleDeserializerFunc));

        return Create<T>(
            response,
            (Func<SseItem<byte[]>, IEnumerable<T>>)(item =>
            {
                using JsonDocument document = JsonDocument.Parse(item.Data);
                return [jsonSingleDeserializerFunc(document.RootElement, BinaryData.FromBytes(item.Data), ModelSerializationExtensions.WireOptions)];
            }),
                cancellationToken,
                additionalDisposalActions,
                lifecycle);
    }

    public static AsyncStreamingResult<SseItem<T>> Create<T>(
        PipelineResponse response,
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, IEnumerable<T>> jsonMultiDeserializerFunc,
        CancellationToken cancellationToken,
        List<Action>? additionalDisposalActions = null,
        SseLifecycle<T>? lifecycle = null)
    {
        Argument.AssertNotNull(jsonMultiDeserializerFunc, nameof(jsonMultiDeserializerFunc));

        return Create<T>(
            response,
            (Func<SseItem<byte[]>, IEnumerable<T>>)(item =>
            {
                using JsonDocument document = JsonDocument.Parse(item.Data);
                return jsonMultiDeserializerFunc(document.RootElement, BinaryData.FromBytes(item.Data), ModelSerializationExtensions.WireOptions);
            }),
            cancellationToken,
            additionalDisposalActions,
            lifecycle);
    }

    public static AsyncStreamingResult<SseItem<T>> Create<T>(
        PipelineResponse response,
        Func<SseItem<byte[]>, IEnumerable<T>> eventDeserializerFunc,
        CancellationToken cancellationToken,
        List<Action>? additionalDisposalActions = null,
        SseLifecycle<T>? lifecycle = null,
        OpenTelemetryResponseStream? unenumeratedResponseStream = null)
    {
        Argument.AssertNotNull(response, nameof(response));
        Argument.AssertNotNull(eventDeserializerFunc, nameof(eventDeserializerFunc));

        return AsyncStreamingResult.Create<SseItem<T>>(
            response,
            (stream, producerCancellationToken) => EnumerateAsync(
                stream,
                eventDeserializerFunc,
                additionalDisposalActions,
                lifecycle,
                unenumeratedResponseStream,
                producerCancellationToken),
            cancellationToken);
    }

    private static async IAsyncEnumerable<SseItem<T>> EnumerateAsync<T>(
        Stream stream,
        Func<SseItem<byte[]>, IEnumerable<T>> eventDeserializerFunc,
        List<Action>? additionalDisposalActions,
        SseLifecycle<T>? lifecycle,
        OpenTelemetryResponseStream? unenumeratedResponseStream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        SseCompletionKind completionKind = SseCompletionKind.Disposed;
        unenumeratedResponseStream?.Detach();
        lifecycle?.OnTypedResponse();

        try
        {
            await using IAsyncEnumerator<SseItem<byte[]>> events = SseParser
                .Create(stream, (_, bytes) => bytes.ToArray())
                .EnumerateAsync(cancellationToken)
                .GetAsyncEnumerator(cancellationToken);

            while (true)
            {
                SseItem<byte[]> item;
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

                if (item.Data.AsSpan().SequenceEqual(TerminalData))
                {
                    completionKind = SseCompletionKind.EndOfStream;
                    yield break;
                }

                IEnumerator<T>? updates = null;
                try
                {
                    try
                    {
                        using (lifecycle?.Enter())
                        {
                            updates = eventDeserializerFunc(item).GetEnumerator();
                        }
                    }

                    catch (Exception exception)
                    {
                        lifecycle?.OnException(exception);
                        throw;
                    }

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
                finally
                {
                    try
                    {
                        updates?.Dispose();
                    }
                    catch (Exception exception)
                    {
                        lifecycle?.OnException(exception);
                        throw;
                    }
                }
            }
        }
        finally
        {
            using (lifecycle?.Enter())
            {
                try
                {
                    foreach (Action additionalDisposalAction in additionalDisposalActions ?? [])
                    {
                        additionalDisposalAction.Invoke();
                    }
                }
                catch (Exception exception)
                {
                    lifecycle?.OnException(exception);
                    throw;
                }
                finally
                {
                    additionalDisposalActions?.Clear();
                    lifecycle?.Complete(completionKind);
                }
            }
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
        OpenTelemetryResponseStream? unenumeratedResponseStream = null;

        try
        {
            using (lifecycle?.Enter())
            {
                result = await _sendRequestAsync().ConfigureAwait(false);
                PipelineResponse response = result.GetRawResponse();
                lifecycle?.OnResponse(response);

                if ((lifecycle is not null) && (response.ContentStream is { } responseStream))
                {
                    unenumeratedResponseStream = new OpenTelemetryResponseStream(
                        responseStream,
                        lifecycle.Enter,
                        exception =>
                        {
                            try
                            {
                                if (exception is not null)
                                {
                                    lifecycle.OnException(exception);
                                }

                                foreach (Action additionalDisposalAction in AdditionalDisposalActions)
                                {
                                    additionalDisposalAction.Invoke();
                                }
                            }
                            catch (Exception disposalException)
                            {
                                lifecycle.OnException(disposalException);
                                throw;
                            }
                            finally
                            {
                                AdditionalDisposalActions.Clear();
                                lifecycle.Complete(SseCompletionKind.Disposed);
                            }
                        });
                    response.ContentStream = unenumeratedResponseStream;
                }
            }
        }
        catch (Exception exception)
        {
            lifecycle?.OnException(exception);
            throw;
        }

        return SseStreamingResult.Create(
            result.GetRawResponse(),
            item =>
            {
                using JsonDocument document = JsonDocument.Parse(item.Data);
                return [_jsonSingleDeserializerFunc(
                    document.RootElement,
                    BinaryData.FromBytes(item.Data),
                    ModelSerializationExtensions.WireOptions)];
            },
            _cancellationToken,
            AdditionalDisposalActions,
            lifecycle,
            unenumeratedResponseStream);
    }
}