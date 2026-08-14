using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;

#nullable enable

namespace OpenAI;

internal static class SseStreamingClientResult
{
    private static ReadOnlySpan<byte> TerminalData => "[DONE]"u8;

#pragma warning disable SCME0005 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    public static AsyncStreamingClientResult<T> Create<T>(
        PipelineResponse response,
        Func<JsonElement, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
        CancellationToken cancellationToken,
        IEnumerable<Action>? additionalDisposalActions = null)
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
            additionalDisposalActions);
    }

    public static AsyncStreamingClientResult<T> Create<T>(
        PipelineResponse response,
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, T> jsonSingleDeserializerFunc,
        CancellationToken cancellationToken,
        IEnumerable<Action>? additionalDisposalActions = null)
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
                additionalDisposalActions);
    }

    public static AsyncStreamingClientResult<T> Create<T>(
        PipelineResponse response,
        Func<JsonElement, BinaryData, ModelReaderWriterOptions, IEnumerable<T>> jsonMultiDeserializerFunc,
        CancellationToken cancellationToken,
        IEnumerable<Action>? additionalDisposalActions = null)
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
            additionalDisposalActions);
    }

    public static AsyncStreamingClientResult<T> Create<T>(
        PipelineResponse response,
        Func<SseItem<byte[]>, IEnumerable<T>> eventDeserializerFunc,
        CancellationToken cancellationToken,
        IEnumerable<Action>? additionalDisposalActions = null)
    {
        Argument.AssertNotNull(response, nameof(response));
        Argument.AssertNotNull(eventDeserializerFunc, nameof(eventDeserializerFunc));

        return AsyncStreamingClientResult.Create<T>(
            response,
            (stream, producerCancellationToken) => EnumerateAsync(stream, eventDeserializerFunc, additionalDisposalActions, producerCancellationToken),
            cancellationToken);
    }
#pragma warning restore SCME0005

    private static async IAsyncEnumerable<T> EnumerateAsync<T>(
        Stream stream,
        Func<SseItem<byte[]>, IEnumerable<T>> eventDeserializerFunc,
        IEnumerable<Action>? additionalDisposalActions,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        try
        {
            await foreach (SseItem<byte[]> item in SseParser.Create(stream, (_, bytes) => bytes.ToArray()).EnumerateAsync(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (item.Data.AsSpan().SequenceEqual(TerminalData))
                {
                    yield break;
                }

                foreach (T update in eventDeserializerFunc(item))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return update;
                }
            }
        }
        finally
        {
            foreach (Action additionalDisposalAction in additionalDisposalActions ?? [])
            {
                additionalDisposalAction.Invoke();
            }
        }
    }
}