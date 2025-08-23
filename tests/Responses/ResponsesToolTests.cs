using NUnit.Framework;
using OpenAI.Responses;
using OpenAI.Tests.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
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

        MCPToolCallApprovalPolicy approvalPolicy = new MCPToolCallApprovalPolicy(GlobalMCPToolCallApprovalPolicy.NeverRequireApproval);

        ResponseCreationOptions options = new()
        {
            Tools = {
                new MCPTool(serverLabel, serverUri)
                {
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        OpenAIResponseClient client = GetTestClient(overrideModel: "gpt-5");

        OpenAIResponse response = await client.CreateResponseAsync("Roll 2d4+1", options);
        Assert.That(response.OutputItems, Has.Count.GreaterThan(0));

        // Check tool list.
        List<MCPToolDefinitionListItem> toolDefinitionListItems = response.OutputItems.OfType<MCPToolDefinitionListItem>().ToList();
        Assert.That(toolDefinitionListItems, Has.Count.EqualTo(1));

        MCPToolDefinitionListItem listItem = toolDefinitionListItems[0];
        Assert.That(listItem.ToolDefinitions, Has.Count.GreaterThan(0));

        MCPToolDefinition rollToolDefinition = listItem.ToolDefinitions.Where(toolDefinition => toolDefinition.Name == "roll").FirstOrDefault();
        Assert.That(rollToolDefinition, Is.Not.Null);
        Assert.That(rollToolDefinition.InputSchema, Is.Not.Null);
        Assert.That(rollToolDefinition.Annotations, Is.Not.Null);

        // Check tool call.
        List<MCPToolCallItem> toolCallItems = response.OutputItems.OfType<MCPToolCallItem>().ToList();
        Assert.That(toolCallItems, Has.Count.EqualTo(1));

        MCPToolCallItem toolCallItem = toolCallItems[0];
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
    [TestCase(true)]
    [TestCase(false)]
    public async Task MCPToolNeverRequiresApproval(bool useGlobalPolicy)
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        MCPToolCallApprovalPolicy approvalPolicy = useGlobalPolicy
            ? new MCPToolCallApprovalPolicy(GlobalMCPToolCallApprovalPolicy.NeverRequireApproval)
            : new MCPToolCallApprovalPolicy(
                new CustomMCPToolCallApprovalPolicy()
                {
                    ToolsNeverRequiringApproval = new MCPToolFilter()
                    {
                        ToolNames = { "roll" }
                    }
                });

        ResponseCreationOptions options = new()
        {
            Tools = {
                new MCPTool(serverLabel, serverUri)
                {
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        OpenAIResponseClient client = GetTestClient(overrideModel: "gpt-5");

        OpenAIResponse response = await client.CreateResponseAsync("Roll 2d4+1", options);
        Assert.That(response.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response.OutputItems.OfType<MCPToolDefinitionListItem>().ToList(), Has.Count.EqualTo(1));

        // Confirm there are no approval requests and that the tool was called.
        Assert.That(response.OutputItems.OfType<MCPToolCallApprovalRequestItem>().ToList(), Has.Count.EqualTo(0));
        Assert.That(response.OutputItems.OfType<MCPToolCallItem>().ToList(), Has.Count.EqualTo(1));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task MCPToolAlwaysRequiresApproval(bool useGlobalPolicy)
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        MCPToolCallApprovalPolicy approvalPolicy = useGlobalPolicy
            ? new MCPToolCallApprovalPolicy(GlobalMCPToolCallApprovalPolicy.AlwaysRequireApproval)
            : new MCPToolCallApprovalPolicy(
                new CustomMCPToolCallApprovalPolicy()
                {
                    ToolsAlwaysRequiringApproval = new MCPToolFilter()
                    {
                        ToolNames = { "roll" }
                    }
                });

        ResponseCreationOptions options = new()
        {
            Tools = {
                new MCPTool(serverLabel, serverUri)
                {
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        OpenAIResponseClient client = GetTestClient(overrideModel: "gpt-5");

        OpenAIResponse response1 = await client.CreateResponseAsync("Roll 2d4+1", options);
        Assert.That(response1.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response1.OutputItems.OfType<MCPToolDefinitionListItem>().ToList(), Has.Count.EqualTo(1));
        Assert.That(response1.OutputItems.OfType<MCPToolCallItem>().ToList(), Has.Count.EqualTo(0));

        // Check that it stopped at the approval request.
        MCPToolCallApprovalRequestItem approvalRequestItem = response1.OutputItems.Last() as MCPToolCallApprovalRequestItem;
        Assert.That(approvalRequestItem, Is.Not.Null);

        // Prepare the response.
        MCPToolCallApprovalResponseItem approvalResponseItem = new(approvalRequestItem.Id, true);
        options.PreviousResponseId = response1.Id;

        OpenAIResponse response2 = await client.CreateResponseAsync([approvalResponseItem], options);
        Assert.That(response2.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response2.OutputItems.OfType<MCPToolCallItem>().ToList(), Has.Count.EqualTo(1));
    }

    [Test]
    public async Task MCPToolWithAllowedTools()
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        MCPToolCallApprovalPolicy approvalPolicy = new MCPToolCallApprovalPolicy(GlobalMCPToolCallApprovalPolicy.NeverRequireApproval);

        ResponseCreationOptions options = new()
        {
            Tools = {
                new MCPTool(serverLabel, serverUri)
                {
                    ToolCallApprovalPolicy = approvalPolicy,
                    AllowedTools = new MCPToolFilter()
                    {
                        ToolNames = { "roll" }
                    }
                }
            }
        };

        OpenAIResponseClient client = GetTestClient(overrideModel: "gpt-5");

        OpenAIResponse response = await client.CreateResponseAsync("Roll 2d4+1", options);
        Assert.That(response.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response.OutputItems.OfType<MCPToolDefinitionListItem>().ToList(), Has.Count.EqualTo(1));
        Assert.That(response.OutputItems.OfType<MCPToolCallApprovalRequestItem>().ToList(), Has.Count.EqualTo(0));

        List<MCPToolCallItem> toolCallItems = response.OutputItems.OfType<MCPToolCallItem>().ToList();
        Assert.That(toolCallItems, Has.Count.EqualTo(1));

        MCPToolCallItem toolCallItem = toolCallItems[0];
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

        MCPToolCallApprovalPolicy approvalPolicy = new MCPToolCallApprovalPolicy(GlobalMCPToolCallApprovalPolicy.NeverRequireApproval);

        ResponseCreationOptions options = new()
        {
            Tools = {
                new MCPTool(serverLabel, serverUri)
                {
                    ToolCallApprovalPolicy = approvalPolicy,
                    AllowedTools = new MCPToolFilter()
                    {
                        ToolNames = { "not_roll" } // This is not a real tool.
                    }
                }
            }
        };

        OpenAIResponseClient client = GetTestClient(overrideModel: "gpt-5");

        OpenAIResponse response = await client.CreateResponseAsync("Roll 2d4+1", options);
        Assert.That(response.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response.OutputItems.OfType<MCPToolDefinitionListItem>().ToList(), Has.Count.EqualTo(1));
        Assert.That(response.OutputItems.OfType<MCPToolCallApprovalRequestItem>().ToList(), Has.Count.EqualTo(0));
        Assert.That(response.OutputItems.OfType<MCPToolCallItem>().ToList(), Has.Count.EqualTo(0));
    }

    private static OpenAIResponseClient GetTestClient(string overrideModel = null) => GetTestClient<OpenAIResponseClient>(TestScenario.Responses, overrideModel);
}