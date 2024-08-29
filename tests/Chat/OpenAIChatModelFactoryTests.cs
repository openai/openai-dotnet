using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenAI.Chat;

namespace OpenAI.Tests.Chat;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public partial class OpenAIChatModelFactoryTests
{
    [Test]
    public void ChatCompletionWithNoPropertiesWorks()
    {
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion();

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
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
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(id: id);

        Assert.That(chatCompletion.Id, Is.EqualTo(id));
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithFinishReasonWorks()
    {
        ChatFinishReason finishReason = ChatFinishReason.ToolCalls;
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(finishReason: finishReason);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(finishReason));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithContentWorks()
    {
        IEnumerable<ChatMessageContentPart> content = [
            ChatMessageContentPart.CreateTextMessageContentPart("first part"),
            ChatMessageContentPart.CreateTextMessageContentPart("second part")
        ];
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(content: content);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content.SequenceEqual(content), Is.True);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithRefusalWorks()
    {
        string refusal = "This is a refusal.";
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(refusal: refusal);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.EqualTo(refusal));
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithToolCallsWorks()
    {
        IEnumerable<ChatToolCall> toolCalls = [
            ChatToolCall.CreateFunctionToolCall("id1", "get_recipe", string.Empty),
            ChatToolCall.CreateFunctionToolCall("id2", "get_location", string.Empty)
        ];
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(toolCalls: toolCalls);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls.SequenceEqual(toolCalls), Is.True);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithRoleWorks()
    {
        ChatMessageRole role = ChatMessageRole.Tool;
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(role: role);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(role));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithFunctionCallWorks()
    {
        ChatFunctionCall functionCall = new ChatFunctionCall("get_recipe", string.Empty);
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(functionCall: functionCall);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.EqualTo(functionCall));
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithContentTokenLogProbabilitiesWorks()
    {
        IEnumerable<ChatTokenLogProbabilityInfo> contentTokenLogProbabilities = [
            OpenAIChatModelFactory.ChatTokenLogProbabilityInfo(logProbability: 1f),
            OpenAIChatModelFactory.ChatTokenLogProbabilityInfo(logProbability: 2f)
        ];
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(contentTokenLogProbabilities: contentTokenLogProbabilities);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
        Assert.That(chatCompletion.ContentTokenLogProbabilities.SequenceEqual(contentTokenLogProbabilities), Is.True);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithRefusalTokenLogProbabilitiesWorks()
    {
        IEnumerable<ChatTokenLogProbabilityInfo> refusalTokenLogProbabilities = [
            OpenAIChatModelFactory.ChatTokenLogProbabilityInfo(logProbability: 1f),
            OpenAIChatModelFactory.ChatTokenLogProbabilityInfo(logProbability: 2f)
        ];
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(refusalTokenLogProbabilities: refusalTokenLogProbabilities);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities.SequenceEqual(refusalTokenLogProbabilities), Is.True);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithCreatedAtWorks()
    {
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(createdAt: createdAt);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(createdAt));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithModelWorks()
    {
        string model = "topmodel";
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(model: model);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.EqualTo(model));
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithSystemFingerprintWorks()
    {
        string systemFingerprint = "footprint";
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(systemFingerprint: systemFingerprint);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.EqualTo(systemFingerprint));
        Assert.That(chatCompletion.Usage, Is.Null);
    }

    [Test]
    public void ChatCompletionWithUsageWorks()
    {
        ChatTokenUsage usage = OpenAIChatModelFactory.ChatTokenUsage(outputTokens: 20);
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(usage: usage);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
        Assert.That(chatCompletion.FunctionCall, Is.Null);
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.EqualTo(usage));
    }

    [Test]
    public void ChatTokenLogProbabilityInfoWithNoPropertiesWorks()
    {
        ChatTokenLogProbabilityInfo chatTokenLogProbabilityInfo = OpenAIChatModelFactory.ChatTokenLogProbabilityInfo();

        Assert.That(chatTokenLogProbabilityInfo.Token, Is.Null);
        Assert.That(chatTokenLogProbabilityInfo.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenLogProbabilityInfo.Utf8ByteValues, Is.Not.Null.And.Empty);
        Assert.That(chatTokenLogProbabilityInfo.TopLogProbabilities, Is.Not.Null.And.Empty);
    }

    [Test]
    public void ChatTokenLogProbabilityInfoWithTokenWorks()
    {
        string token = "a_token_of_appreciation";
        ChatTokenLogProbabilityInfo chatTokenLogProbabilityInfo = OpenAIChatModelFactory.ChatTokenLogProbabilityInfo(token: token);

        Assert.That(chatTokenLogProbabilityInfo.Token, Is.EqualTo(token));
        Assert.That(chatTokenLogProbabilityInfo.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenLogProbabilityInfo.Utf8ByteValues, Is.Not.Null.And.Empty);
        Assert.That(chatTokenLogProbabilityInfo.TopLogProbabilities, Is.Not.Null.And.Empty);
    }

    [Test]
    public void ChatTokenLogProbabilityInfoWithLogProbabilityWorks()
    {
        float logProbability = 3.14f;
        ChatTokenLogProbabilityInfo chatTokenLogProbabilityInfo = OpenAIChatModelFactory.ChatTokenLogProbabilityInfo(logProbability: logProbability);

        Assert.That(chatTokenLogProbabilityInfo.Token, Is.Null);
        Assert.That(chatTokenLogProbabilityInfo.LogProbability, Is.EqualTo(logProbability));
        Assert.That(chatTokenLogProbabilityInfo.Utf8ByteValues, Is.Not.Null.And.Empty);
        Assert.That(chatTokenLogProbabilityInfo.TopLogProbabilities, Is.Not.Null.And.Empty);
    }

    [Test]
    public void ChatTokenLogProbabilityInfoWithUtf8ByteValuesWorks()
    {
        IEnumerable<int> utf8ByteValues = [104, 101, 108, 108, 111];
        ChatTokenLogProbabilityInfo chatTokenLogProbabilityInfo = OpenAIChatModelFactory.ChatTokenLogProbabilityInfo(utf8ByteValues: utf8ByteValues);

        Assert.That(chatTokenLogProbabilityInfo.Token, Is.Null);
        Assert.That(chatTokenLogProbabilityInfo.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenLogProbabilityInfo.Utf8ByteValues.SequenceEqual(utf8ByteValues), Is.True);
        Assert.That(chatTokenLogProbabilityInfo.TopLogProbabilities, Is.Not.Null.And.Empty);
    }

    [Test]
    public void ChatTokenLogProbabilityInfoWithTopLogProbabilitiesWorks()
    {
        IEnumerable<ChatTokenTopLogProbabilityInfo> topLogProbabilities = [
            OpenAIChatModelFactory.ChatTokenTopLogProbabilityInfo(token: "firstToken"),
            OpenAIChatModelFactory.ChatTokenTopLogProbabilityInfo(token: "secondToken")
        ];
        ChatTokenLogProbabilityInfo chatTokenLogProbabilityInfo = OpenAIChatModelFactory.ChatTokenLogProbabilityInfo(topLogProbabilities: topLogProbabilities);

        Assert.That(chatTokenLogProbabilityInfo.Token, Is.Null);
        Assert.That(chatTokenLogProbabilityInfo.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenLogProbabilityInfo.Utf8ByteValues, Is.Not.Null.And.Empty);
        Assert.That(chatTokenLogProbabilityInfo.TopLogProbabilities.SequenceEqual(topLogProbabilities), Is.True);
    }

    [Test]
    public void ChatTokenTopLogProbabilityInfoWithNoPropertiesWorks()
    {
        ChatTokenTopLogProbabilityInfo chatTokenTopLogProbabilityInfo = OpenAIChatModelFactory.ChatTokenTopLogProbabilityInfo();

        Assert.That(chatTokenTopLogProbabilityInfo.Token, Is.Null);
        Assert.That(chatTokenTopLogProbabilityInfo.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenTopLogProbabilityInfo.Utf8ByteValues, Is.Not.Null.And.Empty);
    }

    [Test]
    public void ChatTokenTopLogProbabilityInfoWithTokenWorks()
    {
        string token = "a_token_of_appreciation";
        ChatTokenTopLogProbabilityInfo chatTokenTopLogProbabilityInfo = OpenAIChatModelFactory.ChatTokenTopLogProbabilityInfo(token: token);

        Assert.That(chatTokenTopLogProbabilityInfo.Token, Is.EqualTo(token));
        Assert.That(chatTokenTopLogProbabilityInfo.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenTopLogProbabilityInfo.Utf8ByteValues, Is.Not.Null.And.Empty);
    }

    [Test]
    public void ChatTokenTopLogProbabilityInfoWithLogProbabilityWorks()
    {
        float logProbability = 3.14f;
        ChatTokenTopLogProbabilityInfo chatTokenTopLogProbabilityInfo = OpenAIChatModelFactory.ChatTokenTopLogProbabilityInfo(logProbability: logProbability);

        Assert.That(chatTokenTopLogProbabilityInfo.Token, Is.Null);
        Assert.That(chatTokenTopLogProbabilityInfo.LogProbability, Is.EqualTo(logProbability));
        Assert.That(chatTokenTopLogProbabilityInfo.Utf8ByteValues, Is.Not.Null.And.Empty);
    }

    [Test]
    public void ChatTokenTopLogProbabilityInfoWithUtf8ByteValuesWorks()
    {
        IEnumerable<int> utf8ByteValues = [104, 101, 108, 108, 111];
        ChatTokenTopLogProbabilityInfo chatTokenTopLogProbabilityInfo = OpenAIChatModelFactory.ChatTokenTopLogProbabilityInfo(utf8ByteValues: utf8ByteValues);

        Assert.That(chatTokenTopLogProbabilityInfo.Token, Is.Null);
        Assert.That(chatTokenTopLogProbabilityInfo.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenTopLogProbabilityInfo.Utf8ByteValues.SequenceEqual(utf8ByteValues), Is.True);
    }

    [Test]
    public void ChatTokenUsageWithNoPropertiesWorks()
    {
        ChatTokenUsage chatTokenUsage = OpenAIChatModelFactory.ChatTokenUsage();

        Assert.That(chatTokenUsage.OutputTokens, Is.EqualTo(0));
        Assert.That(chatTokenUsage.InputTokens, Is.EqualTo(0));
        Assert.That(chatTokenUsage.TotalTokens, Is.EqualTo(0));
    }

    [Test]
    public void ChatTokenUsageWithOutputTokensWorks()
    {
        int outputTokens = 271828;
        ChatTokenUsage chatTokenUsage = OpenAIChatModelFactory.ChatTokenUsage(outputTokens: outputTokens);

        Assert.That(chatTokenUsage.OutputTokens, Is.EqualTo(outputTokens));
        Assert.That(chatTokenUsage.InputTokens, Is.EqualTo(0));
        Assert.That(chatTokenUsage.TotalTokens, Is.EqualTo(0));
    }

    [Test]
    public void ChatTokenUsageWithInputTokensWorks()
    {
        int inputTokens = 271828;
        ChatTokenUsage chatTokenUsage = OpenAIChatModelFactory.ChatTokenUsage(inputTokens: inputTokens);

        Assert.That(chatTokenUsage.OutputTokens, Is.EqualTo(0));
        Assert.That(chatTokenUsage.InputTokens, Is.EqualTo(inputTokens));
        Assert.That(chatTokenUsage.TotalTokens, Is.EqualTo(0));
    }

    [Test]
    public void ChatTokenUsageWithTotalTokensWorks()
    {
        int totalTokens = 271828;
        ChatTokenUsage chatTokenUsage = OpenAIChatModelFactory.ChatTokenUsage(totalTokens: totalTokens);

        Assert.That(chatTokenUsage.OutputTokens, Is.EqualTo(0));
        Assert.That(chatTokenUsage.InputTokens, Is.EqualTo(0));
        Assert.That(chatTokenUsage.TotalTokens, Is.EqualTo(totalTokens));
    }

    [Test]
    public void StreamingChatCompletionUpdateWithNoPropertiesWorks()
    {
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate();

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithIdWorks()
    {
        string id = "chat_completion_id";
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(id: id);

        Assert.That(streamingChatCompletionUpdate.Id, Is.EqualTo(id));
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithContentUpdateWorks()
    {
        IEnumerable<ChatMessageContentPart> contentUpdate = [
            ChatMessageContentPart.CreateTextMessageContentPart("first part"),
            ChatMessageContentPart.CreateTextMessageContentPart("second part")
        ];
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(contentUpdate: contentUpdate);

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate.SequenceEqual(contentUpdate), Is.True);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithFunctionCallUpdateWorks()
    {
        StreamingChatFunctionCallUpdate functionCallUpdate = OpenAIChatModelFactory.StreamingChatFunctionCallUpdate(functionName: "get_recipte");
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(functionCallUpdate: functionCallUpdate);

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.EqualTo(functionCallUpdate));
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithToolCallUpdatesWorks()
    {
        IEnumerable<StreamingChatToolCallUpdate> toolCallUpdates = [
            OpenAIChatModelFactory.StreamingChatToolCallUpdate(id: "id1"),
            OpenAIChatModelFactory.StreamingChatToolCallUpdate(id: "id2")
        ];
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(toolCallUpdates: toolCallUpdates);

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates.SequenceEqual(toolCallUpdates), Is.True);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithRoleWorks()
    {
        ChatMessageRole role = ChatMessageRole.Tool;
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(role: role);

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.EqualTo(role));
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithRefusalUpdateWorks()
    {
        string refusalUpdate = "This is a refusal update.";
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(refusalUpdate: refusalUpdate);

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.EqualTo(refusalUpdate));
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithContentTokenLogProbabilitiesWorks()
    {
        IEnumerable<ChatTokenLogProbabilityInfo> contentTokenLogProbabilities = [
            OpenAIChatModelFactory.ChatTokenLogProbabilityInfo(logProbability: 1f),
            OpenAIChatModelFactory.ChatTokenLogProbabilityInfo(logProbability: 2f)
        ];
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(contentTokenLogProbabilities: contentTokenLogProbabilities);

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities.SequenceEqual(contentTokenLogProbabilities), Is.True);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithRefusalTokenLogProbabilitiesWorks()
    {
        IEnumerable<ChatTokenLogProbabilityInfo> refusalTokenLogProbabilities = [
            OpenAIChatModelFactory.ChatTokenLogProbabilityInfo(logProbability: 1f),
            OpenAIChatModelFactory.ChatTokenLogProbabilityInfo(logProbability: 2f)
        ];
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(refusalTokenLogProbabilities: refusalTokenLogProbabilities);

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities.SequenceEqual(refusalTokenLogProbabilities), Is.True);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithFinishReasonWorks()
    {
        ChatFinishReason finishReason = ChatFinishReason.ToolCalls;
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(finishReason: finishReason);

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.EqualTo(finishReason));
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithCreatedAtWorks()
    {
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(createdAt: createdAt);

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(createdAt));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithModelWorks()
    {
        string model = "topmodel";
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(model: model);

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.EqualTo(model));
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithSystemFingerprintWorks()
    {
        string systemFingerprint = "footprint";
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(systemFingerprint: systemFingerprint);

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.EqualTo(systemFingerprint));
        Assert.That(streamingChatCompletionUpdate.Usage, Is.Null);
    }

    [Test]
    public void StreamingChatCompletionUpdateWithUsageWorks()
    {
        ChatTokenUsage usage = OpenAIChatModelFactory.ChatTokenUsage(outputTokens: 20);
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(usage: usage);

        Assert.That(streamingChatCompletionUpdate.Id, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ToolCallUpdates, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.Role, Is.Null);
        Assert.That(streamingChatCompletionUpdate.RefusalUpdate, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(streamingChatCompletionUpdate.FinishReason, Is.Null);
        Assert.That(streamingChatCompletionUpdate.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(streamingChatCompletionUpdate.Model, Is.Null);
        Assert.That(streamingChatCompletionUpdate.SystemFingerprint, Is.Null);
        Assert.That(streamingChatCompletionUpdate.Usage, Is.EqualTo(usage));
    }

    [Test]
    public void StreamingChatFunctionCallUpdateWithNoPropertiesWorks()
    {
        StreamingChatFunctionCallUpdate streamingChatFunctionCallUpdate = OpenAIChatModelFactory.StreamingChatFunctionCallUpdate();

        Assert.That(streamingChatFunctionCallUpdate.FunctionName, Is.Null);
        Assert.That(streamingChatFunctionCallUpdate.FunctionArgumentsUpdate, Is.Null);
    }

    [Test]
    public void StreamingChatFunctionCallUpdateWithFunctionNameWorks()
    {
        string functionName = "Margaret";
        StreamingChatFunctionCallUpdate streamingChatFunctionCallUpdate = OpenAIChatModelFactory.StreamingChatFunctionCallUpdate(functionName: functionName);

        Assert.That(streamingChatFunctionCallUpdate.FunctionName, Is.EqualTo(functionName));
        Assert.That(streamingChatFunctionCallUpdate.FunctionArgumentsUpdate, Is.Null);
    }

    [Test]
    public void StreamingChatFunctionCallUpdateWithFunctionArgumentsUpdateWorks()
    {
        string functionArgumentsUpdate = "arguments_update";
        StreamingChatFunctionCallUpdate streamingChatFunctionCallUpdate = OpenAIChatModelFactory.StreamingChatFunctionCallUpdate(functionArgumentsUpdate: functionArgumentsUpdate);

        Assert.That(streamingChatFunctionCallUpdate.FunctionName, Is.Null);
        Assert.That(streamingChatFunctionCallUpdate.FunctionArgumentsUpdate, Is.EqualTo(functionArgumentsUpdate));
    }

    [Test]
    public void StreamingChatToolCallUpdateWithNoPropertiesWorks()
    {
        StreamingChatToolCallUpdate streamingChatToolCallUpdate = OpenAIChatModelFactory.StreamingChatToolCallUpdate();

        Assert.That(streamingChatToolCallUpdate.Index, Is.EqualTo(0));
        Assert.That(streamingChatToolCallUpdate.Id, Is.Null);
        Assert.That(streamingChatToolCallUpdate.Kind, Is.EqualTo(default(ChatToolCallKind)));
        Assert.That(streamingChatToolCallUpdate.FunctionName, Is.Null);
        Assert.That(streamingChatToolCallUpdate.FunctionArgumentsUpdate, Is.Null);
    }

    [Test]
    public void StreamingChatToolCallUpdateWithIndexWorks()
    {
        int index = 31415;
        StreamingChatToolCallUpdate streamingChatToolCallUpdate = OpenAIChatModelFactory.StreamingChatToolCallUpdate(index: index);

        Assert.That(streamingChatToolCallUpdate.Index, Is.EqualTo(index));
        Assert.That(streamingChatToolCallUpdate.Id, Is.Null);
        Assert.That(streamingChatToolCallUpdate.Kind, Is.EqualTo(default(ChatToolCallKind)));
        Assert.That(streamingChatToolCallUpdate.FunctionName, Is.Null);
        Assert.That(streamingChatToolCallUpdate.FunctionArgumentsUpdate, Is.Null);
    }

    [Test]
    public void StreamingChatToolCallUpdateWithIdWorks()
    {
        string id = "tool_call_id";
        StreamingChatToolCallUpdate streamingChatToolCallUpdate = OpenAIChatModelFactory.StreamingChatToolCallUpdate(id: id);

        Assert.That(streamingChatToolCallUpdate.Index, Is.EqualTo(0));
        Assert.That(streamingChatToolCallUpdate.Id, Is.EqualTo(id));
        Assert.That(streamingChatToolCallUpdate.Kind, Is.EqualTo(default(ChatToolCallKind)));
        Assert.That(streamingChatToolCallUpdate.FunctionName, Is.Null);
        Assert.That(streamingChatToolCallUpdate.FunctionArgumentsUpdate, Is.Null);
    }

    [Test]
    public void StreamingChatToolCallUpdateWithKindWorks()
    {
        ChatToolCallKind kind = ChatToolCallKind.Function;
        StreamingChatToolCallUpdate streamingChatToolCallUpdate = OpenAIChatModelFactory.StreamingChatToolCallUpdate(kind: kind);

        Assert.That(streamingChatToolCallUpdate.Index, Is.EqualTo(0));
        Assert.That(streamingChatToolCallUpdate.Id, Is.Null);
        Assert.That(streamingChatToolCallUpdate.Kind, Is.EqualTo(kind));
        Assert.That(streamingChatToolCallUpdate.FunctionName, Is.Null);
        Assert.That(streamingChatToolCallUpdate.FunctionArgumentsUpdate, Is.Null);
    }

    [Test]
    public void StreamingChatToolCallUpdateWithFunctionNameWorks()
    {
        string functionName = "Margaret";
        StreamingChatToolCallUpdate streamingChatToolCallUpdate = OpenAIChatModelFactory.StreamingChatToolCallUpdate(functionName: functionName);

        Assert.That(streamingChatToolCallUpdate.Index, Is.EqualTo(0));
        Assert.That(streamingChatToolCallUpdate.Id, Is.Null);
        Assert.That(streamingChatToolCallUpdate.Kind, Is.EqualTo(default(ChatToolCallKind)));
        Assert.That(streamingChatToolCallUpdate.FunctionName, Is.EqualTo(functionName));
        Assert.That(streamingChatToolCallUpdate.FunctionArgumentsUpdate, Is.Null);
    }

    [Test]
    public void StreamingChatToolCallUpdateWithFunctionArgumentsUpdateWorks()
    {
        string functionArgumentsUpdate = "arguments_update";
        StreamingChatToolCallUpdate streamingChatToolCallUpdate = OpenAIChatModelFactory.StreamingChatToolCallUpdate(functionArgumentsUpdate: functionArgumentsUpdate);

        Assert.That(streamingChatToolCallUpdate.Index, Is.EqualTo(0));
        Assert.That(streamingChatToolCallUpdate.Id, Is.Null);
        Assert.That(streamingChatToolCallUpdate.Kind, Is.EqualTo(default(ChatToolCallKind)));
        Assert.That(streamingChatToolCallUpdate.FunctionName, Is.Null);
        Assert.That(streamingChatToolCallUpdate.FunctionArgumentsUpdate, Is.EqualTo(functionArgumentsUpdate));
    }
}
