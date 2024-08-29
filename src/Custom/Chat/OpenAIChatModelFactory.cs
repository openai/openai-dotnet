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
        IEnumerable<ChatMessageContentPart> content = null,
        string refusal = null,
        IEnumerable<ChatToolCall> toolCalls = null,
        ChatMessageRole role = default,
        ChatFunctionCall functionCall = null,
        IEnumerable<ChatTokenLogProbabilityInfo> contentTokenLogProbabilities = null,
        IEnumerable<ChatTokenLogProbabilityInfo> refusalTokenLogProbabilities = null,
        DateTimeOffset createdAt = default,
        string model = null,
        string systemFingerprint = null,
        ChatTokenUsage usage = null)
    {
        content ??= new List<ChatMessageContentPart>();
        toolCalls ??= new List<ChatToolCall>();
        contentTokenLogProbabilities ??= new List<ChatTokenLogProbabilityInfo>();
        refusalTokenLogProbabilities ??= new List<ChatTokenLogProbabilityInfo>();

        InternalChatCompletionResponseMessage message = new InternalChatCompletionResponseMessage(
            content.ToList(),
            refusal,
            toolCalls.ToList(),
            role,
            functionCall,
            serializedAdditionalRawData: null);

        InternalCreateChatCompletionResponseChoiceLogprobs logprobs = new InternalCreateChatCompletionResponseChoiceLogprobs(
            contentTokenLogProbabilities.ToList(),
            refusalTokenLogProbabilities.ToList(),
            serializedAdditionalRawData: null);

        IReadOnlyList<InternalCreateChatCompletionResponseChoice> choices = [
            new InternalCreateChatCompletionResponseChoice(
                finishReason,
                index: 0,
                message,
                logprobs,
                serializedAdditionalRawData: null)
        ];

        return new ChatCompletion(
            id,
            choices,
            createdAt,
            model,
            serviceTier: null,
            systemFingerprint,
            InternalCreateChatCompletionResponseObject.ChatCompletion,
            usage,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenLogProbabilityInfo"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenLogProbabilityInfo"/> instance for mocking. </returns>
    public static ChatTokenLogProbabilityInfo ChatTokenLogProbabilityInfo(string token = null, float logProbability = default, IEnumerable<int> utf8ByteValues = null, IEnumerable<ChatTokenTopLogProbabilityInfo> topLogProbabilities = null)
    {
        utf8ByteValues ??= new List<int>();
        topLogProbabilities ??= new List<ChatTokenTopLogProbabilityInfo>();

        return new ChatTokenLogProbabilityInfo(
            token,
            logProbability,
            utf8ByteValues.ToList(),
            topLogProbabilities.ToList(),
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenTopLogProbabilityInfo"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenTopLogProbabilityInfo"/> instance for mocking. </returns>
    public static ChatTokenTopLogProbabilityInfo ChatTokenTopLogProbabilityInfo(string token = null, float logProbability = default, IEnumerable<int> utf8ByteValues = null)
    {
        utf8ByteValues ??= new List<int>();

        return new ChatTokenTopLogProbabilityInfo(
            token,
            logProbability,
            utf8ByteValues.ToList(),
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenUsage"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenUsage"/> instance for mocking. </returns>
    public static ChatTokenUsage ChatTokenUsage(int outputTokens = default, int inputTokens = default, int totalTokens = default)
    {
        return new ChatTokenUsage(
            outputTokens,
            inputTokens,
            totalTokens,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatCompletionUpdate"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.StreamingChatCompletionUpdate"/> instance for mocking. </returns>
    public static StreamingChatCompletionUpdate StreamingChatCompletionUpdate(
        string id = null,
        IEnumerable<ChatMessageContentPart> contentUpdate = null,
        StreamingChatFunctionCallUpdate functionCallUpdate = null,
        IEnumerable<StreamingChatToolCallUpdate> toolCallUpdates = null,
        ChatMessageRole? role = null,
        string refusalUpdate = null,
        IEnumerable<ChatTokenLogProbabilityInfo> contentTokenLogProbabilities = null,
        IEnumerable<ChatTokenLogProbabilityInfo> refusalTokenLogProbabilities = null,
        ChatFinishReason? finishReason = null,
        DateTimeOffset createdAt = default,
        string model = null,
        string systemFingerprint = null,
        ChatTokenUsage usage = null)
    {
        contentUpdate ??= new List<ChatMessageContentPart>();
        toolCallUpdates ??= new List<StreamingChatToolCallUpdate>();
        contentTokenLogProbabilities ??= new List<ChatTokenLogProbabilityInfo>();
        refusalTokenLogProbabilities ??= new List<ChatTokenLogProbabilityInfo>();

        InternalChatCompletionStreamResponseDelta delta = new InternalChatCompletionStreamResponseDelta(
            contentUpdate.ToList(),
            functionCallUpdate,
            toolCallUpdates.ToList(),
            role,
            refusalUpdate,
            serializedAdditionalRawData: null);

        InternalCreateChatCompletionStreamResponseChoiceLogprobs logprobs = new InternalCreateChatCompletionStreamResponseChoiceLogprobs(
            contentTokenLogProbabilities.ToList(),
            refusalTokenLogProbabilities.ToList(),
            serializedAdditionalRawData: null);

        IReadOnlyList<InternalCreateChatCompletionStreamResponseChoice> choices = [
            new InternalCreateChatCompletionStreamResponseChoice(
                delta,
                logprobs,
                finishReason,
                index: 0,
                serializedAdditionalRawData: null)
        ];

        return new StreamingChatCompletionUpdate(
            id,
            choices,
            createdAt,
            model,
            serviceTier: null,
            systemFingerprint,
            InternalCreateChatCompletionStreamResponseObject.ChatCompletionChunk,
            usage,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatFunctionCallUpdate"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.StreamingChatFunctionCallUpdate"/> instance for mocking. </returns>
    public static StreamingChatFunctionCallUpdate StreamingChatFunctionCallUpdate(string functionArgumentsUpdate = null, string functionName = null)
    {
        return new StreamingChatFunctionCallUpdate(
            functionArgumentsUpdate,
            functionName,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatToolCallUpdate"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.StreamingChatToolCallUpdate"/> instance for mocking. </returns>
    public static StreamingChatToolCallUpdate StreamingChatToolCallUpdate(int index = default, string id = null, ChatToolCallKind kind = default, string functionName = null, string functionArgumentsUpdate = null)
    {
        InternalChatCompletionMessageToolCallChunkFunction function = new InternalChatCompletionMessageToolCallChunkFunction(
            functionName,
            functionArgumentsUpdate,
            serializedAdditionalRawData: null);

        return new StreamingChatToolCallUpdate(
            index,
            id,
            kind,
            function,
            serializedAdditionalRawData: null);
    }
}
