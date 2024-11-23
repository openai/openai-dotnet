using NUnit.Framework;
using OpenAI.Embeddings;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Embeddings;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Embeddings")]
public class EmbeddingsTests : SyncAsyncTestBase
{
    private static EmbeddingClient GetTestClient() => GetTestClient<EmbeddingClient>(TestScenario.Embeddings);

    public EmbeddingsTests(bool isAsync) : base(isAsync)
    {
    }

    public enum EmbeddingsInputKind
    {
        UsingStrings,
        UsingIntegers,
    }

    [Test]
    public async Task GenerateSingleEmbedding()
    {
        EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        string input = "Hello, world!";

        OpenAIEmbedding embedding = IsAsync
            ? await client.GenerateEmbeddingAsync(input)
            : client.GenerateEmbedding(input);
        Assert.That(embedding, Is.Not.Null);
        Assert.That(embedding.Index, Is.EqualTo(0));

        ReadOnlyMemory<float> vector = embedding.ToFloats();
        Assert.That(vector, Is.Not.Null);
        Assert.That(vector.Span.Length, Is.EqualTo(1536));

        float[] array = vector.ToArray();
        Assert.That(array.Length, Is.EqualTo(1536));
    }

    [Test]
    [TestCase(EmbeddingsInputKind.UsingStrings)]
    [TestCase(EmbeddingsInputKind.UsingIntegers)]
    public async Task GenerateMultipleEmbeddings(EmbeddingsInputKind embeddingsInputKind)
    {
        EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        const int Dimensions = 456;

        EmbeddingGenerationOptions options = new()
        {
            Dimensions = Dimensions,
        };

        OpenAIEmbeddingCollection embeddings = null;

        if (embeddingsInputKind == EmbeddingsInputKind.UsingStrings)
        {
            List<string> prompts =
            [
                "Hello, world!",
                "This is a test.",
                "Goodbye!"
            ];

            embeddings = IsAsync
                ? await client.GenerateEmbeddingsAsync(prompts, options)
                : client.GenerateEmbeddings(prompts, options);
        }
        else if (embeddingsInputKind == EmbeddingsInputKind.UsingIntegers)
        {
            List<ReadOnlyMemory<int>> prompts =
            [
                new[] { 104, 101, 108, 108, 111 },
                new[] { 119, 111, 114, 108, 100 },
                new[] { 84, 69, 83, 84 }
            ];

            embeddings = IsAsync
                ? await client.GenerateEmbeddingsAsync(prompts, options)
                : client.GenerateEmbeddings(prompts, options);
        }

        Assert.That(embeddings, Is.Not.Null);
        Assert.That(embeddings.Count, Is.EqualTo(3));
        Assert.That(embeddings.Model, Is.EqualTo("text-embedding-3-small"));
        Assert.That(embeddings.Usage.InputTokenCount, Is.GreaterThan(0));
        Assert.That(embeddings.Usage.TotalTokenCount, Is.GreaterThan(0));

        for (int i = 0; i < embeddings.Count; i++)
        {
            Assert.That(embeddings[i].Index, Is.EqualTo(i));

            ReadOnlyMemory<float> vector = embeddings[i].ToFloats();
            Assert.That(vector, Is.Not.Null);
            Assert.That(vector.Span.Length, Is.EqualTo(Dimensions));

            float[] array = vector.ToArray();
            Assert.That(array.Length, Is.EqualTo(Dimensions));
        }
    }

    [Test]
    public async Task BadOptions()
    {
        EmbeddingClient client = GetTestClient();

        EmbeddingGenerationOptions options = new()
        {
            Dimensions = -42,
        };

        Exception caughtException = null;

        try
        {
            _ = IsAsync
                ? await client.GenerateEmbeddingAsync("foo", options)
                : client.GenerateEmbedding("foo", options);
        }
        catch (Exception ex)
        {
            caughtException = ex;
        }

        Assert.That(caughtException, Is.InstanceOf<ClientResultException>());
        Assert.That(caughtException.Message, Contains.Substring("dimensions"));
    }

    [Test]
    [TestCase(EmbeddingsInputKind.UsingStrings)]
    [TestCase(EmbeddingsInputKind.UsingIntegers)]
    public async Task GenerateMultipleEmbeddingsWithBadOptions(EmbeddingsInputKind embeddingsInputKind)
    {
        EmbeddingClient client = GetTestClient();

        EmbeddingGenerationOptions options = new()
        {
            Dimensions = -42,
        };

        Exception caughtException = null;

        try
        {
            if (embeddingsInputKind == EmbeddingsInputKind.UsingStrings)
            {
                _ = IsAsync
                    ? await client.GenerateEmbeddingsAsync(["prompt"], options)
                    : client.GenerateEmbeddings(["prompt"], options);
            }
            else if (embeddingsInputKind == EmbeddingsInputKind.UsingIntegers)
            {
                _ = IsAsync
                    ? await client.GenerateEmbeddingsAsync([new[] { 1 }], options)
                    : client.GenerateEmbeddings([new[] { 1 }], options);
            }
        }
        catch (Exception ex)
        {
            caughtException = ex;
        }

        Assert.That(caughtException, Is.InstanceOf<ClientResultException>());
        Assert.That(caughtException.Message, Contains.Substring("dimensions"));
    }

    [Test]
    public async Task EmbeddingFromStringAndEmbeddingFromTokenIdsAreEqual()
    {
        EmbeddingClient client = GetTestClient();

        List<string> input1 = new List<string> { "Hello, world!" };
        List<ReadOnlyMemory<int>> input2 = new List<ReadOnlyMemory<int>> { new[] { 9906, 11, 1917, 0 } };

        OpenAIEmbeddingCollection results1 = IsAsync
            ? await client.GenerateEmbeddingsAsync(input1)
            : client.GenerateEmbeddings(input1);

        OpenAIEmbeddingCollection results2 = IsAsync
            ? await client.GenerateEmbeddingsAsync(input2)
            : client.GenerateEmbeddings(input2);

        ReadOnlyMemory<float> vector1 = results1[0].ToFloats();
        ReadOnlyMemory<float> vector2 = results2[0].ToFloats();

        for (int i = 0; i < vector1.Length; i++)
        {
            Assert.That(vector1.Span[i], Is.EqualTo(vector2.Span[i]).Within(0.0005));
        }
    }

    [Test]
    public void SerializeEmbeddingCollection()
    {
        // TODO: Add this test.
    }
}
