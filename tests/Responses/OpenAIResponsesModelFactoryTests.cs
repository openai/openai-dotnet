using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenAI.Responses;

namespace OpenAI.Tests.Responses;

[Parallelizable(ParallelScope.All)]
[Category("Responses")]
[Category("Smoke")]
public partial class OpenAIResponsesModelFactoryTests
{
    // [Test]
    // public void OpenAIResponseWorks()
    // {
    //     string id = "response_123";
    //     DateTimeOffset createdAt = DateTimeOffset.UtcNow;
    //     ResponseStatus status = ResponseStatus.Completed;
    //     string model = "gpt-4o";
    //     IEnumerable<ResponseItem> outputItems = [
    //         OpenAIResponsesModelFactory.MessageResponseItem(id: "msg_1", role: MessageRole.User, status: MessageStatus.Completed),
    //         OpenAIResponsesModelFactory.ReasoningResponseItem(id: "reason_1", encryptedContent: "encrypted", status: ReasoningStatus.InProgress, summaryText: "summary")
    //     ];

    //     OpenAIResponse response = OpenAIResponsesModelFactory.OpenAIResponse(
    //         id: id,
    //         createdAt: createdAt,
    //         status: status,
    //         model: model,
    //         outputItems: outputItems);

    //     Assert.That(response.Id, Is.EqualTo(id));
    //     Assert.That(response.CreatedAt, Is.EqualTo(createdAt));
    //     Assert.That(response.Status, Is.EqualTo(status));
    //     Assert.That(response.Model, Is.EqualTo(model));
    //     Assert.That(response.OutputItems.SequenceEqual(outputItems), Is.True);
    // }

    [Test]
    public void MessageResponseItemWorks()
    {
        string id = "message_123";
        MessageRole role = MessageRole.Developer;
        MessageStatus status = MessageStatus.InProgress;

        MessageResponseItem messageItem = OpenAIResponsesModelFactory.MessageResponseItem(
            id: id,
            role: role,
            status: status);

        Assert.That(messageItem.Id, Is.EqualTo(id));
        Assert.That(messageItem.Role, Is.EqualTo(role));
        Assert.That(messageItem.Status, Is.EqualTo(status));
    }

    [Test]
    public void ReasoningResponseItemWorks()
    {
        string id = "reasoning_123";
        string encryptedContent = "encrypted_reasoning_data";
        ReasoningStatus status = ReasoningStatus.Completed;
        var summaryParts = new List<ReasoningSummaryPart> { new ReasoningSummaryTextPart("test summary") };

        ReasoningResponseItem reasoningItem = OpenAIResponsesModelFactory.ReasoningResponseItem(
            id: id,
            encryptedContent: encryptedContent,
            status: status,
            summaryParts: summaryParts);

        Assert.That(reasoningItem.Id, Is.EqualTo(id));
        Assert.That(reasoningItem.EncryptedContent, Is.EqualTo(encryptedContent));
        Assert.That(reasoningItem.Status, Is.EqualTo(status));
        Assert.That(reasoningItem.SummaryParts.SequenceEqual(summaryParts), Is.True);
    }

    [Test]
    public void ReasoningResponseItemWithSummaryTextWorks()
    {
        string id = "reasoning_456";
        string encryptedContent = "encrypted_data";
        ReasoningStatus status = ReasoningStatus.InProgress;
        string summaryText = "This is a reasoning summary";

        ReasoningResponseItem reasoningItem = OpenAIResponsesModelFactory.ReasoningResponseItem(
            id: id,
            encryptedContent: encryptedContent,
            status: status,
            summaryText: summaryText);

        Assert.That(reasoningItem.Id, Is.EqualTo(id));
        Assert.That(reasoningItem.EncryptedContent, Is.EqualTo(encryptedContent));
        Assert.That(reasoningItem.Status, Is.EqualTo(status));
        Assert.That(reasoningItem.GetSummaryText(), Is.EqualTo(summaryText));
    }

    [Test]
    public void ReferenceResponseItemWorks()
    {
        string id = "reference_123";

        ReferenceResponseItem referenceItem = OpenAIResponsesModelFactory.ReferenceResponseItem(id: id);

        Assert.That(referenceItem.Id, Is.EqualTo(id));
    }
}
