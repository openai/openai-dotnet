using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenAI.Moderations;

namespace OpenAI.Tests.Moderations;

[Parallelizable(ParallelScope.All)]
[Category("Moderations")]
[Category("Smoke")]
public partial class OpenAIModerationsModelFactoryTests
{
    [Test]
    public void ModerationCategoryWithNoPropertiesWorks()
    {
        ModerationCategory moderationCategory = OpenAIModerationsModelFactory.ModerationCategory();

        Assert.That(moderationCategory.Flagged, Is.False);
        Assert.That(moderationCategory.Score, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryWithFlagWorks()
    {
        ModerationCategory moderationCategory = OpenAIModerationsModelFactory.ModerationCategory(flagged: true);

        Assert.That(moderationCategory.Flagged, Is.True);
        Assert.That(moderationCategory.Score, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryWithScoreWorks()
    {
        ModerationCategory moderationCategory = OpenAIModerationsModelFactory.ModerationCategory(score: 0.85f);

        Assert.That(moderationCategory.Flagged, Is.False);
        Assert.That(moderationCategory.Score, Is.EqualTo(0.85f));
    }

    [Test]
    public void ModerationResultWithNoPropertiesWorks()
    {
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult();

        Assert.That(moderationResult.Flagged, Is.False);

        Assert.That(moderationResult.Hate, Is.Null);
        Assert.That(moderationResult.HateThreatening, Is.Null);
        Assert.That(moderationResult.Harassment, Is.Null);
        Assert.That(moderationResult.HarassmentThreatening, Is.Null);
        Assert.That(moderationResult.SelfHarm, Is.Null);
        Assert.That(moderationResult.SelfHarmIntent, Is.Null);
        Assert.That(moderationResult.SelfHarmInstructions, Is.Null);
        Assert.That(moderationResult.Sexual, Is.Null);
        Assert.That(moderationResult.SexualMinors, Is.Null);
        Assert.That(moderationResult.Violence, Is.Null);
        Assert.That(moderationResult.ViolenceGraphic, Is.Null);
    }

    [Test]
    public void ModerationResultWithHateWorks()
    {
        float hateScore = 0.85f;
        ModerationCategory category = OpenAIModerationsModelFactory.ModerationCategory(true, hateScore);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(hate: category);

        Assert.That(moderationResult.Hate.Flagged, Is.True);
        Assert.That(moderationResult.Hate.Score, Is.EqualTo(hateScore));

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.HateThreatening, Is.Null);
        Assert.That(moderationResult.Harassment, Is.Null);
        Assert.That(moderationResult.HarassmentThreatening, Is.Null);
        Assert.That(moderationResult.SelfHarm, Is.Null);
        Assert.That(moderationResult.SelfHarmIntent, Is.Null);
        Assert.That(moderationResult.SelfHarmInstructions, Is.Null);
        Assert.That(moderationResult.Sexual, Is.Null);
        Assert.That(moderationResult.SexualMinors, Is.Null);
        Assert.That(moderationResult.Violence, Is.Null);
        Assert.That(moderationResult.ViolenceGraphic, Is.Null);
    }

    [Test]
    public void ModerationResultWithHateThreateningWorks()
    {
        float hateThreateningScore = 0.85f;
        ModerationCategory category = OpenAIModerationsModelFactory.ModerationCategory(true, hateThreateningScore);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(hateThreatening: category);

        Assert.That(moderationResult.HateThreatening.Flagged, Is.True);
        Assert.That(moderationResult.HateThreatening.Score, Is.EqualTo(hateThreateningScore));

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Hate, Is.Null);
        Assert.That(moderationResult.Harassment, Is.Null);
        Assert.That(moderationResult.SelfHarm, Is.Null);
        Assert.That(moderationResult.SelfHarmIntent, Is.Null);
        Assert.That(moderationResult.SelfHarmInstructions, Is.Null);
        Assert.That(moderationResult.Sexual, Is.Null);
        Assert.That(moderationResult.SexualMinors, Is.Null);
        Assert.That(moderationResult.Violence, Is.Null);
        Assert.That(moderationResult.ViolenceGraphic, Is.Null);
    }

    [Test]
    public void ModerationResultWithHarassmentWorks()
    {
        float harassmentScore = 0.85f;
        ModerationCategory category = OpenAIModerationsModelFactory.ModerationCategory(true, harassmentScore);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(harassment: category);

        Assert.That(moderationResult.Harassment.Flagged, Is.True);
        Assert.That(moderationResult.Harassment.Score, Is.EqualTo(harassmentScore));

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Hate, Is.Null);
        Assert.That(moderationResult.HateThreatening, Is.Null);
        Assert.That(moderationResult.HarassmentThreatening, Is.Null);
        Assert.That(moderationResult.SelfHarm, Is.Null);
        Assert.That(moderationResult.SelfHarmIntent, Is.Null);
        Assert.That(moderationResult.SelfHarmInstructions, Is.Null);
        Assert.That(moderationResult.Sexual, Is.Null);
        Assert.That(moderationResult.SexualMinors, Is.Null);
        Assert.That(moderationResult.Violence, Is.Null);
        Assert.That(moderationResult.ViolenceGraphic, Is.Null);
    }

    [Test]
    public void ModerationResultWithHarassmentThreateningWorks()
    {
        float harassmentThreateningScore = 0.85f;
        ModerationCategory category = OpenAIModerationsModelFactory.ModerationCategory(true, harassmentThreateningScore);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(harassmentThreatening: category);

        Assert.That(moderationResult.HarassmentThreatening.Flagged, Is.True);
        Assert.That(moderationResult.HarassmentThreatening.Score, Is.EqualTo(harassmentThreateningScore));

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Hate, Is.Null);
        Assert.That(moderationResult.HateThreatening, Is.Null);
        Assert.That(moderationResult.Harassment, Is.Null);
        Assert.That(moderationResult.SelfHarm, Is.Null);
        Assert.That(moderationResult.SelfHarmIntent, Is.Null);
        Assert.That(moderationResult.SelfHarmInstructions, Is.Null);
        Assert.That(moderationResult.Sexual, Is.Null);
        Assert.That(moderationResult.SexualMinors, Is.Null);
        Assert.That(moderationResult.Violence, Is.Null);
        Assert.That(moderationResult.ViolenceGraphic, Is.Null);
    }

    [Test]
    public void ModerationResultWithSelfHarmWorks()
    {
        float selfHarmScore = 0.85f;
        ModerationCategory category = OpenAIModerationsModelFactory.ModerationCategory(true, selfHarmScore);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(selfHarm: category);

        Assert.That(moderationResult.SelfHarm.Flagged, Is.True);
        Assert.That(moderationResult.SelfHarm.Score, Is.EqualTo(selfHarmScore));

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Hate, Is.Null);
        Assert.That(moderationResult.HateThreatening, Is.Null);
        Assert.That(moderationResult.Harassment, Is.Null);
        Assert.That(moderationResult.HarassmentThreatening, Is.Null);
        Assert.That(moderationResult.SelfHarmIntent, Is.Null);
        Assert.That(moderationResult.SelfHarmInstructions, Is.Null);
        Assert.That(moderationResult.Sexual, Is.Null);
        Assert.That(moderationResult.SexualMinors, Is.Null);
        Assert.That(moderationResult.Violence, Is.Null);
        Assert.That(moderationResult.ViolenceGraphic, Is.Null);
    }

    [Test]
    public void ModerationResultWithSelfHarmIntentWorks()
    {
        float selfHarmIntentScore = 0.85f;
        ModerationCategory category = OpenAIModerationsModelFactory.ModerationCategory(true, selfHarmIntentScore);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(selfHarmIntent: category);

        Assert.That(moderationResult.SelfHarmIntent.Flagged, Is.True);
        Assert.That(moderationResult.SelfHarmIntent.Score, Is.EqualTo(selfHarmIntentScore));

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Hate, Is.Null);
        Assert.That(moderationResult.HateThreatening, Is.Null);
        Assert.That(moderationResult.Harassment, Is.Null);
        Assert.That(moderationResult.HarassmentThreatening, Is.Null);
        Assert.That(moderationResult.SelfHarm, Is.Null);
        Assert.That(moderationResult.SelfHarmInstructions, Is.Null);
        Assert.That(moderationResult.Sexual, Is.Null);
        Assert.That(moderationResult.SexualMinors, Is.Null);
        Assert.That(moderationResult.Violence, Is.Null);
        Assert.That(moderationResult.ViolenceGraphic, Is.Null);
    }

    [Test]
    public void ModerationResultWithSelfHarmInstructionWorks()
    {
        float selfHarmInstructionsScore = 0.85f;
        ModerationCategory category = OpenAIModerationsModelFactory.ModerationCategory(true, selfHarmInstructionsScore);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(selfHarmInstructions: category);

        Assert.That(moderationResult.SelfHarmInstructions.Flagged, Is.True);
        Assert.That(moderationResult.SelfHarmInstructions.Score, Is.EqualTo(selfHarmInstructionsScore));

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Hate, Is.Null);
        Assert.That(moderationResult.HateThreatening, Is.Null);
        Assert.That(moderationResult.Harassment, Is.Null);
        Assert.That(moderationResult.HarassmentThreatening, Is.Null);
        Assert.That(moderationResult.SelfHarm, Is.Null);
        Assert.That(moderationResult.SelfHarmIntent, Is.Null);
        Assert.That(moderationResult.Sexual, Is.Null);
        Assert.That(moderationResult.SexualMinors, Is.Null);
        Assert.That(moderationResult.Violence, Is.Null);
        Assert.That(moderationResult.ViolenceGraphic, Is.Null);
    }

    [Test]
    public void ModerationResultWithSexualWorks()
    {
        float sexualScore = 0.85f;
        ModerationCategory category = OpenAIModerationsModelFactory.ModerationCategory(true, sexualScore);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(sexual: category);

        Assert.That(moderationResult.Sexual.Flagged, Is.True);
        Assert.That(moderationResult.Sexual.Score, Is.EqualTo(sexualScore));

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Hate, Is.Null);
        Assert.That(moderationResult.HateThreatening, Is.Null);
        Assert.That(moderationResult.Harassment, Is.Null);
        Assert.That(moderationResult.HarassmentThreatening, Is.Null);
        Assert.That(moderationResult.SelfHarm, Is.Null);
        Assert.That(moderationResult.SelfHarmIntent, Is.Null);
        Assert.That(moderationResult.SelfHarmInstructions, Is.Null);
        Assert.That(moderationResult.SexualMinors, Is.Null);
        Assert.That(moderationResult.Violence, Is.Null);
        Assert.That(moderationResult.ViolenceGraphic, Is.Null);
    }

    [Test]
    public void ModerationResultWithSexualMinorsWorks()
    {
        float sexualMinorsScore = 0.85f;
        ModerationCategory category = OpenAIModerationsModelFactory.ModerationCategory(true, sexualMinorsScore);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(sexualMinors: category);

        Assert.That(moderationResult.SexualMinors.Flagged, Is.True);
        Assert.That(moderationResult.SexualMinors.Score, Is.EqualTo(sexualMinorsScore));

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Hate, Is.Null);
        Assert.That(moderationResult.HateThreatening, Is.Null);
        Assert.That(moderationResult.Harassment, Is.Null);
        Assert.That(moderationResult.HarassmentThreatening, Is.Null);
        Assert.That(moderationResult.SelfHarm, Is.Null);
        Assert.That(moderationResult.SelfHarmIntent, Is.Null);
        Assert.That(moderationResult.SelfHarmInstructions, Is.Null);
        Assert.That(moderationResult.Sexual, Is.Null);
        Assert.That(moderationResult.Violence, Is.Null);
        Assert.That(moderationResult.ViolenceGraphic, Is.Null);
    }

    [Test]
    public void ModerationResultWithViolenceWorks()
    {
        float violenceScore = 0.85f;
        ModerationCategory category = OpenAIModerationsModelFactory.ModerationCategory(true, violenceScore);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(violence: category);

        Assert.That(moderationResult.Violence.Flagged, Is.True);
        Assert.That(moderationResult.Violence.Score, Is.EqualTo(violenceScore));

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Hate, Is.Null);
        Assert.That(moderationResult.HateThreatening, Is.Null);
        Assert.That(moderationResult.Harassment, Is.Null);
        Assert.That(moderationResult.HarassmentThreatening, Is.Null);
        Assert.That(moderationResult.SelfHarm, Is.Null);
        Assert.That(moderationResult.SelfHarmIntent, Is.Null);
        Assert.That(moderationResult.SelfHarmInstructions, Is.Null);
        Assert.That(moderationResult.Sexual, Is.Null);
        Assert.That(moderationResult.SexualMinors, Is.Null);
        Assert.That(moderationResult.ViolenceGraphic, Is.Null);
    }

    [Test]
    public void ModerationResultWithViolenceGraphicWorks()
    {
        float violenceGraphicScore = 0.85f;
        ModerationCategory category = OpenAIModerationsModelFactory.ModerationCategory(true, violenceGraphicScore);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(violenceGraphic: category);

        Assert.That(moderationResult.ViolenceGraphic.Flagged, Is.True);
        Assert.That(moderationResult.ViolenceGraphic.Score, Is.EqualTo(violenceGraphicScore));

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Hate, Is.Null);
        Assert.That(moderationResult.HateThreatening, Is.Null);
        Assert.That(moderationResult.Harassment, Is.Null);
        Assert.That(moderationResult.HarassmentThreatening, Is.Null);
        Assert.That(moderationResult.SelfHarm, Is.Null);
        Assert.That(moderationResult.SelfHarmIntent, Is.Null);
        Assert.That(moderationResult.SelfHarmInstructions, Is.Null);
        Assert.That(moderationResult.Sexual, Is.Null);
        Assert.That(moderationResult.SexualMinors, Is.Null);
        Assert.That(moderationResult.Violence, Is.Null);
    }

    [Test]
    public void ModerationResultCollectionWithNoPropertiesWorks()
    {
        ModerationResultCollection moderationCollection = OpenAIModerationsModelFactory.ModerationResultCollection();

        Assert.That(moderationCollection.Id, Is.Null);
        Assert.That(moderationCollection.Model, Is.Null);
        Assert.That(moderationCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void ModerationResultCollectionWithIdWorks()
    {
        string id = "moderationId";
        ModerationResultCollection moderationCollection = OpenAIModerationsModelFactory.ModerationResultCollection(id: id);

        Assert.That(moderationCollection.Id, Is.EqualTo(id));
        Assert.That(moderationCollection.Model, Is.Null);
        Assert.That(moderationCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void ModerationResultCollectionWithModelWorks()
    {
        string model = "supermodel";
        ModerationResultCollection moderationCollection = OpenAIModerationsModelFactory.ModerationResultCollection(model: model);

        Assert.That(moderationCollection.Id, Is.Null);
        Assert.That(moderationCollection.Model, Is.EqualTo(model));
        Assert.That(moderationCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void ModerationResultCollectionWithItemsWorks()
    {
        IEnumerable<ModerationResult> items = [
            OpenAIModerationsModelFactory.ModerationResult(flagged: true),
            OpenAIModerationsModelFactory.ModerationResult(flagged: false)
        ];
        ModerationResultCollection moderationCollection = OpenAIModerationsModelFactory.ModerationResultCollection(items: items);

        Assert.That(moderationCollection.Id, Is.Null);
        Assert.That(moderationCollection.Model, Is.Null);
        Assert.That(moderationCollection.SequenceEqual(items), Is.True);
    }
}
