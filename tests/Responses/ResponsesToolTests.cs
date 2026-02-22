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

        CreateResponseOptions options = new([ResponseItem.CreateUserMessageItem("Roll 2d4+1")])
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("gpt-5");

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

        CreateResponseOptions options = new([ResponseItem.CreateUserMessageItem("Roll 2d4+1")])
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

        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("gpt-5");

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

        CreateResponseOptions options = new([ResponseItem.CreateUserMessageItem("Roll 2d4+1")])
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("gpt-5");

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

        CreateResponseOptions options = new([ResponseItem.CreateUserMessageItem("Roll 2d4+1")])
        {
            Tools = {
                new McpTool(serverLabel, serverUri)
                {
                    ServerDescription = "A Dungeons and Dragons MCP server to assist with dice rolling.",
                    ToolCallApprovalPolicy = approvalPolicy
                }
            }
        };

        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("gpt-5");

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

        CreateResponseOptions options = new([ResponseItem.CreateUserMessageItem("Roll 2d4+1")])
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

        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("gpt-5");

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

        CreateResponseOptions options = new([ResponseItem.CreateUserMessageItem("Roll 2d4+1")])
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

        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("gpt-5");

        ResponseResult response = await client.CreateResponseAsync(options);
        Assert.That(response.OutputItems, Has.Count.GreaterThan(0));
        Assert.That(response.OutputItems.OfType<McpToolDefinitionListItem>().ToList(), Has.Count.EqualTo(1));
        Assert.That(response.OutputItems.OfType<McpToolCallApprovalRequestItem>().ToList(), Has.Count.EqualTo(0));
        Assert.That(response.OutputItems.OfType<McpToolCallItem>().ToList(), Has.Count.EqualTo(0));
    }

    [RecordedTest]
    public async Task FileSearch()
    {
        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>();
        OpenAIFile testFile = await fileClient.UploadFileAsync(
            BinaryData.FromString("""
                    Travis's favorite food is pizza.
                    """),
            "test_favorite_foods.txt",
            FileUploadPurpose.UserData);
        Validate(testFile);

        VectorStoreClient vscClient = GetProxiedOpenAIClient<VectorStoreClient>();
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

        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        ResponseResult response = await client.CreateResponseAsync(
            new CreateResponseOptions([ResponseItem.CreateUserMessageItem("Using the file search tool, what's Travis's favorite food?")])
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
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        ResponseTool codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(new CodeInterpreterToolContainer(CodeInterpreterToolContainerConfiguration.CreateAutomaticContainerConfiguration()));
        CreateResponseOptions responseOptions = new([ResponseItem.CreateUserMessageItem("Calculate the factorial of 5 using Python code.")])
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
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        ResponseTool codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(new(new AutomaticCodeInterpreterToolContainerConfiguration()));
        CreateResponseOptions responseOptions = new([ResponseItem.CreateUserMessageItem("Generate a simple chart using matplotlib. Ensure you emit debug logging and include any resulting log file output.")])
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
        ContainerClient containerClient = GetProxiedOpenAIClient<ContainerClient>();
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

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
            CreateResponseOptions responseOptions = new([ResponseItem.CreateUserMessageItem("Calculate the factorial of 5 using Python code.")])
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
        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>();
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

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
            CreateResponseOptions responseOptions = new([ResponseItem.CreateUserMessageItem("Analyze the CSV data in the uploaded file and create a simple visualization. Also run the Python script that was uploaded.")])
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
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        ResponseTool codeInterpreterTool = ResponseTool.CreateCodeInterpreterTool(new CodeInterpreterToolContainer(new AutomaticCodeInterpreterToolContainerConfiguration()));
        CreateResponseOptions responseOptions = new([ResponseItem.CreateUserMessageItem("Calculate the factorial of 5 using Python code and show me the code step by step.")])
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
        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>();
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

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
            CreateResponseOptions responseOptions = new([ResponseItem.CreateUserMessageItem("Load the CSV file and create a simple plot visualization showing the relationship between x and y values.")])
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
    public async Task ComputerToolWithScreenshotRoundTrip()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("computer-use-preview-2025-03-11");
        ResponseTool computerTool = ResponseTool.CreateComputerTool(ComputerToolEnvironment.Windows, 1024, 768);
        CreateResponseOptions responseOptions = new(
            [
                ResponseItem.CreateDeveloperMessageItem("Call tools when the user asks to perform computer-related tasks like clicking interface elements."),
                ResponseItem.CreateUserMessageItem("Click on the Save button.")
            ])
        {
            Tools = { computerTool },
            TruncationMode = ResponseTruncationMode.Auto,
        };
        ResponseResult response = await client.CreateResponseAsync(responseOptions);

        while (true)
        {
            Assert.That(response.OutputItems.Count, Is.GreaterThan(0));
            ResponseItem outputItem = response.OutputItems?.LastOrDefault();
            if (outputItem is ComputerCallResponseItem computerCall)
            {
                if (computerCall.Action.Kind == ComputerCallActionKind.Screenshot)
                {
                    string screenshotPath = Path.Join("Assets", "images_screenshot_with_save_1024_768.png");
                    BinaryData screenshotBytes = BinaryData.FromBytes(File.ReadAllBytes(screenshotPath));
                    ResponseItem screenshotReply = ResponseItem.CreateComputerCallOutputItem(
                        computerCall.CallId,
                        ComputerCallOutput.CreateScreenshotOutput(screenshotBytes, "image/png"));

                    responseOptions.PreviousResponseId = response.Id;
                    responseOptions.InputItems.Clear();
                    responseOptions.InputItems.Add(screenshotReply);
                    response = await client.CreateResponseAsync(responseOptions);
                }
                else if (computerCall.Action.Kind == ComputerCallActionKind.Click)
                {
                    Console.WriteLine($"Instruction from model: click");
                    break;
                }
            }
            else if (outputItem is MessageResponseItem message
                && message.Content?.FirstOrDefault()?.Text?.ToLower() is string assistantText
                && (
                    assistantText.Contains("should i")
                    || assistantText.Contains("shall i")
                    || assistantText.Contains("can you confirm")
                    || assistantText.Contains("could you confirm")
                    || assistantText.Contains("please confirm")))
            {
                responseOptions.PreviousResponseId = response.Id;
                responseOptions.InputItems.Clear();
                responseOptions.InputItems.Add(
                    ResponseItem.CreateAssistantMessageItem("Yes, proceed."));
                response = await client.CreateResponseAsync(responseOptions);
            }
            else
            {
                break;
            }
        }
    }

    [RecordedTest]
    public async Task ImageGenToolWorks()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        CreateResponseOptions options = new(
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
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        const string message = "Draw a gorgeous image of a river made of white owl feathers, snaking its way through a serene winter landscape";

        CreateResponseOptions responseOptions = new([ResponseItem.CreateUserMessageItem(message)])
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

#if NET10_0_OR_GREATER
    [RecordedTest]
    public async Task ImageGenToolInputMaskWithImageBytes()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>(options: new() { NetworkTimeout = TimeSpan.FromMinutes(5) });

        string imagePath = Path.Combine("Assets", "images_empty_room.png");
        string imageMediaType = "image/png";
        BinaryData imageBytes = BinaryData.FromBytes(await File.ReadAllBytesAsync(imagePath));
        Uri imageDataUri = new($"data:{imageMediaType};base64,{Convert.ToBase64String(imageBytes.ToArray())}");

        string maskPath = Path.Combine("Assets", "images_empty_room_with_mask.png");
        string maskMediaType = "image/png";
        BinaryData maskBytes = BinaryData.FromBytes(File.ReadAllBytes(maskPath));
        Uri maskDataUri = new($"data:{maskMediaType};base64,{Convert.ToBase64String(maskBytes.ToArray())}");


        List<ResponseItem> inputItems = [
            ResponseItem.CreateUserMessageItem("Edit this image by adding a big cat with big round eyes and large cat ears, sitting in an empty room and looking at the camera."),
            ResponseItem.CreateUserMessageItem([ResponseContentPart.CreateInputImagePart(imageDataUri)])
        ];

        CreateResponseOptions options = new(inputItems)
        {
            Tools =
            {
                ResponseTool.CreateImageGenerationTool(
                    model: "gpt-image-1-mini",
                    outputFileFormat: ImageGenerationToolOutputFileFormat.Png,
                    size: ImageGenerationToolSize.W1024xH1024,
                    quality: ImageGenerationToolQuality.Low,
                    inputImageMask: new(maskDataUri))
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
#endif

    [RecordedTest]
    public async Task ImageGenToolInputMaskWithImageUri()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>(options: new() { NetworkTimeout = TimeSpan.FromMinutes(5) });

        Uri imageUri = new("https://github.com/openai/openai-dotnet/blob/db6328accdd7927f19915cdc5412eb841f2447c1/tests/Assets/images_empty_room.png?raw=true");
        Uri maskUri = new("https://github.com/openai/openai-dotnet/blob/db6328accdd7927f19915cdc5412eb841f2447c1/tests/Assets/images_empty_room_with_mask.png?raw=true");

        List<ResponseItem> inputItems = [
            ResponseItem.CreateUserMessageItem("Edit this image by adding a big cat with big round eyes and large cat ears, sitting in an empty room and looking at the camera."),
            ResponseItem.CreateUserMessageItem([ResponseContentPart.CreateInputImagePart(imageUri)])
        ];

        CreateResponseOptions options = new(inputItems)
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
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>(options: new() { NetworkTimeout = TimeSpan.FromMinutes(5) });

        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>();

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

        CreateResponseOptions options = new(inputItems)
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

    [RecordedTest]
    public async Task WebSearchCall()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();
        ResponseResult response = await client.CreateResponseAsync(
            new CreateResponseOptions([ResponseItem.CreateUserMessageItem("Searching the internet, what's the weather like in Seattle?")])
            {
                Tools =
                {
                    ResponseTool.CreateWebSearchTool()
                },
                ToolChoice = ResponseToolChoice.CreateWebSearchChoice()
            });

        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        Assert.That(response.OutputItems[0], Is.InstanceOf<WebSearchCallResponseItem>());
        Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

        MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
        Assert.That(message.Content, Has.Count.GreaterThan(0));
        Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));
        Assert.That(message.Content[0].Text, Is.Not.Null.And.Not.Empty);
        Assert.That(message.Content[0].OutputTextAnnotations, Has.Count.GreaterThan(0));

        Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<WebSearchTool>());
    }

    [RecordedTest]
    public async Task WebSearchCallPreview()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();
        ResponseResult response = await client.CreateResponseAsync(
            new CreateResponseOptions([ResponseItem.CreateUserMessageItem("What was a positive news story from today?")])
            {
                Tools =
                {
                    ResponseTool.CreateWebSearchPreviewTool()
                },
                ToolChoice = ResponseToolChoice.CreateWebSearchChoice()
            });

        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        Assert.That(response.OutputItems[0], Is.InstanceOf<WebSearchCallResponseItem>());
        Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

        MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
        Assert.That(message.Content, Has.Count.GreaterThan(0));
        Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));
        Assert.That(message.Content[0].Text, Is.Not.Null.And.Not.Empty);
        Assert.That(message.Content[0].OutputTextAnnotations, Has.Count.GreaterThan(0));

        Assert.That(response.Tools.FirstOrDefault(), Is.TypeOf<WebSearchPreviewTool>());
    }

    [RecordedTest]
    public async Task WebSearchCallStreaming()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        const string message = "Searching the internet, what's the weather like in San Francisco?";

        CreateResponseOptions responseOptions = new([ResponseItem.CreateUserMessageItem(message)])
        {
            Tools =
            {
                ResponseTool.CreateWebSearchTool(
                    userLocation: WebSearchToolLocation.CreateApproximateLocation(city: "San Francisco"),
                    searchContextSize: WebSearchToolContextSize.Low)
            },
            StreamingEnabled = true,
        };

        string searchItemId = null;
        int inProgressCount = 0;
        int searchingCount = 0;
        int completedCount = 0;
        bool gotFinishedSearchItem = false;

        await foreach (StreamingResponseUpdate update
            in client.CreateResponseStreamingAsync(responseOptions))
        {
            if (update is StreamingResponseWebSearchCallInProgressUpdate searchCallInProgressUpdate)
            {
                Assert.That(searchCallInProgressUpdate.ItemId, Is.Not.Null.And.Not.Empty);
                searchItemId ??= searchCallInProgressUpdate.ItemId;
                Assert.That(searchItemId, Is.EqualTo(searchCallInProgressUpdate.ItemId));
                Assert.That(searchCallInProgressUpdate.OutputIndex, Is.EqualTo(0));
                inProgressCount++;
            }
            else if (update is StreamingResponseWebSearchCallSearchingUpdate searchCallSearchingUpdate)
            {
                Assert.That(searchCallSearchingUpdate.ItemId, Is.Not.Null.And.Not.Empty);
                searchItemId ??= searchCallSearchingUpdate.ItemId;
                Assert.That(searchItemId, Is.EqualTo(searchCallSearchingUpdate.ItemId));
                Assert.That(searchCallSearchingUpdate.OutputIndex, Is.EqualTo(0));
                searchingCount++;
            }
            else if (update is StreamingResponseWebSearchCallCompletedUpdate searchCallCompletedUpdate)
            {
                Assert.That(searchCallCompletedUpdate.ItemId, Is.Not.Null.And.Not.Empty);
                searchItemId ??= searchCallCompletedUpdate.ItemId;
                Assert.That(searchItemId, Is.EqualTo(searchCallCompletedUpdate.ItemId));
                Assert.That(searchCallCompletedUpdate.OutputIndex, Is.EqualTo(0));
                completedCount++;
            }
            else if (update is StreamingResponseOutputItemDoneUpdate outputItemDoneUpdate)
            {
                if (outputItemDoneUpdate.Item is WebSearchCallResponseItem webSearchCallItem)
                {
                    Assert.That(webSearchCallItem.Status, Is.EqualTo(WebSearchCallStatus.Completed));
                    Assert.That(webSearchCallItem.Id, Is.EqualTo(searchItemId));
                    gotFinishedSearchItem = true;
                }
            }
        }

        Assert.That(gotFinishedSearchItem, Is.True);
        Assert.That(searchingCount, Is.EqualTo(1));
        Assert.That(inProgressCount, Is.EqualTo(1));
        Assert.That(completedCount, Is.EqualTo(1));
        Assert.That(searchItemId, Is.Not.Null.And.Not.Empty);
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
}