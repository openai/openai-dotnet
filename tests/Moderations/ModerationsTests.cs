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
public class ModerationsTests : SyncAsyncTestBase
{
    public ModerationsTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task ClassifySingleInput()
    {
        ModerationClient client = GetTestClient<ModerationClient>(TestScenario.Moderations);

        const string input = "I am killing all my houseplants!";

        ModerationResult moderation = IsAsync
            ? await client.ClassifyTextAsync(input)
            : client.ClassifyText(input);
        Assert.That(moderation, Is.Not.Null);
        Assert.That(moderation.Flagged, Is.True);
        Assert.That(moderation.Violence.Flagged, Is.True);
        Assert.That(moderation.Violence.Score, Is.GreaterThan(0.5));
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

        ModerationResultCollection moderations = IsAsync
            ? await client.ClassifyTextAsync(inputs)
            : client.ClassifyText(inputs);
        Assert.That(moderations, Is.Not.Null);
        Assert.That(moderations.Count, Is.EqualTo(2));
        Assert.That(moderations.Model, Does.StartWith("text-moderation"));
        Assert.That(moderations.Id, Is.Not.Null.Or.Empty);

        Assert.That(moderations[0], Is.Not.Null);
        Assert.That(moderations[0].Flagged, Is.False);

        Assert.That(moderations[1], Is.Not.Null);
        Assert.That(moderations[1].Flagged, Is.True);
        Assert.That(moderations[1].Violence.Flagged, Is.True);
        Assert.That(moderations[1].Violence.Score, Is.GreaterThan(0.5));
    }
}
