using Microsoft.ClientModel.TestFramework;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using OpenAI.Conversations;
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
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses;

#pragma warning disable OPENAICUA001

[Category("Responses")]
public partial class ResponsesTests : OpenAIRecordedTestBase
{
    public enum ResponsesTestInstructionMethod { InstructionsProperty, SystemMessage, DeveloperMessage }

    public ResponsesTests(bool isAsync) : base(isAsync)
    {
        TestTimeoutInSeconds = 30;
    }

    [OneTimeTearDown]
    protected void Cleanup()
    {
        Console.WriteLine("[Teardown]");

        // Skip cleanup if there is no API key (e.g., if we are not running live tests).
        if (Mode == RecordedTestMode.Playback || string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENAI_API_KEY")))
        {
            Console.WriteLine("[WARNING] Can't clean up");
            return;
        }

        RequestOptions noThrowOptions = new() { ErrorOptions = ClientErrorBehaviors.NoThrow };
        OpenAIFileClient fileClient = TestEnvironment.GetTestClient<OpenAIFileClient>();
        VectorStoreClient vectorStoreClient = TestEnvironment.GetTestClient<VectorStoreClient>();

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

    [RecordedTest]
    public async Task StreamingResponses()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>(); // "computer-use-alpha");

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
                finalResponseText = responseCompletedUpdate.Response.OutputItems[0] is MessageResponseItem messageItem
                    ? messageItem.Content[0].Text
                    : null;
            }
        }
        Assert.That(deltaTextSegments, Has.Count.GreaterThan(0));
        Assert.That(finalResponseText, Is.Not.Null.And.Not.Empty);
        Assert.That(string.Concat(deltaTextSegments), Is.EqualTo(finalResponseText));
    }

    [RecordedTest]
    public async Task StreamingResponsesWithReasoningSummary()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("o3-mini");
        List<ResponseItem> inputItems = [ResponseItem.CreateUserMessageItem("I’m visiting New York for 3 days and love food and art. What’s the best way to plan my trip?")];

        CreateResponseOptions options = new(inputItems)
        {
            ReasoningOptions = new()
            {
                ReasoningSummaryVerbosity = ResponseReasoningSummaryVerbosity.Auto,
                ReasoningEffortLevel = ResponseReasoningEffortLevel.High,
            },
            Instructions = "Perform reasoning over any questions asked by the user.",
            StreamingEnabled = true,
        };

        var partsAdded = 0;
        var partsDone = 0;
        var inPart = false;

        var receivedTextDelta = false;
        var receivedTextDone = false;

        List<string> reasoningTexts = [];
        string finalOutput = null;

        await foreach (StreamingResponseUpdate update in client.CreateResponseStreamingAsync(options))
        {
            if (update is StreamingResponseReasoningSummaryPartAddedUpdate partAdded)
            {
                partsAdded++;
                inPart = true;
            }
            else if (update is StreamingResponseReasoningSummaryPartDoneUpdate partDone)
            {
                partsDone++;
                inPart = false;
            }
            else if (update is StreamingResponseReasoningSummaryTextDeltaUpdate textDelta)
            {
                receivedTextDelta = true;
                reasoningTexts.Add(textDelta.Delta);
            }
            else if (update is StreamingResponseReasoningSummaryTextDoneUpdate textDone)
            {
                receivedTextDone = true;
                finalOutput = textDone.Text;
            }
        }

        Assert.That(partsAdded, Is.GreaterThanOrEqualTo(1), "No reasoning summary parts were added.");
        Assert.That(partsDone, Is.EqualTo(partsAdded), "Parts added/done mismatch.");
        Assert.That(receivedTextDelta, Is.True, "No reasoning summary text delta event received.");
        Assert.That(receivedTextDone, Is.True, "No reasoning summary text done event received.");
        Assert.That(reasoningTexts.Count, Is.GreaterThan(0), "No reasoning summary text accumulated.");
        Assert.That(string.IsNullOrWhiteSpace(finalOutput), Is.False, "Final output text is empty.");
        Assert.That(inPart, Is.False, "Ended while still inside a reasoning summary part.");
    }

    [RecordedTest]
    [TestCase("gpt-4o-mini")]
    [TestCase("computer-use-preview")]
    public async Task ResponsesHelloWorldWithTool(string model)
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>(model);

        CreateResponseOptions options = new(
            [
                ResponseItem.CreateUserMessageItem(
                [
                    ResponseContentPart.CreateInputTextPart("good morning, responses!"),
                ]),
            ])
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
                    strictModeEnabled: false),
            },
            TruncationMode = ResponseTruncationMode.Auto,
        };

        ResponseResult response = await client.CreateResponseAsync(
            options);

        Assert.That(response.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(response.CreatedAt, Is.GreaterThan(Recording.Now - TimeSpan.FromDays(1)));
        // Assert.That(response.Status, Is.EqualTo(ResponsesStatus.Completed));
        Assert.That(response.Model, Is.Not.Null.And.Not.Empty);
        Assert.That(response.PreviousResponseId, Is.Null);
        // Observed: input may not exist on normal responses
        // Assert.That(response.Input.Count, Is.EqualTo(1));
        Assert.That(response.OutputItems.Count, Is.EqualTo(1));
    }

    [RecordedTest]
    public async Task ResponsesWithReasoning()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("gpt-5");

        CreateResponseOptions options = new([ResponseItem.CreateUserMessageItem("What's the best way to fold a burrito?")])
        {
            ReasoningOptions = new()
            {
                ReasoningSummaryVerbosity = ResponseReasoningSummaryVerbosity.Detailed,
                ReasoningEffortLevel = ResponseReasoningEffortLevel.Low,
            },
            Instructions = "Perform reasoning over any questions asked by the user.",
        };

        ResponseResult response = await client.CreateResponseAsync(options);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null);
        Assert.That(response.OutputItems, Has.Count.EqualTo(2));

        ReasoningResponseItem reasoningItem = response.OutputItems[0] as ReasoningResponseItem;
        MessageResponseItem messageItem = response.OutputItems[1] as MessageResponseItem;

        Assert.That(reasoningItem.SummaryParts, Has.Count.GreaterThan(0));
        Assert.That(reasoningItem.GetSummaryText(), Is.Not.Null.And.Not.Empty);
        Assert.That(reasoningItem.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(messageItem.Content?.FirstOrDefault().Text, Has.Length.GreaterThan(0));
    }

    [RecordedTest]
    public async Task ReasoningWithStoreDisabled()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("gpt-5-mini");

        List<ResponseItem> inputItems = [ResponseItem.CreateUserMessageItem("Hello, world!")];
        CreateResponseOptions options = new(inputItems)
        {
            StoredOutputEnabled = false,
            IncludedProperties = { IncludedResponseProperty.ReasoningEncryptedContent }
        };

        // First turn.
        ResponseResult response1 = await client.CreateResponseAsync(options);
        Assert.That(response1, Is.Not.Null);
        Assert.That(response1.OutputItems.OfType<ReasoningResponseItem>().Any(), Is.True);

        // Propagate the output items into the next turn.
        inputItems.AddRange(response1.OutputItems);

        // Add the next user input.
        inputItems.Add(ResponseItem.CreateUserMessageItem("Say that again, but dramatically"));

        // Second turn.
        ResponseResult response2 = await client.CreateResponseAsync(options);
        Assert.That(response2, Is.Not.Null);
        Assert.That(response2.GetOutputText(), Is.Not.Null.Or.Empty);
    }

    [RecordedTest]
    [TestCase("computer-use-preview-2025-03-11")]
    [TestCase("gpt-4o-mini")]
    public async Task HelloWorldStreaming(string model)
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>(model);

        ResponseContentPart contentPart
            = ResponseContentPart.CreateInputTextPart("Hello, responses!");
        ResponseItem inputItem = ResponseItem.CreateUserMessageItem([contentPart]);

        CreateResponseOptions options = new([inputItem])
        {
            TruncationMode = ResponseTruncationMode.Auto,
            StreamingEnabled = true,
        };

        await foreach (StreamingResponseUpdate update in client.CreateResponseStreamingAsync(options))
        {
            Console.WriteLine(ModelReaderWriter.Write(update));
        }
    }

    [RecordedTest]
    public async Task CanDeleteResponse()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        ResponseResult response = await client.CreateResponseAsync("Hello, model!");

        async Task RetrieveThatResponseAsync()
        {
            ResponseResult retrievedResponse = await client.GetResponseAsync(new GetResponseOptions(response.Id));
            Assert.That(retrievedResponse.Id, Is.EqualTo(response.Id));
        }

        Assert.DoesNotThrowAsync(RetrieveThatResponseAsync);

        ResponseDeletionResult deletionResult = await client.DeleteResponseAsync(response.Id);
        Assert.That(deletionResult.Deleted, Is.True);

        Assert.ThrowsAsync<ClientResultException>(RetrieveThatResponseAsync);
    }

    [RecordedTest]
    public async Task CanOptOutOfStorage()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        CreateResponseOptions options = new([ResponseItem.CreateUserMessageItem("Hello, model!")])
        {
            StoredOutputEnabled = false,
        };

        ResponseResult response = await client.CreateResponseAsync(options);

        ClientResultException expectedException = Assert.ThrowsAsync<ClientResultException>(async () => await client.GetResponseAsync(new GetResponseOptions(response.Id)));
        Assert.That(expectedException.Message, Does.Contain("not found"));
    }

    [RecordedTest]
    public async Task ResponseUsingConversations()
    {
        ConversationClient conversationClient = GetProxiedOpenAIClient<ConversationClient>();

        BinaryData createConversationParameters = BinaryData.FromBytes("""
            {
               "metadata": { "topic": "test" },
               "items": [
                   {
                       "type": "message",
                       "role": "user",
                       "content": "tell me a joke"
                   }
               ]
            }
            """u8.ToArray());

        using BinaryContent requestContent = BinaryContent.Create(createConversationParameters);
        var conversationResult = await conversationClient.CreateConversationAsync(requestContent);
        using JsonDocument conversationResultAsJson = JsonDocument.Parse(conversationResult.GetRawResponse().Content.ToString());
        string conversationId = conversationResultAsJson.RootElement.GetProperty("id"u8).GetString();

        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("gpt-4.1");
        ResponseResult response = await client.CreateResponseAsync(
            new CreateResponseOptions([ResponseItem.CreateUserMessageItem("tell me another")])
            {
                ConversationOptions = new(conversationId),
            });

        Assert.That(response, Is.Not.Null);
        Assert.That(response.ConversationOptions.ConversationId, Is.EqualTo(conversationId));

        var conversationResults = conversationClient.GetConversationItemsAsync(conversationId);
        var conversationItems = new List<JsonElement>();
        await foreach (ClientResult result in conversationResults.GetRawPagesAsync())
        {
            using JsonDocument getConversationItemsResultAsJson = JsonDocument.Parse(result.GetRawResponse().Content.ToString());
            foreach (JsonElement element in getConversationItemsResultAsJson.RootElement.GetProperty("data").EnumerateArray())
            {
                conversationItems.Add(element.Clone());
            }
        }

        Assert.That(conversationItems, Is.Not.Empty, "Expected the conversation to contain items.");
        Assert.That(conversationItems.Count, Is.GreaterThanOrEqualTo(2));

        var lastItem = conversationItems[conversationItems.Count - 1];
        var secondLastItem = conversationItems[conversationItems.Count - 2];

        string GetContentText(JsonElement item) =>
            item.GetProperty("content")[0].GetProperty("text").GetString();

        Assert.That(GetContentText(lastItem), Is.EqualTo("tell me a joke"));
        Assert.That(GetContentText(secondLastItem), Is.EqualTo("tell me another"));
    }

    [RecordedTest]
    public async Task ResponseServiceTierWorks()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        MessageResponseItem message = ResponseItem.CreateUserMessageItem("Using a comprehensive evaluation of popular media in the 1970s and 1980s, what were the most common sci-fi themes?");
        CreateResponseOptions options = new([message])
        {
            ServiceTier = ResponseServiceTier.Default,
        };
        ResponseResult response = await client.CreateResponseAsync(options);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.ServiceTier, Is.EqualTo(ResponseServiceTier.Default));
    }

    [RecordedTest]
    public async Task OutputTextMethod()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();
        ResponseResult response = await client.CreateResponseAsync("Respond with only the word hello.");
        var outputText = response.GetOutputText();
        Assert.That(outputText.Length, Is.GreaterThan(0).And.LessThan(7));
        Assert.That(outputText.ToLower(), Does.Contain("hello"));

        response.OutputItems.Add(ResponseItem.CreateAssistantMessageItem("More text!"));
        Assert.That(response.GetOutputText().ToLower(), Does.EndWith("more text!"));

        response = await client.CreateResponseAsync(
            new CreateResponseOptions([ResponseItem.CreateUserMessageItem("How's the weather?")])
            {
                Tools =
                {
                    ResponseTool.CreateFunctionTool(
                        functionName: "get_weather",
                        functionDescription: "gets the weather",
                        functionParameters: BinaryData.FromString("{}"),
                        strictModeEnabled: false)
                },
                ToolChoice = ResponseToolChoice.CreateRequiredChoice(),
            });
        Assert.That(response.GetOutputText(), Is.Null);
    }

    [RecordedTest]
    public async Task MessageHistoryWorks()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        ResponseResult response = await client.CreateResponseAsync(
            [
                ResponseItem.CreateDeveloperMessageItem("You are a helpful assistant."),
                ResponseItem.CreateUserMessageItem("Hello, Assistant, my name is Bob!"),
                ResponseItem.CreateAssistantMessageItem("Hello, Bob. It's a nice, sunny day!"),
                ResponseItem.CreateUserMessageItem("What's my name and what did you tell me the weather was like?"),
            ]);

        Assert.That(response, Is.Not.Null);
    }

#if NET10_0_OR_GREATER
    [RecordedTest]
    public async Task ImageInputWorks()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        string imagePath = Path.Join("Assets", "images_dog_and_cat.png");
        string imageMediaType = "image/png";
        BinaryData imageBytes = BinaryData.FromBytes(await File.ReadAllBytesAsync(imagePath));
        Uri imageDataUri = new($"data:{imageMediaType};base64,{Convert.ToBase64String(imageBytes.ToArray())}");

        ResponseResult response = await client.CreateResponseAsync(
            [
                ResponseItem.CreateUserMessageItem(
                    [
                        ResponseContentPart.CreateInputTextPart("Please describe this picture for me"),
                        ResponseContentPart.CreateInputImagePart(imageDataUri, ResponseImageDetailLevel.Low),
                    ]),
            ]);

        Console.WriteLine(response.GetOutputText());
        Assert.That(response.GetOutputText().ToLowerInvariant(), Does.Contain("dog").Or.Contain("cat").IgnoreCase);
    }
#endif

    [RecordedTest]
    public async Task FileInputFromIdWorks()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();
        OpenAIFileClient fileClient = GetProxiedOpenAIClient<OpenAIFileClient>();
        string filePath = Path.Join("Assets", "files_travis_favorite_food.pdf");

        OpenAIFile newFileToUse;
        using (Recording.DisableRequestBodyRecording()) // Temp pending https://github.com/Azure/azure-sdk-tools/issues/11901
        {
            newFileToUse = await fileClient.UploadFileAsync(
            BinaryData.FromBytes(File.ReadAllBytes(filePath)),
            "test_favorite_foods.pdf",
            FileUploadPurpose.UserData);
        }

        Validate(newFileToUse);

        ResponseItem messageItem = ResponseItem.CreateUserMessageItem(
            [
                ResponseContentPart.CreateInputTextPart("Based on this file, what food should I order for whom?"),
                ResponseContentPart.CreateInputFilePart(newFileToUse.Id),
            ]);

        ResponseResult response = await client.CreateResponseAsync([messageItem]);

        Assert.That(response?.GetOutputText().ToLower(), Does.Contain("pizza"));
    }

    [RecordedTest]
    public async Task FileInputFromBinaryWorks()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        string filePath = Path.Join("Assets", "files_travis_favorite_food.pdf");
        Stream fileStream = File.OpenRead(filePath);
        BinaryData fileBytes = await BinaryData.FromStreamAsync(fileStream);

        ResponseItem messageItem = ResponseItem.CreateUserMessageItem(
            [
                ResponseContentPart.CreateInputTextPart("Based on this file, what food should I order for whom?"),
                ResponseContentPart.CreateInputFilePart(fileBytes, "application/pdf", "test_favorite_foods.pdf"),
            ]);

        ResponseResult response = await client.CreateResponseAsync([messageItem]);

        Assert.That(response?.GetOutputText(), Does.Contain("pizza"));
    }

    [RecordedTest]
    [TestCase(ResponsesTestInstructionMethod.InstructionsProperty)]
    [TestCase(ResponsesTestInstructionMethod.SystemMessage)]
    [TestCase(ResponsesTestInstructionMethod.DeveloperMessage)]
    public async Task AllInstructionMethodsWork(ResponsesTestInstructionMethod instructionMethod)
    {
        const string instructions = "Always begin your replies with 'Arr, matey'";

        List<ResponseItem> messages = new();

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

        CreateResponseOptions options = new(messages);

        if (instructionMethod == ResponsesTestInstructionMethod.InstructionsProperty)
        {
            options.Instructions = instructions;
        }

        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();
        ResponseResult response = await client.CreateResponseAsync(options);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.OutputItems, Is.Not.Null.And.Not.Empty);
        Assert.That(response.OutputItems[0], Is.InstanceOf<MessageResponseItem>());
        Assert.That((response.OutputItems[0] as MessageResponseItem).Content, Is.Not.Null.And.Not.Empty);
        Assert.That((response.OutputItems[0] as MessageResponseItem).Content[0].Text, Does.StartWith("Arr, matey"));

        ResponseResult retrievedResponse = await client.GetResponseAsync(new GetResponseOptions(response.Id));
        Assert.That((retrievedResponse?.OutputItems?.FirstOrDefault() as MessageResponseItem)?.Content?.FirstOrDefault()?.Text, Does.StartWith("Arr, matey"));

        if (instructionMethod == ResponsesTestInstructionMethod.InstructionsProperty)
        {
            Assert.That(retrievedResponse.Instructions, Is.EqualTo(instructions));
        }

        List<ResponseItem> listedItems = [];
        await foreach (var item in client.GetResponseInputItemsAsync(response.Id))
        {
            listedItems.Add(item);
        }

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

    [RecordedTest]
    public async Task TwoTurnCrossModel()
    {
        ResponsesClient client1 = GetProxiedOpenAIClient<ResponsesClient>("gpt-4o-mini");
        ResponsesClient client2 = GetProxiedOpenAIClient<ResponsesClient>("o3-mini");

        ResponseResult response1 = await client1.CreateResponseAsync("Hello, Assistant! My name is Travis.");
        ResponseResult response2 = await client2.CreateResponseAsync("What's my name?", response1.Id);

        Assert.That(response1.Model.StartsWith("gpt-4o-mini"), Is.True);
        Assert.That(response2.Model.StartsWith("o3-mini"), Is.True);
    }

    [RecordedTest]
    [TestCase("gpt-4o-mini")]
    [TestCase("computer-use-preview", Ignore = "Not yet supported with computer-use-preview")]
    public async Task StructuredOutputs(string modelName)
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>(modelName);

        ResponseResult response = await client.CreateResponseAsync(
            new CreateResponseOptions([ResponseItem.CreateUserMessageItem("Write a JSON document with a list of five animals")])
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

    [RecordedTest]
    public async Task FunctionCallWorks()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        CreateResponseOptions options = new([ResponseItem.CreateUserMessageItem("What should I wear for the weather in San Francisco, CA?")])
        {
            Tools = { s_GetWeatherAtLocationTool }
        };

        ResponseResult response = await client.CreateResponseAsync(
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

        ResponseItem functionReply = ResponseItem.CreateFunctionCallOutputItem(functionCall.CallId, "22 celcius and windy");
        CreateResponseOptions turn2Options = new([functionReply])
        {
            PreviousResponseId = response.Id,
            Tools = { s_GetWeatherAtLocationTool },
        };

        ResponseResult turn2Response = await client.CreateResponseAsync(
            turn2Options);
        Assert.That(turn2Response.OutputItems?.Count, Is.EqualTo(1));
        MessageResponseItem turn2Message = turn2Response!.OutputItems[0] as MessageResponseItem;
        Assert.That(turn2Message, Is.Not.Null);
        Assert.That(turn2Message!.Role, Is.EqualTo(MessageRole.Assistant));
        Assert.That(turn2Message.Content, Has.Count.EqualTo(1));
        Assert.That(turn2Message.Content[0].Text, Does.Contain("22"));

        await foreach (ResponseItem item in client.GetResponseInputItemsAsync(new ResponseItemCollectionOptions(turn2Response.Id)))
        { }
    }

    [RecordedTest]
    public async Task FunctionCallStreamingWorks()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        CreateResponseOptions options = new([ResponseItem.CreateUserMessageItem("What should I wear for the weather in San Francisco, CA?")])
        {
            Tools = { s_GetWeatherAtLocationTool },
            StreamingEnabled = true,
        };

        AsyncCollectionResult<StreamingResponseUpdate> responseUpdates = client.CreateResponseStreamingAsync(
            options);

        int functionCallArgumentsDeltaUpdateCount = 0;
        int functionCallArgumentsDoneUpdateCount = 0;

        StringBuilder argumentsBuilder = new StringBuilder();

        await foreach (StreamingResponseUpdate update in responseUpdates)
        {
            if (update is StreamingResponseFunctionCallArgumentsDeltaUpdate functionCallArgumentsDeltaUpdate)
            {
                functionCallArgumentsDeltaUpdateCount++;

                BinaryData delta = functionCallArgumentsDeltaUpdate.Delta;
                Assert.That(delta, Is.Not.Null);

                if (!delta.ToMemory().IsEmpty)
                {
                    argumentsBuilder.AppendLine(functionCallArgumentsDeltaUpdate.Delta.ToString());
                }
            }

            if (update is StreamingResponseFunctionCallArgumentsDoneUpdate functionCallArgumentsDoneUpdate)
            {
                functionCallArgumentsDoneUpdateCount++;

                BinaryData functionArguments = functionCallArgumentsDoneUpdate.FunctionArguments;
                Assert.That(functionArguments, Is.Not.Null);
                Assert.That(functionArguments.ToString(), Is.EqualTo(argumentsBuilder.ToString().ReplaceLineEndings(string.Empty)));

                argumentsBuilder.Clear();
            }
        }

        Assert.That(functionCallArgumentsDoneUpdateCount, Is.GreaterThan(0));
        Assert.That(functionCallArgumentsDeltaUpdateCount, Is.GreaterThanOrEqualTo(functionCallArgumentsDoneUpdateCount));
    }

    [RecordedTest]
    public async Task MaxTokens()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        ResponseResult response = await client.CreateResponseAsync(
            new CreateResponseOptions([ResponseItem.CreateUserMessageItem("Write three haikus about tropical fruit")])
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

    [RecordedTest]
    public async Task FunctionToolChoiceWorks()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        ResponseToolChoice toolChoice
            = ResponseToolChoice.CreateFunctionChoice(s_GetWeatherAtLocationToolName);

        CreateResponseOptions options = new([ResponseItem.CreateUserMessageItem("What should I wear for the weather in San Francisco, CA?")])
        {
            Tools = { s_GetWeatherAtLocationTool },
            ToolChoice = toolChoice,
        };

        ResponseResult response = await client.CreateResponseAsync(
            options);

        Assert.That(response.ToolChoice, Is.Not.Null);
        Assert.That(response.ToolChoice.Kind, Is.EqualTo(ResponseToolChoiceKind.Function));
        Assert.That(response.ToolChoice.FunctionName, Is.EqualTo(toolChoice.FunctionName));

        FunctionCallResponseItem functionCall = response.OutputItems.FirstOrDefault() as FunctionCallResponseItem;
        Assert.That(functionCall, Is.Not.Null);
        Assert.That(functionCall.FunctionName, Is.EqualTo(toolChoice.FunctionName));
    }

    [RecordedTest]
    public async Task CanStreamBackgroundResponses()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("gpt-4.1-mini");

        CreateResponseOptions createOptions = new([ResponseItem.CreateUserMessageItem("Tell me a bedtime story.")])
        {
            BackgroundModeEnabled = true,
            StreamingEnabled = true,
        };

        AsyncCollectionResult<StreamingResponseUpdate> updates = client.CreateResponseStreamingAsync(createOptions);

        string queuedResponseId = null;
        int lastSequenceNumber = 0;

        await foreach (StreamingResponseUpdate update in updates)
        {
            if (update is StreamingResponseQueuedUpdate queuedUpdate)
            {
                // Confirm that the response has been queued and break.
                queuedResponseId = queuedUpdate.Response.Id;
                lastSequenceNumber = queuedUpdate.SequenceNumber;
                break;
            }
        }

        Assert.That(queuedResponseId, Is.Not.Null.And.Not.Empty);
        Assert.That(lastSequenceNumber, Is.GreaterThan(0));

        // Try getting the response without streaming it.
        ResponseResult retrievedResponse = await client.GetResponseAsync(queuedResponseId);

        Assert.That(retrievedResponse, Is.Not.Null);
        Assert.That(retrievedResponse.Id, Is.EqualTo(queuedResponseId));
        Assert.That(retrievedResponse.BackgroundModeEnabled, Is.True);

        // Now try continuing the stream.
        GetResponseOptions getOptions = new(queuedResponseId)
        {
            StartingAfter = lastSequenceNumber,
            StreamingEnabled = true
        };

        AsyncCollectionResult<StreamingResponseUpdate> continuedUpdates = client.GetResponseStreamingAsync(getOptions);

        ResponseResult completedResponse = null;
        int? firstContinuedSequenceNumber = null;

        await foreach (StreamingResponseUpdate update in continuedUpdates)
        {
            if (firstContinuedSequenceNumber is null)
            {
                firstContinuedSequenceNumber = update.SequenceNumber;
            }

            if (update is StreamingResponseCompletedUpdate completedUpdate)
            {
                completedResponse = completedUpdate.Response;
            }
        }

        Assert.That(firstContinuedSequenceNumber, Is.EqualTo(lastSequenceNumber + 1));
        Assert.That(completedResponse?.Id, Is.EqualTo(queuedResponseId));
        Assert.That(completedResponse?.OutputItems?.FirstOrDefault(), Is.Not.Null);
    }

    [RecordedTest]
    public async Task CanCancelBackgroundResponses()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>("gpt-4.1-mini");

        CreateResponseOptions options = new([ResponseItem.CreateUserMessageItem("Hello, model!")])
        {
            BackgroundModeEnabled = true,
        };

        ResponseResult response = await client.CreateResponseAsync(options);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(response.BackgroundModeEnabled, Is.True);
        Assert.That(response.Status, Is.EqualTo(ResponseStatus.Queued));

        ResponseResult cancelledResponse = await client.CancelResponseAsync(response.Id);
        Assert.That(cancelledResponse.Id, Is.EqualTo(response.Id));
        Assert.That(cancelledResponse.Status, Is.EqualTo(ResponseStatus.Cancelled));
    }

    [RecordedTest]
    public async Task GetResponseNoOptions()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        ResponseResult createdResponse = await client.CreateResponseAsync("This is a test.");

        ResponseResult retrievedResponse = await client.GetResponseAsync(createdResponse.Id);

        Assert.That(createdResponse.Id, Is.EqualTo(retrievedResponse.Id));
    }

    [RecordedTest]
    public async Task CanGetInputTokenCounts()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        BinaryData inputTokensRequestBody = BinaryData.FromBytes("""
            {
                "model": "gpt-4o-mini",
                "input": "Tell me a joke."
            }
            """u8.ToArray());

        using BinaryContent requestContent = BinaryContent.Create(inputTokensRequestBody);
        ClientResult result = await client.GetInputTokenCountAsync("application/json", requestContent);

        Assert.That(result, Is.Not.Null);

        using JsonDocument responseJson = JsonDocument.Parse(result.GetRawResponse().Content.ToString());
        JsonElement root = responseJson.RootElement;

        Assert.That(root.GetProperty("object").GetString(), Is.EqualTo("response.input_tokens"));
        Assert.That(root.GetProperty("input_tokens").GetInt32(), Is.GreaterThan(0));
    }

    [RecordedTest]
    public async Task CanCompactConversation()
    {
        ResponsesClient client = GetProxiedOpenAIClient<ResponsesClient>();

        // First, create a response to get a real assistant message for compaction.
        ResponseResult initialResponse = await client.CreateResponseAsync("Create a simple landing page for a dog petting café.");
        Assert.That(initialResponse, Is.Not.Null);
        Assert.That(initialResponse.GetOutputText(), Is.Not.Null.And.Not.Empty);

        string assistantText = initialResponse.GetOutputText();

        BinaryData compactRequestBody = BinaryData.FromString($$"""
            {
                "model": "{{initialResponse.Model}}",
                "input": [
                    {
                        "role": "user",
                        "content": "Create a simple landing page for a dog petting café."
                    },
                    {
                        "id": "{{initialResponse.OutputItems[0].Id}}",
                        "type": "message",
                        "status": "completed",
                        "content": [
                            {
                                "type": "output_text",
                                "text": {{JsonSerializer.Serialize(assistantText)}}
                            }
                        ],
                        "role": "assistant"
                    }
                ]
            }
            """);

        using BinaryContent requestContent = BinaryContent.Create(compactRequestBody);
        ClientResult result = await client.CompactResponseAsync("application/json", requestContent);

        Assert.That(result, Is.Not.Null);

        using JsonDocument responseJson = JsonDocument.Parse(result.GetRawResponse().Content.ToString());
        JsonElement root = responseJson.RootElement;

        Assert.That(root.GetProperty("object").GetString(), Is.EqualTo("response.compaction"));
        Assert.That(root.TryGetProperty("output", out JsonElement outputElement), Is.True);
        Assert.That(outputElement.GetArrayLength(), Is.GreaterThan(0));

        // Verify that the output contains a compaction item.
        bool hasCompactionItem = false;
        foreach (JsonElement item in outputElement.EnumerateArray())
        {
            if (item.GetProperty("type").GetString() == "compaction")
            {
                hasCompactionItem = true;
                Assert.That(item.TryGetProperty("encrypted_content", out _), Is.True);
            }
        }
        Assert.That(hasCompactionItem, Is.True, "Expected the compaction output to contain a compaction item.");

        // Verify usage information is present.
        Assert.That(root.TryGetProperty("usage", out JsonElement usageElement), Is.True);
        Assert.That(usageElement.GetProperty("input_tokens").GetInt32(), Is.GreaterThan(0));
        Assert.That(usageElement.GetProperty("total_tokens").GetInt32(), Is.GreaterThan(0));
    }

    private List<string> FileIdsToDelete = [];
    private List<string> VectorStoreIdsToDelete = [];

    private static readonly string s_GetWeatherAtLocationToolName = "get_weather_at_location";
    private static readonly ResponseTool s_GetWeatherAtLocationTool = ResponseTool.CreateFunctionTool(
        functionName: s_GetWeatherAtLocationToolName,
        functionDescription: "Gets the weather at a specified location, optionally specifying units for temperature",
        functionParameters: BinaryData.FromString("""
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
        strictModeEnabled: false);
}