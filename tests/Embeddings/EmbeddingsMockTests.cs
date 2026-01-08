using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Embeddings;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Embeddings;

[Parallelizable(ParallelScope.All)]
[Category("Embeddings")]
[Category("Smoke")]
public class EmbeddingsMockTests : ClientTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    public EmbeddingsMockTests(bool isAsync)
        : base(isAsync)
    {
    }

    [Test]
    public async Task GenerateEmbeddingDeserializesVector()
    {
        string base64Embedding = Convert.ToBase64String([
            0x00,
            0x00,
            0x80,
            0x3F, // 1.0f
            0x00,
            0x00,
            0x00,
            0x40, // 2.0f
            0x00,
            0x00,
            0x40,
            0x40  // 3.0f
        ]);
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "data": [
                {
                    "embedding": "{{base64Embedding}}"
                }
            ]
        }
        """);
        EmbeddingClient client = CreateProxyFromClient(new EmbeddingClient("model", s_fakeCredential, clientOptions));

        OpenAIEmbedding embedding = await client.GenerateEmbeddingAsync("prompt");

        float[] vector = embedding.ToFloats().ToArray();
        Assert.That(vector.SequenceEqual([1f, 2f, 3f]));
    }

    [Test]
    public void GenerateEmbeddingRespectsTheCancellationToken()
    {
        EmbeddingClient client = CreateProxyFromClient(new EmbeddingClient("model", s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GenerateEmbeddingAsync("prompt", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public async Task GenerateEmbeddingsWithStringsDeserializesUsage()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [],
            "usage": {
                "prompt_tokens": 10,
                "total_tokens": 20
            }
        }
        """);
        EmbeddingClient client = CreateProxyFromClient(new EmbeddingClient("model", s_fakeCredential, clientOptions));

        OpenAIEmbeddingCollection embeddings = await client.GenerateEmbeddingsAsync(["prompt"]);

        Assert.That(embeddings.Usage.InputTokenCount, Is.EqualTo(10));
        Assert.That(embeddings.Usage.TotalTokenCount, Is.EqualTo(20));
    }

    [Test]
    public async Task GenerateEmbeddingsWithStringsDeserializesVector()
    {
        string base64Embedding = Convert.ToBase64String([
            0x00,
            0x00,
            0x80,
            0x3F, // 1.0f
            0x00,
            0x00,
            0x00,
            0x40, // 2.0f
            0x00,
            0x00,
            0x40,
            0x40  // 3.0f
        ]);
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "data": [
                {
                    "embedding": "{{base64Embedding}}"
                }
            ]
        }
        """);
        EmbeddingClient client = CreateProxyFromClient(new EmbeddingClient("model", s_fakeCredential, clientOptions));

        OpenAIEmbeddingCollection embeddings = await client.GenerateEmbeddingsAsync(["prompt"]);
        OpenAIEmbedding embedding = embeddings.Single();

        float[] vector = embedding.ToFloats().ToArray();
        Assert.That(vector.SequenceEqual([1f, 2f, 3f]));
    }

    [Test]
    public void GenerateEmbeddingsWithStringsRespectsTheCancellationToken()
    {
        EmbeddingClient client = CreateProxyFromClient(new EmbeddingClient("model", s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GenerateEmbeddingsAsync(["prompt"], cancellationToken: cancellationSource.Token),
            Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public async Task GenerateEmbeddingsWithIntegersDeserializesUsage()
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "data": [],
            "usage": {
                "prompt_tokens": 10,
                "total_tokens": 20
            }
        }
        """);
        EmbeddingClient client = CreateProxyFromClient(new EmbeddingClient("model", s_fakeCredential, clientOptions));

        OpenAIEmbeddingCollection embeddings = await client.GenerateEmbeddingsAsync([new[] { 1 }]);

        Assert.That(embeddings.Usage.InputTokenCount, Is.EqualTo(10));
        Assert.That(embeddings.Usage.TotalTokenCount, Is.EqualTo(20));
    }

    [Test]
    public async Task GenerateEmbeddingsWithIntegersDeserializesVector()
    {
        string base64Embedding = Convert.ToBase64String([
            0x00,
            0x00,
            0x80,
            0x3F, // 1.0f
            0x00,
            0x00,
            0x00,
            0x40, // 2.0f
            0x00,
            0x00,
            0x40,
            0x40  // 3.0f
        ]);
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, $$"""
        {
            "data": [
                {
                    "embedding": "{{base64Embedding}}"
                }
            ]
        }
        """);
        EmbeddingClient client = CreateProxyFromClient(new EmbeddingClient("model", s_fakeCredential, clientOptions));

        OpenAIEmbeddingCollection embeddings = await client.GenerateEmbeddingsAsync([new[] { 1 }]);
        OpenAIEmbedding embedding = embeddings.Single();

        float[] vector = embedding.ToFloats().ToArray();
        Assert.That(vector.SequenceEqual([1f, 2f, 3f]));
    }

    [Test]
    public void GenerateEmbeddingsWithIntegersRespectsTheCancellationToken()
    {
        EmbeddingClient client = CreateProxyFromClient(new EmbeddingClient("model", s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GenerateEmbeddingsAsync([new[] { 1 }], cancellationToken: cancellationSource.Token),
            Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public void SerializeEmbeddingCollection()
    {
        // TODO: Add this test.
    }

    [Test]
    public void JsonArraySupport()
    {
        string json = """
        {
          "object":"list",
          "data":[
            {
              "object":"embedding",
              "embedding":[-0.011229509,0.107915245,-0.15163477]
            }
          ]
        }
        """;

        BinaryData binaryData = BinaryData.FromString(json);

        OpenAIEmbeddingCollection embeddings = ModelReaderWriter.Read<OpenAIEmbeddingCollection>(binaryData);

        Assert.That(embeddings, Is.Not.Null);
        Assert.That(embeddings.Count, Is.EqualTo(1));
        var embedding = embeddings[0];
        Assert.That(embedding, Is.Not.Null);
        ReadOnlySpan<float> vector = embedding.ToFloats().Span;
        Assert.That(vector.Length, Is.EqualTo(3));
        Assert.That(vector[0], Is.EqualTo(-0.011229509f));
        Assert.That(vector[1], Is.EqualTo(0.107915245f));
        Assert.That(vector[2], Is.EqualTo(-0.15163477f));
    }

    private OpenAIClientOptions GetClientOptionsWithMockResponse(int status, string content)
    {
        MockPipelineResponse response = new MockPipelineResponse(status).WithContent(content);

        return new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(_ => response)
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
    }
}
