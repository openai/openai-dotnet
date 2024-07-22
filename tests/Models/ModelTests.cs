using NUnit.Framework;
using OpenAI.Models;
using OpenAI.Tests.Utility;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        ModelClient client = new();

        OpenAIModelInfoCollection allModels = IsAsync
            ? await client.GetModelsAsync()
            : client.GetModels();
        Assert.That(allModels, Is.Not.Null.Or.Empty);
        Assert.That(allModels.Any(modelInfo => modelInfo.Id.Contains("whisper", StringComparison.InvariantCultureIgnoreCase)));
        Console.WriteLine($"Total model count: {allModels.Count}");
    }

    [Test]
    public async Task GetModelInfo()
    {
        ModelClient client = new();

        string modelName = "gpt-4o-mini";

        OpenAIModelInfo model = IsAsync
            ? await client.GetModelAsync(modelName)
            : client.GetModel(modelName);
        Assert.That(model, Is.Not.Null);
        Assert.That(model.OwnedBy.ToLowerInvariant(), Does.Contain("system"));
    }

    [Test]
    public void SerializeModelCollection()
    {
        // TODO: Add this test.
    }
}
