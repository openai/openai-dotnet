using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenAI.Chat;

namespace OpenAI.Tests.Chat;

[Parallelizable(ParallelScope.All)]
[Category("Chat")]
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
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
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
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
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
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
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
        ChatMessageContent content = [
            ChatMessageContentPart.CreateTextPart("first part"),
            ChatMessageContentPart.CreateTextPart("second part")
        ];
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(content: content);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content.SequenceEqual(content), Is.True);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
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
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
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
            ChatToolCall.CreateFunctionToolCall("id1", "get_recipe", BinaryData.FromBytes("{}"u8.ToArray())),
            ChatToolCall.CreateFunctionToolCall("id2", "get_location", BinaryData.FromBytes("{}"u8.ToArray()))
        ];
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(toolCalls: toolCalls);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls.SequenceEqual(toolCalls), Is.True);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
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
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
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
#pragma warning disable CS0618
        ChatFunctionCall functionCall = new ChatFunctionCall("get_recipe", BinaryData.FromBytes("{}"u8.ToArray()));
#pragma warning restore CS0618
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(functionCall: functionCall);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.EqualTo(functionCall));
#pragma warning restore CS0618
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
        IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = [
            OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(logProbability: 1f),
            OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(logProbability: 2f)
        ];
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(contentTokenLogProbabilities: contentTokenLogProbabilities);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
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
        IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = [
            OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(logProbability: 1f),
            OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(logProbability: 2f)
        ];
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(refusalTokenLogProbabilities: refusalTokenLogProbabilities);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
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
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
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
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
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
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
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
        ChatTokenUsage usage = OpenAIChatModelFactory.ChatTokenUsage(outputTokenCount: 20);
        ChatCompletion chatCompletion = OpenAIChatModelFactory.ChatCompletion(usage: usage);

        Assert.That(chatCompletion.Id, Is.Null);
        Assert.That(chatCompletion.FinishReason, Is.EqualTo(default(ChatFinishReason)));
        Assert.That(chatCompletion.Content, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Refusal, Is.Null);
        Assert.That(chatCompletion.ToolCalls, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.Role, Is.EqualTo(default(ChatMessageRole)));
#pragma warning disable CS0618
        Assert.That(chatCompletion.FunctionCall, Is.Null);
#pragma warning restore CS0618
        Assert.That(chatCompletion.ContentTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.RefusalTokenLogProbabilities, Is.Not.Null.And.Empty);
        Assert.That(chatCompletion.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(chatCompletion.Model, Is.Null);
        Assert.That(chatCompletion.SystemFingerprint, Is.Null);
        Assert.That(chatCompletion.Usage, Is.EqualTo(usage));
    }

    [Test]
    public void ChatTokenLogProbabilityDetailsWithNoPropertiesWorks()
    {
        ChatTokenLogProbabilityDetails chatTokenLogProbabilityDetails = OpenAIChatModelFactory.ChatTokenLogProbabilityDetails();

        Assert.That(chatTokenLogProbabilityDetails.Token, Is.Null);
        Assert.That(chatTokenLogProbabilityDetails.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenLogProbabilityDetails.Utf8Bytes, Is.Null);
        Assert.That(chatTokenLogProbabilityDetails.TopLogProbabilities, Is.Not.Null.And.Empty);
    }

    [Test]
    public void ChatTokenLogProbabilityDetailsWithTokenWorks()
    {
        string token = "a_token_of_appreciation";
        ChatTokenLogProbabilityDetails chatTokenLogProbabilityDetails = OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(token: token);

        Assert.That(chatTokenLogProbabilityDetails.Token, Is.EqualTo(token));
        Assert.That(chatTokenLogProbabilityDetails.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenLogProbabilityDetails.Utf8Bytes, Is.Null);
        Assert.That(chatTokenLogProbabilityDetails.TopLogProbabilities, Is.Not.Null.And.Empty);
    }

    [Test]
    public void ChatTokenLogProbabilityDetailsWithLogProbabilityWorks()
    {
        float logProbability = 3.14f;
        ChatTokenLogProbabilityDetails chatTokenLogProbabilityDetails = OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(logProbability: logProbability);

        Assert.That(chatTokenLogProbabilityDetails.Token, Is.Null);
        Assert.That(chatTokenLogProbabilityDetails.LogProbability, Is.EqualTo(logProbability));
        Assert.That(chatTokenLogProbabilityDetails.Utf8Bytes, Is.Null);
        Assert.That(chatTokenLogProbabilityDetails.TopLogProbabilities, Is.Not.Null.And.Empty);
    }

    [Test]
    public void ChatTokenLogProbabilityDetailsWithUtf8ByteValuesWorks()
    {
        ReadOnlyMemory<byte> utf8Bytes = "hello"u8.ToArray();
        ChatTokenLogProbabilityDetails chatTokenLogProbabilityDetails = OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(utf8Bytes: utf8Bytes);

        Assert.That(chatTokenLogProbabilityDetails.Token, Is.Null);
        Assert.That(chatTokenLogProbabilityDetails.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenLogProbabilityDetails.Utf8Bytes.Value.ToArray().SequenceEqual(utf8Bytes.ToArray()), Is.True);
        Assert.That(chatTokenLogProbabilityDetails.TopLogProbabilities, Is.Not.Null.And.Empty);
    }

    [Test]
    public void ChatTokenLogProbabilityDetailsWithTopLogProbabilitiesWorks()
    {
        IEnumerable<ChatTokenTopLogProbabilityDetails> topLogProbabilities = [
            OpenAIChatModelFactory.ChatTokenTopLogProbabilityDetails(token: "firstToken"),
            OpenAIChatModelFactory.ChatTokenTopLogProbabilityDetails(token: "secondToken")
        ];
        ChatTokenLogProbabilityDetails chatTokenLogProbabilityDetails = OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(topLogProbabilities: topLogProbabilities);

        Assert.That(chatTokenLogProbabilityDetails.Token, Is.Null);
        Assert.That(chatTokenLogProbabilityDetails.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenLogProbabilityDetails.Utf8Bytes, Is.Null);
        Assert.That(chatTokenLogProbabilityDetails.TopLogProbabilities.SequenceEqual(topLogProbabilities), Is.True);
    }

    [Test]
    public void ChatTokenTopLogProbabilityDetailsWithNoPropertiesWorks()
    {
        ChatTokenTopLogProbabilityDetails chatTokenTopLogProbabilityDetails = OpenAIChatModelFactory.ChatTokenTopLogProbabilityDetails();

        Assert.That(chatTokenTopLogProbabilityDetails.Token, Is.Null);
        Assert.That(chatTokenTopLogProbabilityDetails.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenTopLogProbabilityDetails.Utf8Bytes, Is.Null);
    }

    [Test]
    public void ChatTokenTopLogProbabilityDetailsWithTokenWorks()
    {
        string token = "a_token_of_appreciation";
        ChatTokenTopLogProbabilityDetails chatTokenTopLogProbabilityDetails = OpenAIChatModelFactory.ChatTokenTopLogProbabilityDetails(token: token);

        Assert.That(chatTokenTopLogProbabilityDetails.Token, Is.EqualTo(token));
        Assert.That(chatTokenTopLogProbabilityDetails.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenTopLogProbabilityDetails.Utf8Bytes, Is.Null);
    }

    [Test]
    public void ChatTokenTopLogProbabilityDetailsWithLogProbabilityWorks()
    {
        float logProbability = 3.14f;
        ChatTokenTopLogProbabilityDetails chatTokenTopLogProbabilityDetails = OpenAIChatModelFactory.ChatTokenTopLogProbabilityDetails(logProbability: logProbability);

        Assert.That(chatTokenTopLogProbabilityDetails.Token, Is.Null);
        Assert.That(chatTokenTopLogProbabilityDetails.LogProbability, Is.EqualTo(logProbability));
        Assert.That(chatTokenTopLogProbabilityDetails.Utf8Bytes, Is.Null);
    }

    [Test]
    public void ChatTokenTopLogProbabilityDetailsWithUtf8ByteValuesWorks()
    {
        ReadOnlyMemory<byte> utf8Bytes = "hello"u8.ToArray();
        ChatTokenTopLogProbabilityDetails chatTokenTopLogProbabilityDetails = OpenAIChatModelFactory.ChatTokenTopLogProbabilityDetails(utf8Bytes: utf8Bytes);

        Assert.That(chatTokenTopLogProbabilityDetails.Token, Is.Null);
        Assert.That(chatTokenTopLogProbabilityDetails.LogProbability, Is.EqualTo(0f));
        Assert.That(chatTokenTopLogProbabilityDetails.Utf8Bytes.Value.ToArray().SequenceEqual(utf8Bytes.ToArray()), Is.True);
    }

    [Test]
    public void ChatTokenUsageWithNoPropertiesWorks()
    {
        ChatTokenUsage chatTokenUsage = OpenAIChatModelFactory.ChatTokenUsage();

        Assert.That(chatTokenUsage.OutputTokenCount, Is.EqualTo(0));
        Assert.That(chatTokenUsage.InputTokenCount, Is.EqualTo(0));
        Assert.That(chatTokenUsage.TotalTokenCount, Is.EqualTo(0));
    }

    [Test]
    public void ChatTokenUsageWithOutputTokensWorks()
    {
        int outputTokens = 271828;
        ChatTokenUsage chatTokenUsage = OpenAIChatModelFactory.ChatTokenUsage(outputTokenCount: outputTokens);

        Assert.That(chatTokenUsage.OutputTokenCount, Is.EqualTo(outputTokens));
        Assert.That(chatTokenUsage.InputTokenCount, Is.EqualTo(0));
        Assert.That(chatTokenUsage.TotalTokenCount, Is.EqualTo(0));
    }

    [Test]
    public void ChatTokenUsageWithInputTokensWorks()
    {
        int inputTokens = 271828;
        ChatTokenUsage chatTokenUsage = OpenAIChatModelFactory.ChatTokenUsage(inputTokenCount: inputTokens);

        Assert.That(chatTokenUsage.OutputTokenCount, Is.EqualTo(0));
        Assert.That(chatTokenUsage.InputTokenCount, Is.EqualTo(inputTokens));
        Assert.That(chatTokenUsage.TotalTokenCount, Is.EqualTo(0));
    }

    [Test]
    public void ChatTokenUsageWithTotalTokensWorks()
    {
        int totalTokens = 271828;
        ChatTokenUsage chatTokenUsage = OpenAIChatModelFactory.ChatTokenUsage(totalTokenCount: totalTokens);

        Assert.That(chatTokenUsage.OutputTokenCount, Is.EqualTo(0));
        Assert.That(chatTokenUsage.InputTokenCount, Is.EqualTo(0));
        Assert.That(chatTokenUsage.TotalTokenCount, Is.EqualTo(totalTokens));
    }

    [Test]
    public void StreamingChatCompletionUpdateWithNoPropertiesWorks()
    {
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate();

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(completionId: id);

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.EqualTo(id));
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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
        ChatMessageContent contentUpdate = [
            ChatMessageContentPart.CreateTextPart("first part"),
            ChatMessageContentPart.CreateTextPart("second part")
        ];
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(contentUpdate: contentUpdate);

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate.SequenceEqual(contentUpdate), Is.True);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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
#pragma warning disable CS0618
        StreamingChatFunctionCallUpdate functionCallUpdate = OpenAIChatModelFactory.StreamingChatFunctionCallUpdate(functionName: "get_recipte");
#pragma warning restore CS0618
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(functionCallUpdate: functionCallUpdate);

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.EqualTo(functionCallUpdate));
#pragma warning restore CS0618
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
            OpenAIChatModelFactory.StreamingChatToolCallUpdate(toolCallId: "id1"),
            OpenAIChatModelFactory.StreamingChatToolCallUpdate(toolCallId: "id2")
        ];
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(toolCallUpdates: toolCallUpdates);

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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
        IEnumerable<ChatTokenLogProbabilityDetails> contentTokenLogProbabilities = [
            OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(logProbability: 1f),
            OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(logProbability: 2f)
        ];
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(contentTokenLogProbabilities: contentTokenLogProbabilities);

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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
        IEnumerable<ChatTokenLogProbabilityDetails> refusalTokenLogProbabilities = [
            OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(logProbability: 1f),
            OpenAIChatModelFactory.ChatTokenLogProbabilityDetails(logProbability: 2f)
        ];
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(refusalTokenLogProbabilities: refusalTokenLogProbabilities);

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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
        ChatTokenUsage usage = OpenAIChatModelFactory.ChatTokenUsage(outputTokenCount: 20);
        StreamingChatCompletionUpdate streamingChatCompletionUpdate = OpenAIChatModelFactory.StreamingChatCompletionUpdate(usage: usage);

        Assert.That(streamingChatCompletionUpdate.CompletionId, Is.Null);
        Assert.That(streamingChatCompletionUpdate.ContentUpdate, Is.Not.Null.And.Empty);
#pragma warning disable CS0618
        Assert.That(streamingChatCompletionUpdate.FunctionCallUpdate, Is.Null);
#pragma warning restore CS0618
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
#pragma warning disable CS0618
        StreamingChatFunctionCallUpdate streamingChatFunctionCallUpdate = OpenAIChatModelFactory.StreamingChatFunctionCallUpdate();
#pragma warning restore CS0618

        Assert.That(streamingChatFunctionCallUpdate.FunctionName, Is.Null);
        Assert.That(streamingChatFunctionCallUpdate.FunctionArgumentsUpdate, Is.Null);
    }

    [Test]
    public void StreamingChatFunctionCallUpdateWithFunctionNameWorks()
    {
        string functionName = "Margaret";
#pragma warning disable CS0618
        StreamingChatFunctionCallUpdate streamingChatFunctionCallUpdate = OpenAIChatModelFactory.StreamingChatFunctionCallUpdate(functionName: functionName);
#pragma warning restore CS0618

        Assert.That(streamingChatFunctionCallUpdate.FunctionName, Is.EqualTo(functionName));
        Assert.That(streamingChatFunctionCallUpdate.FunctionArgumentsUpdate, Is.Null);
    }

    [Test]
    public void StreamingChatFunctionCallUpdateWithFunctionArgumentsUpdateWorks()
    {
        BinaryData functionArgumentsUpdate = BinaryData.FromBytes("arguments_update"u8.ToArray());
#pragma warning disable CS0618
        StreamingChatFunctionCallUpdate streamingChatFunctionCallUpdate = OpenAIChatModelFactory.StreamingChatFunctionCallUpdate(functionArgumentsUpdate: functionArgumentsUpdate);
#pragma warning restore CS0618

        Assert.That(streamingChatFunctionCallUpdate.FunctionName, Is.Null);
        Assert.That(streamingChatFunctionCallUpdate.FunctionArgumentsUpdate, Is.EqualTo(functionArgumentsUpdate));
    }

    [Test]
    public void StreamingChatToolCallUpdateWithNoPropertiesWorks()
    {
        StreamingChatToolCallUpdate streamingChatToolCallUpdate = OpenAIChatModelFactory.StreamingChatToolCallUpdate();

        Assert.That(streamingChatToolCallUpdate.Index, Is.EqualTo(0));
        Assert.That(streamingChatToolCallUpdate.ToolCallId, Is.Null);
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
        Assert.That(streamingChatToolCallUpdate.ToolCallId, Is.Null);
        Assert.That(streamingChatToolCallUpdate.Kind, Is.EqualTo(default(ChatToolCallKind)));
        Assert.That(streamingChatToolCallUpdate.FunctionName, Is.Null);
        Assert.That(streamingChatToolCallUpdate.FunctionArgumentsUpdate, Is.Null);
    }

    [Test]
    public void StreamingChatToolCallUpdateWithIdWorks()
    {
        string id = "tool_call_id";
        StreamingChatToolCallUpdate streamingChatToolCallUpdate = OpenAIChatModelFactory.StreamingChatToolCallUpdate(toolCallId: id);

        Assert.That(streamingChatToolCallUpdate.Index, Is.EqualTo(0));
        Assert.That(streamingChatToolCallUpdate.ToolCallId, Is.EqualTo(id));
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
        Assert.That(streamingChatToolCallUpdate.ToolCallId, Is.Null);
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
        Assert.That(streamingChatToolCallUpdate.ToolCallId, Is.Null);
        Assert.That(streamingChatToolCallUpdate.Kind, Is.EqualTo(default(ChatToolCallKind)));
        Assert.That(streamingChatToolCallUpdate.FunctionName, Is.EqualTo(functionName));
        Assert.That(streamingChatToolCallUpdate.FunctionArgumentsUpdate, Is.Null);
    }

    [Test]
    public void StreamingChatToolCallUpdateWithFunctionArgumentsUpdateWorks()
    {
        BinaryData functionArgumentsUpdate = BinaryData.FromBytes("arguments_update"u8.ToArray());
        StreamingChatToolCallUpdate streamingChatToolCallUpdate = OpenAIChatModelFactory.StreamingChatToolCallUpdate(functionArgumentsUpdate: functionArgumentsUpdate);

        Assert.That(streamingChatToolCallUpdate.Index, Is.EqualTo(0));
        Assert.That(streamingChatToolCallUpdate.ToolCallId, Is.Null);
        Assert.That(streamingChatToolCallUpdate.Kind, Is.EqualTo(default(ChatToolCallKind)));
        Assert.That(streamingChatToolCallUpdate.FunctionName, Is.Null);
        Assert.That(streamingChatToolCallUpdate.FunctionArgumentsUpdate, Is.EqualTo(functionArgumentsUpdate));
    }
}
