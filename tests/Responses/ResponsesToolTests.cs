using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Containers;
using OpenAI.Files;
using OpenAI.Responses;
using OpenAI.Tests.Utility;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Responses;

[Category("Responses")]
[Category("ResponsesTools")]
public partial class ResponsesToolTests : OpenAIRecordedTestBase
{
    public ResponsesToolTests(bool isAsync) : base(isAsync)
    {
        TestTimeoutInSeconds = 30;
    }

    [RecordedTest]
    public async Task MCPToolWorks()
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        McpToolCallApprovalPolicy approvalPolicy = new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval);

        CreateResponseOptions options = new("gpt-5", [ResponseItem.CreateUserMessageItem("Roll 2d4+1")])
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        ResponsesClient client = GetTestClient(overrideModel: "gpt-5");

        ResponseResult response = await client.CreateResponseAsync(options);
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

    [RecordedTest]
    public async Task MCPToolStreamingWorks()
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        McpToolCallApprovalPolicy approvalPolicy = new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval);

        CreateResponseOptions options = new("gpt-5", [ResponseItem.CreateUserMessageItem("Roll 2d4+1")])
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
                    ToolCallApprovalPolicy = approvalPolicy
                }
            },
            StreamingEnabled = true,
        };

        ResponsesClient client = GetTestClient(overrideModel: "gpt-5");

        AsyncCollectionResult<StreamingResponseUpdate> responseUpdates = client.CreateResponseStreamingAsync(options);

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

    [RecordedTest]
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

        CreateResponseOptions options = new("gpt-5", [ResponseItem.CreateUserMessageItem("Roll 2d4+1")])
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        ResponsesClient client = GetTestClient(overrideModel: "gpt-5");

        ResponseResult response = await client.CreateResponseAsync(options);
        Assert.That(response.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response.OutputItems.OfType<McpToolDefinitionListItem>().ToList(), Has.Count.EqualTo(1));

        // Confirm there are no approval requests and that the tool was called.
        Assert.That(response.OutputItems.OfType<McpToolCallApprovalRequestItem>().ToList(), Has.Count.EqualTo(0));
        Assert.That(response.OutputItems.OfType<McpToolCallItem>().ToList(), Has.Count.EqualTo(1));
    }

    [RecordedTest]
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

        CreateResponseOptions options = new("gpt-5", [ResponseItem.CreateUserMessageItem("Roll 2d4+1")])
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        ResponsesClient client = GetTestClient(overrideModel: "gpt-5");

        ResponseResult response1 = await client.CreateResponseAsync(options);
        Assert.That(response1.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response1.OutputItems.OfType<McpToolDefinitionListItem>().ToList(), Has.Count.EqualTo(1));
        Assert.That(response1.OutputItems.OfType<McpToolCallItem>().ToList(), Has.Count.EqualTo(0));

        // Check that it stopped at the approval request.
        McpToolCallApprovalRequestItem approvalRequestItem = response1.OutputItems.Last() as McpToolCallApprovalRequestItem;
        Assert.That(approvalRequestItem, Is.Not.Null);

        // Prepare the response.
        McpToolCallApprovalResponseItem approvalResponseItem = new(approvalRequestItem.Id, true);
        options.PreviousResponseId = response1.Id;
        options.InputItems.Clear();
        options.InputItems.Add(approvalResponseItem);

        ResponseResult response2 = await client.CreateResponseAsync(options);
        Assert.That(response2.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response2.OutputItems.OfType<McpToolCallItem>().ToList(), Has.Count.EqualTo(1));
    }

    [RecordedTest]
    public async Task MCPToolWithAllowedTools()
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        McpToolCallApprovalPolicy approvalPolicy = new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval);

        CreateResponseOptions options = new("gpt-5", [ResponseItem.CreateUserMessageItem("Roll 2d4+1")])
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
                    ToolCallApprovalPolicy = approvalPolicy,
                    AllowedTools = new McpToolFilter()
                    {
                        ToolNames = { "roll" }
                    }
                }
            }
        };

        ResponsesClient client = GetTestClient(overrideModel: "gpt-5");

        ResponseResult response = await client.CreateResponseAsync(options);
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

    [RecordedTest]
    public async Task MCPToolWithDisallowedTools()
    {
        string serverLabel = "dmcp";
        Uri serverUri = new Uri("https://dmcp-server.deno.dev/sse");

        McpToolCallApprovalPolicy approvalPolicy = new McpToolCallApprovalPolicy(GlobalMcpToolCallApprovalPolicy.NeverRequireApproval);

        CreateResponseOptions options = new("gpt-5", [ResponseItem.CreateUserMessageItem("Roll 2d4+1")])
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
                    ToolCallApprovalPolicy = approvalPolicy,
                    AllowedTools = new McpToolFilter()
                    {
                        ToolNames = { "not_roll" } // This is not a real tool. We use this to implicitly disallow everything else.
                    }
                }
            }
        };

        ResponsesClient client = GetTestClient(overrideModel: "gpt-5");

        ResponseResult response = await client.CreateResponseAsync(options);
        Assert.That(response.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response.OutputItems.OfType<McpToolDefinitionListItem>().ToList(), Has.Count.EqualTo(1));
        Assert.That(response.OutputItems.OfType<McpToolCallApprovalRequestItem>().ToList(), Has.Count.EqualTo(0));
        Assert.That(response.OutputItems.OfType<McpToolCallItem>().ToList(), Has.Count.EqualTo(0));
    }

    [RecordedTest]
    public async Task FileSearch()
    {
        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>(TestScenario.Files);
        OpenAIFile testFile = await fileClient.UploadFileAsync(
            BinaryData.FromString("""
                    Travis's favorite food is pizza.
                    """),
            "test_favorite_foods.txt",
            FileUploadPurpose.UserData);
        Validate(testFile);

        VectorStoreClient vscClient = GetProxiedOpenAIClient<VectorStoreClient>(TestScenario.VectorStores);
        VectorStore vectorStore = await vscClient.CreateVectorStoreAsync(
            new VectorStoreCreationOptions()
            {
                FileIds = { testFile.Id },
            });
        Validate(vectorStore);

        if (Mode != RecordedTestMode.Playback)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        ResponsesClient client = GetTestClient();

        ResponseResult response = await client.CreateResponseAsync(
            new("gpt-4o-mini", [ResponseItem.CreateUserMessageItem("Using the file search tool, what's Travis's favorite food?")])
            {
                Tools =
                {
                    ResponseTool.CreateFileSearchTool(vectorStoreIds: [vectorStore.Id]),
                }
            });
        Assert.That(response.OutputItems?.Count, Is.EqualTo(2));
        FileSearchCallResponseItem fileSearchCall = response.OutputItems[0] as FileSearchCallResponseItem;
        Assert.That(fileSearchCall, Is.Not.Null);
        Assert.That(fileSearchCall?.Status, Is.EqualTo(FileSearchCallStatus.Completed));
        Assert.That(fileSearchCall?.Queries, Has.Count.GreaterThan(0));
        MessageResponseItem message = response.OutputItems[1] as MessageResponseItem;
        Assert.That(message, Is.Not.Null);
        ResponseContentPart messageContentPart = message.Content?.FirstOrDefault();
        Assert.That(messageContentPart, Is.Not.Null);
        Assert.That(messageContentPart.Text, Does.Contain("pizza"));
        Assert.That(messageContentPart.OutputTextAnnotations, Is.Not.Null.And.Not.Empty);
        FileCitationMessageAnnotation annotation = messageContentPart.OutputTextAnnotations[0] as FileCitationMessageAnnotation;
        Assert.That(annotation.FileId, Is.EqualTo(testFile.Id));
        Assert.That(annotation.Filename, Is.EqualTo(testFile.Filename));
        Assert.That(annotation.Index, Is.GreaterThan(0));

        await foreach (ResponseItem inputItem in client.GetResponseInputItemsAsync(new ResponseItemCollectionOptions(response.Id)))
        {
            Console.WriteLine(ModelReaderWriter.Write(inputItem).ToString());
        }
    }

    [RecordedTest]
    public async Task CodeInterpreterToolWithoutFileIds()
    {
        ResponsesClient client = GetTestClient();

        ResponseTool codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(new CodeInterpreterToolContainer(CodeInterpreterToolContainerConfiguration.CreateAutomaticContainerConfiguration()));
        CreateResponseOptions responseOptions = new("gpt-4o-mini", [ResponseItem.CreateUserMessageItem("Calculate the factorial of 5 using Python code.")])
        {
            Tools = { codeInterpreterTool },
        };

        ResponseResult response = await client.CreateResponseAsync(
            responseOptions);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        Assert.That(response.OutputItems[0], Is.InstanceOf<CodeInterpreterCallResponseItem>());
        Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

        MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
        Assert.That(message.Content, Has.Count.GreaterThan(0));
        Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));
        Assert.That(message.Content[0].Text, Is.Not.Null.And.Not.Empty);

        // Basic validation that the response was created successfully
        Assert.That(response.Id, Is.Not.Null.And.Not.Empty);

        Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<CodeInterpreterTool>());
    }

    [RecordedTest]
    public async Task CodeInterpreterToolWithEmptyFileIds()
    {
        ResponsesClient client = GetTestClient();

        ResponseTool codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(new(new AutomaticCodeInterpreterToolContainerConfiguration()));
        CreateResponseOptions responseOptions = new("gpt-4o-mini", [ResponseItem.CreateUserMessageItem("Generate a simple chart using matplotlib. Ensure you emit debug logging and include any resulting log file output.")])
        {
            Tools = { codeInterpreterTool },
        };

        ResponseResult response = await client.CreateResponseAsync(
            responseOptions);

        Assert.That(response, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        Assert.That(response.OutputItems[0], Is.InstanceOf<CodeInterpreterCallResponseItem>());
        Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

        MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
        Assert.That(message.Content, Has.Count.GreaterThan(0));
        Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));
        Assert.That(message.Content[0].Text, Is.Not.Null.And.Not.Empty);

        // Basic validation that the response was created successfully
        Assert.That(response.Id, Is.Not.Null.And.Not.Empty);

        Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<CodeInterpreterTool>());
    }

    [RecordedTest]
    public async Task CodeInterpreterToolWithContainerIdFromContainerApi()
    {
        ContainerClient containerClient = GetProxiedOpenAIClient<ContainerClient>(TestScenario.Containers);
        ResponsesClient client = GetTestClient();

        // Create a container first using the Containers API
        CreateContainerBody containerBody = new("test-container-for-code-interpreter");
        var containerResult = await containerClient.CreateContainerAsync(containerBody);
        Assert.That(containerResult.Value, Is.Not.Null);
        Assert.That(containerResult.Value.Id, Is.Not.Null.And.Not.Empty);

        string containerId = containerResult.Value.Id;

        try
        {
            // Create CodeInterpreter tool with the container ID
            ResponseTool codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(new(containerId));
            CreateResponseOptions responseOptions = new("gpt-4o-mini", [ResponseItem.CreateUserMessageItem("Calculate the factorial of 5 using Python code.")])
            {
                Tools = { codeInterpreterTool },
            };

            ResponseResult response = await client.CreateResponseAsync(
                responseOptions);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.OutputItems, Has.Count.EqualTo(2));
            Assert.That(response.OutputItems[0], Is.InstanceOf<CodeInterpreterCallResponseItem>());
            Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

            MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
            Assert.That(message.Content, Has.Count.GreaterThan(0));
            Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));
            Assert.That(message.Content[0].Text, Is.Not.Null.And.Not.Empty);

            // Basic validation that the response was created successfully
            Assert.That(response.Id, Is.Not.Null.And.Not.Empty);

            Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<CodeInterpreterTool>());
        }
        finally
        {
            // Clean up the container
            try
            {
                await containerClient.DeleteContainerAsync(containerId);
            }
            catch
            {
                // Best effort cleanup - don't fail test if cleanup fails
            }
        }
    }

    [RecordedTest]
    public async Task CodeInterpreterToolWithUploadedFileIds()
    {
        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>(TestScenario.Files);
        ResponsesClient client = GetTestClient();

        // Create some test files to upload
        string csvContent = "name,age,city\nAlice,30,New York\nBob,25,Los Angeles\nCharlie,35,Chicago";
        string pythonContent = "# This is a simple Python file\ndef hello():\n    print('Hello from uploaded file!')\n\nif __name__ == '__main__':\n    hello()";

        List<string> fileIds = new();

        try
        {
            // Upload CSV file
            using Stream csvStream = BinaryData.FromString(csvContent).ToStream();
            OpenAIFile csvFile = await fileClient.UploadFileAsync(csvStream, "test_data.csv", FileUploadPurpose.Assistants);
            Validate(csvFile);
            fileIds.Add(csvFile.Id);

            // Upload Python file  
            using Stream pythonStream = BinaryData.FromString(pythonContent).ToStream();
            OpenAIFile pythonFile = await fileClient.UploadFileAsync(pythonStream, "test_script.py", FileUploadPurpose.Assistants);
            Validate(pythonFile);
            fileIds.Add(pythonFile.Id);

            // Create CodeInterpreter tool with uploaded file IDs
            ResponseTool codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(new(CodeInterpreterToolContainerConfiguration.CreateAutomaticContainerConfiguration(fileIds)));
            CreateResponseOptions responseOptions = new("gpt-4o-mini", [ResponseItem.CreateUserMessageItem("Analyze the CSV data in the uploaded file and create a simple visualization. Also run the Python script that was uploaded.")])
            {
                Tools = { codeInterpreterTool },
            };

            ResponseResult response = await client.CreateResponseAsync(
                responseOptions);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.OutputItems, Is.Not.Null.And.Not.Empty);

            // Basic validation that the response was created successfully
            Assert.That(response.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<CodeInterpreterTool>());
        }
        catch
        {
            // If the test fails, still try to clean up the files immediately
            // (They'll also be cleaned up in OneTimeTearDown, but this is more immediate)
            foreach (string fileId in fileIds)
            {
                try
                {
                    await fileClient.DeleteFileAsync(fileId);
                }
                catch
                {
                    // Best effort cleanup
                }
            }
            throw;
        }
    }

    [RecordedTest]
    public async Task CodeInterpreterToolStreaming()
    {
        ResponsesClient client = GetTestClient();

        ResponseTool codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(new CodeInterpreterToolContainer(new AutomaticCodeInterpreterToolContainerConfiguration()));
        CreateResponseOptions responseOptions = new("gpt-4o-mini", [ResponseItem.CreateUserMessageItem("Calculate the factorial of 5 using Python code and show me the code step by step.")])
        {
            Tools = { codeInterpreterTool },
            StreamingEnabled = true,
        };

        int inProgressCount = 0;
        int interpretingCount = 0;
        int codeDeltaCount = 0;
        int codeDoneCount = 0;
        int completedCount = 0;
        bool gotFinishedCodeInterpreterItem = false;
        StringBuilder codeBuilder = new StringBuilder();

        await foreach (StreamingResponseUpdate update
            in client.CreateResponseStreamingAsync(responseOptions))
        {
            ValidateCodeInterpreterEvent(ref inProgressCount, ref interpretingCount, ref codeDeltaCount, ref codeDoneCount, ref completedCount, ref gotFinishedCodeInterpreterItem, codeBuilder, update);
        }

        Assert.That(gotFinishedCodeInterpreterItem, Is.True);
        Assert.That(inProgressCount, Is.GreaterThan(0));
        Assert.That(interpretingCount, Is.GreaterThan(0));
        Assert.That(codeDeltaCount, Is.GreaterThan(0)); // We should get at least some delta events
        Assert.That(codeDoneCount, Is.EqualTo(1)); // Should be exactly one "done" event
        Assert.That(completedCount, Is.GreaterThan(0));
    }

    [RecordedTest]
    public async Task CodeInterpreterToolStreamingWithFiles()
    {
        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>(TestScenario.Files);
        ResponsesClient client = GetTestClient();

        // Create test CSV data
        string csvContent = "x,y\n1,2\n2,4\n3,6\n4,8\n5,10";
        List<string> fileIds = new();

        try
        {
            // Upload CSV file
            using Stream csvStream = BinaryData.FromString(csvContent).ToStream();
            OpenAIFile csvFile = await fileClient.UploadFileAsync(csvStream, "test_data.csv", FileUploadPurpose.Assistants);
            Validate(csvFile);
            fileIds.Add(csvFile.Id);

            // Create CodeInterpreter tool with uploaded file IDs
            ResponseTool codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(new CodeInterpreterToolContainer(CodeInterpreterToolContainerConfiguration.CreateAutomaticContainerConfiguration(fileIds)));
            CreateResponseOptions responseOptions = new("gpt-4o-mini", [ResponseItem.CreateUserMessageItem("Load the CSV file and create a simple plot visualization showing the relationship between x and y values.")])
            {
                Tools = { codeInterpreterTool },
                StreamingEnabled = true,
            };

            int inProgressCount = 0;
            int interpretingCount = 0;
            int codeDeltaCount = 0;
            int codeDoneCount = 0;
            int completedCount = 0;
            bool gotFinishedCodeInterpreterItem = false;
            StringBuilder codeBuilder = new StringBuilder();

            await foreach (StreamingResponseUpdate update
                in client.CreateResponseStreamingAsync(responseOptions))
            {
                ValidateCodeInterpreterEvent(ref inProgressCount, ref interpretingCount, ref codeDeltaCount, ref codeDoneCount, ref completedCount, ref gotFinishedCodeInterpreterItem, codeBuilder, update);
            }

            Assert.That(gotFinishedCodeInterpreterItem, Is.True);
            Assert.That(inProgressCount, Is.GreaterThan(0));
            Assert.That(interpretingCount, Is.GreaterThan(0));
            Assert.That(codeDeltaCount, Is.GreaterThan(0));
            Assert.That(codeDoneCount, Is.GreaterThanOrEqualTo(1)); // Should be at least one "done" event
            Assert.That(completedCount, Is.GreaterThan(0));
        }
        catch
        {
            // If the test fails, still try to clean up the files immediately
            foreach (string fileId in fileIds)
            {
                try
                {
                    await fileClient.DeleteFileAsync(fileId);
                }
                catch
                {
                    // Best effort cleanup
                }
            }
            throw;
        }
    }

    [RecordedTest]
    public async Task ImageGenToolWorks()
    {
        ResponsesClient client = GetTestClient();

        CreateResponseOptions options = new(
            "gpt-4o-mini",
            [ResponseItem.CreateUserMessageItem("Generate an image of gray tabby cat hugging an otter with an orange scarf")])
        {
            Tools =
            {
                ResponseTool.CreateImageGenerationTool(
                    model: "gpt-image-1",
                    quality: ImageGenerationToolQuality.High,
                    size: ImageGenerationToolSize.W1024xH1024,
                    outputFileFormat: ImageGenerationToolOutputFileFormat.Png,
                    moderationLevel: ImageGenerationToolModerationLevel.Auto,
                    background: ImageGenerationToolBackground.Transparent,
                    inputFidelity: ImageGenerationToolInputFidelity.High)
            }
        };

        ResponseResult response = await client.CreateResponseAsync(options);

        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        Assert.That(response.OutputItems[0], Is.InstanceOf<ImageGenerationCallResponseItem>());
        Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

        MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
        Assert.That(message.Content, Has.Count.GreaterThan(0));
        Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));

        Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<ImageGenerationTool>());

        ImageGenerationCallResponseItem imageGenResponse = (ImageGenerationCallResponseItem)response.OutputItems[0];
        Assert.That(imageGenResponse.Status, Is.EqualTo(ImageGenerationCallStatus.Completed));
        Assert.That(imageGenResponse.ImageResultBytes.ToArray(), Is.Not.Null.And.Not.Empty);
    }

    [RecordedTest]
    public async Task ImageGenToolStreaming()
    {
        ResponsesClient client = GetTestClient();

        const string message = "Draw a gorgeous image of a river made of white owl feathers, snaking its way through a serene winter landscape";

        CreateResponseOptions responseOptions = new("gpt-4o-mini", [ResponseItem.CreateUserMessageItem(message)])
        {
            Tools =
            {
                ResponseTool.CreateImageGenerationTool(
                    model: "gpt-image-1",
                    quality: ImageGenerationToolQuality.High,
                    size: ImageGenerationToolSize.W1024xH1024,
                    outputFileFormat: ImageGenerationToolOutputFileFormat.Png,
                    moderationLevel: ImageGenerationToolModerationLevel.Auto,
                    background: ImageGenerationToolBackground.Transparent)
            },
            StreamingEnabled = true,
        };

        string imageGenItemId = null;
        int partialCount = 0;
        int inProgressCount = 0;
        int generateCount = 0;
        bool gotCompletedImageGenItem = false;
        bool gotCompletedResponseItem = false;

        await foreach (StreamingResponseUpdate update
            in client.CreateResponseStreamingAsync(responseOptions))
        {
            if (update is StreamingResponseImageGenerationCallPartialImageUpdate imageGenCallInPartialUpdate)
            {
                Assert.That(imageGenCallInPartialUpdate.ItemId, Is.Not.Null.And.Not.Empty);
                imageGenItemId ??= imageGenCallInPartialUpdate.ItemId;
                Assert.That(imageGenItemId, Is.EqualTo(imageGenCallInPartialUpdate.ItemId));
                Assert.That(imageGenCallInPartialUpdate.OutputIndex, Is.EqualTo(0));
                Assert.That(imageGenCallInPartialUpdate.PartialImageBytes, Is.Not.Null);
                partialCount++;
            }
            else if (update is StreamingResponseImageGenerationCallInProgressUpdate imageGenCallInProgressUpdate)
            {
                Assert.That(imageGenCallInProgressUpdate.ItemId, Is.Not.Null.And.Not.Empty);
                imageGenItemId ??= imageGenCallInProgressUpdate.ItemId;
                Assert.That(imageGenItemId, Is.EqualTo(imageGenCallInProgressUpdate.ItemId));
                Assert.That(imageGenCallInProgressUpdate.OutputIndex, Is.EqualTo(0));
                inProgressCount++;
            }
            else if (update is StreamingResponseImageGenerationCallGeneratingUpdate imageGenCallGeneratingUpdate)
            {
                Assert.That(imageGenCallGeneratingUpdate.ItemId, Is.Not.Null.And.Not.Empty);
                imageGenItemId ??= imageGenCallGeneratingUpdate.ItemId;
                Assert.That(imageGenItemId, Is.EqualTo(imageGenCallGeneratingUpdate.ItemId));
                Assert.That(imageGenCallGeneratingUpdate.OutputIndex, Is.EqualTo(0));
                generateCount++;
            }
            else if (update is StreamingResponseImageGenerationCallCompletedUpdate outputItemCompleteUpdate)
            {
                Assert.That(outputItemCompleteUpdate.ItemId, Is.Not.Null.And.Not.Empty);
                imageGenItemId ??= outputItemCompleteUpdate.ItemId;
                Assert.That(imageGenItemId, Is.EqualTo(outputItemCompleteUpdate.ItemId));
                Assert.That(outputItemCompleteUpdate.OutputIndex, Is.EqualTo(0));
                gotCompletedImageGenItem = true;
            }
            else if (update is StreamingResponseOutputItemDoneUpdate outputItemDoneUpdate)
            {
                if (outputItemDoneUpdate.Item is ImageGenerationCallResponseItem imageGenCallItem)
                {
                    Assert.That(imageGenCallItem.Id, Is.Not.Null.And.Not.Empty);
                    imageGenItemId ??= imageGenCallItem.Id;
                    Assert.That(imageGenItemId, Is.EqualTo(outputItemDoneUpdate.Item.Id));
                    Assert.That(outputItemDoneUpdate.OutputIndex, Is.EqualTo(0));
                    gotCompletedResponseItem = true;
                }
            }
        }

        Assert.That(gotCompletedResponseItem || gotCompletedImageGenItem, Is.True);
        Assert.That(partialCount, Is.EqualTo(1));
        Assert.That(inProgressCount, Is.EqualTo(1));
        Assert.That(generateCount, Is.EqualTo(1));
        Assert.That(imageGenItemId, Is.Not.Null.And.Not.Empty);
    }

    [RecordedTest]
    [LiveOnly(Reason = "Temp due to the recording framework timing out")]
    public async Task ImageGenToolInputMaskWithImageBytes()
    {
        ResponsesClient client = GetTestClient(options: new() { NetworkTimeout = TimeSpan.FromMinutes(5) });

        string imageFilename = "images_empty_room.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        BinaryData imageBytes = BinaryData.FromBytes(File.ReadAllBytes(imagePath));

        string maskFilename = "images_empty_room_with_mask.png";
        string maskPath = Path.Combine("Assets", maskFilename);
        BinaryData maskBytes = BinaryData.FromBytes(File.ReadAllBytes(maskPath));

        List<ResponseItem> inputItems = [
            ResponseItem.CreateUserMessageItem("Edit this image by adding a big cat with big round eyes and large cat ears, sitting in an empty room and looking at the camera."),
            ResponseItem.CreateUserMessageItem([ResponseContentPart.CreateInputImagePart(imageBytes, "image/png")])
        ];

        CreateResponseOptions options = new("gpt-4o-mini", inputItems)
        {
            Tools =
            {
                ResponseTool.CreateImageGenerationTool(
                    model: "gpt-image-1-mini",
                    outputFileFormat: ImageGenerationToolOutputFileFormat.Png,
                    size: ImageGenerationToolSize.W1024xH1024,
                    quality: ImageGenerationToolQuality.Low,
                    inputImageMask: new(maskBytes, "image/png"))
            }
        };

        ResponseResult response = await client.CreateResponseAsync(options);

        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        Assert.That(response.OutputItems[0], Is.InstanceOf<ImageGenerationCallResponseItem>());
        Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

        MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
        Assert.That(message.Content, Has.Count.GreaterThan(0));
        Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));

        Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<ImageGenerationTool>());

        ImageGenerationCallResponseItem imageGenResponse = (ImageGenerationCallResponseItem)response.OutputItems[0];
        Assert.That(imageGenResponse.Status, Is.EqualTo(ImageGenerationCallStatus.Completed));
        Assert.That(imageGenResponse.ImageResultBytes.ToArray(), Is.Not.Null.And.Not.Empty);
    }

    [RecordedTest]
    public async Task ImageGenToolInputMaskWithImageUri()
    {
        ResponsesClient client = GetTestClient(options: new() { NetworkTimeout = TimeSpan.FromMinutes(5) });

        Uri imageUri = new("https://github.com/openai/openai-dotnet/blob/db6328accdd7927f19915cdc5412eb841f2447c1/tests/Assets/images_empty_room.png?raw=true");
        Uri maskUri = new("https://github.com/openai/openai-dotnet/blob/db6328accdd7927f19915cdc5412eb841f2447c1/tests/Assets/images_empty_room_with_mask.png?raw=true");

        List<ResponseItem> inputItems = [
            ResponseItem.CreateUserMessageItem("Edit this image by adding a big cat with big round eyes and large cat ears, sitting in an empty room and looking at the camera."),
            ResponseItem.CreateUserMessageItem([ResponseContentPart.CreateInputImagePart(imageUri)])
        ];

        CreateResponseOptions options = new("gpt-4o-mini", inputItems)
        {
            Tools =
            {
                ResponseTool.CreateImageGenerationTool(
                    model: "gpt-image-1-mini",
                    outputFileFormat: ImageGenerationToolOutputFileFormat.Png,
                    size: ImageGenerationToolSize.W1024xH1024,
                    quality: ImageGenerationToolQuality.Low,
                    inputImageMask: new(maskUri))
            }
        };

        ResponseResult response = await client.CreateResponseAsync(options);

        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        Assert.That(response.OutputItems[0], Is.InstanceOf<ImageGenerationCallResponseItem>());
        Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

        MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
        Assert.That(message.Content, Has.Count.GreaterThan(0));
        Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));

        Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<ImageGenerationTool>());

        ImageGenerationCallResponseItem imageGenResponse = (ImageGenerationCallResponseItem)response.OutputItems[0];
        Assert.That(imageGenResponse.Status, Is.EqualTo(ImageGenerationCallStatus.Completed));
        Assert.That(imageGenResponse.ImageResultBytes.ToArray(), Is.Not.Null.And.Not.Empty);
    }

    [RecordedTest]
    [Category("MPFD")]
    public async Task ImageGenToolInputMaskWithFileId()
    {
        ResponsesClient client = GetTestClient(options: new() { NetworkTimeout = TimeSpan.FromMinutes(5) });

        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>(TestScenario.Files);

        string imageFilename = "images_empty_room.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        BinaryData imageBytes = BinaryData.FromBytes(File.ReadAllBytes(imagePath));

        string maskFilename = "images_empty_room_with_mask.png";
        string maskPath = Path.Combine("Assets", maskFilename);
        BinaryData maskBytes = BinaryData.FromBytes(File.ReadAllBytes(maskPath));

        OpenAIFile imageFile;
        using (Recording.DisableRequestBodyRecording()) // Temp pending https://github.com/Azure/azure-sdk-tools/issues/11901
        {
            imageFile = await fileClient.UploadFileAsync(imageBytes, imageFilename, FileUploadPurpose.UserData);
        }
        Validate(imageFile);

        OpenAIFile maskFile;
        using (Recording.DisableRequestBodyRecording()) // Temp pending https://github.com/Azure/azure-sdk-tools/issues/11901
        {
            maskFile = await fileClient.UploadFileAsync(maskBytes, maskFilename, FileUploadPurpose.UserData);
        }
        Validate(imageFile);

        if (Mode != RecordedTestMode.Playback)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        List<ResponseItem> inputItems = [
            ResponseItem.CreateUserMessageItem("Edit this image by adding a big cat with big round eyes and large cat ears, sitting in an empty room and looking at the camera."),
            ResponseItem.CreateUserMessageItem([ResponseContentPart.CreateInputImagePart(imageFileId: imageFile.Id)])
        ];

        CreateResponseOptions options = new("gpt-4o-mini", inputItems)
        {
            Tools =
            {
                ResponseTool.CreateImageGenerationTool(
                    model: "gpt-image-1-mini",
                    outputFileFormat: ImageGenerationToolOutputFileFormat.Png,
                    size: ImageGenerationToolSize.W1024xH1024,
                    quality: ImageGenerationToolQuality.Low,
                    inputImageMask: new(fileId: maskFile.Id))
            }
        };

        ResponseResult response = await client.CreateResponseAsync(options);

        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        Assert.That(response.OutputItems[0], Is.InstanceOf<ImageGenerationCallResponseItem>());
        Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

        MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
        Assert.That(message.Content, Has.Count.GreaterThan(0));
        Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));

        Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<ImageGenerationTool>());

        ImageGenerationCallResponseItem imageGenResponse = (ImageGenerationCallResponseItem)response.OutputItems[0];
        Assert.That(imageGenResponse.Status, Is.EqualTo(ImageGenerationCallStatus.Completed));
        Assert.That(imageGenResponse.ImageResultBytes.ToArray(), Is.Not.Null.And.Not.Empty);
    }

    private List<string> FileIdsToDelete = [];
    private List<string> VectorStoreIdsToDelete = [];

    private void Validate<T>(T input) where T : class
    {
        if (input is OpenAIFile file)
        {
            FileIdsToDelete.Add(file.Id);
        }
        if (input is VectorStore vectorStore)
        {
            VectorStoreIdsToDelete.Add(vectorStore.Id);
        }
    }

    private static void ValidateCodeInterpreterEvent(ref int inProgressCount, ref int interpretingCount, ref int codeDeltaCount, ref int codeDoneCount, ref int completedCount, ref bool gotFinishedCodeInterpreterItem, StringBuilder codeBuilder, StreamingResponseUpdate update)
    {
        if (update is StreamingResponseCodeInterpreterCallInProgressUpdate codeInterpreterInProgressUpdate)
        {
            Assert.That(codeInterpreterInProgressUpdate.OutputIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(codeInterpreterInProgressUpdate.SequenceNumber, Is.GreaterThan(0));
            Assert.That(codeInterpreterInProgressUpdate.ItemId, Is.Not.Null.And.Not.Empty);
            inProgressCount++;
        }
        else if (update is StreamingResponseCodeInterpreterCallInterpretingUpdate codeInterpreterInterpretingUpdate)
        {
            Assert.That(codeInterpreterInterpretingUpdate.OutputIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(codeInterpreterInterpretingUpdate.SequenceNumber, Is.GreaterThan(0));
            Assert.That(codeInterpreterInterpretingUpdate.ItemId, Is.Not.Null.And.Not.Empty);
            interpretingCount++;
        }
        else if (update is StreamingResponseCodeInterpreterCallCodeDeltaUpdate codeInterpreterCodeDeltaUpdate)
        {
            Assert.That(codeInterpreterCodeDeltaUpdate.OutputIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(codeInterpreterCodeDeltaUpdate.SequenceNumber, Is.GreaterThan(0));
            Assert.That(codeInterpreterCodeDeltaUpdate.ItemId, Is.Not.Null.And.Not.Empty);
            Assert.That(codeInterpreterCodeDeltaUpdate.Delta, Is.Not.Null.And.Not.Empty);
            codeBuilder.Append(codeInterpreterCodeDeltaUpdate.Delta);
            codeDeltaCount++;
        }
        else if (update is StreamingResponseCodeInterpreterCallCodeDoneUpdate codeInterpreterCodeDoneUpdate)
        {
            Assert.That(codeInterpreterCodeDoneUpdate.OutputIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(codeInterpreterCodeDoneUpdate.SequenceNumber, Is.GreaterThan(0));
            Assert.That(codeInterpreterCodeDoneUpdate.ItemId, Is.Not.Null.And.Not.Empty);
            Assert.That(codeInterpreterCodeDoneUpdate.Code, Is.Not.Null.And.Not.Empty);
            // Verify that the accumulated deltas match the final code (if we got meaningful deltas)
            if (codeBuilder.Length > 0)
            {
                Assert.That(codeBuilder.ToString(), Does.Contain(codeInterpreterCodeDoneUpdate.Code));
            }
            codeDoneCount++;
        }
        else if (update is StreamingResponseCodeInterpreterCallCompletedUpdate codeInterpreterCompletedUpdate)
        {
            Assert.That(codeInterpreterCompletedUpdate.OutputIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(codeInterpreterCompletedUpdate.SequenceNumber, Is.GreaterThan(0));
            Assert.That(codeInterpreterCompletedUpdate.ItemId, Is.Not.Null.And.Not.Empty);
            completedCount++;
        }
        else if (update is StreamingResponseOutputItemDoneUpdate outputItemDoneUpdate)
        {
            if (outputItemDoneUpdate.Item is MessageResponseItem mri)
            {
                Assert.That(mri.Status, Is.EqualTo(MessageStatus.Completed));
                gotFinishedCodeInterpreterItem = true;
            }
        }
    }

    private ResponsesClient GetTestClient(string overrideModel = null, OpenAIClientOptions options = null) => GetProxiedOpenAIClient<ResponsesClient>(TestScenario.Responses, overrideModel, options: options);
}