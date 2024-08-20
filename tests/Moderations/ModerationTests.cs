using NUnit.Framework;
using OpenAI.Moderations;
using OpenAI.Tests.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Moderations;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Moderations")]
public partial class ModerationTests : SyncAsyncTestBase
{
    public ModerationTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task ClassifySingleInput()
    {
        ModerationClient client = GetTestClient<ModerationClient>(TestScenario.Moderations);

        const string input = "I am killing all my houseplants!";

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextInputAsync(input)
            : client.ClassifyTextInput(input);
        Assert.That(moderation, Is.Not.Null);
        Assert.That(moderation.Flagged, Is.True);
        Assert.That(moderation.Categories.Violence, Is.True);
        Assert.That(moderation.CategoryScores.Violence, Is.GreaterThan(0.5));
    }

    [Test]
    public async Task ClassifyMultipleInputs()
    {
        ModerationClient client = GetTestClient<ModerationClient>(TestScenario.Moderations);

        List<string> inputs =
            [
                "I forgot to water my houseplants!",
                "I am killing all my houseplants!"
            ];

        ModerationCollection moderations = IsAsync
            ? await client.ClassifyTextInputsAsync(inputs)
            : client.ClassifyTextInputs(inputs);
        Assert.That(moderations, Is.Not.Null);
        Assert.That(moderations.Count, Is.EqualTo(2));
        Assert.That(moderations.Model, Does.StartWith("text-moderation"));
        Assert.That(moderations.Id, Is.Not.Null.Or.Empty);

        Assert.That(moderations[0], Is.Not.Null);
        Assert.That(moderations[0].Flagged, Is.False);

        Assert.That(moderations[1], Is.Not.Null);
        Assert.That(moderations[1].Flagged, Is.True);
        Assert.That(moderations[1].Categories.Violence, Is.True);
        Assert.That(moderations[1].CategoryScores.Violence, Is.GreaterThan(0.5));
    }

    [Test]
    public void SerializeModerationCollection()
    {
        // TODO: Add this test.
    }
}
