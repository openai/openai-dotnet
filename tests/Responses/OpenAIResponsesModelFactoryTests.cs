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
            summary: summaryParts);

        Assert.That(reasoningItem.Id, Is.EqualTo(id));
        Assert.That(reasoningItem.EncryptedContent, Is.EqualTo(encryptedContent));
        Assert.That(reasoningItem.Status, Is.EqualTo(status));
        Assert.That(reasoningItem.Summary.SequenceEqual(summaryParts), Is.True);
    }

    [Test]
    public void ReferenceResponseItemWorks()
    {
        string id = "reference_123";

        ReferenceResponseItem referenceItem = OpenAIResponsesModelFactory.ReferenceResponseItem(id: id);

        Assert.That(referenceItem.Id, Is.EqualTo(id));
    }
}
