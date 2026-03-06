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
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        RealtimeMcpToolCallApprovalPolicy approvalPolicy =
            new RealtimeMcpToolCallApprovalPolicy(RealtimeDefaultMcpToolCallApprovalPolicy.NeverRequireApproval);

        RealtimeMcpTool mcpTool = new(serverLabel, serverUri)
        {
            // ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
            ToolCallApprovalPolicy = approvalPolicy
        };

        RealtimeClient client = GetTestClient();

        using RealtimeSessionClient sessionClient = await client.StartConversationSessionAsync(
            model: GetTestModel(),
            cancellationToken: CancellationToken);

        // Configure session with the MCP tool and text-only output, then wait
        // for the MCP tool listing to complete before triggering a response.
        RealtimeConversationSessionOptions sessionOptions = new()
        {
            Instructions = "Use the available tools to help the user.",
            OutputModalities = { RealtimeOutputModality.Text },
            Tools = { mcpTool },
        };

        List<RealtimeServerUpdate> setupUpdates =
            await ConfigureSessionAndWaitForMcpToolsAsync(sessionClient, sessionOptions);

        // Now send the user message and request a response.
        await sessionClient.AddItemAsync(
            RealtimeItem.CreateUserMessageItem("Roll 2d4+1"),
            CancellationToken);

        await sessionClient.StartResponseAsync(
            new RealtimeResponseOptions { OutputModalities = { RealtimeOutputModality.Text } },
            CancellationToken);

        int mcpCallArgumentsDeltaUpdateCount = 0;
        int mcpCallArgumentsDoneUpdateCount = 0;
        int mcpCallCompletedUpdateCount = 0;
        int mcpCallInProgressUpdateCount = 0;
        int conversationItemDoneUpdateCount = 0;
        int responseDoneUpdateCount = 0;
        RealtimeMcpToolCallItem toolCallItem = null;
        RealtimeMcpToolDefinitionListItem toolDefinitionListItem = null;

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
                toolCallItem = responseDone.Response.OutputItems.OfType<RealtimeMcpToolCallItem>().FirstOrDefault();

                Assert.That(toolCallItem, Is.Not.Null);
                Assert.That(toolCallItem.ServerLabel, Is.EqualTo(serverLabel));
                Assert.That(toolCallItem.ToolName, Is.EqualTo("roll"));
                Assert.That(toolCallItem.ToolArguments, Is.Not.Null);
                Assert.That(toolCallItem.Error, Is.Null);
            }

            if (update is RealtimeServerUpdateResponseMcpCallInProgress)
            {
                mcpCallInProgressUpdateCount++;
            }

            if (update is RealtimeServerUpdateConversationItemDone { Item: RealtimeMcpToolDefinitionListItem listItem })
            {
                conversationItemDoneUpdateCount++;
                toolDefinitionListItem = listItem;

                Assert.That(listItem.ToolDefinitions, Has.Count.GreaterThan(0));

                RealtimeMcpToolDefinition rollToolDefinition = listItem.ToolDefinitions
                    .Where(td => td.Name == "roll").FirstOrDefault();
                Assert.That(rollToolDefinition, Is.Not.Null);
                Assert.That(rollToolDefinition.InputSchema, Is.Not.Null);
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
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        RealtimeMcpToolCallApprovalPolicy approvalPolicy = useGlobalPolicy
            ? new RealtimeMcpToolCallApprovalPolicy(RealtimeDefaultMcpToolCallApprovalPolicy.NeverRequireApproval)
            : new RealtimeMcpToolCallApprovalPolicy(
                new RealtimeCustomMcpToolCallApprovalPolicy()
                {
                    ToolsNeverRequiringApproval = new RealtimeMcpToolFilter()
                    {
                        ToolNames = { "roll" }
                    }
                });

        RealtimeMcpTool mcpTool = new(serverLabel, serverUri)
        {
            ToolCallApprovalPolicy = approvalPolicy
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

        List<RealtimeServerUpdate> setupUpdates =
            await ConfigureSessionAndWaitForMcpToolsAsync(sessionClient, sessionOptions);

        await sessionClient.AddItemAsync(
            RealtimeItem.CreateUserMessageItem("Roll 2d4+1"),
            CancellationToken);

        await sessionClient.StartResponseAsync(
            new RealtimeResponseOptions { OutputModalities = { RealtimeOutputModality.Text } },
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

                // Confirm there are no approval requests and that the tool was called.
                var outputItems = responseDone.Response.OutputItems;
                Assert.That(outputItems.OfType<RealtimeMcpToolCallApprovalRequestItem>().ToList(), Has.Count.EqualTo(0));
                Assert.That(outputItems.OfType<RealtimeMcpToolCallItem>().ToList(), Has.Count.EqualTo(1));
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

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task MCPToolAlwaysRequiresApproval(bool useGlobalPolicy)
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        RealtimeMcpToolCallApprovalPolicy approvalPolicy = useGlobalPolicy
            ? new RealtimeMcpToolCallApprovalPolicy(RealtimeDefaultMcpToolCallApprovalPolicy.AlwaysRequireApproval)
            : new RealtimeMcpToolCallApprovalPolicy(
                new RealtimeCustomMcpToolCallApprovalPolicy()
                {
                    ToolsAlwaysRequiringApproval = new RealtimeMcpToolFilter()
                    {
                        ToolNames = { "roll" }
                    }
                });

        RealtimeMcpTool mcpTool = new(serverLabel, serverUri)
        {
            ToolCallApprovalPolicy = approvalPolicy
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

        List<RealtimeServerUpdate> setupUpdates =
            await ConfigureSessionAndWaitForMcpToolsAsync(sessionClient, sessionOptions);

        await sessionClient.AddItemAsync(
            RealtimeItem.CreateUserMessageItem("Roll 2d4+1"),
            CancellationToken);

        await sessionClient.StartResponseAsync(
            new RealtimeResponseOptions { OutputModalities = { RealtimeOutputModality.Text } },
            CancellationToken);

        // Single loop: the approval request arrives as a conversation.item.done event.
        // When found, approve it inline and keep listening for the tool call to complete.
        bool approvalSent = false;
        int approvalRequestUpdateCount = 0;
        int mcpCallCompletedUpdateCount = 0;
        int conversationItemDoneUpdateCount = 0;
        int responseDoneWithToolCallCount = 0;

        await foreach (RealtimeServerUpdate update in sessionClient.ReceiveUpdatesAsync(CancellationToken))
        {
            if (update is RealtimeServerUpdateConversationItemDone { Item: RealtimeMcpToolDefinitionListItem })
            {
                conversationItemDoneUpdateCount++;
            }

            if (!approvalSent
                && update is RealtimeServerUpdateConversationItemDone { Item: RealtimeMcpToolCallApprovalRequestItem approvalItem })
            {
                approvalRequestUpdateCount++;

                // Approve the tool call and request another response.
                await sessionClient.AddItemAsync(
                    new RealtimeMcpToolCallApprovalResponseItem(approvalItem.Id, approved: true),
                    CancellationToken);
                await sessionClient.StartResponseAsync(
                    new RealtimeResponseOptions { OutputModalities = { RealtimeOutputModality.Text } },
                    CancellationToken);
                approvalSent = true;
            }

            if (update is RealtimeServerUpdateResponseDone responseDone)
            {
                if (responseDone.Response.OutputItems.OfType<RealtimeMcpToolCallItem>().Any())
                {
                    responseDoneWithToolCallCount++;
                }
            }

            if (approvalSent && update is RealtimeServerUpdateResponseMcpCallCompleted)
            {
                mcpCallCompletedUpdateCount++;
                break;
            }
        }

        Assert.That(approvalRequestUpdateCount, Is.GreaterThan(0));
        Assert.That(conversationItemDoneUpdateCount, Is.GreaterThan(0));
        Assert.That(responseDoneWithToolCallCount, Is.GreaterThan(0));
        Assert.That(mcpCallCompletedUpdateCount, Is.GreaterThan(0));
    }

    [Test]
    public async Task MCPToolWithAllowedTools()
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        RealtimeMcpToolCallApprovalPolicy approvalPolicy =
            new RealtimeMcpToolCallApprovalPolicy(RealtimeDefaultMcpToolCallApprovalPolicy.NeverRequireApproval);

        RealtimeMcpTool mcpTool = new(serverLabel, serverUri)
        {
            ToolCallApprovalPolicy = approvalPolicy,
            AllowedTools = new RealtimeMcpToolFilter()
            {
                ToolNames = { "roll" }
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

        List<RealtimeServerUpdate> setupUpdates =
            await ConfigureSessionAndWaitForMcpToolsAsync(sessionClient, sessionOptions);

        await sessionClient.AddItemAsync(
            RealtimeItem.CreateUserMessageItem("Roll 2d4+1"),
            CancellationToken);

        await sessionClient.StartResponseAsync(
            new RealtimeResponseOptions { OutputModalities = { RealtimeOutputModality.Text } },
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
                Assert.That(outputItems.OfType<RealtimeMcpToolCallItem>().ToList(), Has.Count.EqualTo(1));

                RealtimeMcpToolCallItem toolCallItem = outputItems.OfType<RealtimeMcpToolCallItem>().First();
                Assert.That(toolCallItem.ServerLabel, Is.EqualTo(serverLabel));
                Assert.That(toolCallItem.ToolName, Is.EqualTo("roll"));
                Assert.That(toolCallItem.ToolArguments, Is.Not.Null);
                Assert.That(toolCallItem.Error, Is.Null);
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

            if (update is RealtimeServerUpdateMcpListToolsCompleted or
                RealtimeServerUpdateError)
            {
                break;
            }
        }

        // Guard: fail fast if there is an error in setup.
        RealtimeServerUpdateError setupError = setupUpdates.OfType<RealtimeServerUpdateError>().FirstOrDefault();
        Assert.That(setupError, Is.Null, () => $"Setup error: {ModelReaderWriter.Write(setupError)}");

        return setupUpdates;
    }
}
