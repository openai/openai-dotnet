using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OpenAI.Agents;
using OpenAI.Chat;
using OpenAI.Embeddings;

namespace OpenAI.Tests.Utility;

[TestFixture]
[Category("Utility")]
public class ChatToolsTests
{
    private class TestTools
    {
        public static string Echo(string message) => message;
        public static int Add(int a, int b) => a + b;
        public static double Multiply(double x, double y) => x * y;
        public static bool IsGreaterThan(long value1, long value2) => value1 > value2;
        public static float Divide(float numerator, float denominator) => numerator / denominator;
        public static string ConcatWithBool(string text, bool flag) => $"{text}:{flag}";
    }

    private class TestToolsAsync
    {
        public static async Task<string> EchoAsync(string message)
        {
            await Task.Delay(1); // Simulate async work
            return message;
        }

        public static async Task<int> AddAsync(int a, int b)
        {
            await Task.Delay(1); // Simulate async work
            return a + b;
        }

        public static async Task<double> MultiplyAsync(double x, double y)
        {
            await Task.Delay(1); // Simulate async work
            return x * y;
        }
            
            public static async Task<bool> IsGreaterThanAsync(long value1, long value2)
            {
                await Task.Delay(1); // Simulate async work
                return value1 > value2;
            }
                
            public static async Task<float> DivideAsync(float numerator, float denominator)
            {
                await Task.Delay(1); // Simulate async work
                return numerator / denominator;
            }

            public static async Task<string> ConcatWithBoolAsync(string text, bool flag)
            {
                await Task.Delay(1); // Simulate async work
                return $"{text}:{flag}";
            }
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
        tools.AddFunctionTools(typeof(TestTools));

        Assert.That(tools.Tools, Has.Count.EqualTo(6));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "Echo"));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "Add"));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "Multiply"));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "IsGreaterThan"));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "Divide"));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "ConcatWithBool"));
    }

    [Test]
    public void CanAddAsyncLocalTools()
    {
        var tools = new ChatTools();
        tools.AddFunctionTools(typeof(TestToolsAsync));

        Assert.That(tools.Tools, Has.Count.EqualTo(6));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "EchoAsync"));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "AddAsync"));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "MultiplyAsync"));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "IsGreaterThanAsync"));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "DivideAsync"));
        Assert.That(tools.Tools.Any(t => t.FunctionName == "ConcatWithBoolAsync"));
    }

    [Test]
    public async Task CanCallToolsAsync()
    {
        var tools = new ChatTools();
        tools.AddFunctionTools(typeof(TestTools));

        var toolCalls = new[]
        {
            ChatToolCall.CreateFunctionToolCall("call1", "Echo", BinaryData.FromString(@"{""message"": ""Hello""}")),
            ChatToolCall.CreateFunctionToolCall("call2", "Add", BinaryData.FromString(@"{""a"": 2, ""b"": 3}")),
            ChatToolCall.CreateFunctionToolCall("call3", "Multiply", BinaryData.FromString(@"{""x"": 2.5, ""y"": 3.0}")),
            ChatToolCall.CreateFunctionToolCall("call4", "IsGreaterThan", BinaryData.FromString(@"{""value1"": 100, ""value2"": 50}")),
            ChatToolCall.CreateFunctionToolCall("call5", "Divide", BinaryData.FromString(@"{""numerator"": 10.0, ""denominator"": 2.0}")),
            ChatToolCall.CreateFunctionToolCall("call6", "ConcatWithBool", BinaryData.FromString(@"{""text"": ""Test"", ""flag"": true}"))
        };

        var results = await tools.CallAsync(toolCalls);
        var resultsList = results.ToList();

        Assert.That(resultsList, Has.Count.EqualTo(6));
        Assert.That(resultsList[0].ToolCallId, Is.EqualTo("call1"));
        Assert.That(resultsList[0].Content[0].Text, Is.EqualTo("Hello"));
        Assert.That(resultsList[1].ToolCallId, Is.EqualTo("call2"));
        Assert.That(resultsList[1].Content[0].Text, Is.EqualTo("5"));
        Assert.That(resultsList[2].ToolCallId, Is.EqualTo("call3"));
        Assert.That(resultsList[2].Content[0].Text, Is.EqualTo("7.5"));
        Assert.That(resultsList[3].ToolCallId, Is.EqualTo("call4"));
        Assert.That(resultsList[3].Content[0].Text, Is.EqualTo("True"));
        Assert.That(resultsList[4].ToolCallId, Is.EqualTo("call5"));
        Assert.That(resultsList[4].Content[0].Text, Is.EqualTo("5"));
        Assert.That(resultsList[5].ToolCallId, Is.EqualTo("call6"));
        Assert.That(resultsList[5].Content[0].Text, Is.EqualTo("Test:True"));
    }

    [Test]
    public async Task CanCallAsyncToolsAsync()
    {
        var tools = new ChatTools();
        tools.AddFunctionTools(typeof(TestToolsAsync));

        var toolCalls = new[]
        {
            ChatToolCall.CreateFunctionToolCall("call1", "EchoAsync", BinaryData.FromString(@"{""message"": ""Hello""}")),
            ChatToolCall.CreateFunctionToolCall("call2", "AddAsync", BinaryData.FromString(@"{""a"": 2, ""b"": 3}")),
            ChatToolCall.CreateFunctionToolCall("call3", "MultiplyAsync", BinaryData.FromString(@"{""x"": 2.5, ""y"": 3.0}")),
            ChatToolCall.CreateFunctionToolCall("call4", "IsGreaterThanAsync", BinaryData.FromString(@"{""value1"": 100, ""value2"": 50}")),
            ChatToolCall.CreateFunctionToolCall("call5", "DivideAsync", BinaryData.FromString(@"{""numerator"": 10.0, ""denominator"": 2.0}")),
            ChatToolCall.CreateFunctionToolCall("call6", "ConcatWithBoolAsync", BinaryData.FromString(@"{""text"": ""Test"", ""flag"": true}"))
        };

        var results = await tools.CallAsync(toolCalls);
        var resultsList = results.ToList();

        Assert.That(resultsList, Has.Count.EqualTo(6));
        Assert.That(resultsList[0].ToolCallId, Is.EqualTo("call1"));
        Assert.That(resultsList[0].Content[0].Text, Is.EqualTo("Hello"));
        Assert.That(resultsList[1].ToolCallId, Is.EqualTo("call2"));
        Assert.That(resultsList[1].Content[0].Text, Is.EqualTo("5"));
        Assert.That(resultsList[2].ToolCallId, Is.EqualTo("call3"));
        Assert.That(resultsList[2].Content[0].Text, Is.EqualTo("7.5"));
        Assert.That(resultsList[3].ToolCallId, Is.EqualTo("call4"));
        Assert.That(resultsList[3].Content[0].Text, Is.EqualTo("True"));
        Assert.That(resultsList[4].ToolCallId, Is.EqualTo("call5"));
        Assert.That(resultsList[4].Content[0].Text, Is.EqualTo("5"));
        Assert.That(resultsList[5].ToolCallId, Is.EqualTo("call6"));
        Assert.That(resultsList[5].Content[0].Text, Is.EqualTo("Test:True"));
    }

    [Test]
    public void CreatesCompletionOptionsWithTools()
    {
        var tools = new ChatTools();
        tools.AddFunctionTools(typeof(TestTools));

        var options = tools.ToChatCompletionOptions();

        Assert.That(options.Tools, Has.Count.EqualTo(6));
        Assert.That(options.Tools.Any(t => t.FunctionName == "Echo"));
        Assert.That(options.Tools.Any(t => t.FunctionName == "Add"));
        Assert.That(options.Tools.Any(t => t.FunctionName == "Multiply"));
        Assert.That(options.Tools.Any(t => t.FunctionName == "IsGreaterThan"));
        Assert.That(options.Tools.Any(t => t.FunctionName == "Divide"));
        Assert.That(options.Tools.Any(t => t.FunctionName == "ConcatWithBool"));
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
        tools.AddFunctionTools(typeof(TestTools));

        var options = await tools.ToChatCompletionOptions("Need to add two numbers", 1, 0.5f);

        Assert.That(options.Tools, Has.Count.LessThanOrEqualTo(1));
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
        var options1 = await tools.ToChatCompletionOptions("calculate 2+2", 1, 0.5f);
        Assert.That(options1.Tools, Has.Count.EqualTo(1));

        // Test with maxTools = 2
        var options2 = await tools.ToChatCompletionOptions("calculate 2+2", 2, 0.5f);
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