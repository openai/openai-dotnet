using Moq;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Embeddings;
using System;
using System.Collections.Generic;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Utility;

[TestFixture]
[Category("Utility")]
public class ChatToolsTests
{
    private class TestTools
    {
        public static string Echo(string message) => message;
        public static int Add(int a, int b) => a + b;
    }

    private Mock<EmbeddingClient> mockEmbeddingClient;

    [SetUp]
    public void Setup()
    {
        mockEmbeddingClient = new Mock<EmbeddingClient>("text-embedding-ada-002", new ApiKeyCredential("test-key"));
    }

    [Test]
    public void CanAddLocalTools()
    {
        var tools = new ChatTools();
        tools.AddLocalTools(typeof(TestTools));

        Assert.That(tools.Tools, Has.Count.EqualTo(2));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "Echo"));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "Add"));
    }

    [Test]
    public async Task CanCallToolsAsync()
    {
        var tools = new ChatTools();
        tools.AddLocalTools(typeof(TestTools));

        var toolCalls = new[]
        {
            ChatToolCall.CreateFunctionToolCall("call1", "Echo", BinaryData.FromString(@"{""message"": ""Hello""}")),
            ChatToolCall.CreateFunctionToolCall("call2", "Add", BinaryData.FromString(@"{""a"": 2, ""b"": 3}"))
        };

        var results = await tools.CallAsync(toolCalls);
        var resultsList = results.ToList();

        Assert.That(resultsList, Has.Count.EqualTo(2));
        Assert.That(resultsList[0].ToolCallId, Is.EqualTo("call1"));
        Assert.That(resultsList[0].Content[0].Text, Is.EqualTo("Hello"));
        Assert.That(resultsList[1].ToolCallId, Is.EqualTo("call2"));
        Assert.That(resultsList[1].Content[0].Text, Is.EqualTo("5"));
    }

    [Test]
    public void CreatesCompletionOptionsWithTools()
    {
        var tools = new ChatTools();
        tools.AddLocalTools(typeof(TestTools));

        var options = tools.CreateCompletionOptions();

        Assert.That(options.Tools, Has.Count.EqualTo(2));
        Assert.That(options.Tools.Any(t => t.FunctionName == "Echo"));
        Assert.That(options.Tools.Any(t => t.FunctionName == "Add"));
    }

    [Test]
    public async Task CanFilterToolsByRelevance()
    {
        // Setup mock embedding client to return a mock response
        var embedding = OpenAIEmbeddingsModelFactory.OpenAIEmbedding(vector: new[] { 0.5f, 0.5f });
        var embeddingCollection = OpenAIEmbeddingsModelFactory.OpenAIEmbeddingCollection(
            items: new[] { embedding },
            model: "text-embedding-ada-002",
            usage: OpenAIEmbeddingsModelFactory.EmbeddingTokenUsage(10, 10));
        var mockResponse = new MockPipelineResponse(200);

        mockEmbeddingClient
            .Setup(c => c.GenerateEmbeddingAsync(
                It.IsAny<string>(),
                It.IsAny<EmbeddingGenerationOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientResult.FromValue(embedding, mockResponse));

        var tools = new ChatTools(mockEmbeddingClient.Object);
        tools.AddLocalTools(typeof(TestTools));

        var options = await Task.Run(() => tools.CreateCompletionOptions("Need to add two numbers", 1, 0.5f));

        Assert.That(options.Tools, Has.Count.LessThanOrEqualTo(1));
    }

    [Test]
    public void ImplicitConversionToCompletionOptions()
    {
        var tools = new ChatTools();
        tools.AddLocalTools(typeof(TestTools));

        ChatCompletionOptions options = tools;

        Assert.That(options.Tools, Has.Count.EqualTo(2));
        Assert.That(options.Tools.Any(t => t.FunctionName == "Echo"));
        Assert.That(options.Tools.Any(t => t.FunctionName == "Add"));
    }

    [Test]
    public void ThrowsWhenCallingNonExistentTool()
    {
        var tools = new ChatTools();
        var toolCalls = new[]
        {
            ChatToolCall.CreateFunctionToolCall("call1", "NonExistentTool", BinaryData.FromString("{}"))
        };

        Assert.ThrowsAsync<InvalidOperationException>(() => tools.CallAsync(toolCalls));
    }
}