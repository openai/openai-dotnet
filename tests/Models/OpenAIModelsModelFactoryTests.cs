using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenAI.Models;

namespace OpenAI.Tests.Models;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public partial class OpenAIModelsModelFactoryTests
{
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
