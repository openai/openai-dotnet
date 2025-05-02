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
using OpenAI.Agents;

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

    [Test]
    public async Task AddMcpToolsAsync_AddsToolsCorrectly()
    {
        // Arrange
        var mcpEndpoint = new Uri("http://localhost:1234");
        var mockMcpClient = new Mock<McpClient>(mcpEndpoint);
        var tools = new ChatTools();

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
            .Returns(mcpEndpoint);

        // Act
        await tools.AddMcpToolsAsync(mockMcpClient.Object);

        // Assert
        Assert.That(tools.Tools, Has.Count.EqualTo(2));
        var toolNames = tools.Tools.Select(t => t.FunctionName).ToList();
        Assert.That(toolNames, Contains.Item("localhost1234_-_mcp-tool-1"));
        Assert.That(toolNames, Contains.Item("localhost1234_-_mcp-tool-2"));

        // Verify we can call the tools
        var toolCall = ChatToolCall.CreateFunctionToolCall("call1", "localhost1234_-_mcp-tool-1", BinaryData.FromString(@"{""param1"": ""test""}"));
        var result = await tools.CallAsync(new[] { toolCall });
        var resultsList = result.ToList();

        Assert.That(resultsList, Has.Count.EqualTo(1));
        Assert.That(resultsList[0].ToolCallId, Is.EqualTo("call1"));
        Assert.That(resultsList[0].Content[0].Text, Is.EqualTo("\"test result\""));
    }

    [Test]
    public async Task CreateCompletionOptions_WithMaxToolsParameter_FiltersTools()
    {
        // Arrange
        var mcpEndpoint = new Uri("http://localhost:1234");
        var mockMcpClient = new Mock<McpClient>(mcpEndpoint);
        var tools = new ChatTools(mockEmbeddingClient.Object);

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
            .Returns(mcpEndpoint);

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
        var options1 = await Task.Run(() => tools.CreateCompletionOptions("calculate 2+2", 1, 0.5f));
        Assert.That(options1.Tools, Has.Count.EqualTo(1));

        // Test with maxTools = 2
        var options2 = await Task.Run(() => tools.CreateCompletionOptions("calculate 2+2", 2, 0.5f));
        Assert.That(options2.Tools, Has.Count.EqualTo(2));

        // Test that we can call the tools after filtering
        var toolCall = ChatToolCall.CreateFunctionToolCall(
            "call1",
            "localhost1234_-_math-tool",
            BinaryData.FromString(@"{""expression"": ""2+2""}"));
        var result = await tools.CallAsync(new[] { toolCall });
        Assert.That(result.First().ToolCallId, Is.EqualTo("call1"));
        Assert.That(result.First().Content[0].Text, Is.EqualTo("\"math-tool result\""));

        // Verify expected interactions
        mockMcpClient.Verify(c => c.StartAsync(), Times.Once);
        mockMcpClient.Verify(c => c.ListToolsAsync(), Times.Once);
    }
}