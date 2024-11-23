using NUnit.Framework;
using OpenAI.Embeddings;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Embeddings;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Embeddings")]
[Category("Smoke")]
public class EmbeddingsMockTests : SyncAsyncTestBase
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
        EmbeddingClient client = new EmbeddingClient("model", s_fakeCredential, clientOptions);

        OpenAIEmbedding embedding = IsAsync
            ? await client.GenerateEmbeddingAsync("prompt")
            : client.GenerateEmbedding("prompt");

        float[] vector = embedding.ToFloats().ToArray();
        Assert.That(vector.SequenceEqual([1f, 2f, 3f]));
    }

    [Test]
    public void GenerateEmbeddingRespectsTheCancellationToken()
    {
        EmbeddingClient client = new EmbeddingClient("model", s_fakeCredential);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GenerateEmbeddingAsync("prompt", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GenerateEmbedding("prompt", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
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
        EmbeddingClient client = new EmbeddingClient("model", s_fakeCredential, clientOptions);

        OpenAIEmbeddingCollection embeddings = IsAsync
            ? await client.GenerateEmbeddingsAsync(["prompt"])
            : client.GenerateEmbeddings(["prompt"]);

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
        EmbeddingClient client = new EmbeddingClient("model", s_fakeCredential, clientOptions);

        OpenAIEmbeddingCollection embeddings = IsAsync
            ? await client.GenerateEmbeddingsAsync(["prompt"])
            : client.GenerateEmbeddings(["prompt"]);
        OpenAIEmbedding embedding = embeddings.Single();

        float[] vector = embedding.ToFloats().ToArray();
        Assert.That(vector.SequenceEqual([1f, 2f, 3f]));
    }

    [Test]
    public void GenerateEmbeddingsWithStringsRespectsTheCancellationToken()
    {
        EmbeddingClient client = new EmbeddingClient("model", s_fakeCredential);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GenerateEmbeddingsAsync(["prompt"], cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GenerateEmbeddings(["prompt"], cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
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
        EmbeddingClient client = new EmbeddingClient("model", s_fakeCredential, clientOptions);

        OpenAIEmbeddingCollection embeddings = IsAsync
            ? await client.GenerateEmbeddingsAsync([new[] { 1 }])
            : client.GenerateEmbeddings([new[] { 1 }]);

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
        EmbeddingClient client = new EmbeddingClient("model", s_fakeCredential, clientOptions);

        OpenAIEmbeddingCollection embeddings = IsAsync
            ? await client.GenerateEmbeddingsAsync([new[] { 1 }])
            : client.GenerateEmbeddings([new[] { 1 }]);
        OpenAIEmbedding embedding = embeddings.Single();

        float[] vector = embedding.ToFloats().ToArray();
        Assert.That(vector.SequenceEqual([1f, 2f, 3f]));
    }

    [Test]
    public void GenerateEmbeddingsWithIntegersRespectsTheCancellationToken()
    {
        EmbeddingClient client = new EmbeddingClient("model", s_fakeCredential);
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.GenerateEmbeddingsAsync([new[] { 1 }], cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.GenerateEmbeddings([new[] { 1 }], cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }

    private OpenAIClientOptions GetClientOptionsWithMockResponse(int status, string content)
    {
        MockPipelineResponse response = new MockPipelineResponse(status);
        response.SetContent(content);

        return new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };
    }
}
