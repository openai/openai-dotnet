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

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.UserChatMessage"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.UserChatMessage"/> instance for mocking. </returns>
    public static UserChatMessage UserChatMessage(
        string content = null,
        string participantName = null)
    {
        content ??= "User message content";
        var message = new UserChatMessage(content);
        if (participantName != null)
        {
            message.ParticipantName = participantName;
        }
        return message;
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.AssistantChatMessage"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.AssistantChatMessage"/> instance for mocking. </returns>
    public static AssistantChatMessage AssistantChatMessage(
        string content = null,
        IEnumerable<ChatToolCall> toolCalls = null,
        string participantName = null)
    {
        AssistantChatMessage message;
        if (toolCalls != null)
        {
            message = new AssistantChatMessage(toolCalls);
        }
        else
        {
            content ??= "Assistant message content";
            message = new AssistantChatMessage(content);
        }
        
        if (participantName != null)
        {
            message.ParticipantName = participantName;
        }
        return message;
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.SystemChatMessage"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.SystemChatMessage"/> instance for mocking. </returns>
    public static SystemChatMessage SystemChatMessage(
        string content = null,
        string participantName = null)
    {
        content ??= "System message content";
        var message = new SystemChatMessage(content);
        if (participantName != null)
        {
            message.ParticipantName = participantName;
        }
        return message;
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Chat.ToolChatMessage"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Chat.ToolChatMessage"/> instance for mocking. </returns>
    public static ToolChatMessage ToolChatMessage(
        string toolCallId = "tool_call_id",
        string content = null)
    {
        content ??= "Tool message content";
        return new ToolChatMessage(toolCallId, content);
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
}