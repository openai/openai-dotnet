using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OpenAI.Embeddings;
using OpenAI.Responses;
using System.ClientModel;
using System.ClientModel.Primitives;
using OpenAI.Agents;

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
        Assert.That(result.FunctionOutput, Is.EqualTo("Hello"));

        var addCall = new FunctionCallResponseItem("call2", "Add", BinaryData.FromString(@"{""a"": 2, ""b"": 3}"));
        result = await tools.CallAsync(addCall);

        Assert.That(result.CallId, Is.EqualTo("call2"));
        Assert.That(result.FunctionOutput, Is.EqualTo("5"));
    }

    [Test]
    public void CreatesResponseOptionsWithTools()
    {
        var tools = new ResponseTools();
        tools.AddLocalTools(typeof(TestTools));

        var options = tools.CreateResponseOptions();

        Assert.That(options.Tools, Has.Count.EqualTo(2));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Echo")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Add")));
    }

    [Test]
    public async Task CanFilterToolsByRelevance()
    {
        // Setup mock embedding client to return a mock response
        var embeddings = new[]
        {
            OpenAIEmbeddingsModelFactory.OpenAIEmbedding(vector: new[] { 0.8f, 0.5f }),
            OpenAIEmbeddingsModelFactory.OpenAIEmbedding(vector: new[] { 0.6f, 0.4f }),
            OpenAIEmbeddingsModelFactory.OpenAIEmbedding(vector: new[] { 0.3f, 0.2f })
        };
        var embeddingCollection = OpenAIEmbeddingsModelFactory.OpenAIEmbeddingCollection(
            items: embeddings,
            model: "text-embedding-ada-002",
            usage: OpenAIEmbeddingsModelFactory.EmbeddingTokenUsage(30, 30));
        var mockResponse = new MockPipelineResponse(200);

        mockEmbeddingClient
            .Setup(c => c.GenerateEmbeddingAsync(
                It.IsAny<string>(),
                It.IsAny<EmbeddingGenerationOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientResult.FromValue(embeddings[0], mockResponse));

        mockEmbeddingClient
            .Setup(c => c.GenerateEmbeddingsAsync(
                It.IsAny<IList<string>>(),
                It.IsAny<EmbeddingGenerationOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientResult.FromValue(embeddingCollection, mockResponse));

        var tools = new ResponseTools(mockEmbeddingClient.Object);
        tools.AddLocalTools(typeof(TestTools));

        var options = await Task.Run(() => tools.CreateResponseOptions("Need to add two numbers", 1, 0.5f));

        Assert.That(options.Tools, Has.Count.LessThanOrEqualTo(1));
    }

    [Test]
    public void ImplicitConversionToResponseOptions()
    {
        var tools = new ResponseTools();
        tools.AddLocalTools(typeof(TestTools));

        ResponseCreationOptions options = tools;

        Assert.That(options.Tools, Has.Count.EqualTo(2));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Echo")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Add")));
    }

    [Test]
    public async Task ReturnsErrorForNonExistentTool()
    {
        var tools = new ResponseTools();
        var toolCall = new FunctionCallResponseItem("call1", "NonExistentTool", BinaryData.FromString("{}"));

        var result = await tools.CallAsync(toolCall);
        Assert.That(result.FunctionOutput, Does.StartWith("I don't have a tool called"));
    }

    [Test]
    public async Task AddMcpToolsAsync_AddsToolsCorrectly()
    {
        // Arrange
        var mockMcpClient = new Mock<McpClient>(new Uri("http://localhost:1234"));
        var tools = new ResponseTools();

        var mockToolsResponse = BinaryData.FromString(@"
        {
            ""tools"": [
                {
                    ""name"": ""mcp-tool-1"",
                    ""description"": ""This is the first MCP tool."",
                    ""inputSchema"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""param1"": {
                                ""type"": ""string"",
                                ""description"": ""The first param.""
                            },
                            ""param2"": {
                                ""type"": ""string"",
                                ""description"": ""The second param.""
                            }
                        },
                        ""required"": [""param1""]
                    }
                },
                {
                    ""name"": ""mcp-tool-2"",
                    ""description"": ""This is the second MCP tool."",
                    ""inputSchema"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""param1"": {
                                ""type"": ""string"",
                                ""description"": ""The first param.""
                            },
                            ""param2"": {
                                ""type"": ""string"",
                                ""description"": ""The second param.""
                            }
                        },
                        ""required"": []
                    }
                }
            ]
        }");

        mockMcpClient.Setup(c => c.StartAsync())
            .Returns(Task.CompletedTask);
        mockMcpClient.Setup(c => c.ListToolsAsync())
            .ReturnsAsync(mockToolsResponse);
        mockMcpClient.Setup(c => c.CallToolAsync(It.IsAny<string>(), It.IsAny<BinaryData>()))
            .ReturnsAsync(BinaryData.FromString("\"test result\""));
        mockMcpClient.SetupGet(c => c.Endpoint)
            .Returns(new Uri("http://localhost:1234"));

        // Act
        await tools.AddMcpToolsAsync(mockMcpClient.Object);

        // Assert
        Assert.That(tools.Tools, Has.Count.EqualTo(2));
        var toolNames = tools.Tools.Select(t => (string)t.GetType().GetProperty("Name").GetValue(t)).ToList();
        Assert.That(toolNames, Contains.Item("localhost1234_-_mcp-tool-1"));
        Assert.That(toolNames, Contains.Item("localhost1234_-_mcp-tool-2"));

        // Verify we can call the tools
        var toolCall = new FunctionCallResponseItem("call1", "localhost1234_-_mcp-tool-1", BinaryData.FromString(@"{""param1"": ""test""}"));
        var result = await tools.CallAsync(toolCall);
        Assert.That(result.FunctionOutput, Is.EqualTo("\"test result\""));

        // Verify expected interactions
        mockMcpClient.Verify(c => c.StartAsync(), Times.Once);
        mockMcpClient.Verify(c => c.ListToolsAsync(), Times.Once);
        mockMcpClient.Verify(c => c.CallToolAsync("mcp-tool-1", It.IsAny<BinaryData>()), Times.Once);
    }

    [Test]
    public async Task CreateResponseOptions_WithMaxToolsParameter_FiltersTools()
    {
        // Arrange
        var mockMcpClient = new Mock<McpClient>(new Uri("http://localhost:1234"));
        var tools = new ResponseTools(mockEmbeddingClient.Object);

        var mockToolsResponse = BinaryData.FromString(@"
        {
            ""tools"": [
                {
                    ""name"": ""math-tool"",
                    ""description"": ""Tool for performing mathematical calculations"",
                    ""inputSchema"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""expression"": {
                                ""type"": ""string"",
                                ""description"": ""The mathematical expression to evaluate""
                            }
                        }
                    }
                },
                {
                    ""name"": ""weather-tool"",
                    ""description"": ""Tool for getting weather information"",
                    ""inputSchema"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""location"": {
                                ""type"": ""string"",
                                ""description"": ""The location to get weather for""
                            }
                        }
                    }
                },
                {
                    ""name"": ""translate-tool"",
                    ""description"": ""Tool for translating text between languages"",
                    ""inputSchema"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""text"": {
                                ""type"": ""string"",
                                ""description"": ""Text to translate""
                            },
                            ""targetLanguage"": {
                                ""type"": ""string"",
                                ""description"": ""Target language code""
                            }
                        }
                    }
                }
            ]
        }");

        // Setup mock responses
        var embeddings = new[]
        {
            OpenAIEmbeddingsModelFactory.OpenAIEmbedding(vector: new[] { 0.8f, 0.5f }),
            OpenAIEmbeddingsModelFactory.OpenAIEmbedding(vector: new[] { 0.6f, 0.4f }),
            OpenAIEmbeddingsModelFactory.OpenAIEmbedding(vector: new[] { 0.3f, 0.2f })
        };
        var embeddingCollection = OpenAIEmbeddingsModelFactory.OpenAIEmbeddingCollection(
            items: embeddings,
            model: "text-embedding-ada-002",
            usage: OpenAIEmbeddingsModelFactory.EmbeddingTokenUsage(30, 30));
        var mockResponse = new MockPipelineResponse(200);

        mockMcpClient.Setup(c => c.StartAsync())
            .Returns(Task.CompletedTask);
        mockMcpClient.Setup(c => c.ListToolsAsync())
            .ReturnsAsync(mockToolsResponse);
        mockMcpClient.Setup(c => c.CallToolAsync("math-tool", It.IsAny<BinaryData>()))
            .ReturnsAsync(BinaryData.FromString("\"math-tool result\""));
        mockMcpClient.SetupGet(c => c.Endpoint)
            .Returns(new Uri("http://localhost:1234"));

        mockEmbeddingClient
            .Setup(c => c.GenerateEmbeddingAsync(
                It.IsAny<string>(),
                It.IsAny<EmbeddingGenerationOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientResult.FromValue(embeddings[0], mockResponse));

        mockEmbeddingClient
            .Setup(c => c.GenerateEmbeddingsAsync(
                It.IsAny<IList<string>>(),
                It.IsAny<EmbeddingGenerationOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientResult.FromValue(embeddingCollection, mockResponse));

        // Add the tools
        await tools.AddMcpToolsAsync(mockMcpClient.Object);

        // Act & Assert
        // Test with maxTools = 1
        var options1 = await Task.Run(() => tools.CreateResponseOptions("calculate 2+2", 1, 0.5f));
        Assert.That(options1.Tools, Has.Count.EqualTo(1));

        // Test with maxTools = 2
        var options2 = await Task.Run(() => tools.CreateResponseOptions("calculate 2+2", 2, 0.5f));
        Assert.That(options2.Tools, Has.Count.EqualTo(2));

        // Test that tool choice affects results
        var optionsWithToolChoice = await Task.Run(() =>
        {
            var opts = tools.CreateResponseOptions("calculate 2+2", 1, 0.5f);
            opts.ToolChoice = ResponseToolChoice.CreateRequiredChoice();
            return opts;
        });
        Assert.That(optionsWithToolChoice.ToolChoice, Is.Not.Null);
        Assert.That(optionsWithToolChoice.Tools, Has.Count.EqualTo(1));

        // Verify we can still call the filtered tools
        var toolCall = new FunctionCallResponseItem(
            "call1",
            "localhost1234_-_math-tool",
            BinaryData.FromString(@"{""expression"": ""2+2""}"));
        var result = await tools.CallAsync(toolCall);
        Assert.That(result.CallId, Is.EqualTo("call1"));
        Assert.That(result.FunctionOutput, Is.EqualTo("\"math-tool result\""));

        // Verify expected interactions
        mockMcpClient.Verify(c => c.StartAsync(), Times.Once);
        mockMcpClient.Verify(c => c.ListToolsAsync(), Times.Once);
    }
}