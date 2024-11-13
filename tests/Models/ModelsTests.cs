using NUnit.Framework;
using OpenAI.Models;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Linq;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Models;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Models")]
public class ModelsTests : SyncAsyncTestBase
{
    public ModelsTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task ListModels()
    {
        OpenAIModelClient client = GetTestClient<OpenAIModelClient>(TestScenario.Models);

        OpenAIModelCollection allModels = IsAsync
            ? await client.GetModelsAsync()
            : client.GetModels();

        OpenAIModel whisper = allModels.First(m => m.Id.Contains("whisper", StringComparison.InvariantCultureIgnoreCase));
        OpenAIModel turbo = allModels.First(m => m.Id.Contains("turbo", StringComparison.InvariantCultureIgnoreCase));
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
        OpenAIModelClient client = GetTestClient<OpenAIModelClient>(TestScenario.Models);
        string modelId = "gpt-4o-mini";

        OpenAIModel model = IsAsync
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
        OpenAIModelClient client = GetTestClient<OpenAIModelClient>(TestScenario.Models);
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
        OpenAIModelClient client = GetTestClient<OpenAIModelClient>(TestScenario.Models);
        ClientResultException ex = null;

        if (IsAsync)
        {
            ex = Assert.ThrowsAsync<ClientResultException>(async () => await client.DeleteModelAsync("fake_id"));
        }
        else
        {
            ex = Assert.Throws<ClientResultException>(() => client.DeleteModel("fake_id"));
        }

        // If the model exists but the user doesn't own it, the service returns 403.
        // If the model doesn't exist at all, the service returns 404.
        // The service has changed the behavior in the past.
        Assert.That((ex.Status == 403 || ex.Status == 404), Is.True);
    }

    [Test]
    public void SerializeModelCollection()
    {
        // TODO: Add this test.
    }
}
