using Moq;
using NUnit.Framework;
using OpenAI.Embeddings;
using OpenAI.Responses;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests.Utility;

[TestFixture]
[Category("Utility")]
public class ResponseToolsTests
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
        var tools = new ResponseTools();
        tools.AddLocalTools(typeof(TestTools));

        Assert.That(tools.Tools, Has.Count.EqualTo(2));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Echo")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Add")));
    }

    [Test]
    public async Task CanCallToolAsync()
    {
        var tools = new ResponseTools();
        tools.AddLocalTools(typeof(TestTools));

        var toolCall = new FunctionCallResponseItem("call1", "Echo", BinaryData.FromString(@"{""message"": ""Hello""}"));
        var result = await tools.CallAsync(toolCall);

        Assert.That(result.CallId, Is.EqualTo("call1"));
        Assert.That(result.ToString(), Is.EqualTo("Hello"));

        var addCall = new FunctionCallResponseItem("call2", "Add", BinaryData.FromString(@"{""a"": 2, ""b"": 3}"));
        result = await tools.CallAsync(addCall);

        Assert.That(result.CallId, Is.EqualTo("call2"));
        Assert.That(result.ToString(), Is.EqualTo("5"));
    }

    [Test]
    public void CreatesResponseOptionsWithTools()
    {
        var tools = new ResponseTools();
        tools.AddLocalTools(typeof(TestTools));

        var options = tools.CreateResponseOptions();

        Assert.That(options.Tools, Has.Count.EqualTo(2));
        Assert.That(options.Tools.Any(t => t.ToString().Contains("Echo")));
        Assert.That(options.Tools.Any(t => t.ToString().Contains("Add")));
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
            .Setup(c => c.GenerateEmbeddingsAsync(
                It.IsAny<BinaryContent>(),
                It.IsAny<RequestOptions>()))
            .ReturnsAsync(ClientResult.FromValue(embeddingCollection, mockResponse));

        var tools = new ResponseTools(mockEmbeddingClient.Object);
        tools.AddLocalTools(typeof(TestTools));

        var options = await Task.Run(() => tools.CreateResponseOptions("Need to add two numbers", 1, 0.5f));

        Assert.That(options.Tools, Has.Count.LessThanOrEqualTo(1));
        mockEmbeddingClient.Verify(
            c => c.GenerateEmbeddingsAsync(
                It.IsAny<BinaryContent>(),
                It.IsAny<RequestOptions>()),
            Times.Once);
    }

    [Test]
    public void ImplicitConversionToResponseOptions()
    {
        var tools = new ResponseTools();
        tools.AddLocalTools(typeof(TestTools));

        ResponseCreationOptions options = tools;

        Assert.That(options.Tools, Has.Count.EqualTo(2));
        Assert.That(options.Tools.Any(t => t.ToString().Contains("Echo")));
        Assert.That(options.Tools.Any(t => t.ToString().Contains("Add")));
    }

    [Test]
    public async Task ReturnsErrorForNonExistentTool()
    {
        var tools = new ResponseTools();
        var toolCall = new FunctionCallResponseItem("call1", "NonExistentTool", BinaryData.FromString("{}"));

        var result = await tools.CallAsync(toolCall);
        Assert.That(result.ToString(), Does.StartWith("I don't have a tool called"));
    }
}