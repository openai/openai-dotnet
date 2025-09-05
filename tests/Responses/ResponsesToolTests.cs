using NUnit.Framework;
using OpenAI.Responses;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Responses;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.Fixtures)]
[Category("Responses")]
[Category("MCP")]
public partial class ResponsesToolTests : SyncAsyncTestBase
{
    public ResponsesToolTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task MCPToolWorks()
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        McpToolCallApprovalPolicy approvalPolicy = new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval);

        ResponseCreationOptions options = new()
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        OpenAIResponseClient client = GetTestClient(overrideModel: "gpt-5");

        OpenAIResponse response = await client.CreateResponseAsync("Roll 2d4+1", options);
        Assert.That(response.OutputItems, Has.Count.GreaterThan(0));

        // Check tool list.
        List<McpToolDefinitionListItem> toolDefinitionListItems = response.OutputItems.OfType<McpToolDefinitionListItem>().ToList();
        Assert.That(toolDefinitionListItems, Has.Count.EqualTo(1));

        McpToolDefinitionListItem listItem = toolDefinitionListItems[0];
        Assert.That(listItem.ToolDefinitions, Has.Count.GreaterThan(0));

        McpToolDefinition rollToolDefinition = listItem.ToolDefinitions.Where(toolDefinition => toolDefinition.Name == "roll").FirstOrDefault();
        Assert.That(rollToolDefinition, Is.Not.Null);
        Assert.That(rollToolDefinition.InputSchema, Is.Not.Null);
        Assert.That(rollToolDefinition.Annotations, Is.Not.Null);

        // Check tool call.
        List<McpToolCallItem> toolCallItems = response.OutputItems.OfType<McpToolCallItem>().ToList();
        Assert.That(toolCallItems, Has.Count.EqualTo(1));

        McpToolCallItem toolCallItem = toolCallItems[0];
        Assert.That(toolCallItem.ServerLabel, Is.EqualTo(serverLabel));
        Assert.That(toolCallItem.ToolName, Is.EqualTo("roll"));
        Assert.That(toolCallItem.ToolArguments, Is.Not.Null);
        Assert.That(toolCallItem.ToolOutput, Is.Not.Null.Or.Empty);
        Assert.That(toolCallItem.Error, Is.Null);

        // Check assistant message.
        MessageResponseItem assistantMessageItem = response.OutputItems.Last() as MessageResponseItem;
        Assert.That(assistantMessageItem, Is.Not.Null);
    }

    [Test]
    public async Task MCPToolStreamingWorks()
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        McpToolCallApprovalPolicy approvalPolicy = new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval);

        ResponseCreationOptions options = new()
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        OpenAIResponseClient client = GetTestClient(overrideModel: "gpt-5");

        AsyncCollectionResult<StreamingResponseUpdate> responseUpdates = client.CreateResponseStreamingAsync("Roll 2d4+1", options);

        int mcpCallArgumentsDeltaUpdateCount = 0;
        int mcpCallArgumentsDoneUpdateCount = 0;
        int mcpCallCompletedUpdateCount = 0;
        int mcpCallFailedUpdateCount = 0;
        int mcpCallInProgressUpdateCount = 0;
        int mcpListToolsCompletedUpdateCount = 0;
        int mcpListToolsFailedUpdateCount = 0;
        int mcpListToolsInProgressUpdateCount = 0;

        StringBuilder argumentsBuilder = new StringBuilder();

        await foreach (StreamingResponseUpdate update in responseUpdates)
        {
            if (update is StreamingResponseMcpCallArgumentsDeltaUpdate mcpCallArgumentsDeltaUpdate)
            {
                mcpCallArgumentsDeltaUpdateCount++;

                BinaryData delta = mcpCallArgumentsDeltaUpdate.Delta;
                Assert.That(delta, Is.Not.Null);

                if (!delta.ToMemory().IsEmpty)
                {
                    argumentsBuilder.AppendLine(mcpCallArgumentsDeltaUpdate.Delta.ToString());
                }
            }

            if (update is StreamingResponseMcpCallArgumentsDoneUpdate mcpCallArgumentsDoneUpdate)
            {
                mcpCallArgumentsDoneUpdateCount++;

                BinaryData toolArguments = mcpCallArgumentsDoneUpdate.ToolArguments;
                Assert.That(toolArguments, Is.Not.Null);
                Assert.That(toolArguments.ToString(), Is.EqualTo(argumentsBuilder.ToString().ReplaceLineEndings(string.Empty)));

                argumentsBuilder.Clear();
            }

            if (update is StreamingResponseMcpCallCompletedUpdate mcpCallCompletedUpdate)
            {
                mcpCallCompletedUpdateCount++;
            }

            if (update is StreamingResponseMcpCallFailedUpdate mcpCallFailedUpdate)
            {
                mcpCallFailedUpdateCount++;
            }

            if (update is StreamingResponseMcpCallInProgressUpdate mcpCallInProgressUpdate)
            {
                mcpCallInProgressUpdateCount++;
            }

            if (update is StreamingResponseMcpListToolsCompletedUpdate mcpListToolsCompletedUpdate)
            {
                mcpListToolsCompletedUpdateCount++;
            }

            if (update is StreamingResponseMcpListToolsFailedUpdate mcpListToolsFailedUpdate)
            {
                mcpListToolsFailedUpdateCount++;
            }

            if (update is StreamingResponseMcpListToolsInProgressUpdate mcpListToolsInProgressUpdate)
            {
                mcpListToolsInProgressUpdateCount++;
            }
        }

        Assert.That(mcpListToolsFailedUpdateCount, Is.GreaterThanOrEqualTo(0));
        Assert.That(mcpListToolsInProgressUpdateCount, Is.GreaterThan(0));
        Assert.That(mcpListToolsCompletedUpdateCount, Is.EqualTo(mcpListToolsInProgressUpdateCount - mcpListToolsFailedUpdateCount));

        Assert.That(mcpCallFailedUpdateCount, Is.GreaterThanOrEqualTo(0));
        Assert.That(mcpCallInProgressUpdateCount, Is.GreaterThan(0));
        Assert.That(mcpCallCompletedUpdateCount, Is.EqualTo(mcpListToolsInProgressUpdateCount - mcpListToolsFailedUpdateCount));

        Assert.That(mcpCallArgumentsDoneUpdateCount, Is.GreaterThan(0));
        Assert.That(mcpCallArgumentsDeltaUpdateCount, Is.GreaterThanOrEqualTo(mcpCallArgumentsDoneUpdateCount));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task MCPToolNeverRequiresApproval(bool useGlobalPolicy)
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        McpToolCallApprovalPolicy approvalPolicy = useGlobalPolicy
            ? new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval)
            : new McpToolCallApprovalPolicy(
                new CustomMcpToolCallApprovalPolicy()
                {
                    ToolsNeverRequiringApproval = new McpToolFilter()
                    {
                        ToolNames = { "roll" }
                    }
                });

        ResponseCreationOptions options = new()
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        OpenAIResponseClient client = GetTestClient(overrideModel: "gpt-5");

        OpenAIResponse response = await client.CreateResponseAsync("Roll 2d4+1", options);
        Assert.That(response.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response.OutputItems.OfType<McpToolDefinitionListItem>().ToList(), Has.Count.EqualTo(1));

        // Confirm there are no approval requests and that the tool was called.
        Assert.That(response.OutputItems.OfType<McpToolCallApprovalRequestItem>().ToList(), Has.Count.EqualTo(0));
        Assert.That(response.OutputItems.OfType<McpToolCallItem>().ToList(), Has.Count.EqualTo(1));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task MCPToolAlwaysRequiresApproval(bool useGlobalPolicy)
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        McpToolCallApprovalPolicy approvalPolicy = useGlobalPolicy
            ? new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.AlwaysRequireApproval)
            : new McpToolCallApprovalPolicy(
                new CustomMcpToolCallApprovalPolicy()
                {
                    ToolsAlwaysRequiringApproval = new McpToolFilter()
                    {
                        ToolNames = { "roll" }
                    }
                });

        ResponseCreationOptions options = new()
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        OpenAIResponseClient client = GetTestClient(overrideModel: "gpt-5");

        OpenAIResponse response1 = await client.CreateResponseAsync("Roll 2d4+1", options);
        Assert.That(response1.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response1.OutputItems.OfType<McpToolDefinitionListItem>().ToList(), Has.Count.EqualTo(1));
        Assert.That(response1.OutputItems.OfType<McpToolCallItem>().ToList(), Has.Count.EqualTo(0));

        // Check that it stopped at the approval request.
        McpToolCallApprovalRequestItem approvalRequestItem = response1.OutputItems.Last() as McpToolCallApprovalRequestItem;
        Assert.That(approvalRequestItem, Is.Not.Null);

        // Prepare the response.
        McpToolCallApprovalResponseItem approvalResponseItem = new(approvalRequestItem.Id, true);
        options.PreviousResponseId = response1.Id;

        OpenAIResponse response2 = await client.CreateResponseAsync([approvalResponseItem], options);
        Assert.That(response2.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response2.OutputItems.OfType<McpToolCallItem>().ToList(), Has.Count.EqualTo(1));
    }

    [Test]
    public async Task MCPToolWithAllowedTools()
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        McpToolCallApprovalPolicy approvalPolicy = new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval);

        ResponseCreationOptions options = new()
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ToolCallApprovalPolicy = approvalPolicy,
                    AllowedTools = new McpToolFilter()
                    {
                        ToolNames = { "roll" }
                    }
                }
            }
        };

        OpenAIResponseClient client = GetTestClient(overrideModel: "gpt-5");

        OpenAIResponse response = await client.CreateResponseAsync("Roll 2d4+1", options);
        Assert.That(response.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response.OutputItems.OfType<McpToolDefinitionListItem>().ToList(), Has.Count.EqualTo(1));
        Assert.That(response.OutputItems.OfType<McpToolCallApprovalRequestItem>().ToList(), Has.Count.EqualTo(0));

        List<McpToolCallItem> toolCallItems = response.OutputItems.OfType<McpToolCallItem>().ToList();
        Assert.That(toolCallItems, Has.Count.EqualTo(1));

        McpToolCallItem toolCallItem = toolCallItems[0];
        Assert.That(toolCallItem.ServerLabel, Is.EqualTo(serverLabel));
        Assert.That(toolCallItem.ToolName, Is.EqualTo("roll"));
        Assert.That(toolCallItem.ToolArguments, Is.Not.Null);
        Assert.That(toolCallItem.ToolOutput, Is.Not.Null.Or.Empty);
        Assert.That(toolCallItem.Error, Is.Null);
    }

    [Test]
    public async Task MCPToolWithDisallowedTools()
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        McpToolCallApprovalPolicy approvalPolicy = new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval);

        ResponseCreationOptions options = new()
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ToolCallApprovalPolicy = approvalPolicy,
                    AllowedTools = new McpToolFilter()
                    {
                        ToolNames = { "not_roll" } // This is not a real tool.
                    }
                }
            }
        };

        OpenAIResponseClient client = GetTestClient(overrideModel: "gpt-5");

        OpenAIResponse response = await client.CreateResponseAsync("Roll 2d4+1", options);
        Assert.That(response.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response.OutputItems.OfType<McpToolDefinitionListItem>().ToList(), Has.Count.EqualTo(1));
        Assert.That(response.OutputItems.OfType<McpToolCallApprovalRequestItem>().ToList(), Has.Count.EqualTo(0));
        Assert.That(response.OutputItems.OfType<McpToolCallItem>().ToList(), Has.Count.EqualTo(0));
    }

    private static OpenAIResponseClient GetTestClient(string overrideModel = null) => GetTestClient<OpenAIResponseClient>(TestScenario.Responses, overrideModel);
}