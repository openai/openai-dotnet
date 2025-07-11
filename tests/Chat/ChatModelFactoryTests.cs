using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenAI.Chat;

namespace OpenAI.Tests.Chat;

[Parallelizable(ParallelScope.All)]
[Category("Chat")]
[Category("Smoke")]
public partial class ChatModelFactoryTests
{
    [Test]
    public void ChatCompletionWithNoPropertiesWorks()
    {
        ChatCompletion chatCompletion = ChatModelFactory.ChatCompletion();

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithIdWorks()
    {
        string id = "chat_completion_id";
        ChatCompletion chatCompletion = ChatModelFactory.ChatCompletion(id: id);

        Assert.That(chatCompletion.Id, Is.EqualTo(id));
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithAllPropertiesWorks()
    {
        string id = "chat_completion_id";
        ChatFinishReason finishReason = ChatFinishReason.Stop;
        ChatMessageContent content = [ChatMessageContentPart.CreateTextPart("Hello, world!")];
        string refusal = "I cannot comply";
        IEnumerable<ChatToolCall> toolCalls = [
            ChatToolCall.CreateFunctionToolCall("id1", "get_weather", BinaryData.FromString("{}"))
        ];
        ChatMessageRole role = ChatMessageRole.Assistant;
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        string model = "gpt-4";
        string systemFingerprint = "fp_123";
        ChatTokenUsage usage = ChatModelFactory.ChatTokenUsage(inputTokenCount: 10, outputTokenCount: 20);

        ChatCompletion chatCompletion = ChatModelFactory.ChatCompletion(
            id: id,
            finishReason: finishReason,
            content: content,
            refusal: refusal,
            toolCalls: toolCalls,
            role: role,
            createdAt: createdAt,
            model: model,
            systemFingerprint: systemFingerprint,
            usage: usage);

        Assert.That(chatCompletion.Id, Is.EqualTo(id));
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(finishReason));
        Assert.That(chatCompletion.Content.SequenceEqual(content), Is.True);
        Assert.That(chatCompletion.Refusal, Is.EqualTo(refusal));
        Assert.That(chatCompletion.ToolCalls.SequenceEqual(toolCalls), Is.True);
        Assert.That(chatCompletion.Role, Is.EqualTo(role));
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(createdAt));
        Assert.That(chatCompletion.Model, Is.EqualTo(model));
        Assert.That(chatCompletion.SystemFingerprint, Is.EqualTo(systemFingerprint));
        Assert.That(chatCompletion.Usage, Is.EqualTo(usage));
    }

    [Test]
    public void StreamingChatCompletionUpdateWithNoPropertiesWorks()
    {
        StreamingChatCompletionUpdate update = ChatModelFactory.StreamingChatCompletionUpdate();

        Assert.That(update.CompletionId, Is.Null);
        Assert.That(update.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(update.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(update.Role, Is.Null);
        Assert.That(update.RefusalUpdate, Is.Null);
        Assert.That(update.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(update.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(update.FinishReason, Is.Null);
        Assert.That(update.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(update.Model, Is.Null);
        Assert.That(update.SystemFingerprint, Is.Null);
        Assert.That(update.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithPropertiesWorks()
    {
        string completionId = "completion_id";
        ChatMessageContent contentUpdate = [ChatMessageContentPart.CreateTextPart("Hello")];
        ChatMessageRole role = ChatMessageRole.Assistant;
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        string model = "gpt-4";

        StreamingChatCompletionUpdate update = ChatModelFactory.StreamingChatCompletionUpdate(
            completionId: completionId,
            contentUpdate: contentUpdate,
            role: role,
            createdAt: createdAt,
            model: model);

        Assert.That(update.CompletionId, Is.EqualTo(completionId));
        Assert.That(update.ContentUpdate.SequenceEqual(contentUpdate), Is.True);
        Assert.That(update.Role, Is.EqualTo(role));
        Assert.That(update.CreatedAt, Is.EqualTo(createdAt));
        Assert.That(update.Model, Is.EqualTo(model));
    }

    [Test]
    public void ChatCompletionDeletionResultWorks()
    {
        string chatCompletionId = "completion_id";
        bool deleted = true;

        ChatCompletionDeletionResult result = ChatModelFactory.ChatCompletionDeletionResult(
            deleted: deleted,
            chatCompletionId: chatCompletionId);

        Assert.That(result.Deleted, Is.EqualTo(deleted));
        Assert.That(result.ChatCompletionId, Is.EqualTo(chatCompletionId));
    }

    [Test]
    public void UserChatMessageWorks()
    {
        string content = "Hello, assistant!";
        string participantName = "user1";

        UserChatMessage message = ChatModelFactory.UserChatMessage(
            content: content,
            participantName: participantName);

        Assert.That(message.Content[0].Text, Is.EqualTo(content));
        Assert.That(message.ParticipantName, Is.EqualTo(participantName));
        Assert.That(message.Role, Is.EqualTo(ChatMessageRole.User));
    }

    [Test]
    public void AssistantChatMessageWorks()
    {
        string content = "Hello, user!";
        IEnumerable<ChatToolCall> toolCalls = [
            ChatToolCall.CreateFunctionToolCall("id1", "get_weather", BinaryData.FromString("{}"))
        ];
        string participantName = "assistant1";

        AssistantChatMessage message = ChatModelFactory.AssistantChatMessage(
            content: content,
            participantName: participantName);

        Assert.That(message.Content[0].Text, Is.EqualTo(content));
        Assert.That(message.ParticipantName, Is.EqualTo(participantName));
        Assert.That(message.Role, Is.EqualTo(ChatMessageRole.Assistant));

        // Test with tool calls
        AssistantChatMessage messageWithToolCalls = ChatModelFactory.AssistantChatMessage(
            toolCalls: toolCalls,
            participantName: participantName);

        Assert.That(messageWithToolCalls.ToolCalls.SequenceEqual(toolCalls), Is.True);
        Assert.That(messageWithToolCalls.ParticipantName, Is.EqualTo(participantName));
        Assert.That(messageWithToolCalls.Role, Is.EqualTo(ChatMessageRole.Assistant));
    }

    [Test]
    public void SystemChatMessageWorks()
    {
        string content = "You are a helpful assistant";
        string participantName = "system";

        SystemChatMessage message = ChatModelFactory.SystemChatMessage(
            content: content,
            participantName: participantName);

        Assert.That(message.Content[0].Text, Is.EqualTo(content));
        Assert.That(message.ParticipantName, Is.EqualTo(participantName));
        Assert.That(message.Role, Is.EqualTo(ChatMessageRole.System));
    }

    [Test]
    public void ToolChatMessageWorks()
    {
        string toolCallId = "tool_call_id";
        string content = "Weather is sunny";

        ToolChatMessage message = ChatModelFactory.ToolChatMessage(
            toolCallId: toolCallId,
            content: content);

        Assert.That(message.Content[0].Text, Is.EqualTo(content));
        Assert.That(message.ToolCallId, Is.EqualTo(toolCallId));
        Assert.That(message.Role, Is.EqualTo(ChatMessageRole.Tool));
    }

    [Test]
    public void ChatTokenUsageWorks()
    {
        int inputTokenCount = 100;
        int outputTokenCount = 50;
        int totalTokenCount = 150;

        ChatTokenUsage usage = ChatModelFactory.ChatTokenUsage(
            inputTokenCount: inputTokenCount,
            outputTokenCount: outputTokenCount,
            totalTokenCount: totalTokenCount);

        Assert.That(usage.InputTokenCount, Is.EqualTo(inputTokenCount));
        Assert.That(usage.OutputTokenCount, Is.EqualTo(outputTokenCount));
        Assert.That(usage.TotalTokenCount, Is.EqualTo(totalTokenCount));
    }

    [Test]
    public void ChatTokenLogProbabilityDetailsWorks()
    {
        string token = "hello";
        float logProbability = -0.5f;
        ReadOnlyMemory<byte> utf8Bytes = "hello"u8.ToArray();

        ChatTokenLogProbabilityDetails details = ChatModelFactory.ChatTokenLogProbabilityDetails(
            token: token,
            logProbability: logProbability,
            utf8Bytes: utf8Bytes);

        Assert.That(details.Token, Is.EqualTo(token));
        Assert.That(details.LogProbability, Is.EqualTo(logProbability));
        Assert.That(details.Utf8Bytes.Value.ToArray().SequenceEqual(utf8Bytes.ToArray()), Is.True);
        Assert.That(details.TopLogProbabilities, Is.Not.Null.And.Empty);
    }

    [Test]
    public void ChatTokenTopLogProbabilityDetailsWorks()
    {
        string token = "hello";
        float logProbability = -0.5f;
        ReadOnlyMemory<byte> utf8Bytes = "hello"u8.ToArray();

        ChatTokenTopLogProbabilityDetails details = ChatModelFactory.ChatTokenTopLogProbabilityDetails(
            token: token,
            logProbability: logProbability,
            utf8Bytes: utf8Bytes);

        Assert.That(details.Token, Is.EqualTo(token));
        Assert.That(details.LogProbability, Is.EqualTo(logProbability));
        Assert.That(details.Utf8Bytes.Value.ToArray().SequenceEqual(utf8Bytes.ToArray()), Is.True);
    }

    [Test]
    public void StreamingChatToolCallUpdateWorks()
    {
        int index = 0;
        string toolCallId = "tool_call_id";
        ChatToolCallKind kind = ChatToolCallKind.Function;
        string functionName = "get_weather";
        BinaryData functionArgumentsUpdate = BinaryData.FromString("{\"location\":");

        StreamingChatToolCallUpdate update = ChatModelFactory.StreamingChatToolCallUpdate(
            index: index,
            toolCallId: toolCallId,
            kind: kind,
            functionName: functionName,
            functionArgumentsUpdate: functionArgumentsUpdate);

        Assert.That(update.Index, Is.EqualTo(index));
        Assert.That(update.ToolCallId, Is.EqualTo(toolCallId));
        Assert.That(update.Kind, Is.EqualTo(kind));
        Assert.That(update.FunctionName, Is.EqualTo(functionName));
        Assert.That(update.FunctionArgumentsUpdate, Is.EqualTo(functionArgumentsUpdate));
    }

    [Test]
    public void ChatMessageAnnotationWorks()
    {
        int startIndex = 5;
        int endIndex = 10;
        Uri webResourceUri = new Uri("https://example.com");
        string webResourceTitle = "Example Resource";

        ChatMessageAnnotation annotation = ChatModelFactory.ChatMessageAnnotation(
            startIndex: startIndex,
            endIndex: endIndex,
            webResourceUri: webResourceUri,
            webResourceTitle: webResourceTitle);

        Assert.That(annotation.StartIndex, Is.EqualTo(startIndex));
        Assert.That(annotation.EndIndex, Is.EqualTo(endIndex));
        Assert.That(annotation.WebResourceUri, Is.EqualTo(webResourceUri));
        Assert.That(annotation.WebResourceTitle, Is.EqualTo(webResourceTitle));
    }

    [Test]
    public void ChatInputTokenUsageDetailsWorks()
    {
        int audioTokenCount = 25;
        int cachedTokenCount = 75;

        ChatInputTokenUsageDetails details = ChatModelFactory.ChatInputTokenUsageDetails(
            audioTokenCount: audioTokenCount,
            cachedTokenCount: cachedTokenCount);

        Assert.That(details.AudioTokenCount, Is.EqualTo(audioTokenCount));
        Assert.That(details.CachedTokenCount, Is.EqualTo(cachedTokenCount));
    }

    [Test]
    public void ChatOutputTokenUsageDetailsWorks()
    {
        int reasoningTokenCount = 30;
        int audioTokenCount = 15;
        int acceptedPredictionTokenCount = 5;
        int rejectedPredictionTokenCount = 2;

        ChatOutputTokenUsageDetails details = ChatModelFactory.ChatOutputTokenUsageDetails(
            reasoningTokenCount: reasoningTokenCount,
            audioTokenCount: audioTokenCount,
            acceptedPredictionTokenCount: acceptedPredictionTokenCount,
            rejectedPredictionTokenCount: rejectedPredictionTokenCount);

        Assert.That(details.ReasoningTokenCount, Is.EqualTo(reasoningTokenCount));
        Assert.That(details.AudioTokenCount, Is.EqualTo(audioTokenCount));
        Assert.That(details.AcceptedPredictionTokenCount, Is.EqualTo(acceptedPredictionTokenCount));
        Assert.That(details.RejectedPredictionTokenCount, Is.EqualTo(rejectedPredictionTokenCount));
    }
}