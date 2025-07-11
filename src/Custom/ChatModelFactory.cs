using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OpenAI.Chat;

namespace OpenAI;

/// <summary> Model factory for Chat models. </summary>
public static partial class ChatModelFactory
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
        IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null,
        IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null,
        DateTimeOffset createdAt = default,
        string model = null,
        string systemFingerprint = null,
        ChatTokenUsage usage = default)
    {
        return OpenAI.Chat.OpenAIChatModelFactory.ChatCompletion(
            id: id,
            finishReason: finishReason,
            content: content,
            refusal: refusal,
            toolCalls: toolCalls,
            role: role,
            functionCall: default,
            contentTokenLogProbabilities: contentTokenLogProbabilities,
            refusalTokenLogProbabilities: refusalTokenLogProbabilities,
            createdAt: createdAt,
            model: model,
            systemFingerprint: systemFingerprint,
            usage: usage);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatCompletionUpdate"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.StreamingChatCompletionUpdate"/> instance for mocking. </returns>
    public static StreamingChatCompletionUpdate StreamingChatCompletionUpdate(
        string completionId = null,
        ChatMessageContent contentUpdate = null,
        IEnumerable<StreamingChatToolCallUpdate> toolCallUpdates = null,
        ChatMessageRole? role = default,
        string refusalUpdate = null,
        IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = null,
        IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = null,
        ChatFinishReason? finishReason = default,
        DateTimeOffset createdAt = default,
        string model = null,
        string systemFingerprint = null,
        ChatTokenUsage usage = default)
    {
        return OpenAI.Chat.OpenAIChatModelFactory.StreamingChatCompletionUpdate(
            completionId: completionId,
            contentUpdate: contentUpdate,
            functionCallUpdate: default,
            toolCallUpdates: toolCallUpdates,
            role: role,
            refusalUpdate: refusalUpdate,
            contentTokenLogProbabilities: contentTokenLogProbabilities,
            refusalTokenLogProbabilities: refusalTokenLogProbabilities,
            finishReason: finishReason,
            createdAt: createdAt,
            model: model,
            systemFingerprint: systemFingerprint,
            usage: usage);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatCompletionDeletionResult"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatCompletionDeletionResult"/> instance for mocking. </returns>
    public static ChatCompletionDeletionResult ChatCompletionDeletionResult(
        bool deleted = true, 
        string chatCompletionId = null)
    {
        return new ChatCompletionDeletionResult(
            deleted: deleted,
            @object: "chat.completion.deleted",
            chatCompletionId: chatCompletionId,
            additionalBinaryDataProperties: null);
    }



    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenUsage"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenUsage"/> instance for mocking. </returns>
    public static ChatTokenUsage ChatTokenUsage(
        int outputTokenCount = default, 
        int inputTokenCount = default, 
        int totalTokenCount = default)
    {
        return OpenAI.Chat.OpenAIChatModelFactory.ChatTokenUsage(
            outputTokenCount: outputTokenCount,
            inputTokenCount: inputTokenCount,
            totalTokenCount: totalTokenCount);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenLogProbabilityDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenLogProbabilityDetails"/> instance for mocking. </returns>
    public static ChatTokenLogProbabilityDetails ChatTokenLogProbabilityDetails(
        string token = null, 
        float logProbability = default, 
        ReadOnlyMemory<byte>? utf8Bytes = null, 
        IEnumerable<ChatTokenTopLogProbabilityDetails> topLogProbabilities = null)
    {
        return OpenAI.Chat.OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(
            token: token,
            logProbability: logProbability,
            utf8Bytes: utf8Bytes,
            topLogProbabilities: topLogProbabilities);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatTokenTopLogProbabilityDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatTokenTopLogProbabilityDetails"/> instance for mocking. </returns>
    public static ChatTokenTopLogProbabilityDetails ChatTokenTopLogProbabilityDetails(
        string token = null, 
        float logProbability = default, 
        ReadOnlyMemory<byte>? utf8Bytes = null)
    {
        return OpenAI.Chat.OpenAIChatModelFactory.ChatTokenTopLogProbabilityDetails(
            token: token,
            logProbability: logProbability,
            utf8Bytes: utf8Bytes);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.StreamingChatToolCallUpdate"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.StreamingChatToolCallUpdate"/> instance for mocking. </returns>
    public static StreamingChatToolCallUpdate StreamingChatToolCallUpdate(
        int index = default, 
        string toolCallId = null, 
        ChatToolCallKind kind = default, 
        string functionName = null, 
        BinaryData functionArgumentsUpdate = null)
    {
        return OpenAI.Chat.OpenAIChatModelFactory.StreamingChatToolCallUpdate(
            index: index,
            toolCallId: toolCallId,
            kind: kind,
            functionName: functionName,
            functionArgumentsUpdate: functionArgumentsUpdate);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatMessageAnnotation"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatMessageAnnotation"/> instance for mocking. </returns>
    public static ChatMessageAnnotation ChatMessageAnnotation(
        int startIndex = default,
        int endIndex = default,
        Uri webResourceUri = default,
        string webResourceTitle = default)
    {
        return OpenAI.Chat.OpenAIChatModelFactory.ChatMessageAnnotation(
            startIndex: startIndex,
            endIndex: endIndex,
            webResourceUri: webResourceUri,
            webResourceTitle: webResourceTitle);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatInputTokenUsageDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatInputTokenUsageDetails"/> instance for mocking. </returns>
    public static ChatInputTokenUsageDetails ChatInputTokenUsageDetails(
        int audioTokenCount = default, 
        int cachedTokenCount = default)
    {
        return OpenAI.Chat.OpenAIChatModelFactory.ChatInputTokenUsageDetails(
            audioTokenCount: audioTokenCount,
            cachedTokenCount: cachedTokenCount);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ChatOutputTokenUsageDetails"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ChatOutputTokenUsageDetails"/> instance for mocking. </returns>
    public static ChatOutputTokenUsageDetails ChatOutputTokenUsageDetails(
        int reasoningTokenCount = default, 
        int audioTokenCount = default, 
        int acceptedPredictionTokenCount = default, 
        int rejectedPredictionTokenCount = default)
    {
        return OpenAI.Chat.OpenAIChatModelFactory.ChatOutputTokenUsageDetails(
            reasoningTokenCount: reasoningTokenCount,
            audioTokenCount: audioTokenCount,
            acceptedPredictionTokenCount: acceptedPredictionTokenCount,
            rejectedPredictionTokenCount: rejectedPredictionTokenCount);
    }
}