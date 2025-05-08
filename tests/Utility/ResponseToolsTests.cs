using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OpenAI.Agents;
using OpenAI.Embeddings;
using OpenAI.Responses;

namespace OpenAI.Tests.Utility;

[TestFixture]
[Category("Utility")]
public class ResponseToolsTests : ToolsTestsBase
{
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
        tools.AddFunctionTools(typeof(TestTools));

        Assert.That(tools.Tools, Has.Count.EqualTo(6));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Echo")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Add")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Multiply")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("IsGreaterThan")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Divide")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("ConcatWithBool")));
    }

    [Test]
    public void CanAddLocalAsyncTools()
    {
        var tools = new ResponseTools();
        tools.AddFunctionTools(typeof(TestToolsAsync));

        Assert.That(tools.Tools, Has.Count.EqualTo(6));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("EchoAsync")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("AddAsync")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("MultiplyAsync")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("IsGreaterThanAsync")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("DivideAsync")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("ConcatWithBoolAsync")));
    }

    [Test]
    public async Task CanCallToolAsync()
    {
        var tools = new ResponseTools();
        tools.AddFunctionTools(typeof(TestTools));

        var toolCalls = new[]
        {
            new FunctionCallResponseItem("call1", "Echo", BinaryData.FromString(@"{""message"": ""Hello""}")),
            new FunctionCallResponseItem("call2", "Add", BinaryData.FromString(@"{""a"": 2, ""b"": 3}")),
            new FunctionCallResponseItem("call3", "Multiply", BinaryData.FromString(@"{""x"": 2.5, ""y"": 3.0}")),
            new FunctionCallResponseItem("call4", "IsGreaterThan", BinaryData.FromString(@"{""value1"": 100, ""value2"": 50}")),
            new FunctionCallResponseItem("call5", "Divide", BinaryData.FromString(@"{""numerator"": 10.0, ""denominator"": 2.0}")),
            new FunctionCallResponseItem("call6", "ConcatWithBool", BinaryData.FromString(@"{""text"": ""Test"", ""flag"": true}"))
        };

        foreach (var toolCall in toolCalls)
        {
            var result = await tools.CallAsync(toolCall);
            Assert.That(result.CallId, Is.EqualTo(toolCall.CallId));
            switch (toolCall.CallId)
            {
                case "call1":
                    Assert.That(result.FunctionOutput, Is.EqualTo("Hello"));
                    break;
                case "call2":
                    Assert.That(result.FunctionOutput, Is.EqualTo("5"));
                    break;
                case "call3":
                    Assert.That(result.FunctionOutput, Is.EqualTo("7.5"));
                    break;
                case "call4":
                    Assert.That(result.FunctionOutput, Is.EqualTo("True"));
                    break;
                case "call5":
                    Assert.That(result.FunctionOutput, Is.EqualTo("5"));
                    break;
                case "call6":
                    Assert.That(result.FunctionOutput, Is.EqualTo("Test:True"));
                    break;
            }
        }
    }

    [Test]
    public async Task CanCallAsyncToolsAsync()
    {
        var tools = new ResponseTools();
        tools.AddFunctionTools(typeof(TestToolsAsync));

        var toolCalls = new[]
        {
            new FunctionCallResponseItem("call1", "EchoAsync", BinaryData.FromString(@"{""message"": ""Hello""}")),
            new FunctionCallResponseItem("call2", "AddAsync", BinaryData.FromString(@"{""a"": 2, ""b"": 3}")),
            new FunctionCallResponseItem("call3", "MultiplyAsync", BinaryData.FromString(@"{""x"": 2.5, ""y"": 3.0}")),
            new FunctionCallResponseItem("call4", "IsGreaterThanAsync", BinaryData.FromString(@"{""value1"": 100, ""value2"": 50}")),
            new FunctionCallResponseItem("call5", "DivideAsync", BinaryData.FromString(@"{""numerator"": 10.0, ""denominator"": 2.0}")),
            new FunctionCallResponseItem("call6", "ConcatWithBoolAsync", BinaryData.FromString(@"{""text"": ""Test"", ""flag"": true}"))
        };

        foreach (var toolCall in toolCalls)
        {
            var result = await tools.CallAsync(toolCall);
            Assert.That(result.CallId, Is.EqualTo(toolCall.CallId));
            switch (toolCall.CallId)
            {
                case "call1":
                    Assert.That(result.FunctionOutput, Is.EqualTo("Hello"));
                    break;
                case "call2":
                    Assert.That(result.FunctionOutput, Is.EqualTo("5"));
                    break;
                case "call3":
                    Assert.That(result.FunctionOutput, Is.EqualTo("7.5"));
                    break;
                case "call4":
                    Assert.That(result.FunctionOutput, Is.EqualTo("True"));
                    break;
                case "call5":
                    Assert.That(result.FunctionOutput, Is.EqualTo("5"));
                    break;
                case "call6":
                    Assert.That(result.FunctionOutput, Is.EqualTo("Test:True"));
                    break;
            }
        }
    }

    [Test]
    public void CreatesResponseOptionsWithTools()
    {
        var tools = new ResponseTools();
        tools.AddFunctionTools(typeof(TestTools));

        var options = tools.ToResponseCreationOptions();

        Assert.That(options.Tools, Has.Count.EqualTo(6));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Echo")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Add")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Multiply")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("IsGreaterThan")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("Divide")));
        Assert.That(tools.Tools.Any(t => ((string)t.GetType().GetProperty("Name").GetValue(t)).Contains("ConcatWithBool")));
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
        tools.AddFunctionTools(typeof(TestTools));

        var options = await tools.ToResponseCreationOptionsAsync("Need to add two numbers", 1, 0.5f);

        Assert.That(options.Tools, Has.Count.LessThanOrEqualTo(1));
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
        var mcpEndpoint = new Uri("http://localhost:1234");
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

        var responsesByTool = new Dictionary<string, string>
        {
            ["mcp-tool-1"] = "\"tool1 result\"",
            ["mcp-tool-2"] = "\"tool2 result\""
        };

        var testClient = new TestMcpClient(
            mcpEndpoint,
            mockToolsResponse,
            toolName => BinaryData.FromString(responsesByTool[toolName.Split('_').Last()]));
        var tools = new ResponseTools();

        // Act
        await tools.AddMcpToolsAsync(testClient);

        // Assert
        Assert.That(tools.Tools, Has.Count.EqualTo(2));
        var toolNames = tools.Tools.Select(t => (string)t.GetType().GetProperty("Name").GetValue(t)).ToList();
        Assert.That(toolNames, Contains.Item("localhost1234_-_mcp-tool-1"));
        Assert.That(toolNames, Contains.Item("localhost1234_-_mcp-tool-2"));

        // Verify we can call the tools with different responses
        var toolCall = new FunctionCallResponseItem("call1", "localhost1234_-_mcp-tool-1", BinaryData.FromString(@"{""param1"": ""test""}"));
        var result = await tools.CallAsync(toolCall);
        Assert.That(result.FunctionOutput, Is.EqualTo("\"tool1 result\""));

        var toolCall2 = new FunctionCallResponseItem("call2", "localhost1234_-_mcp-tool-2", BinaryData.FromString(@"{""param2"": ""test""}"));
        var result2 = await tools.CallAsync(toolCall2);
        Assert.That(result2.FunctionOutput, Is.EqualTo("\"tool2 result\""));
    }

    [Test]
    public async Task CreateResponseOptions_WithMaxToolsParameter_FiltersTools()
    {
        // Arrange
        var mcpEndpoint = new Uri("http://localhost:1234");
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

        // Setup mock embedding responses
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

        var responsesByTool = new Dictionary<string, string>
        {
            ["math-tool"] = "\"math result\"",
            ["weather-tool"] = "\"weather result\"",
            ["translate-tool"] = "\"translate result\""
        };

        var testClient = new TestMcpClient(
            mcpEndpoint,
            mockToolsResponse,
            toolName => BinaryData.FromString(responsesByTool[toolName.Split('_').Last()]));
        var tools = new ResponseTools(mockEmbeddingClient.Object);

        // Add the tools
        await tools.AddMcpToolsAsync(testClient);

        // Act & Assert
        // Test with maxTools = 1
        var options1 = await tools.ToResponseCreationOptionsAsync("calculate 2+2", 1, 0.5f);
        Assert.That(options1.Tools, Has.Count.EqualTo(1));

        // Test with maxTools = 2
        var options2 = await tools.ToResponseCreationOptionsAsync("calculate 2+2", 2, 0.5f);
        Assert.That(options2.Tools, Has.Count.EqualTo(2));

        // Test that tool choice affects results
        var optionsWithToolChoice = await tools.ToResponseCreationOptionsAsync("calculate 2+2", 1, 0.5f);
        optionsWithToolChoice.ToolChoice = ResponseToolChoice.CreateRequiredChoice();

        Assert.That(optionsWithToolChoice.ToolChoice, Is.Not.Null);
        Assert.That(optionsWithToolChoice.Tools, Has.Count.EqualTo(1));

        // Verify we can still call the filtered tools
        var toolCall = new FunctionCallResponseItem(
            "call1",
            "localhost1234_-_math-tool",
            BinaryData.FromString(@"{""expression"": ""2+2""}"));
        var result = await tools.CallAsync(toolCall);
        Assert.That(result.CallId, Is.EqualTo("call1"));
        Assert.That(result.FunctionOutput, Is.EqualTo("\"math result\""));
    }
}