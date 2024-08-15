using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenAI.Moderations;

namespace OpenAI.Tests.Moderations;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public partial class OpenAIModerationsModelFactoryTests
{
    [Test]
    public void ModerationCategoriesWithNoPropertiesWorks()
    {
        ModerationCategories moderationCategories = OpenAIModerationsModelFactory.ModerationCategories();

        Assert.That(moderationCategories.Hate, Is.False);
        Assert.That(moderationCategories.HateThreatening, Is.False);
        Assert.That(moderationCategories.Harassment, Is.False);
        Assert.That(moderationCategories.HarassmentThreatening, Is.False);
        Assert.That(moderationCategories.SelfHarm, Is.False);
        Assert.That(moderationCategories.SelfHarmIntent, Is.False);
        Assert.That(moderationCategories.SelfHarmInstructions, Is.False);
        Assert.That(moderationCategories.Sexual, Is.False);
        Assert.That(moderationCategories.SexualMinors, Is.False);
        Assert.That(moderationCategories.Violence, Is.False);
        Assert.That(moderationCategories.ViolenceGraphic, Is.False);
    }

    [Test]
    public void ModerationCategoriesWithHateWorks()
    {
        ModerationCategories moderationCategories = OpenAIModerationsModelFactory.ModerationCategories(hate: true);

        Assert.That(moderationCategories.Hate, Is.True);
        Assert.That(moderationCategories.HateThreatening, Is.False);
        Assert.That(moderationCategories.Harassment, Is.False);
        Assert.That(moderationCategories.HarassmentThreatening, Is.False);
        Assert.That(moderationCategories.SelfHarm, Is.False);
        Assert.That(moderationCategories.SelfHarmIntent, Is.False);
        Assert.That(moderationCategories.SelfHarmInstructions, Is.False);
        Assert.That(moderationCategories.Sexual, Is.False);
        Assert.That(moderationCategories.SexualMinors, Is.False);
        Assert.That(moderationCategories.Violence, Is.False);
        Assert.That(moderationCategories.ViolenceGraphic, Is.False);
    }

    [Test]
    public void ModerationCategoriesWithHateThreateningWorks()
    {
        ModerationCategories moderationCategories = OpenAIModerationsModelFactory.ModerationCategories(hateThreatening: true);

        Assert.That(moderationCategories.Hate, Is.False);
        Assert.That(moderationCategories.HateThreatening, Is.True);
        Assert.That(moderationCategories.Harassment, Is.False);
        Assert.That(moderationCategories.HarassmentThreatening, Is.False);
        Assert.That(moderationCategories.SelfHarm, Is.False);
        Assert.That(moderationCategories.SelfHarmIntent, Is.False);
        Assert.That(moderationCategories.SelfHarmInstructions, Is.False);
        Assert.That(moderationCategories.Sexual, Is.False);
        Assert.That(moderationCategories.SexualMinors, Is.False);
        Assert.That(moderationCategories.Violence, Is.False);
        Assert.That(moderationCategories.ViolenceGraphic, Is.False);
    }

    [Test]
    public void ModerationCategoriesWithHarassmentWorks()
    {
        ModerationCategories moderationCategories = OpenAIModerationsModelFactory.ModerationCategories(harassment: true);

        Assert.That(moderationCategories.Hate, Is.False);
        Assert.That(moderationCategories.HateThreatening, Is.False);
        Assert.That(moderationCategories.Harassment, Is.True);
        Assert.That(moderationCategories.HarassmentThreatening, Is.False);
        Assert.That(moderationCategories.SelfHarm, Is.False);
        Assert.That(moderationCategories.SelfHarmIntent, Is.False);
        Assert.That(moderationCategories.SelfHarmInstructions, Is.False);
        Assert.That(moderationCategories.Sexual, Is.False);
        Assert.That(moderationCategories.SexualMinors, Is.False);
        Assert.That(moderationCategories.Violence, Is.False);
        Assert.That(moderationCategories.ViolenceGraphic, Is.False);
    }

    [Test]
    public void ModerationCategoriesWithHarassmentThreateningWorks()
    {
        ModerationCategories moderationCategories = OpenAIModerationsModelFactory.ModerationCategories(harassmentThreatening: true);

        Assert.That(moderationCategories.Hate, Is.False);
        Assert.That(moderationCategories.HateThreatening, Is.False);
        Assert.That(moderationCategories.Harassment, Is.False);
        Assert.That(moderationCategories.HarassmentThreatening, Is.True);
        Assert.That(moderationCategories.SelfHarm, Is.False);
        Assert.That(moderationCategories.SelfHarmIntent, Is.False);
        Assert.That(moderationCategories.SelfHarmInstructions, Is.False);
        Assert.That(moderationCategories.Sexual, Is.False);
        Assert.That(moderationCategories.SexualMinors, Is.False);
        Assert.That(moderationCategories.Violence, Is.False);
        Assert.That(moderationCategories.ViolenceGraphic, Is.False);
    }

    [Test]
    public void ModerationCategoriesWithSelfHarmWorks()
    {
        ModerationCategories moderationCategories = OpenAIModerationsModelFactory.ModerationCategories(selfHarm: true);

        Assert.That(moderationCategories.Hate, Is.False);
        Assert.That(moderationCategories.HateThreatening, Is.False);
        Assert.That(moderationCategories.Harassment, Is.False);
        Assert.That(moderationCategories.HarassmentThreatening, Is.False);
        Assert.That(moderationCategories.SelfHarm, Is.True);
        Assert.That(moderationCategories.SelfHarmIntent, Is.False);
        Assert.That(moderationCategories.SelfHarmInstructions, Is.False);
        Assert.That(moderationCategories.Sexual, Is.False);
        Assert.That(moderationCategories.SexualMinors, Is.False);
        Assert.That(moderationCategories.Violence, Is.False);
        Assert.That(moderationCategories.ViolenceGraphic, Is.False);
    }

    [Test]
    public void ModerationCategoriesWithSelfHarmIntentWorks()
    {
        ModerationCategories moderationCategories = OpenAIModerationsModelFactory.ModerationCategories(selfHarmIntent: true);

        Assert.That(moderationCategories.Hate, Is.False);
        Assert.That(moderationCategories.HateThreatening, Is.False);
        Assert.That(moderationCategories.Harassment, Is.False);
        Assert.That(moderationCategories.HarassmentThreatening, Is.False);
        Assert.That(moderationCategories.SelfHarm, Is.False);
        Assert.That(moderationCategories.SelfHarmIntent, Is.True);
        Assert.That(moderationCategories.SelfHarmInstructions, Is.False);
        Assert.That(moderationCategories.Sexual, Is.False);
        Assert.That(moderationCategories.SexualMinors, Is.False);
        Assert.That(moderationCategories.Violence, Is.False);
        Assert.That(moderationCategories.ViolenceGraphic, Is.False);
    }

    [Test]
    public void ModerationCategoriesWithSelfHarmInstructionWorks()
    {
        ModerationCategories moderationCategories = OpenAIModerationsModelFactory.ModerationCategories(selfHarmInstructions: true);

        Assert.That(moderationCategories.Hate, Is.False);
        Assert.That(moderationCategories.HateThreatening, Is.False);
        Assert.That(moderationCategories.Harassment, Is.False);
        Assert.That(moderationCategories.HarassmentThreatening, Is.False);
        Assert.That(moderationCategories.SelfHarm, Is.False);
        Assert.That(moderationCategories.SelfHarmIntent, Is.False);
        Assert.That(moderationCategories.SelfHarmInstructions, Is.True);
        Assert.That(moderationCategories.Sexual, Is.False);
        Assert.That(moderationCategories.SexualMinors, Is.False);
        Assert.That(moderationCategories.Violence, Is.False);
        Assert.That(moderationCategories.ViolenceGraphic, Is.False);
    }

    [Test]
    public void ModerationCategoriesWithSexualWorks()
    {
        ModerationCategories moderationCategories = OpenAIModerationsModelFactory.ModerationCategories(sexual: true);

        Assert.That(moderationCategories.Hate, Is.False);
        Assert.That(moderationCategories.HateThreatening, Is.False);
        Assert.That(moderationCategories.Harassment, Is.False);
        Assert.That(moderationCategories.HarassmentThreatening, Is.False);
        Assert.That(moderationCategories.SelfHarm, Is.False);
        Assert.That(moderationCategories.SelfHarmIntent, Is.False);
        Assert.That(moderationCategories.SelfHarmInstructions, Is.False);
        Assert.That(moderationCategories.Sexual, Is.True);
        Assert.That(moderationCategories.SexualMinors, Is.False);
        Assert.That(moderationCategories.Violence, Is.False);
        Assert.That(moderationCategories.ViolenceGraphic, Is.False);
    }

    [Test]
    public void ModerationCategoriesWithSexualMinorsWorks()
    {
        ModerationCategories moderationCategories = OpenAIModerationsModelFactory.ModerationCategories(sexualMinors: true);

        Assert.That(moderationCategories.Hate, Is.False);
        Assert.That(moderationCategories.HateThreatening, Is.False);
        Assert.That(moderationCategories.Harassment, Is.False);
        Assert.That(moderationCategories.HarassmentThreatening, Is.False);
        Assert.That(moderationCategories.SelfHarm, Is.False);
        Assert.That(moderationCategories.SelfHarmIntent, Is.False);
        Assert.That(moderationCategories.SelfHarmInstructions, Is.False);
        Assert.That(moderationCategories.Sexual, Is.False);
        Assert.That(moderationCategories.SexualMinors, Is.True);
        Assert.That(moderationCategories.Violence, Is.False);
        Assert.That(moderationCategories.ViolenceGraphic, Is.False);
    }

    [Test]
    public void ModerationCategoriesWithViolenceWorks()
    {
        ModerationCategories moderationCategories = OpenAIModerationsModelFactory.ModerationCategories(violence: true);

        Assert.That(moderationCategories.Hate, Is.False);
        Assert.That(moderationCategories.HateThreatening, Is.False);
        Assert.That(moderationCategories.Harassment, Is.False);
        Assert.That(moderationCategories.HarassmentThreatening, Is.False);
        Assert.That(moderationCategories.SelfHarm, Is.False);
        Assert.That(moderationCategories.SelfHarmIntent, Is.False);
        Assert.That(moderationCategories.SelfHarmInstructions, Is.False);
        Assert.That(moderationCategories.Sexual, Is.False);
        Assert.That(moderationCategories.SexualMinors, Is.False);
        Assert.That(moderationCategories.Violence, Is.True);
        Assert.That(moderationCategories.ViolenceGraphic, Is.False);
    }

    [Test]
    public void ModerationCategoriesWithViolenceGraphicWorks()
    {
        ModerationCategories moderationCategories = OpenAIModerationsModelFactory.ModerationCategories(violenceGraphic: true);

        Assert.That(moderationCategories.Hate, Is.False);
        Assert.That(moderationCategories.HateThreatening, Is.False);
        Assert.That(moderationCategories.Harassment, Is.False);
        Assert.That(moderationCategories.HarassmentThreatening, Is.False);
        Assert.That(moderationCategories.SelfHarm, Is.False);
        Assert.That(moderationCategories.SelfHarmIntent, Is.False);
        Assert.That(moderationCategories.SelfHarmInstructions, Is.False);
        Assert.That(moderationCategories.Sexual, Is.False);
        Assert.That(moderationCategories.SexualMinors, Is.False);
        Assert.That(moderationCategories.Violence, Is.False);
        Assert.That(moderationCategories.ViolenceGraphic, Is.True);
    }

    [Test]
    public void ModerationCategoryScoresWithNoPropertiesWorks()
    {
        ModerationCategoryScores moderationCategoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores();

        Assert.That(moderationCategoryScores.Hate, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HateThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Harassment, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HarassmentThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarm, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmIntent, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmInstructions, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Sexual, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SexualMinors, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Violence, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.ViolenceGraphic, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryScoresWithHateWorks()
    {
        float hate = 0.85f;
        ModerationCategoryScores moderationCategoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores(hate: hate);

        Assert.That(moderationCategoryScores.Hate, Is.EqualTo(hate));
        Assert.That(moderationCategoryScores.HateThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Harassment, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HarassmentThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarm, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmIntent, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmInstructions, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Sexual, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SexualMinors, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Violence, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.ViolenceGraphic, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryScoresWithHateThreateningWorks()
    {
        float hateThreatening = 0.85f;
        ModerationCategoryScores moderationCategoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores(hateThreatening: hateThreatening);

        Assert.That(moderationCategoryScores.Hate, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HateThreatening, Is.EqualTo(hateThreatening));
        Assert.That(moderationCategoryScores.Harassment, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HarassmentThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarm, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmIntent, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmInstructions, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Sexual, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SexualMinors, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Violence, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.ViolenceGraphic, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryScoresWithHarassmentWorks()
    {
        float harassment = 0.85f;
        ModerationCategoryScores moderationCategoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores(harassment: harassment);

        Assert.That(moderationCategoryScores.Hate, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HateThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Harassment, Is.EqualTo(harassment));
        Assert.That(moderationCategoryScores.HarassmentThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarm, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmIntent, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmInstructions, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Sexual, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SexualMinors, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Violence, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.ViolenceGraphic, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryScoresWithHarassmentThreateningWorks()
    {
        float harassmentThreatening = 0.85f;
        ModerationCategoryScores moderationCategoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores(harassmentThreatening: harassmentThreatening);

        Assert.That(moderationCategoryScores.Hate, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HateThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Harassment, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HarassmentThreatening, Is.EqualTo(harassmentThreatening));
        Assert.That(moderationCategoryScores.SelfHarm, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmIntent, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmInstructions, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Sexual, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SexualMinors, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Violence, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.ViolenceGraphic, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryScoresWithSelfHarmWorks()
    {
        float selfHarm = 0.85f;
        ModerationCategoryScores moderationCategoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores(selfHarm: selfHarm);

        Assert.That(moderationCategoryScores.Hate, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HateThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Harassment, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HarassmentThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarm, Is.EqualTo(selfHarm));
        Assert.That(moderationCategoryScores.SelfHarmIntent, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmInstructions, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Sexual, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SexualMinors, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Violence, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.ViolenceGraphic, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryScoresWithSelfHarmIntentWorks()
    {
        float selfHarmIntent = 0.85f;
        ModerationCategoryScores moderationCategoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores(selfHarmIntent: selfHarmIntent);

        Assert.That(moderationCategoryScores.Hate, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HateThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Harassment, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HarassmentThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarm, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmIntent, Is.EqualTo(selfHarmIntent));
        Assert.That(moderationCategoryScores.SelfHarmInstructions, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Sexual, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SexualMinors, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Violence, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.ViolenceGraphic, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryScoresWithSelfHarmInstructionWorks()
    {
        float selfHarmInstructions = 0.85f;
        ModerationCategoryScores moderationCategoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores(selfHarmInstructions: selfHarmInstructions);

        Assert.That(moderationCategoryScores.Hate, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HateThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Harassment, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HarassmentThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarm, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmIntent, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmInstructions, Is.EqualTo(selfHarmInstructions));
        Assert.That(moderationCategoryScores.Sexual, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SexualMinors, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Violence, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.ViolenceGraphic, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryScoresWithSexualWorks()
    {
        float sexual = 0.85f;
        ModerationCategoryScores moderationCategoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores(sexual: sexual);

        Assert.That(moderationCategoryScores.Hate, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HateThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Harassment, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HarassmentThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarm, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmIntent, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmInstructions, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Sexual, Is.EqualTo(sexual));
        Assert.That(moderationCategoryScores.SexualMinors, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Violence, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.ViolenceGraphic, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryScoresWithSexualMinorsWorks()
    {
        float sexualMinors = 0.85f;
        ModerationCategoryScores moderationCategoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores(sexualMinors: sexualMinors);

        Assert.That(moderationCategoryScores.Hate, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HateThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Harassment, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HarassmentThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarm, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmIntent, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmInstructions, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Sexual, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SexualMinors, Is.EqualTo(sexualMinors));
        Assert.That(moderationCategoryScores.Violence, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.ViolenceGraphic, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryScoresWithViolenceWorks()
    {
        float violence = 0.85f;
        ModerationCategoryScores moderationCategoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores(violence: violence);

        Assert.That(moderationCategoryScores.Hate, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HateThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Harassment, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HarassmentThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarm, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmIntent, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmInstructions, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Sexual, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SexualMinors, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Violence, Is.EqualTo(violence));
        Assert.That(moderationCategoryScores.ViolenceGraphic, Is.EqualTo(0f));
    }

    [Test]
    public void ModerationCategoryScoresWithViolenceGraphicWorks()
    {
        float violenceGraphic = 0.85f;
        ModerationCategoryScores moderationCategoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores(violenceGraphic: violenceGraphic);

        Assert.That(moderationCategoryScores.Hate, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HateThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Harassment, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.HarassmentThreatening, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarm, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmIntent, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SelfHarmInstructions, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Sexual, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.SexualMinors, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.Violence, Is.EqualTo(0f));
        Assert.That(moderationCategoryScores.ViolenceGraphic, Is.EqualTo(violenceGraphic));
    }

    [Test]
    public void ModerationCollectionWithNoPropertiesWorks()
    {
        ModerationCollection moderationCollection = OpenAIModerationsModelFactory.ModerationCollection();

        Assert.That(moderationCollection.Id, Is.Null);
        Assert.That(moderationCollection.Model, Is.Null);
        Assert.That(moderationCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void ModerationCollectionWithIdWorks()
    {
        string id = "moderationId";
        ModerationCollection moderationCollection = OpenAIModerationsModelFactory.ModerationCollection(id: id);

        Assert.That(moderationCollection.Id, Is.EqualTo(id));
        Assert.That(moderationCollection.Model, Is.Null);
        Assert.That(moderationCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void ModerationCollectionWithModelWorks()
    {
        string model = "supermodel";
        ModerationCollection moderationCollection = OpenAIModerationsModelFactory.ModerationCollection(model: model);

        Assert.That(moderationCollection.Id, Is.Null);
        Assert.That(moderationCollection.Model, Is.EqualTo(model));
        Assert.That(moderationCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void ModerationCollectionWithItemsWorks()
    {
        IEnumerable<ModerationResult> items = [
            OpenAIModerationsModelFactory.ModerationResult(flagged: true),
            OpenAIModerationsModelFactory.ModerationResult(flagged: false)
        ];
        ModerationCollection moderationCollection = OpenAIModerationsModelFactory.ModerationCollection(items: items);

        Assert.That(moderationCollection.Id, Is.Null);
        Assert.That(moderationCollection.Model, Is.Null);
        Assert.That(moderationCollection.SequenceEqual(items), Is.True);
    }

    [Test]
    public void ModerationResultWithNoPropertiesWorks()
    {
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult();

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Categories, Is.Null);
        Assert.That(moderationResult.CategoryScores, Is.Null);
    }

    [Test]
    public void ModerationResultWithFlaggedWorks()
    {
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(flagged: true);

        Assert.That(moderationResult.Flagged, Is.True);
        Assert.That(moderationResult.Categories, Is.Null);
        Assert.That(moderationResult.CategoryScores, Is.Null);
    }

    [Test]
    public void ModerationResultWithCategoriesWorks()
    {
        ModerationCategories categories = OpenAIModerationsModelFactory.ModerationCategories(hate: true);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(categories: categories);

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Categories, Is.EqualTo(categories));
        Assert.That(moderationResult.CategoryScores, Is.Null);
    }

    [Test]
    public void ModerationResultWithCategoryScoresWorks()
    {
        ModerationCategoryScores categoryScores = OpenAIModerationsModelFactory.ModerationCategoryScores(hate: 0.85f);
        ModerationResult moderationResult = OpenAIModerationsModelFactory.ModerationResult(categoryScores: categoryScores);

        Assert.That(moderationResult.Flagged, Is.False);
        Assert.That(moderationResult.Categories, Is.Null);
        Assert.That(moderationResult.CategoryScores, Is.EqualTo(categoryScores));
    }
}
