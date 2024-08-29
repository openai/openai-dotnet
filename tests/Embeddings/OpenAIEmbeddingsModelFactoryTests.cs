using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenAI.Embeddings;

namespace OpenAI.Tests.Embeddings;

[Parallelizable(ParallelScope.All)]
[Category("Smoke")]
public partial class OpenAIEmbeddingsModelFactoryTests
{
    [Test]
    public void EmbeddingWithNoPropertiesWorks()
    {
        Embedding embedding = OpenAIEmbeddingsModelFactory.Embedding();

        Assert.That(embedding.Index, Is.EqualTo(default(int)));
        Assert.That(embedding.Vector.ToArray(), Is.Not.Null.And.Empty);
    }

    [Test]
    public void EmbeddingWithIndexWorks()
    {
        int index = 10;
        Embedding embedding = OpenAIEmbeddingsModelFactory.Embedding(index: index);

        Assert.That(embedding.Index, Is.EqualTo(index));
        Assert.That(embedding.Vector.ToArray(), Is.Not.Null.And.Empty);
    }

    [Test]
    public void EmbeddingWithVectorWorks()
    {
        IEnumerable<float> vector = [1f, 2f, 3f];
        Embedding embedding = OpenAIEmbeddingsModelFactory.Embedding(vector: vector);

        Assert.That(embedding.Index, Is.EqualTo(default(int)));
        Assert.That(embedding.Vector.ToArray().SequenceEqual(vector), Is.True);
    }

    [Test]
    public void EmbeddingCollectionWithNoPropertiesWorks()
    {
        EmbeddingCollection embeddingCollection = OpenAIEmbeddingsModelFactory.EmbeddingCollection();

        Assert.That(embeddingCollection.Count, Is.EqualTo(0));
        Assert.That(embeddingCollection.Model, Is.Null);
        Assert.That(embeddingCollection.Usage, Is.Null);
    }

    [Test]
    public void EmbeddingCollectionWithItemsWorks()
    {
        IEnumerable<Embedding> items = [
            OpenAIEmbeddingsModelFactory.Embedding(index: 10),
            OpenAIEmbeddingsModelFactory.Embedding(index: 20)
        ];
        EmbeddingCollection embeddingCollection = OpenAIEmbeddingsModelFactory.EmbeddingCollection(items: items);

        Assert.That(embeddingCollection.SequenceEqual(items), Is.True);
        Assert.That(embeddingCollection.Model, Is.Null);
        Assert.That(embeddingCollection.Usage, Is.Null);
    }

    [Test]
    public void EmbeddingCollectionWithModelWorks()
    {
        string model = "supermodel";
        EmbeddingCollection embeddingCollection = OpenAIEmbeddingsModelFactory.EmbeddingCollection(model: model);

        Assert.That(embeddingCollection.Count, Is.EqualTo(0));
        Assert.That(embeddingCollection.Model, Is.EqualTo(model));
        Assert.That(embeddingCollection.Usage, Is.Null);
    }

    [Test]
    public void EmbeddingCollectionWithUsageWorks()
    {
        EmbeddingTokenUsage usage = OpenAIEmbeddingsModelFactory.EmbeddingTokenUsage(inputTokens: 10);
        EmbeddingCollection embeddingCollection = OpenAIEmbeddingsModelFactory.EmbeddingCollection(usage: usage);

        Assert.That(embeddingCollection.Count, Is.EqualTo(0));
        Assert.That(embeddingCollection.Model, Is.Null);
        Assert.That(embeddingCollection.Usage, Is.EqualTo(usage));
    }

    [Test]
    public void EmbeddingTokenUsageWithNoPropertiesWorks()
    {
        EmbeddingTokenUsage embeddingTokenUsage = OpenAIEmbeddingsModelFactory.EmbeddingTokenUsage();

        Assert.That(embeddingTokenUsage.InputTokens, Is.EqualTo(default(int)));
        Assert.That(embeddingTokenUsage.TotalTokens, Is.EqualTo(default(int)));
    }

    [Test]
    public void EmbeddingTokenUsageWithInputTokensWorks()
    {
        int inputTokens = 10;
        EmbeddingTokenUsage embeddingTokenUsage = OpenAIEmbeddingsModelFactory.EmbeddingTokenUsage(inputTokens: inputTokens);

        Assert.That(embeddingTokenUsage.InputTokens, Is.EqualTo(10));
        Assert.That(embeddingTokenUsage.TotalTokens, Is.EqualTo(default(int)));
    }

    [Test]
    public void EmbeddingTokenUsageWithTotalTokensWorks()
    {
        int totalTokens = 10;
        EmbeddingTokenUsage embeddingTokenUsage = OpenAIEmbeddingsModelFactory.EmbeddingTokenUsage(totalTokens: totalTokens);

        Assert.That(embeddingTokenUsage.InputTokens, Is.EqualTo(default(int)));
        Assert.That(embeddingTokenUsage.TotalTokens, Is.EqualTo(totalTokens));
    }
}
