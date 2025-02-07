using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Chat;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIChatModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatCompletion"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatCompletion"/> instance for mocking. </returns>
    public static ChatCompletion ChatCompletion(
        string id = null,
        ChatFinishReason finishReason = default,
        ChatMessageContent content = null,
        string refusal = null,
        IEnumerable<ChatToolCall> toolCalls = null,
        ChatMessageRole role = default,
        ChatFunctionCall functionCall = null,
        IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null,
        IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null,
        DateTimeOffset createdAt = default,
        string model = null,
        string systemFingerprint = null,
        ChatTokenUsage usage = null,
        ChatOutputAudio outputAudio = null)
    {
        content ??= new ChatMessageContent();
        toolCalls ??= new List<ChatToolCall>();
        contentTokenLogProbabilities ??= new List<ChatTokenLogProbabilityDetails>();
        refusalTokenLogProbabilities ??= new List<ChatTokenLogProbabilityDetails>();

        InternalChatCompletionResponseMessage message = new InternalChatCompletionResponseMessage(
            refusal,
            toolCalls.ToList(),
            outputAudio,
            role,
            content,
            functionCall,
            additionalBinaryDataProperties: null);

        InternalCreateChatCompletionResponseChoiceLogprobs logprobs = new InternalCreateChatCompletionResponseChoiceLogprobs(
            contentTokenLogProbabilities.ToList(),
            refusalTokenLogProbabilities.ToList(),
            additionalBinaryDataProperties: null);

        IReadOnlyList<InternalCreateChatCompletionResponseChoice> choices = [
            new InternalCreateChatCompletionResponseChoice(
                finishReason,
                index: 0,
                message,
                logprobs,
                additionalBinaryDataProperties: null)
        ];

        return new ChatCompletion(
            id,
            model,
            systemFingerprint,
            usage,
            InternalCreateChatCompletionResponseObject.ChatCompletion,
            serviceTier: null,
            choices,
            createdAt,
            additionalBinaryDataProperties: null);
    }


    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenLogProbabilityDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenLogProbabilityDetails"/> instance for mocking. </returns>
    public static ChatTokenLogProbabilityDetails ChatTokenLogProbabilityDetails(string token = null, float logProbability = default, ReadOnlyMemory<byte>? utf8Bytes = null, IEnumerable<ChatTokenTopLogProbabilityDetails> topLogProbabilities = null)
    {
        topLogProbabilities ??= new List<ChatTokenTopLogProbabilityDetails>();

        return new ChatTokenLogProbabilityDetails(
            token,
            logProbability,
            utf8Bytes,
            topLogProbabilities.ToList(),
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenTopLogProbabilityDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenTopLogProbabilityDetails"/> instance for mocking. </returns>
    public static ChatTokenTopLogProbabilityDetails ChatTokenTopLogProbabilityDetails(string token = null, float logProbability = default, ReadOnlyMemory<byte>? utf8Bytes = null)
    {
        return new ChatTokenTopLogProbabilityDetails(
            token,
            logProbability,
            utf8Bytes,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenUsage"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenUsage"/> instance for mocking. </returns>
    public static ChatTokenUsage ChatTokenUsage(int outputTokenCount = default, int inputTokenCount = default, int totalTokenCount = default, ChatOutputTokenUsageDetails outputTokenDetails = null, ChatInputTokenUsageDetails inputTokenDetails = null)
    {
        return new ChatTokenUsage(
            outputTokenCount,
            inputTokenCount,
            totalTokenCount,
            outputTokenDetails,
            inputTokenDetails,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatInputTokenUsageDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatInputTokenUsageDetails"/> instance for mocking. </returns>
    public static ChatInputTokenUsageDetails ChatInputTokenUsageDetails(int audioTokenCount = default, int cachedTokenCount = default)
    {
        return new ChatInputTokenUsageDetails(
            audioTokenCount: audioTokenCount,
            cachedTokenCount: cachedTokenCount,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatOutputTokenUsageDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatOutputTokenusageDetails"/> instance for mocking. </returns>
    public static ChatOutputTokenUsageDetails ChatOutputTokenUsageDetails(int reasoningTokenCount = default, int audioTokenCount = default, int acceptedPredictionTokenCount = default, int rejectedPredictionTokenCount = 0)
    {
        return new ChatOutputTokenUsageDetails(
            audioTokenCount: audioTokenCount,
            reasoningTokenCount: reasoningTokenCount,
            acceptedPredictionTokenCount: acceptedPredictionTokenCount,
            rejectedPredictionTokenCount: rejectedPredictionTokenCount,
            additionalBinaryDataProperties: null);
    }

    public static ChatOutputAudio ChatOutputAudio(BinaryData audioBytes, string id = null, string transcript = null, DateTimeOffset expiresAt = default)
    {
        return new ChatOutputAudio(
            id,
            expiresAt,
            transcript,
            audioBytes,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatCompletionUpdate"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.StreamingChatCompletionUpdate"/> instance for mocking. </returns>
    public static StreamingChatCompletionUpdate StreamingChatCompletionUpdate(
        string completionId = null,
        ChatMessageContent contentUpdate = null,
        StreamingChatFunctionCallUpdate functionCallUpdate = null,
        IEnumerable<StreamingChatToolCallUpdate> toolCallUpdates = null,
        ChatMessageRole? role = null,
        string refusalUpdate = null,
        IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null,
        IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null,
        ChatFinishReason? finishReason = null,
        DateTimeOffset createdAt = default,
        string model = null,
        string systemFingerprint = null,
        ChatTokenUsage usage = null,
        StreamingChatOutputAudioUpdate outputAudioUpdate = null)
    {
        contentUpdate ??= new ChatMessageContent();
        toolCallUpdates ??= new List<StreamingChatToolCallUpdate>();
        contentTokenLogProbabilities ??= new List<ChatTokenLogProbabilityDetails>();
        refusalTokenLogProbabilities ??= new List<ChatTokenLogProbabilityDetails>();

        InternalChatCompletionStreamResponseDelta delta = new InternalChatCompletionStreamResponseDelta(
            outputAudioUpdate,
            functionCallUpdate,
            toolCallUpdates.ToList(),
            refusalUpdate,
            role,
            contentUpdate,
            additionalBinaryDataProperties: null);

        InternalCreateChatCompletionStreamResponseChoiceLogprobs logprobs = new InternalCreateChatCompletionStreamResponseChoiceLogprobs(
            contentTokenLogProbabilities.ToList(),
            refusalTokenLogProbabilities.ToList(),
            additionalBinaryDataProperties: null);

        IReadOnlyList<InternalCreateChatCompletionStreamResponseChoice> choices = [
            new InternalCreateChatCompletionStreamResponseChoice(
                delta,
                logprobs,
                index: 0,
                finishReason,
                additionalBinaryDataProperties: null)
        ];

        return new StreamingChatCompletionUpdate(
            model,
            systemFingerprint,
            InternalCreateChatCompletionStreamResponseObject.ChatCompletionChunk,
            completionId,
            serviceTier: null,
            choices,
            createdAt,
            usage,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatFunctionCallUpdate"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.StreamingChatFunctionCallUpdate"/> instance for mocking. </returns>
    [Obsolete($"This class is obsolete. Please use {nameof(StreamingChatToolCallUpdate)} instead.")]
    public static StreamingChatFunctionCallUpdate StreamingChatFunctionCallUpdate(string functionName = null, BinaryData functionArgumentsUpdate = null)
    {
        return new StreamingChatFunctionCallUpdate(
            functionName,
            functionArgumentsUpdate,
            additionalBinaryDataProperties: null);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatOutputAudioUpdate"/>.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="expiresAt"></param>
    /// <param name="transcriptUpdate"></param>
    /// <param name="audioBytesUpdate"></param>
    /// <returns></returns>
    public static StreamingChatOutputAudioUpdate StreamingChatOutputAudioUpdate(
        string id = null,
        DateTimeOffset? expiresAt = null,
        string transcriptUpdate = null,
        BinaryData audioBytesUpdate = null)
    {
        return new StreamingChatOutputAudioUpdate(
            id,
            expiresAt,
            transcriptUpdate,
            audioBytesUpdate,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatToolCallUpdate"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.StreamingChatToolCallUpdate"/> instance for mocking. </returns>
    public static StreamingChatToolCallUpdate StreamingChatToolCallUpdate(int index = default, string toolCallId = null, ChatToolCallKind kind = default, string functionName = null, BinaryData functionArgumentsUpdate = null)
    {
        InternalChatCompletionMessageToolCallChunkFunction function = new InternalChatCompletionMessageToolCallChunkFunction(
            functionName,
            functionArgumentsUpdate,
            additionalBinaryDataProperties: null);

        return new StreamingChatToolCallUpdate(
            index,
            function,
            kind,
            toolCallId,
            additionalBinaryDataProperties: null);
    }
}
