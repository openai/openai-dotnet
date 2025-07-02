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
using System.Text.Json;
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
    public async Task ComputerToolWithScreenshotRoundTrip()
    {
        OpenAIResponseClient client = GetTestClient("computer-use-preview-2025-03-11");
        ResponseTool computerTool = ResponseTool.CreateComputerTool(ComputerToolEnvironment.Windows, 1024, 768);
        ResponseCreationOptions responseOptions = new()
        {
            Tools = { computerTool },
            TruncationMode = ResponseTruncationMode.Auto,
        };
        OpenAIResponse response = await client.CreateResponseAsync(
            inputItems:
            [
                ResponseItem.CreateDeveloperMessageItem("Call tools when the user asks to perform computer-related tasks like clicking interface elements."),
                ResponseItem.CreateUserMessageItem("Click on the Save button.")
            ],
            responseOptions);

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

                    responseOptions.PreviousResponseId = response.Id;
                    response = await client.CreateResponseAsync([screenshotReply], responseOptions);
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
                response = await client.CreateResponseAsync(
                    "Yes, proceed.",
                    responseOptions);
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
                    WebSearchUserLocation.CreateApproximateLocation(city: "San Francisco"),
                    WebSearchContextSize.Low)
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

    [Test]
    public async Task StreamingResponses()
    {
        OpenAIResponseClient client = GetTestClient("gpt-4o-mini"); // "computer-use-alpha");

        List<ResponseItem> inputItems = [ResponseItem.CreateUserMessageItem("Hello, world!")];
        List<string> deltaTextSegments = [];
        string finalResponseText = null;
        await foreach (StreamingResponseUpdate update in client.CreateResponseStreamingAsync(inputItems))
        {
            Console.WriteLine(ModelReaderWriter.Write(update));
            if (update is StreamingResponseOutputTextDeltaUpdate outputTextDeltaUpdate)
            {
                deltaTextSegments.Add(outputTextDeltaUpdate.Delta);
                Console.Write(outputTextDeltaUpdate.Delta);
            }
            else if (update is StreamingResponseCompletedUpdate responseCompletedUpdate)
            {
                finalResponseText = responseCompletedUpdate.Response.GetOutputText();
            }
        }
        Assert.That(deltaTextSegments, Has.Count.GreaterThan(0));
        Assert.That(finalResponseText, Is.Not.Null.And.Not.Empty);
        Assert.That(string.Join(string.Empty, deltaTextSegments), Is.EqualTo(finalResponseText));
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
            TruncationMode = ResponseTruncationMode.Auto,
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

    [Ignore("Temporarily disabled awaiting org verification.")]
    [Test]
    public async Task ResponsesWithReasoning()
    {
        OpenAIResponseClient client = GetTestClient("o3-mini");

        ResponseCreationOptions options = new()
        {
            ReasoningOptions = new()
            {
                ReasoningSummaryVerbosity = ResponseReasoningSummaryVerbosity.Detailed,
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
        Assert.That(response.TruncationMode, Is.EqualTo(ResponseTruncationMode.Disabled));
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
        Assert.That(reasoningItem.SummaryParts, Has.Count.GreaterThan(0));
        Assert.That(reasoningItem.GetSummaryText(), Is.Not.Null.And.Not.Empty);
        Assert.That(reasoningItem.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(messageItem.Content?.FirstOrDefault().Text, Has.Length.GreaterThan(0));
    }

    [Test]
    [TestCase("computer-use-preview-2025-03-11")]
    [TestCase("gpt-4o-mini")]
    public async Task HelloWorldStreaming(string model)
    {
        OpenAIResponseClient client = GetTestClient(model);

        ResponseContentPart contentPart
            = ResponseContentPart.CreateInputTextPart("Hello, responses!");
        ResponseItem inputItem = ResponseItem.CreateUserMessageItem([contentPart]);

        await foreach (StreamingResponseUpdate update
            in client.CreateResponseStreamingAsync(
                [inputItem],
                new ResponseCreationOptions()
                {
                    TruncationMode = ResponseTruncationMode.Auto,
                }))
        {
            Console.WriteLine(ModelReaderWriter.Write(update));
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
    public async Task OutputTextMethod()
    {
        OpenAIResponseClient client = GetTestClient();
        OpenAIResponse response = await client.CreateResponseAsync(
            "Respond with only the word hello.");
        Assert.That(response?.GetOutputText()?.Length, Is.GreaterThan(0).And.LessThan(7));
        Assert.That(response?.GetOutputText()?.ToLower(), Does.Contain("hello"));

        response.OutputItems.Add(ResponseItem.CreateAssistantMessageItem("More text!"));
        Assert.That(response?.GetOutputText()?.ToLower(), Does.EndWith("more text!"));

        response = await client.CreateResponseAsync(
            "How's the weather?",
            new ResponseCreationOptions()
            {
                Tools = { ResponseTool.CreateFunctionTool("get_weather", "gets the weather", BinaryData.FromString("{}")) },
                ToolChoice = ResponseToolChoice.CreateRequiredChoice(),
            });
        Assert.That(response.GetOutputText(), Is.Null);
    }

    [Test]
    public async Task MessageHistoryWorks()
    {
        OpenAIResponseClient client = GetTestClient();

        OpenAIResponse response = await client.CreateResponseAsync(
            [
                ResponseItem.CreateDeveloperMessageItem("You are a helpful assistant."),
                ResponseItem.CreateUserMessageItem("Hello, Assistant, my name is Bob!"),
                ResponseItem.CreateAssistantMessageItem("Hello, Bob. It's a nice, sunny day!"),
                ResponseItem.CreateUserMessageItem("What's my name and what did you tell me the weather was like?"),
            ]);

        Assert.That(response, Is.Not.Null);
    }

    [Ignore("Temporarily disabled due to service instability.")]
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

        Console.WriteLine(response.GetOutputText());
        Assert.That(response.GetOutputText().ToLowerInvariant(), Does.Contain("dog").Or.Contain("cat").IgnoreCase);
    }

    [Test]
    public async Task FileInputFromIdWorks()
    {
        OpenAIResponseClient client = GetTestClient();
        OpenAIFileClient fileClient = GetTestClient<OpenAIFileClient>(TestScenario.Files);
        string filePath = Path.Join("Assets", "files_travis_favorite_food.pdf");

        OpenAIFile newFileToUse = await fileClient.UploadFileAsync(
            BinaryData.FromBytes(File.ReadAllBytes(filePath)),
            "test_favorite_foods.pdf",
            FileUploadPurpose.UserData);
                Validate(newFileToUse);

        ResponseItem messageItem = ResponseItem.CreateUserMessageItem(
            [
                ResponseContentPart.CreateInputTextPart("Based on this file, what food should I order for whom?"),
                ResponseContentPart.CreateInputFilePart(newFileToUse.Id),
            ]);

        OpenAIResponse response = await client.CreateResponseAsync([messageItem]);

        Assert.That(response?.GetOutputText()?.ToLower(), Does.Contain("pizza"));
    }

    [Test]
    public async Task FileInputFromBinaryWorks()
    {
        OpenAIResponseClient client = GetTestClient();

        string filePath = Path.Join("Assets", "files_travis_favorite_food.pdf");
        Stream fileStream = File.OpenRead(filePath);
        BinaryData fileBytes = await BinaryData.FromStreamAsync(fileStream);

        ResponseItem messageItem = ResponseItem.CreateUserMessageItem(
            [
                ResponseContentPart.CreateInputTextPart("Based on this file, what food should I order for whom?"),
                ResponseContentPart.CreateInputFilePart(fileBytes, "application/pdf", "test_favorite_foods.pdf"),
            ]);

        OpenAIResponse response = await client.CreateResponseAsync([messageItem]);

        Assert.That(response?.GetOutputText()?.ToLower(), Does.Contain("pizza"));
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
    [TestCase("gpt-4o-mini")]
    [TestCase("computer-use-preview", Ignore = "Not yet supported with computer-use-preview")]
    public async Task StructuredOutputs(string modelName)
    {
        OpenAIResponseClient client = GetTestClient(modelName);

        OpenAIResponse response = await client.CreateResponseAsync(
            "Write a JSON document with a list of five animals",
            new ResponseCreationOptions()
            {
                TextOptions = new ResponseTextOptions()
                {
                    TextFormat = ResponseTextFormat.CreateJsonSchemaFormat(
                        "data_list",
                        BinaryData.FromString("""
                            {
                              "type": "object",
                              "properties": {
                                "animal_data_list": {
                                  "type": "array",
                                  "items": {
                                    "type": "string"
                                  }
                                }
                              },
                              "required": ["animal_data_list"],
                              "additionalProperties": false
                            }
                            """)),
                }
            });

        Assert.That(
            response?.TextOptions?.TextFormat?.Kind,
            Is.EqualTo(ResponseTextFormatKind.JsonSchema));
        Assert.That(response?.OutputItems, Has.Count.EqualTo(1));
        MessageResponseItem message = response.OutputItems[0] as MessageResponseItem;
        Assert.That(message?.Content, Has.Count.EqualTo(1));
        Assert.That(message.Content[0].Text, Is.Not.Null.And.Not.Empty);

        Assert.DoesNotThrow(() =>
        {
            using JsonDocument document = JsonDocument.Parse(message.Content[0].Text);
            bool hasListElement = document.RootElement.TryGetProperty("animal_data_list", out JsonElement listElement);
            Assert.That(hasListElement, Is.True);
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

        Assert.DoesNotThrow(() =>
        {
            using JsonDocument document = JsonDocument.Parse(functionCall.FunctionArguments);
            _ = document.RootElement.GetProperty("location");
        });

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

        await foreach (ResponseItem item in client.GetResponseInputItemsAsync(turn2Response.Id))
        { }
    }

    [Test]
    public async Task MaxTokens()
    {
        OpenAIResponseClient client = GetTestClient();

        OpenAIResponse response = await client.CreateResponseAsync(
            "Write three haikus about tropical fruit",
            new ResponseCreationOptions()
            {
                MaxOutputTokenCount = 20,
            });

        Assert.That(
            response?.IncompleteStatusDetails?.Reason,
            Is.EqualTo(ResponseIncompleteStatusReason.MaxOutputTokens));
        MessageResponseItem message = response?.OutputItems?.FirstOrDefault() as MessageResponseItem; ;
        Assert.That(message?.Content?.FirstOrDefault(), Is.Not.Null);
        Assert.That(message?.Status, Is.EqualTo(MessageStatus.Incomplete));
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
            if (update is StreamingResponseCreatedUpdate responseCreatedUpdate)
            {
            }
            else if (update is StreamingResponseFunctionCallArgumentsDeltaUpdate functionCallArgumentsDeltaUpdate)
            {
            }
        }
    }

    [Test]
    public async Task FunctionToolChoiceWorks()
    {
        OpenAIResponseClient client = GetTestClient();

        ResponseToolChoice toolChoice
            = ResponseToolChoice.CreateFunctionChoice(s_GetWeatherAtLocationToolName);

        ResponseCreationOptions options = new()
        {
            Tools = { s_GetWeatherAtLocationTool },
            ToolChoice = toolChoice,
        };

        OpenAIResponse response = await client.CreateResponseAsync(
            [ResponseItem.CreateUserMessageItem("What should I wear for the weather in San Francisco, CA?")],
            options);

        Assert.That(response.ToolChoice, Is.Not.Null);
        Assert.That(response.ToolChoice.Kind, Is.EqualTo(ResponseToolChoiceKind.Function));
        Assert.That(response.ToolChoice.FunctionName, Is.EqualTo(toolChoice.FunctionName));

        FunctionCallResponseItem functionCall = response.OutputItems.FirstOrDefault() as FunctionCallResponseItem;
        Assert.That(functionCall, Is.Not.Null);
        Assert.That(functionCall.FunctionName, Is.EqualTo(toolChoice.FunctionName));
    }

    [Test]
    public async Task CanUseStreamingBackgroundResponses()
    {
        OpenAIResponseClient client = GetTestClient("gpt-4.1-mini");

        string queuedResponseId = null;

        await foreach (StreamingResponseUpdate update
            in client.CreateResponseStreamingAsync(
                "Hello, model!",
                new ResponseCreationOptions()
                {
                    Background = true,
                }))
        {
            if (update is StreamingResponseQueuedUpdate queuedUpdate)
            {
                queuedResponseId = queuedUpdate.Response.Id;
                break;
            }
        }

        Assert.That(queuedResponseId, Is.Not.Null.And.Not.Empty);

        OpenAIResponse retrievedResponse = await client.GetResponseAsync(queuedResponseId);
        Assert.That(retrievedResponse?.Id, Is.EqualTo(queuedResponseId));

        OpenAIResponse finalStreamedResponse = null;

        await foreach (StreamingResponseUpdate update
            in client.GetResponseStreamingAsync(queuedResponseId, startingAfter: 2))
        {
            Assert.That(update.SequenceNumber, Is.GreaterThan(2));

            if (update is StreamingResponseCompletedUpdate completedUpdate)
            {
                finalStreamedResponse = completedUpdate.Response;
            }
        }

        Assert.That(finalStreamedResponse?.Id, Is.EqualTo(queuedResponseId));
        Assert.That(finalStreamedResponse?.OutputItems?.FirstOrDefault(), Is.Not.Null);
    }

    [Test]
    public async Task CanCancelBackgroundResponses()
    {
        OpenAIResponseClient client = GetTestClient("gpt-4.1-mini");

        OpenAIResponse response = await client.CreateResponseAsync(
            "Hello, model!",
            new ResponseCreationOptions()
            {
                Background = true,
            });
        Assert.That(response?.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(response?.Status, Is.EqualTo(ResponseStatus.Queued));

        OpenAIResponse cancelledResponse = await client.CancelResponseAsync(response.Id);
        Assert.That(cancelledResponse.Id, Is.EqualTo(response.Id));
        Assert.That(cancelledResponse.Status, Is.EqualTo(ResponseStatus.Cancelled));
    }

    private static readonly string s_GetWeatherAtLocationToolName = "get_weather_at_location";
    private static readonly ResponseTool s_GetWeatherAtLocationTool = ResponseTool.CreateFunctionTool(
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

    private static OpenAIResponseClient GetTestClient(string overrideModel = null) => GetTestClient<OpenAIResponseClient>(TestScenario.Responses, overrideModel);
}