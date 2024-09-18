using System;
using System.ClientModel;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenAI.Models;
using OpenAI.Tests.Utility;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Models;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Models")]
public partial class ModelTests : SyncAsyncTestBase
{
    public ModelTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task ListModels()
    {
        ModelClient client = GetTestClient<ModelClient>(TestScenario.Models);

        OpenAIModelInfoCollection allModels = IsAsync
            ? await client.GetModelsAsync()
            : client.GetModels();

        OpenAIModelInfo whisper = allModels.First(m => m.Id.Contains("whisper", StringComparison.InvariantCultureIgnoreCase));
        OpenAIModelInfo turbo = allModels.First(m => m.Id.Contains("turbo", StringComparison.InvariantCultureIgnoreCase));
        long unixTime2020 = (new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(whisper.Id, Is.Not.EqualTo(turbo.Id));
        Assert.That(whisper.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2020));
        Assert.That(turbo.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2020));
        Assert.That(whisper.OwnedBy.ToLowerInvariant(), Contains.Substring("system").Or.Contains("openai"));
        Assert.That(turbo.OwnedBy.ToLowerInvariant(), Contains.Substring("system").Or.Contains("openai"));

        Console.WriteLine($"Total model count: {allModels.Count}");
    }

    [Test]
    public async Task GetModelInfo()
    {
        ModelClient client = GetTestClient<ModelClient>(TestScenario.Models);
        string modelId = "gpt-4o-mini";

        OpenAIModelInfo model = IsAsync
            ? await client.GetModelAsync(modelId)
            : client.GetModel(modelId);

        long unixTime2020 = (new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero)).ToUnixTimeSeconds();

        Assert.That(model, Is.Not.Null);
        Assert.That(model.Id, Is.EqualTo(modelId));
        Assert.That(model.CreatedAt.ToUnixTimeSeconds(), Is.GreaterThan(unixTime2020));
        Assert.That(model.OwnedBy.ToLowerInvariant(), Does.Contain("system"));
    }

    [Test]
    public void GetModelCanParseServiceError()
    {
        ModelClient client = GetTestClient<ModelClient>(TestScenario.Models);
        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.GetModelAsync("fake_id"));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.GetModel("fake_id"));
        }

        Assert.That(ex.Status, Is.EqualTo(404));
    }

    [Test]
    public void DeleteModelCanParseServiceError()
    {
        ModelClient client = GetTestClient<ModelClient>(TestScenario.Models);
        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.DeleteModelAsync("fake_id"));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.DeleteModel("fake_id"));
        }

        Assert.That(ex.Status, Is.EqualTo(403));
    }

    [Test]
    public void SerializeModelCollection()
    {
        // TODO: Add this test.
    }
}
