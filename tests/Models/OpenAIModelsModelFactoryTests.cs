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
    public void OpenAIModelWithNoPropertiesWorks()
    {
        OpenAIModel openAIModel = OpenAIModelsModelFactory.OpenAIModel();

        Assert.That(openAIModel.Id, Is.Null);
        Assert.That(openAIModel.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIModel.OwnedBy, Is.Null);
    }

    [Test]
    public void OpenAIModelWithIdWorks()
    {
        string id = "modelId";
        OpenAIModel openAIModel = OpenAIModelsModelFactory.OpenAIModel(id: id);

        Assert.That(openAIModel.Id, Is.EqualTo(id));
        Assert.That(openAIModel.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIModel.OwnedBy, Is.Null);
    }

    [Test]
    public void OpenAIModelWithCreatedAtWorks()
    {
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        OpenAIModel openAIModel = OpenAIModelsModelFactory.OpenAIModel(createdAt: createdAt);

        Assert.That(openAIModel.Id, Is.Null);
        Assert.That(openAIModel.CreatedAt, Is.EqualTo(createdAt));
        Assert.That(openAIModel.OwnedBy, Is.Null);
    }

    [Test]
    public void OpenAIModelWithOwnedByWorks()
    {
        string ownedBy = "The people";
        OpenAIModel openAIModel = OpenAIModelsModelFactory.OpenAIModel(ownedBy: ownedBy);

        Assert.That(openAIModel.Id, Is.Null);
        Assert.That(openAIModel.CreatedAt, Is.EqualTo(default(DateTimeOffset)));
        Assert.That(openAIModel.OwnedBy, Is.EqualTo(ownedBy));
    }

    [Test]
    public void OpenAIModelCollectionWithNoPropertiesWorks()
    {
        OpenAIModelCollection openAIModelCollection = OpenAIModelsModelFactory.OpenAIModelCollection();

        Assert.That(openAIModelCollection.Count, Is.EqualTo(0));
    }

    [Test]
    public void OpenAIModelCollectionWithItemsWorks()
    {
        IEnumerable<OpenAIModel> items = [
            OpenAIModelsModelFactory.OpenAIModel(id: "firstModel"),
            OpenAIModelsModelFactory.OpenAIModel(id: "secondModel")
        ];
        OpenAIModelCollection openAIModelCollection = OpenAIModelsModelFactory.OpenAIModelCollection(items: items);

        Assert.That(openAIModelCollection.SequenceEqual(items), Is.True);
    }
}
