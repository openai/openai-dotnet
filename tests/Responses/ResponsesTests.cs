using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
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
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Responses;

#pragma warning disable OPENAICUA001

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.Fixtures)]
[Category("Responses")]
public partial class ResponsesTests : SyncAsyncTestBase
{
    public ResponsesTests(bool isAsync) : base(isAsync)
    {
    }

    [OneTimeTearDown]
    protected void Cleanup()
    {
        Console.WriteLine("[Teardown]");

        // Skip cleanup if there is no API key (e.g., if we are not running live tests).
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENAI_API_KEY")))
        {
            Console.WriteLine("[WARNING] Can't clean up");
            return;
        }

        RequestOptions noThrowOptions = new() { ErrorOptions = ClientErrorBehaviors.NoThrow };
        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        VectorStoreClient vectorStoreClient = GetTestClient<VectorStoreClient>(TestScenario.VectorStores);

        foreach (string fileId in FileIdsToDelete)
        {
            Console.WriteLine($"[File cleanup] {fileId}");
            fileClient.DeleteFile(fileId, noThrowOptions);
        }

        foreach (string vectorStoreId in VectorStoreIdsToDelete)
        {
            Console.WriteLine($"[Vector store cleanup] {vectorStoreId}");
            vectorStoreClient.DeleteVectorStore(vectorStoreId, noThrowOptions);
        }
    }

    private List<string> FileIdsToDelete = [];
    private List<string> VectorStoreIdsToDelete = [];

    private void Validate<T>(T input) where T : class
    {
        if (input is OpenAIFile file)
        {
            FileIdsToDelete.Add(file.Id);
        }
        if (input is CreateVectorStoreOperation operation)
        {
            VectorStoreIdsToDelete.Add(operation.VectorStoreId);
        }
    }

    [Test]
    public async Task FileSearch()
    {
        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        OpenAIFile testFile = await fileClient.UploadFileAsync(
            BinaryData.FromString("""
                    Travis's favorite food is pizza.
                    """),
            "test_favorite_foods.txt",
            FileUploadPurpose.UserData);
        Validate(testFile);

        VectorStoreClient vscClient = GetTestClient<VectorStoreClient>(TestScenario.VectorStores);
        CreateVectorStoreOperation createStoreOp = await vscClient.CreateVectorStoreAsync(
            waitUntilCompleted: true,
            new VectorStoreCreationOptions()
            {
                FileIds = { testFile.Id },
            });
        Validate(createStoreOp);

        OpenAIResponseClient client = GetTestClient();

        OpenAIResponse response = await client.CreateResponseAsync(
            "Using the file search tool, what's Travis's favorite food?",
            new ResponseCreationOptions()
            {
                Tools =
                {
                    ResponseTool.CreateFileSearchTool([createStoreOp.VectorStoreId], null),
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
        Assert.That(messageContentPart.OutputTextAnnotations[0].FileCitationFileId, Is.EqualTo(testFile.Id));
        Assert.That(messageContentPart.OutputTextAnnotations[0].FileCitationIndex, Is.GreaterThan(0));


        await foreach (ResponseItem inputItem in client.GetResponseInputItemsAsync(response.Id))
        {
            Console.WriteLine(ModelReaderWriter.Write(inputItem).ToString());
        }
    }

    [Test]
    public async Task ChatWithCua()
    {
        OpenAIResponseClient client = GetTestClient("computer-use-preview-2025-03-11");

        ResponseTool computerTool = ResponseTool.CreateComputerTool(1024, 768, ComputerToolEnvironment.Windows);
        OpenAIResponse response = await client.CreateResponseAsync(
            inputItems:
            [
                ResponseItem.CreateDeveloperMessageItem("Call tools when the user asks to perform computer-related tasks like clicking interface elements."),
                ResponseItem.CreateUserMessageItem("Click on the Save button.")
            ],
            new ResponseCreationOptions() { Tools = { computerTool } });

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
                        [],
                        screenshotBytes,
                        "image/png");

                    response = await client.CreateResponseAsync(
                        [screenshotReply],
                        new ResponseCreationOptions()
                        {
                            PreviousResponseId = response.Id,
                            Tools = { computerTool },
                        });
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
                response = await client.CreateResponseAsync(
                    "Yes, proceed.",
                    new ResponseCreationOptions()
                    {
                        PreviousResponseId = response.Id,
                        Tools = { computerTool }
                    });
            }
            else
            {
                break;
            }
        }
    }

    [Test]
    public async Task WebSearchCall()
    {
        OpenAIResponseClient client = GetTestClient();
        OpenAIResponse response = await client.CreateResponseAsync(
            "What was a positive news story from today?",
            new ResponseCreationOptions()
            {
                Tools =
                {
                    ResponseTool.CreateWebSearchTool()
                }
            });

        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        Assert.That(response.OutputItems[0], Is.InstanceOf<WebSearchCallResponseItem>());
        Assert.That(response.OutputItems[1], Is.InstanceOf<MessageResponseItem>());

        MessageResponseItem message = (MessageResponseItem)response.OutputItems[1];
        Assert.That(message.Content, Has.Count.GreaterThan(0));
        Assert.That(message.Content[0].Kind, Is.EqualTo(ResponseContentPartKind.OutputText));
        Assert.That(message.Content[0].Text, Is.Not.Null.And.Not.Empty);
        Assert.That(message.Content[0].OutputTextAnnotations, Has.Count.GreaterThan(0));
    }

    [Test]
    public async Task WebSearchCallStreaming()
    {
        OpenAIResponseClient client = GetTestClient();

        const string message = "Searching the internet, what's the weather like in San Francisco?";

        ResponseCreationOptions responseOptions = new()
        {
            Tools =
            {
                ResponseTool.CreateWebSearchTool(
                    WebSearchToolLocation.CreateApproximateLocation(city: "San Francisco"),
                    WebSearchToolContextSize.Low)
            }
        };

        string searchItemId = null;
        int inProgressCount = 0;
        int searchingCount = 0;
        int completedCount = 0;
        bool gotFinishedSearchItem = false;

        await foreach (StreamingResponseUpdate update
            in client.CreateResponseStreamingAsync(message, responseOptions))
        {
            if (update is StreamingResponseWebSearchCallUpdate webCallUpdate)
            {
                Assert.That(webCallUpdate.OutputItemId, Is.Not.Null.And.Not.Empty);
                searchItemId ??= webCallUpdate.OutputItemId;
                Assert.That(searchItemId, Is.EqualTo(webCallUpdate.OutputItemId));
                Assert.That(webCallUpdate.OutputItemIndex, Is.EqualTo(0));
                if (webCallUpdate.Kind == StreamingResponseUpdateKind.ResponseWebSearchCallInProgress)
                {
                    inProgressCount++;
                }
                else if (webCallUpdate.Kind == StreamingResponseUpdateKind.ResponseWebSearchCallSearching)
                {
                    searchingCount++;
                }
                else if (webCallUpdate.Kind == StreamingResponseUpdateKind.ResponseWebSearchCallCompleted)
                {
                    completedCount++;
                }
            }
            else if (update is StreamingResponseItemUpdate itemUpdate
                && itemUpdate.Kind == StreamingResponseUpdateKind.ResponseOutputItemDone)
            {
                if (itemUpdate.Item is WebSearchCallResponseItem finishedWebSearchCall)
                {
                    Assert.That(finishedWebSearchCall.Status, Is.EqualTo(WebSearchCallStatus.Completed));
                    Assert.That(finishedWebSearchCall.Id, Is.EqualTo(searchItemId));
                    gotFinishedSearchItem = true;
                }
            }
            // Console.WriteLine(ModelReaderWriter.Write(update).ToString());
        }
        Assert.Multiple(() =>
        {
            Assert.That(gotFinishedSearchItem, Is.True);
            Assert.That(searchingCount, Is.EqualTo(1));
            Assert.That(inProgressCount, Is.EqualTo(1));
            Assert.That(completedCount, Is.EqualTo(1));
            Assert.That(searchItemId, Is.Not.Null.And.Not.Empty);
        });
    }

    [Test]
    public async Task StreamingResponses()
    {
        OpenAIResponseClient client = GetTestClient("gpt-4o-mini"); // "computer-use-alpha");

        List<ResponseItem> inputItems = [ResponseItem.CreateUserMessageItem("Hello, world!")];
        await foreach (StreamingResponseUpdate update in client.CreateResponseStreamingAsync(inputItems))
        {
            Console.WriteLine(ModelReaderWriter.Write(update));
            if (update is StreamingResponseContentPartDeltaUpdate deltaUpdate)
            {
                Console.Write(deltaUpdate.Text);
            }
        }
    }

    [Test]
    [Category("Smoke")]
    public void DeserializeStreamingWrapper()
    {
        string serializedEvent = """
            {"type":"response.output_text.delta","item_id":"msg_67cbd19634ac8190a8d1fe7d421960e0","output_index":0,"content_index":0,"delta":"Hello"}
            """;
        StreamingResponseContentPartDeltaUpdate deltaUpdate = ModelReaderWriter.Read<StreamingResponseContentPartDeltaUpdate>(BinaryData.FromString(serializedEvent));
        Assert.That(deltaUpdate, Is.Not.Null);
        Assert.That(deltaUpdate.Text, Is.EqualTo("Hello"));
    }

    [Test]
    [TestCase("gpt-4o-mini")]
    [TestCase("computer-use-preview")]
    public async Task ResponsesHelloWorldWithTool(string model)
    {
        OpenAIResponseClient client = GetTestClient(model);

        ResponseCreationOptions options = new()
        {
            Tools =
            {
                ResponseTool.CreateFunctionTool(
                    functionName: "get_custom_greeting",
                    functionDescription: "invoked when user provides a typical greeting",
                    functionParameters: BinaryData.FromString(
                        """
                        {
                          "type": "object",
                          "properties": {
                            "time_of_day": {
                              "type": "string"
                            }
                          }
                        }
                        """),
                    functionSchemaIsStrict: false),
            },
        };

        OpenAIResponse response = await client.CreateResponseAsync(
            [
                ResponseItem.CreateUserMessageItem(
                [
                    ResponseContentPart.CreateInputTextPart("good morning, responses!"),
                ]),
            ],
            options);

        Assert.That(response.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(response.CreatedAt, Is.GreaterThan(DateTimeOffset.Now - TimeSpan.FromDays(1)));
        // Assert.That(response.Status, Is.EqualTo(ResponsesStatus.Completed));
        Assert.That(response.Model, Is.Not.Null.And.Not.Empty);
        Assert.That(response.PreviousResponseId, Is.Null);
        // Observed: input may not exist on normal responses
        // Assert.That(response.Input.Count, Is.EqualTo(1));
        Assert.That(response.OutputItems.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task ResponsesWithReasoning()
    {
        OpenAIResponseClient client = GetTestClient("o3-mini");

        ResponseCreationOptions options = new()
        {
            ReasoningOptions = new()
            {
                ReasoningEffortLevel = ResponseReasoningEffortLevel.Low,
            },
            Metadata =
            {
                ["superfluous_key"] = "superfluous_value",
            },
            Instructions = "Perform reasoning over any questions asked by the user.",
        };

        OpenAIResponse response = await client.CreateResponseAsync([ResponseItem.CreateUserMessageItem("What's the best way to fold a burrito?")], options);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null);
        Assert.That(response.CreatedAt, Is.GreaterThan(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)));
        Assert.That(response.TruncationMode, Is.EqualTo(ResponseTruncationMode.Auto));
        Assert.That(response.MaxOutputTokenCount, Is.Null);
        Assert.That(response.Model, Does.StartWith("o3-mini"));
        Assert.That(response.Usage, Is.Not.Null);
        Assert.That(response.Usage.OutputTokenDetails, Is.Not.Null);
        Assert.That(response.Usage.OutputTokenDetails.ReasoningTokenCount, Is.GreaterThan(0));
        Assert.That(response.Metadata, Is.Not.Null.Or.Empty);
        Assert.That(response.Metadata["superfluous_key"], Is.EqualTo("superfluous_value"));
        Assert.That(response.OutputItems, Has.Count.EqualTo(2));
        ReasoningResponseItem reasoningItem = response.OutputItems[0] as ReasoningResponseItem;
        MessageResponseItem messageItem = response.OutputItems[1] as MessageResponseItem;
        Assert.That(reasoningItem.SummaryTextParts, Is.Not.Null);
        Assert.That(reasoningItem.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(messageItem.Content?.FirstOrDefault().Text, Has.Length.GreaterThan(0));
    }

    [Test]
    [TestCase("computer-use-preview-2025-02-04")]
    [TestCase("gpt-4o-mini")]
    public async Task HelloWorldStreaming(string model)
    {
        OpenAIResponseClient client = GetTestClient(model);

        ResponseContentPart contentPart = ResponseContentPart.CreateInputTextPart("Hello, responses!");
        ResponseItem inputItem = ResponseItem.CreateUserMessageItem([contentPart]);

        await foreach (StreamingResponseUpdate update in client.CreateResponseStreamingAsync([inputItem]))
        {
            Console.WriteLine(ModelReaderWriter.Write(update));
            //if (update is ResponsesItemStreamingPartDeltaUpdate partDeltaUpdate)
            //{
            //    Console.Write(partDeltaUpdate.Text);
            //}
        }
    }

    [Test]
    public async Task CanDeleteResponse()
    {
        OpenAIResponseClient client = GetTestClient();

        OpenAIResponse response = await client.CreateResponseAsync([ResponseItem.CreateUserMessageItem("Hello, model!")]);

        async Task RetrieveThatResponseAsync()
        {
            OpenAIResponse retrievedResponse = await client.GetResponseAsync(response.Id);
            Assert.That(retrievedResponse.Id, Is.EqualTo(response.Id));
        }

        Assert.DoesNotThrowAsync(RetrieveThatResponseAsync);

        ResponseDeletionResult deletionResult = await client.DeleteResponseAsync(response.Id);
        Assert.That(deletionResult.Deleted, Is.True);

        Assert.ThrowsAsync<ClientResultException>(RetrieveThatResponseAsync);
    }

    [Test]
    public async Task CanOptOutOfStorage()
    {
        OpenAIResponseClient client = GetTestClient();

        OpenAIResponse response = await client.CreateResponseAsync(
            [ResponseItem.CreateUserMessageItem("Hello, model!")],
            new ResponseCreationOptions()
            {
                StoredOutputEnabled = false,
            });

        ClientResultException expectedException = Assert.ThrowsAsync<ClientResultException>(async () => await client.GetResponseAsync(response.Id));
        Assert.That(expectedException.Message, Does.Contain("not found"));
    }

    [Test]
    [Ignore("Assistant message needs an ID.")]
    public async Task MessageHistoryWorks()
    {
        OpenAIResponseClient client = GetTestClient();

        OpenAIResponse response = await client.CreateResponseAsync(
            [
                ResponseItem.CreateDeveloperMessageItem("You are a helpful assistant."),
                ResponseItem.CreateUserMessageItem("Hello, Assistant, my name is Bob!"),
                ResponseItem.CreateAssistantMessageItem("1", "Hello, Bob. It's a nice, sunny day!"),
                ResponseItem.CreateUserMessageItem("What's my name and what did you tell me the weather was like?"),
            ]);

        Assert.That(response, Is.Not.Null);
    }

    [Test]
    public async Task ImageInputWorks()
    {
        OpenAIResponseClient client = GetTestClient();

        string imagePath = Path.Join("Assets", "images_dog_and_cat.png");
        BinaryData imageBytes = BinaryData.FromBytes(await File.ReadAllBytesAsync(imagePath));

        OpenAIResponse response = await client.CreateResponseAsync(
            [
                ResponseItem.CreateUserMessageItem(
                    [
                        ResponseContentPart.CreateInputTextPart("Please describe this picture for me"),
                        ResponseContentPart.CreateInputImagePart(imageBytes, "image/png", ResponseImageDetailLevel.Low),
                    ]),
            ]);
    }

    public enum ResponsesTestInstructionMethod
    {
        InstructionsProperty,
        SystemMessage,
        DeveloperMessage
    }

    [Test]
    [TestCase(ResponsesTestInstructionMethod.InstructionsProperty)]
    [TestCase(ResponsesTestInstructionMethod.SystemMessage)]
    [TestCase(ResponsesTestInstructionMethod.DeveloperMessage)]
    public async Task AllInstructionMethodsWork(ResponsesTestInstructionMethod instructionMethod)
    {
        const string instructions = "Always begin your replies with 'Arr, matey'";

        List<MessageResponseItem> messages = new();

        if (instructionMethod == ResponsesTestInstructionMethod.SystemMessage)
        {
            messages.Add(ResponseItem.CreateSystemMessageItem(instructions));
        }
        else if (instructionMethod == ResponsesTestInstructionMethod.DeveloperMessage)
        {
            messages.Add(ResponseItem.CreateDeveloperMessageItem(instructions));
        }

        const string userMessage = "Hello, model!";
        messages.Add(ResponseItem.CreateUserMessageItem(userMessage));

        ResponseCreationOptions options = new();

        if (instructionMethod == ResponsesTestInstructionMethod.InstructionsProperty)
        {
            options.Instructions = instructions;
        }

        OpenAIResponseClient client = GetTestClient();
        OpenAIResponse response = await client.CreateResponseAsync(messages, options);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.OutputItems, Is.Not.Null.And.Not.Empty);
        Assert.That(response.OutputItems[0], Is.InstanceOf<MessageResponseItem>());
        Assert.That((response.OutputItems[0] as MessageResponseItem).Content, Is.Not.Null.And.Not.Empty);
        Assert.That((response.OutputItems[0] as MessageResponseItem).Content[0].Text, Does.StartWith("Arr, matey"));

        OpenAIResponse retrievedResponse = await client.GetResponseAsync(response.Id);
        Assert.That((retrievedResponse?.OutputItems?.FirstOrDefault() as MessageResponseItem)?.Content?.FirstOrDefault()?.Text, Does.StartWith("Arr, matey"));

        if (instructionMethod == ResponsesTestInstructionMethod.InstructionsProperty)
        {
            Assert.That(retrievedResponse.Instructions, Is.EqualTo(instructions));
        }

        List<ResponseItem> listedItems = [];
        await client.GetResponseInputItemsAsync(response.Id).ForEachAsync(item => listedItems.Add(item));

        if (instructionMethod == ResponsesTestInstructionMethod.InstructionsProperty)
        {
            Assert.That(listedItems, Has.Count.EqualTo(1));
            Assert.That((listedItems[0] as MessageResponseItem)?.Content?.FirstOrDefault()?.Text, Is.EqualTo(userMessage));
        }
        else
        {
            Assert.That(listedItems, Has.Count.EqualTo(2));
            MessageResponseItem systemOrDeveloperMessage = listedItems[1] as MessageResponseItem;
            Assert.That(systemOrDeveloperMessage, Is.Not.Null);
            Assert.That(systemOrDeveloperMessage.Role, Is.EqualTo(instructionMethod switch
            {
                ResponsesTestInstructionMethod.DeveloperMessage => MessageRole.Developer,
                ResponsesTestInstructionMethod.SystemMessage => MessageRole.System,
                _ => throw new ArgumentException()
            }));
            Assert.That(systemOrDeveloperMessage.Content?.FirstOrDefault()?.Text, Is.EqualTo(instructions));
            Assert.That((listedItems[0] as MessageResponseItem)?.Content?.FirstOrDefault()?.Text, Is.EqualTo(userMessage));
        }
    }

    [Test]
    public async Task TwoTurnCrossModel()
    {
        OpenAIResponseClient client = GetTestClient("gpt-4o-mini");
        OpenAIResponseClient client2 = GetTestClient("o3-mini");


        OpenAIResponse response = await client.CreateResponseAsync(
            [ResponseItem.CreateUserMessageItem("Hello, Assistant! My name is Travis.")]);
        OpenAIResponse response2 = await client2.CreateResponseAsync(
            [ResponseItem.CreateUserMessageItem("What's my name?")],
            new ResponseCreationOptions()
            {
                PreviousResponseId = response.Id,
            });
    }

    [Test]
    public async Task FunctionCall()
    {
        OpenAIResponseClient client = GetTestClient();

        ResponseCreationOptions options = new()
        {
            Tools = { s_GetWeatherAtLocationTool }
        };

        OpenAIResponse response = await client.CreateResponseAsync(
            [ResponseItem.CreateUserMessageItem("What should I wear for the weather in San Francisco, CA?")],
            options);

        Assert.That(response.OutputItems, Has.Count.EqualTo(1));
        FunctionCallResponseItem functionCall = response.OutputItems[0] as FunctionCallResponseItem;
        Assert.That(functionCall, Is.Not.Null);
        Assert.That(functionCall!.Id, Has.Length.GreaterThan(0));
        Assert.That(functionCall.FunctionName, Is.EqualTo("get_weather_at_location"));
        Assert.That(functionCall.FunctionArguments, Is.Not.Null);

        ResponseCreationOptions turn2Options = new()
        {
            PreviousResponseId = response.Id,
            Tools = { s_GetWeatherAtLocationTool },
        };

        ResponseItem functionReply = ResponseItem.CreateFunctionCallOutputItem(functionCall.CallId, "22 celcius and windy");
        OpenAIResponse turn2Response = await client.CreateResponseAsync(
            [functionReply],
            turn2Options);
        Assert.That(turn2Response.OutputItems?.Count, Is.EqualTo(1));
        MessageResponseItem turn2Message = turn2Response!.OutputItems[0] as MessageResponseItem;
        Assert.That(turn2Message, Is.Not.Null);
        Assert.That(turn2Message!.Role, Is.EqualTo(MessageRole.Assistant));
        Assert.That(turn2Message.Content, Has.Count.EqualTo(1));
        Assert.That(turn2Message.Content[0].Text, Does.Contain("22"));
    }

    [Test]
    public async Task FunctionCallStreaming()
    {
        OpenAIResponseClient client = GetTestClient();

        await foreach (StreamingResponseUpdate update
            in client.CreateResponseStreamingAsync(
                "What should I wear for the weather in San Francisco right now?",
                new ResponseCreationOptions() { Tools = { s_GetWeatherAtLocationTool } }))
        {
            if (update is StreamingResponseStatusUpdate statusUpdate)
            {
                Console.WriteLine($"{statusUpdate.Kind}: {statusUpdate.Response.Id}");
            }
            else if (update is StreamingResponseContentPartDeltaUpdate deltaUpdate)
            {
                Console.Write(deltaUpdate.FunctionArguments);
            }
        }
    }

    private static string s_GetWeatherAtLocationToolName = "get_weather_at_location";
    private static ResponseTool s_GetWeatherAtLocationTool = ResponseTool.CreateFunctionTool(
            s_GetWeatherAtLocationToolName,
            "Gets the weather at a specified location, optionally specifying units for temperature",
            BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                    "location": {
                        "type": "string"
                    },
                    "unit": {
                        "type": "string",
                        "enum": ["C", "F", "K"]
                    }
                    },
                    "required": ["location"]
                }
                """),
            false);

    private static OpenAIResponseClient GetTestClient(string overrideModel = null)
        => GetTestClient<OpenAIResponseClient>(TestScenario.Responses, overrideModel);
}