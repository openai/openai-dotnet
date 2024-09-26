using NUnit.Framework;
using OpenAI.Embeddings;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Tests.Embeddings;

[Parallelizable(ParallelScope.All)]
[Category("Embeddings")]
[Category("Smoke")]
public class OpenAIEmbeddingsModelFactoryTests
{
    [Test]
    public void EmbeddingWithNoPropertiesWorks()
    {
        OpenAIEmbedding embedding = OpenAIEmbeddingsModelFactory.OpenAIEmbedding();

        Assert.That(embedding.Index, Is.EqualTo(default(int)));
        Assert.That(embedding.ToFloats().ToArray(), Is.Not.Null.And.Empty);
    }

    [Test]
    public void EmbeddingWithIndexWorks()
    {
        int index = 10;
        OpenAIEmbedding embedding = OpenAIEmbeddingsModelFactory.OpenAIEmbedding(index: index);

        Assert.That(embedding.Index, Is.EqualTo(index));
        Assert.That(embedding.ToFloats().ToArray(), Is.Not.Null.And.Empty);
    }

    [Test]
    public void EmbeddingWithVectorWorks()
    {
        IEnumerable<float> vector = [1f, 2f, 3f];
        OpenAIEmbedding embedding = OpenAIEmbeddingsModelFactory.OpenAIEmbedding(vector: vector);

        Assert.That(embedding.Index, Is.EqualTo(default(int)));
        Assert.That(embedding.ToFloats().ToArray().SequenceEqual(vector), Is.True);
    }

    [Test]
    public void EmbeddingCollectionWithNoPropertiesWorks()
    {
        OpenAIEmbeddingCollection embeddingCollection = OpenAIEmbeddingsModelFactory.OpenAIEmbeddingCollection();

        Assert.That(embeddingCollection.Count, Is.EqualTo(0));
        Assert.That(embeddingCollection.Model, Is.Null);
        Assert.That(embeddingCollection.Usage, Is.Null);
    }

    [Test]
    public void EmbeddingCollectionWithItemsWorks()
    {
        IEnumerable<OpenAIEmbedding> items = [
            OpenAIEmbeddingsModelFactory.OpenAIEmbedding(index: 10),
            OpenAIEmbeddingsModelFactory.OpenAIEmbedding(index: 20)
        ];
        OpenAIEmbeddingCollection embeddingCollection = OpenAIEmbeddingsModelFactory.OpenAIEmbeddingCollection(items: items);

        Assert.That(embeddingCollection.SequenceEqual(items), Is.True);
        Assert.That(embeddingCollection.Model, Is.Null);
        Assert.That(embeddingCollection.Usage, Is.Null);
    }

    [Test]
    public void EmbeddingCollectionWithModelWorks()
    {
        string model = "supermodel";
        OpenAIEmbeddingCollection embeddingCollection = OpenAIEmbeddingsModelFactory.OpenAIEmbeddingCollection(model: model);

        Assert.That(embeddingCollection.Count, Is.EqualTo(0));
        Assert.That(embeddingCollection.Model, Is.EqualTo(model));
        Assert.That(embeddingCollection.Usage, Is.Null);
    }

    [Test]
    public void EmbeddingCollectionWithUsageWorks()
    {
        EmbeddingTokenUsage usage = OpenAIEmbeddingsModelFactory.EmbeddingTokenUsage(inputTokenCount: 10);
        OpenAIEmbeddingCollection embeddingCollection = OpenAIEmbeddingsModelFactory.OpenAIEmbeddingCollection(usage: usage);

        Assert.That(embeddingCollection.Count, Is.EqualTo(0));
        Assert.That(embeddingCollection.Model, Is.Null);
        Assert.That(embeddingCollection.Usage, Is.EqualTo(usage));
    }

    [Test]
    public void EmbeddingTokenUsageWithNoPropertiesWorks()
    {
        EmbeddingTokenUsage embeddingTokenUsage = OpenAIEmbeddingsModelFactory.EmbeddingTokenUsage();

        Assert.That(embeddingTokenUsage.InputTokenCount, Is.EqualTo(default(int)));
        Assert.That(embeddingTokenUsage.TotalTokenCount, Is.EqualTo(default(int)));
    }

    [Test]
    public void EmbeddingTokenUsageWithInputTokensWorks()
    {
        int inputTokens = 10;
        EmbeddingTokenUsage embeddingTokenUsage = OpenAIEmbeddingsModelFactory.EmbeddingTokenUsage(inputTokenCount: inputTokens);

        Assert.That(embeddingTokenUsage.InputTokenCount, Is.EqualTo(10));
        Assert.That(embeddingTokenUsage.TotalTokenCount, Is.EqualTo(default(int)));
    }

    [Test]
    public void EmbeddingTokenUsageWithTotalTokensWorks()
    {
        int totalTokens = 10;
        EmbeddingTokenUsage embeddingTokenUsage = OpenAIEmbeddingsModelFactory.EmbeddingTokenUsage(totalTokenCount: totalTokens);

        Assert.That(embeddingTokenUsage.InputTokenCount, Is.EqualTo(default(int)));
        Assert.That(embeddingTokenUsage.TotalTokenCount, Is.EqualTo(totalTokens));
    }
}
