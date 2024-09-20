using NUnit.Framework;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Tests.Models;

[Parallelizable(ParallelScope.All)]
[Category("Models")]
[Category("Smoke")]
public class OpenAIModelsModelFactoryTests
{
    [Test]
    public void ModelDeletionResultWithNoPropertiesWorks()
    {
        ModelDeletionResult modelDeletionResult = OpenAIModelsModelFactory.ModelDeletionResult();

        Assert.That(modelDeletionResult.ModelId, Is.Null);
        Assert.That(modelDeletionResult.Deleted, Is.EqualTo(false));
    }

    [Test]
    public void ModelDeletionResultWithModelIdWorks()
    {
        string modelId = "modelId";
        ModelDeletionResult modelDeletionResult = OpenAIModelsModelFactory.ModelDeletionResult(modelId: modelId);

        Assert.That(modelDeletionResult.ModelId, Is.EqualTo(modelId));
        Assert.That(modelDeletionResult.Deleted, Is.EqualTo(false));
    }

    [Test]
    public void ModelDeletionResultWithDeletedWorks()
    {
        bool deleted = true;
        ModelDeletionResult modelDeletionResult = OpenAIModelsModelFactory.ModelDeletionResult(deleted: deleted);

        Assert.That(modelDeletionResult.ModelId, Is.Null);
        Assert.That(modelDeletionResult.Deleted, Is.EqualTo(deleted));
    }

    [Test]
    public void OpenAIModelInfoWithNoPropertiesWorks()
    {
        OpenAIModelInfo openAIModelInfo = OpenAIModelsModelFactory.OpenAIModelInfo();

        Assert.That(openAIModelInfo.Id, Is.Null);
        Assert.That(openAIModelInfo.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIModelInfo.OwnedBy, Is.Null);
    }

    [Test]
    public void OpenAIModelInfoWithIdWorks()
    {
        string id = "modelId";
        OpenAIModelInfo openAIModelInfo = OpenAIModelsModelFactory.OpenAIModelInfo(id: id);

        Assert.That(openAIModelInfo.Id, Is.EqualTo(id));
        Assert.That(openAIModelInfo.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIModelInfo.OwnedBy, Is.Null);
    }

    [Test]
    public void OpenAIModelInfoWithCreatedAtWorks()
    {
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        OpenAIModelInfo openAIModelInfo = OpenAIModelsModelFactory.OpenAIModelInfo(createdAt: createdAt);

        Assert.That(openAIModelInfo.Id, Is.Null);
        Assert.That(openAIModelInfo.CreatedAt, Is.EqualTo(createdAt));
        Assert.That(openAIModelInfo.OwnedBy, Is.Null);
    }

    [Test]
    public void OpenAIModelInfoWithOwnedByWorks()
    {
        string ownedBy = "The people";
        OpenAIModelInfo openAIModelInfo = OpenAIModelsModelFactory.OpenAIModelInfo(ownedBy: ownedBy);

        Assert.That(openAIModelInfo.Id, Is.Null);
        Assert.That(openAIModelInfo.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIModelInfo.OwnedBy, Is.EqualTo(ownedBy));
    }

    [Test]
    public void OpenAIModelInfoCollectionWithNoPropertiesWorks()
    {
        OpenAIModelInfoCollection openAIModelInfoCollection = OpenAIModelsModelFactory.OpenAIModelInfoCollection();

        Assert.That(openAIModelInfoCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void OpenAIModelInfoCollectionWithItemsWorks()
    {
        IEnumerable<OpenAIModelInfo> items = [
            OpenAIModelsModelFactory.OpenAIModelInfo(id: "firstModel"),
            OpenAIModelsModelFactory.OpenAIModelInfo(id: "secondModel")
        ];
        OpenAIModelInfoCollection openAIModelInfoCollection = OpenAIModelsModelFactory.OpenAIModelInfoCollection(items: items);

        Assert.That(openAIModelInfoCollection.SequenceEqual(items), Is.True);
    }
}
