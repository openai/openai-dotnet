using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests.Realtime;

#pragma warning disable OPENAI002

[LiveOnly(Reason = "Test framework doesn't support recording with web sockets yet")]
public class RealtimeToolTests : RealtimeTestFixtureBase
{
    public RealtimeToolTests(bool isAsync) : base(isAsync, RecordedTestMode.Live)
    {
        TestTimeoutInSeconds = 30;
    }

    [Test]
    public async Task MCPToolWorks()
    {
        string serverLabel = "microsoft-learn";
        Uri serverUri = new Uri("https://learn.microsoft.com/api/mcp");
        string toolName = "microsoft_docs_search";

        RealtimeMcpToolCallApprovalPolicy approvalPolicy =
            new RealtimeMcpToolCallApprovalPolicy(RealtimeDefaultMcpToolCallApprovalPolicy.NeverRequireApproval);

        RealtimeMcpTool mcpTool = new(serverLabel, serverUri)
        {
            // ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
            ToolCallApprovalPolicy = approvalPolicy,
            AllowedTools = new RealtimeMcpToolFilter()
            {
                ToolNames = { toolName }
            }
        };

        RealtimeClient client = GetTestClient();

        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        RealtimeConversationSessionOptions sessionOptions = new()
        {
            Instructions = "Use the available tools to help the user.",
            OutputModalities = { RealtimeOutputModality.Text },
            Tools = { mcpTool },
        };

        List<RealtimeServerUpdate> setupUpdates = await ConfigureSessionAndWaitForMcpToolsAsync(sessionClient, sessionOptions);

        // Now send the user message and request a response.
        await sessionClient.AddItemAsync(
            RealtimeItem.CreateUserMessageItem("Search Microsoft Learn documentation for the OpenAI service. You can only call an MCP tool at most once."),
            CancellationToken);

        await sessionClient.StartResponseAsync(
            new RealtimeResponseOptions
            {
                OutputModalities = { RealtimeOutputModality.Text },
            },
            CancellationToken);

        int mcpCallArgumentsDeltaUpdateCount = 0;
        int mcpCallArgumentsDoneUpdateCount = 0;
        int mcpCallCompletedUpdateCount = 0;
        int mcpCallInProgressUpdateCount = 0;
        int conversationItemDoneUpdateCount = 0;
        int responseDoneUpdateCount = 0;
        RealtimeMcpToolCallItem toolCallItem = null;

        await foreach (RealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            if (update is RealtimeServerUpdateResponseMcpCallArgumentsDelta)
            {
                mcpCallArgumentsDeltaUpdateCount++;
            }

            if (update is RealtimeServerUpdateResponseMcpCallArgumentsDone)
            {
                mcpCallArgumentsDoneUpdateCount++;
            }

            if (update is RealtimeServerUpdateResponseDone responseDone)
            {
                responseDoneUpdateCount++;
                toolCallItem = responseDone.Response.OutputItems
                    .OfType<RealtimeMcpToolCallItem>()
                    .FirstOrDefault(item => item.ToolName == toolName);

                Assert.That(toolCallItem, Is.Not.Null);
                Assert.That(toolCallItem!.ServerLabel, Is.EqualTo(serverLabel));
                Assert.That(toolCallItem!.ToolName, Is.EqualTo(toolName));
                Assert.That(toolCallItem!.ToolArguments, Is.Not.Null);
                Assert.That(toolCallItem!.Error, Is.Null);
            }

            if (update is RealtimeServerUpdateResponseMcpCallInProgress)
            {
                mcpCallInProgressUpdateCount++;
            }

            if (update is RealtimeServerUpdateConversationItemDone { Item: RealtimeMcpToolDefinitionListItem listItem })
            {
                conversationItemDoneUpdateCount++;

                Assert.That(listItem.ToolDefinitions, Has.Count.GreaterThan(0));

                RealtimeMcpToolDefinition searchToolDefinition = listItem.ToolDefinitions
                    .Where(td => td.Name == toolName).FirstOrDefault();
                Assert.That(searchToolDefinition, Is.Not.Null);
                Assert.That(searchToolDefinition!.InputSchema, Is.Not.Null);
            }

            if (update is RealtimeServerUpdateResponseMcpCallCompleted)
            {
                mcpCallCompletedUpdateCount++;
                break;
            }
        }

        // Validate MCP list tools lifecycle events (from setup phase).
        Assert.That(setupUpdates.OfType<RealtimeServerUpdateMcpListToolsInProgress>().Count(), Is.GreaterThan(0));
        Assert.That(setupUpdates.OfType<RealtimeServerUpdateMcpListToolsCompleted>().Count(), Is.GreaterThan(0));

        // Validate MCP call lifecycle events.
        Assert.That(mcpCallArgumentsDoneUpdateCount, Is.GreaterThan(0));
        Assert.That(mcpCallArgumentsDeltaUpdateCount, Is.GreaterThanOrEqualTo(mcpCallArgumentsDoneUpdateCount));
        Assert.That(mcpCallInProgressUpdateCount, Is.GreaterThan(0));
        Assert.That(mcpCallCompletedUpdateCount, Is.GreaterThan(0));

        // Check tool definition list and tool call were received.
        Assert.That(conversationItemDoneUpdateCount, Is.GreaterThan(0));
        Assert.That(responseDoneUpdateCount, Is.GreaterThan(0));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task MCPToolNeverRequiresApproval(bool useGlobalPolicy)
    {
        string serverLabel = "microsoft-learn";
        Uri serverUri = new Uri("https://learn.microsoft.com/api/mcp");
        string toolName = "microsoft_docs_search";

        RealtimeMcpToolCallApprovalPolicy approvalPolicy = useGlobalPolicy
            ? new RealtimeMcpToolCallApprovalPolicy(RealtimeDefaultMcpToolCallApprovalPolicy.NeverRequireApproval)
            : new RealtimeMcpToolCallApprovalPolicy(
                new RealtimeCustomMcpToolCallApprovalPolicy()
                {
                    ToolsNeverRequiringApproval = new RealtimeMcpToolFilter()
                    {
                        ToolNames = { toolName }
                    }
                });

        RealtimeMcpTool mcpTool = new(serverLabel, serverUri)
        {
            ToolCallApprovalPolicy = approvalPolicy,
            AllowedTools = new RealtimeMcpToolFilter()
            {
                ToolNames = { toolName }
            }
        };

        RealtimeClient client = GetTestClient();

        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        RealtimeConversationSessionOptions sessionOptions = new()
        {
            Instructions = "Use the available tools to help the user.",
            OutputModalities = { RealtimeOutputModality.Text },
            Tools = { mcpTool },
        };

        await ConfigureSessionAndWaitForMcpToolsAsync(sessionClient, sessionOptions);

        await sessionClient.AddItemAsync(
            RealtimeItem.CreateUserMessageItem("Search Microsoft Learn documentation for the OpenAI service. You can only call an MCP tool at most once."),
            CancellationToken);

        await sessionClient.StartResponseAsync(
            new RealtimeResponseOptions
            {
                OutputModalities = { RealtimeOutputModality.Text },
            },
            CancellationToken);

        int conversationItemDoneMcpToolApprovalRequestUpdateCount = 0;
        int conversationItemDoneMcpToolCallUpdateCount = 0;
        int mcpCallCompletedUpdateCount = 0;
        int responseDoneUpdateCount = 0;

        await foreach (RealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            if (update is RealtimeServerUpdateConversationItemDone conversationItemDone)
            {
                if (conversationItemDone.Item is RealtimeMcpToolCallApprovalRequestItem approvalItem)
                {
                    conversationItemDoneMcpToolApprovalRequestUpdateCount++;

                    Assert.Fail("Approvals should not be required.");
                }
                else if (conversationItemDone.Item is RealtimeMcpToolCallItem mcpToolCallItem)
                {
                    conversationItemDoneMcpToolCallUpdateCount++;

                    Assert.That(mcpToolCallItem.ToolName, Is.EqualTo(toolName));
                    Assert.That(mcpToolCallItem.ServerLabel, Is.EqualTo(serverLabel));
                }
            }

            if (update is RealtimeServerUpdateResponseMcpCallCompleted mcpCallCompleted)
            {
                mcpCallCompletedUpdateCount++;
            }

            if (update is RealtimeServerUpdateResponseDone responseDone)
            {
                responseDoneUpdateCount++;
            }

            if (mcpCallCompletedUpdateCount >= 1 && conversationItemDoneMcpToolCallUpdateCount >= 1 && responseDoneUpdateCount >= 1)
            {
                break;
            }
        }

        Assert.That(conversationItemDoneMcpToolApprovalRequestUpdateCount, Is.EqualTo(0));
        Assert.That(conversationItemDoneMcpToolCallUpdateCount, Is.GreaterThanOrEqualTo(1));
        Assert.That(mcpCallCompletedUpdateCount, Is.GreaterThanOrEqualTo(1));
        Assert.That(responseDoneUpdateCount, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task MCPToolAlwaysRequiresApproval(bool useGlobalPolicy)
    {
        string serverLabel = "microsoft-learn";
        Uri serverUri = new Uri("https://learn.microsoft.com/api/mcp");
        string toolName = "microsoft_docs_search";

        RealtimeMcpToolCallApprovalPolicy approvalPolicy = useGlobalPolicy
            ? new RealtimeMcpToolCallApprovalPolicy(RealtimeDefaultMcpToolCallApprovalPolicy.AlwaysRequireApproval)
            : new RealtimeMcpToolCallApprovalPolicy(
                new RealtimeCustomMcpToolCallApprovalPolicy()
                {
                    ToolsAlwaysRequiringApproval = new RealtimeMcpToolFilter()
                    {
                        ToolNames = { toolName }
                    }
                });

        RealtimeMcpTool mcpTool = new(serverLabel, serverUri)
        {
            ToolCallApprovalPolicy = approvalPolicy,
            AllowedTools = new RealtimeMcpToolFilter()
            {
                ToolNames = { toolName }
            }
        };

        RealtimeClient client = GetTestClient();

        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        RealtimeConversationSessionOptions sessionOptions = new()
        {
            Instructions = "Use the available tools to help the user.",
            OutputModalities = { RealtimeOutputModality.Text },
            Tools = { mcpTool },
        };

        await ConfigureSessionAndWaitForMcpToolsAsync(sessionClient, sessionOptions);

        await sessionClient.AddItemAsync(
            RealtimeItem.CreateUserMessageItem("Search Microsoft Learn documentation for the OpenAI service. You can only call an MCP tool at most once."),
            CancellationToken);

        await sessionClient.StartResponseAsync(
            new RealtimeResponseOptions
            {
                OutputModalities = { RealtimeOutputModality.Text },
            },
            CancellationToken);

        int conversationItemDoneMcpToolApprovalRequestUpdateCount = 0;
        int conversationItemDoneMcpToolCallUpdateCount = 0;
        int mcpCallCompletedUpdateCount = 0;
        int responseDoneUpdateCount = 0;

        await foreach (RealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            if (update is RealtimeServerUpdateConversationItemDone conversationItemDone)
            {
                if (conversationItemDone.Item is RealtimeMcpToolCallApprovalRequestItem approvalItem)
                {
                    conversationItemDoneMcpToolApprovalRequestUpdateCount++;

                    Assert.That(approvalItem.ToolName, Is.EqualTo(toolName));
                    Assert.That(approvalItem.ServerLabel, Is.EqualTo(serverLabel));

                    // Approve the tool call and request another response.
                    await sessionClient.AddItemAsync(
                        new RealtimeMcpToolCallApprovalResponseItem(approvalItem.Id, approved: true),
                        CancellationToken);

                    await sessionClient.StartResponseAsync(
                        new RealtimeResponseOptions
                        {
                            OutputModalities = { RealtimeOutputModality.Text },
                        },
                        CancellationToken);
                }
                else if (conversationItemDone.Item is RealtimeMcpToolCallItem mcpToolCallItem)
                {
                    conversationItemDoneMcpToolCallUpdateCount++;

                    Assert.That(mcpToolCallItem.ToolName, Is.EqualTo(toolName));
                    Assert.That(mcpToolCallItem.ServerLabel, Is.EqualTo(serverLabel));
                }
            }

            if (update is RealtimeServerUpdateResponseMcpCallCompleted)
            {
                mcpCallCompletedUpdateCount++;
            }

            if (update is RealtimeServerUpdateResponseDone responseDone)
            {
                responseDoneUpdateCount++;
            }

            if (mcpCallCompletedUpdateCount >= 1 && conversationItemDoneMcpToolCallUpdateCount >= 1 && responseDoneUpdateCount >= 2)
            {
                break;
            }
        }

        Assert.That(conversationItemDoneMcpToolApprovalRequestUpdateCount, Is.GreaterThanOrEqualTo(1));
        Assert.That(conversationItemDoneMcpToolCallUpdateCount, Is.GreaterThanOrEqualTo(1));
        Assert.That(mcpCallCompletedUpdateCount, Is.GreaterThanOrEqualTo(1));
        Assert.That(responseDoneUpdateCount, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public async Task MCPToolWithAllowedTools()
    {
        string serverLabel = "microsoft-learn";
        Uri serverUri = new Uri("https://learn.microsoft.com/api/mcp");
        string toolName = "microsoft_docs_search";

        RealtimeMcpToolCallApprovalPolicy approvalPolicy =
            new RealtimeMcpToolCallApprovalPolicy(RealtimeDefaultMcpToolCallApprovalPolicy.NeverRequireApproval);

        RealtimeMcpTool mcpTool = new(serverLabel, serverUri)
        {
            ToolCallApprovalPolicy = approvalPolicy,
            AllowedTools = new RealtimeMcpToolFilter()
            {
                ToolNames = { toolName }
            }
        };

        RealtimeClient client = GetTestClient();

        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        RealtimeConversationSessionOptions sessionOptions = new()
        {
            Instructions = "Use the available tools to help the user.",
            OutputModalities = { RealtimeOutputModality.Text },
            Tools = { mcpTool },
        };

        await ConfigureSessionAndWaitForMcpToolsAsync(sessionClient, sessionOptions);

        await sessionClient.AddItemAsync(
            RealtimeItem.CreateUserMessageItem("Search Microsoft Learn documentation for the OpenAI service. You can only call an MCP tool at most once."),
            CancellationToken);

        await sessionClient.StartResponseAsync(
            new RealtimeResponseOptions
            {
                OutputModalities = { RealtimeOutputModality.Text },
            },
            CancellationToken);

        int mcpCallCompletedUpdateCount = 0;
        int conversationItemDoneUpdateCount = 0;
        int responseDoneUpdateCount = 0;

        await foreach (RealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            if (update is RealtimeServerUpdateConversationItemDone { Item: RealtimeMcpToolDefinitionListItem })
            {
                conversationItemDoneUpdateCount++;
            }

            if (update is RealtimeServerUpdateResponseDone responseDone)
            {
                responseDoneUpdateCount++;

                var outputItems = responseDone.Response.OutputItems;
                Assert.That(outputItems.OfType<RealtimeMcpToolCallApprovalRequestItem>().ToList(), Has.Count.EqualTo(0));
                Assert.That(outputItems.OfType<RealtimeMcpToolCallItem>().ToList(), Has.Count.GreaterThanOrEqualTo(1));

                RealtimeMcpToolCallItem toolCallItem = outputItems
                    .OfType<RealtimeMcpToolCallItem>()
                    .FirstOrDefault(item => item.ToolName == toolName);
                Assert.That(toolCallItem, Is.Not.Null);
                Assert.That(toolCallItem!.ServerLabel, Is.EqualTo(serverLabel));
                Assert.That(toolCallItem!.ToolName, Is.EqualTo(toolName));
                Assert.That(toolCallItem!.ToolArguments, Is.Not.Null);
                Assert.That(toolCallItem!.Error, Is.Null);
            }

            if (update is RealtimeServerUpdateResponseMcpCallCompleted)
            {
                mcpCallCompletedUpdateCount++;
                break;
            }
        }

        Assert.That(conversationItemDoneUpdateCount, Is.GreaterThan(0));
        Assert.That(responseDoneUpdateCount, Is.GreaterThan(0));
        Assert.That(mcpCallCompletedUpdateCount, Is.GreaterThan(0));
    }

    /// <summary>
    /// Helper that configures the session with an MCP tool, then waits for the
    /// <c>mcp_list_tools.completed</c> event before returning. This ensures the
    /// server has finished discovering the remote MCP tools so that a subsequent
    /// <c>response.create</c> will actually use them.
    /// </summary>
    private async Task<List<RealtimeServerUpdate>> ConfigureSessionAndWaitForMcpToolsAsync(
        RealtimeSessionClient sessionClient,
        RealtimeConversationSessionOptions sessionOptions)
    {
        await sessionClient.ConfigureConversationSessionAsync(sessionOptions, CancellationToken);

        List<RealtimeServerUpdate> setupUpdates = [];

        await foreach (RealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            setupUpdates.Add(update);

            if (update is RealtimeServerUpdateMcpListToolsCompleted)
            {
                break;
            }
            else if (update is RealtimeServerUpdateMcpListToolsFailed or RealtimeServerUpdateError)
            {
                // Guard: fail fast if there is a problem with retrieving MCP tools.
                Assert.Fail($"{update.Kind.ToString()}: {ModelReaderWriter.Write(update)}");
            }
        }

        return setupUpdates;
    }
}
