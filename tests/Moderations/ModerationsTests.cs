using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Moderations;
using OpenAI.Tests.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Moderations;

[Category("Moderations")]
public class ModerationsTests : OpenAIRecordedTestBase
{
    public ModerationsTests(bool isAsync) : base(isAsync)
    {
    }

    [RecordedTest]
    public async Task ClassifyTextWithSingleString()
    {
        ModerationClient client = GetProxiedOpenAIClient<ModerationClient>(TestScenario.Moderations);

        const string input = "I am killing all my houseplants!";

        ModerationResult moderation = await client.ClassifyTextAsync(input);
        Assert.That(moderation, Is.Not.Null);
        Assert.That(moderation.Flagged, Is.True);
        Assert.That(moderation.Violence.Flagged, Is.True);
        Assert.That(moderation.Violence.Score, Is.GreaterThan(0.2));
    }

    [RecordedTest]
    public async Task ClassifyTextWithMultipleStrings()
    {
        ModerationClient client = GetProxiedOpenAIClient<ModerationClient>(TestScenario.Moderations);

        List<string> inputs =
            [
                "I forgot to water my houseplants!",
                "I am killing all my houseplants!"
            ];

        ModerationResultCollection moderations = await client.ClassifyTextAsync(inputs);
        Assert.That(moderations, Is.Not.Null);
        Assert.That(moderations.Count, Is.EqualTo(2));
        Assert.That(moderations.Model, Does.StartWith("omni"));
        Assert.That(moderations.Id, Is.Not.Null.Or.Empty);

        Assert.That(moderations[0], Is.Not.Null);
        Assert.That(moderations[0].Flagged, Is.False);

        Assert.That(moderations[1], Is.Not.Null);
        Assert.That(moderations[1].Flagged, Is.True);
        Assert.That(moderations[1].Violence.Flagged, Is.True);
        Assert.That(moderations[1].Violence.Score, Is.GreaterThan(0.2));
    }

    [RecordedTest]
    public async Task ClassifyInputsWithSingleImage()
    {
        ModerationClient client = GetProxiedOpenAIClient<ModerationClient>(TestScenario.Moderations);

        List<ModerationInputPart> inputs =
            [
                ModerationInputPart.CreateImagePart(new Uri("https://avatars.githubusercontent.com/u/14957082"))
            ];

        ModerationResult moderation = await client.ClassifyInputsAsync(inputs);
        Assert.That(moderation, Is.Not.Null);
        Assert.That(moderation.Flagged, Is.False);
        Assert.That(moderation.Violence.Flagged, Is.False);
        Assert.That(moderation.Violence.Score, Is.LessThan(0.2));
        Assert.That(moderation.Violence.ApplicableInputKinds, Is.EqualTo(ModerationApplicableInputKinds.Image));
    }

    [RecordedTest]
    public async Task ClassifyInputsWithMultipleTexts()
    {
        ModerationClient client = GetProxiedOpenAIClient<ModerationClient>(TestScenario.Moderations);

        List<ModerationInputPart> inputs =
            [
                ModerationInputPart.CreateTextPart("I forgot to water my houseplants!"),
                ModerationInputPart.CreateTextPart("I am killing all my houseplants!")
            ];

        ModerationResult moderation = await client.ClassifyInputsAsync(inputs);
        Assert.That(moderation, Is.Not.Null);
        Assert.That(moderation.Flagged, Is.True);
        Assert.That(moderation.Violence.Flagged, Is.True);
        Assert.That(moderation.Violence.Score, Is.GreaterThan(0.2));
        Assert.That(moderation.Violence.ApplicableInputKinds, Is.EqualTo(ModerationApplicableInputKinds.Text));
    }

    [RecordedTest]
    public async Task ClassifyInputsWithTextAndImage()
    {
        ModerationClient client = GetProxiedOpenAIClient<ModerationClient>(TestScenario.Moderations);

        List<ModerationInputPart> inputs =
            [
                ModerationInputPart.CreateTextPart("I forgot to water my houseplants!"),
                ModerationInputPart.CreateImagePart(new Uri("https://avatars.githubusercontent.com/u/14957082"))
            ];

        ModerationResult moderation = await client.ClassifyInputsAsync(inputs);
        Assert.That(moderation, Is.Not.Null);
        Assert.That(moderation.Flagged, Is.False);
        Assert.That(moderation.Violence.Flagged, Is.False);
        Assert.That(moderation.Violence.Score, Is.LessThan(0.2));
        Assert.That(moderation.Violence.ApplicableInputKinds, Is.EqualTo(ModerationApplicableInputKinds.Text | ModerationApplicableInputKinds.Image));
    }

    [RecordedTest]
    public async Task ClassifyInputsWithMultipleTextsAndSingleImage()
    {
        ModerationClient client = GetProxiedOpenAIClient<ModerationClient>(TestScenario.Moderations);

        List<ModerationInputPart> inputs =
            [
                ModerationInputPart.CreateTextPart("I forgot to water my houseplants!"),
                ModerationInputPart.CreateTextPart("I am killing all my houseplants!"),
                ModerationInputPart.CreateImagePart(new Uri("https://avatars.githubusercontent.com/u/14957082"))
            ];

        ModerationResult moderation = await client.ClassifyInputsAsync(inputs);
        Assert.That(moderation, Is.Not.Null);
        Assert.That(moderation.Flagged, Is.True);
        Assert.That(moderation.Violence.Flagged, Is.True);
        Assert.That(moderation.Violence.Score, Is.GreaterThan(0.2));
        Assert.That(moderation.Violence.ApplicableInputKinds, Is.EqualTo(ModerationApplicableInputKinds.Text | ModerationApplicableInputKinds.Image));
    }
}
