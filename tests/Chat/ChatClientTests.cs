using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Chat;

[TestFixture(true)]
[TestFixture(false)]
public partial class ChatClientTests : SyncAsyncTestBase
{
    public ChatClientTests(bool isAsync)
        : base(isAsync)
    {
    }

    [Test]
    public async Task HelloWorldChat()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [new UserChatMessage("Hello, world!")];
        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);
        Assert.That(result, Is.InstanceOf<ClientResult<ChatCompletion>>());
        Assert.That(result.Value.Content[0].Kind, Is.EqualTo(ChatMessageContentPartKind.Text));
        Assert.That(result.Value.Content[0].Text.Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task HelloWorldWithTopLevelClient()
    {
        OpenAIClient client = GetTestClient<OpenAIClient>(TestScenario.TopLevel);
        ChatClient chatClient = client.GetChatClient("gpt-3.5-turbo");
        IEnumerable<ChatMessage> messages = [new UserChatMessage("Hello, world!")];
        ClientResult<ChatCompletion> result = IsAsync
            ? await chatClient.CompleteChatAsync(messages)
            : chatClient.CompleteChat(messages);
        Assert.That(result.Value.Content[0].Text.Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task MultiMessageChat()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new SystemChatMessage("You are a helpful assistant. You always talk like a pirate."),
            new UserChatMessage("Hello, assistant! Can you help me train my parrot?"),
        ];
        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);
        Assert.That(new string[] { "aye", "arr", "hearty" }.Any(pirateWord => result.Value.Content[0].Text.ToLowerInvariant().Contains(pirateWord)));
    }

    [Test]
    public void StreamingChat()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")
        ];

        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();

        ResultCollection<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreaming(messages);
        Assert.That(streamingResult, Is.InstanceOf<ResultCollection<StreamingChatCompletionUpdate>>());
        int updateCount = 0;

        foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
            updateCount++;
        }
        Assert.That(updateCount, Is.GreaterThan(1));
        Assert.That(latestTokenReceiptTime - firstTokenReceiptTime > TimeSpan.FromMilliseconds(500));

        // Validate that network stream was disposed - this will show up as the
        // the raw response holding an empty content stream.
        PipelineResponse response = streamingResult.GetRawResponse();
        Assert.That(response.ContentStream.Length, Is.EqualTo(0));
    }

    [Test]
    public async Task StreamingChatAsync()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")
        ];

        TimeSpan? firstTokenReceiptTime = null;
        TimeSpan? latestTokenReceiptTime = null;
        Stopwatch stopwatch = Stopwatch.StartNew();

        AsyncResultCollection<StreamingChatCompletionUpdate> streamingResult = client.CompleteChatStreamingAsync(messages);
        Assert.That(streamingResult, Is.InstanceOf<AsyncResultCollection<StreamingChatCompletionUpdate>>());
        int updateCount = 0;
        ChatTokenUsage usage = null;

        await foreach (StreamingChatCompletionUpdate chatUpdate in streamingResult)
        {
            firstTokenReceiptTime ??= stopwatch.Elapsed;
            latestTokenReceiptTime = stopwatch.Elapsed;
            usage ??= chatUpdate.Usage;
            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
            updateCount++;
        }
        Assert.That(updateCount, Is.GreaterThan(1));
        Assert.That(latestTokenReceiptTime - firstTokenReceiptTime > TimeSpan.FromMilliseconds(500));
        Assert.That(usage, Is.Not.Null);
        Assert.That(usage?.InputTokens, Is.GreaterThan(0));
        Assert.That(usage?.OutputTokens, Is.GreaterThan(0));
        Assert.That(usage.InputTokens + usage.OutputTokens, Is.EqualTo(usage.TotalTokens));

        // Validate that network stream was disposed - this will show up as the
        // the raw response holding an empty content stream.
        PipelineResponse response = streamingResult.GetRawResponse();
        Assert.That(response.ContentStream.Length, Is.EqualTo(0));
    }

    #region Tools
    private const string GetCurrentLocationFunctionName = "get_current_location";

    private const string GetCurrentWeatherFunctionName = "get_current_weather";

    private static readonly ChatTool getCurrentLocationFunction = ChatTool.CreateFunctionTool(
        functionName: GetCurrentLocationFunctionName,
        functionDescription: "Get the user's current location"
    );

    private static readonly ChatTool getCurrentWeatherFunction = ChatTool.CreateFunctionTool(
        functionName: GetCurrentWeatherFunctionName,
        functionDescription: "Get the current weather in a given location",
        functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "location": {
                            "type": "string",
                            "description": "The city and state, e.g. Boston, MA"
                        },
                        "unit": {
                            "type": "string",
                            "enum": [ "celsius", "fahrenheit" ],
                            "description": "The temperature unit to use. Infer this from the specified location."
                        }
                    },
                    "required": [ "location" ]
                }
                """)
    );
    #endregion

    private ClientResult GetStreamingMockUpdate()
    {
        MockPipelineResponse response = new();
        response.SetContent("""
            data: {"id":"chatcmpl-9OrT0Ib1h95fQhtdfsMC1Pn8arrXk", "object":"chat.completion.chunk", "created":1715712426, "model":"gpt-3.5-turbo-0125", "system_fingerprint":null, "choices":[ { "index":0, "delta":{ "role":"assistant", "content":null, "tool_calls":[ { "index":0, "id":"call_KTeiNDFMuy7BO18eMZnaXdpn", "type":"function", "function":{ "name":"get_current_weather", "arguments":"" } }, { "index":0, "id":"call_KTeiNDFMuy7BO18eMZnaXdpn", "type":"function", "function":{ "name":"get_current_weather", "arguments":"" } }, { "index":0, "id":"call_KTeiNDFMuy7BO18eMZnaXdpn", "type":"function", "function":{ "name":"get_current_weather", "arguments":"" } } ] }, "logprobs":null, "finish_reason":null } ], "usage":null }

            data: [DONE]


            """);
        return ClientResult.FromResponse(response);
    }

    //[Test]
    //public void MockStreamingChatWithToolsAsync()
    //{
    //    StreamingChatUpdateCollection updates = new(GetStreamingMockUpdate);

    //    int updateCount = 0;
    //    foreach (StreamingChatUpdate chatUpdate in updates)
    //    {
    //        updateCount++;
    //    }

    //    Assert.That(updateCount, Is.GreaterThan(1));
    //}

    [Test]
    public async Task TwoTurnChat()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);

        List<ChatMessage> messages =
        [
            new UserChatMessage("In geometry, what are the different kinds of triangles, as defined by lengths of their sides?"),
        ];
        ClientResult<ChatCompletion> firstResult = IsAsync
            ? await client.CompleteChatAsync(messages)
            : client.CompleteChat(messages);
        Assert.That(firstResult?.Value, Is.Not.Null);
        Assert.That(firstResult.Value.Content[0].Text.ToLowerInvariant(), Contains.Substring("isosceles"));
        messages.Add(new AssistantChatMessage(firstResult.Value));
        messages.Add(new UserChatMessage("Which of those is the one where exactly two sides are the same length?"));
        ClientResult<ChatCompletion> secondResult = client.CompleteChat(messages);
        Assert.That(secondResult?.Value, Is.Not.Null);
        Assert.That(secondResult.Value.Content[0].Text.ToLowerInvariant(), Contains.Substring("isosceles"));
    }

    [Test]
    public async Task AuthFailure()
    {
        string fakeApiKey = "not-a-real-key-but-should-be-sanitized";
        ChatClient client = new("gpt-3.5-turbo", new ApiKeyCredential(fakeApiKey));
        IEnumerable<ChatMessage> messages = [new UserChatMessage("Uh oh, this isn't going to work with that key")];
        ClientResultException clientResultException = null;
        try
        {
            _ = IsAsync
                ? await client.CompleteChatAsync(messages)
                : client.CompleteChat(messages);
        }
        catch (ClientResultException ex)
        {
            clientResultException = ex;
        }
        Assert.That(clientResultException, Is.Not.Null);
        Assert.That(clientResultException.Status, Is.EqualTo((int)HttpStatusCode.Unauthorized));
        Assert.That(clientResultException.Message, Does.Contain("API key"));
        Assert.That(clientResultException.Message, Does.Not.Contain(fakeApiKey));
    }

    [Test]
    public void AuthFailureStreaming()
    {
        string fakeApiKey = "not-a-real-key-but-should-be-sanitized";
        ChatClient client = new("gpt-3.5-turbo", new ApiKeyCredential(fakeApiKey));
        Exception caughtException = null;
        try
        {
            foreach (var _ in client.CompleteChatStreaming(
                [new UserChatMessage("Uh oh, this isn't going to work with that key")]))
            { }
        }
        catch (Exception ex)
        {
            caughtException = ex;
        }
        var clientResultException = caughtException as ClientResultException;
        Assert.That(clientResultException, Is.Not.Null);
        Assert.That(clientResultException.Status, Is.EqualTo((int)HttpStatusCode.Unauthorized));
        Assert.That(clientResultException.Message, Does.Contain("API key"));
        Assert.That(clientResultException.Message, Does.Not.Contain(fakeApiKey));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task TokenLogProbabilities(bool includeLogProbabilities)
    {
        const int topLogProbabilityCount = 3;
        ChatClient client = new("gpt-3.5-turbo");
        IList<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];
        ChatCompletionOptions options;
        
        if (includeLogProbabilities)
        {
            options = new()
            {
                IncludeLogProbabilities = true,
                TopLogProbabilityCount = topLogProbabilityCount
            };
        }
        else
        {
            options = new();
        }

        ChatCompletion chatCompletions = await client.CompleteChatAsync(messages, options);
        Assert.That(chatCompletions, Is.Not.Null);

        if (includeLogProbabilities)
        {
            IReadOnlyList<ChatTokenLogProbabilityInfo> chatTokenLogProbabilities = chatCompletions.ContentTokenLogProbabilities;
            Assert.That(chatTokenLogProbabilities, Is.Not.Null.Or.Empty);

            foreach (ChatTokenLogProbabilityInfo tokenLogProbs in chatTokenLogProbabilities)
            {
                Assert.That(tokenLogProbs.Token, Is.Not.Null.Or.Empty);
                Assert.That(tokenLogProbs.Utf8ByteValues, Is.Not.Null);
                Assert.That(tokenLogProbs.TopLogProbabilities, Is.Not.Null.Or.Empty);
                Assert.That(tokenLogProbs.TopLogProbabilities, Has.Count.EqualTo(topLogProbabilityCount));

                foreach (ChatTokenTopLogProbabilityInfo tokenTopLogProbs in tokenLogProbs.TopLogProbabilities)
                {
                    Assert.That(tokenTopLogProbs.Token, Is.Not.Null.Or.Empty);
                    Assert.That(tokenTopLogProbs.Utf8ByteValues, Is.Not.Null);
                }
            }
        }
        else
        {
            Assert.That(chatCompletions.ContentTokenLogProbabilities, Is.Not.Null);
            Assert.That(chatCompletions.ContentTokenLogProbabilities, Is.Empty);
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task TokenLogProbabilitiesStreaming(bool includeLogProbabilities)
    {
        const int topLogProbabilityCount = 3;
        ChatClient client = new("gpt-3.5-turbo");
        IList<ChatMessage> messages = [new UserChatMessage("What are the best pizza toppings? Give me a breakdown on the reasons.")];
        ChatCompletionOptions options;

        if (includeLogProbabilities)
        {
            options = new()
            {
                IncludeLogProbabilities = true,
                TopLogProbabilityCount = topLogProbabilityCount
            };
        }
        else
        {
            options = new();
        }

        AsyncResultCollection<StreamingChatCompletionUpdate> chatCompletionUpdates = client.CompleteChatStreamingAsync(messages, options);
        Assert.That(chatCompletionUpdates, Is.Not.Null);

        await foreach (StreamingChatCompletionUpdate chatCompletionUpdate in chatCompletionUpdates)
        {
            // Token log probabilities are streamed together with their corresponding content update.
            if (includeLogProbabilities
                && chatCompletionUpdate.ContentUpdate.Count > 0
                && !string.IsNullOrWhiteSpace(chatCompletionUpdate.ContentUpdate[0].Text))
            {
                Assert.That(chatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null.Or.Empty);
                Assert.That(chatCompletionUpdate.ContentTokenLogProbabilities, Has.Count.EqualTo(1));

                foreach (ChatTokenLogProbabilityInfo tokenLogProbs in chatCompletionUpdate.ContentTokenLogProbabilities)
                {
                    Assert.That(tokenLogProbs.Token, Is.Not.Null.Or.Empty);
                    Assert.That(tokenLogProbs.Utf8ByteValues, Is.Not.Null);
                    Assert.That(tokenLogProbs.TopLogProbabilities, Is.Not.Null.Or.Empty);
                    Assert.That(tokenLogProbs.TopLogProbabilities, Has.Count.EqualTo(topLogProbabilityCount));

                    foreach (ChatTokenTopLogProbabilityInfo tokenTopLogProbs in tokenLogProbs.TopLogProbabilities)
                    {
                        Assert.That(tokenTopLogProbs.Token, Is.Not.Null.Or.Empty);
                        Assert.That(tokenTopLogProbs.Utf8ByteValues, Is.Not.Null);
                    }
                }
            }
            else
            {
                Assert.That(chatCompletionUpdate.ContentTokenLogProbabilities, Is.Not.Null);
                Assert.That(chatCompletionUpdate.ContentTokenLogProbabilities, Is.Empty);
            }
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeChatToolChoiceAsString(bool fromRawJson)
    {
        ChatToolChoice choice;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($"\"auto\"");

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            choice = ModelReaderWriter.Read<ChatToolChoice>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            choice = ChatToolChoice.Auto;
        }

        BinaryData serializedChoice = ModelReaderWriter.Write(choice);
        using JsonDocument choiceAsJson = JsonDocument.Parse(serializedChoice);
        Assert.That(choiceAsJson.RootElement, Is.Not.Null);
        Assert.That(choiceAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(choiceAsJson.RootElement.ToString(), Is.EqualTo("auto"));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeChatToolChoiceAsObject(bool fromRawJson)
    {
        const string functionName = "my_function_name";
        ChatToolChoice choice;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($$"""
            {
                "type": "function",
                "function": {
                    "name": "{{functionName}}"
                },
                "additional_property": true
            }
            """);

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            choice = ModelReaderWriter.Read<ChatToolChoice>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            choice = new ChatToolChoice(ChatTool.CreateFunctionTool(functionName));
        }

        BinaryData serializedChoice = ModelReaderWriter.Write(choice);
        using JsonDocument choiceAsJson = JsonDocument.Parse(serializedChoice);
        Assert.That(choiceAsJson.RootElement, Is.Not.Null);
        Assert.That(choiceAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(choiceAsJson.RootElement.TryGetProperty("type", out JsonElement typeProperty), Is.True);
        Assert.That(typeProperty, Is.Not.Null);
        Assert.That(typeProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(typeProperty.ToString(), Is.EqualTo("function"));

        Assert.That(choiceAsJson.RootElement.TryGetProperty("function", out JsonElement functionProperty), Is.True);
        Assert.That(functionProperty, Is.Not.Null);
        Assert.That(functionProperty.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(functionProperty.TryGetProperty("name", out JsonElement functionNameProperty), Is.True);
        Assert.That(functionNameProperty, Is.Not.Null);
        Assert.That(functionNameProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(functionNameProperty.ToString(), Is.EqualTo(functionName));

        if (fromRawJson)
        {
            // Confirm that we also have the additional data.
            Assert.That(choiceAsJson.RootElement.TryGetProperty("additional_property", out JsonElement additionalPropertyProperty), Is.True);
            Assert.That(additionalPropertyProperty, Is.Not.Null);
            Assert.That(additionalPropertyProperty.ValueKind, Is.EqualTo(JsonValueKind.True));
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeChatFunctionChoiceAsString(bool fromRawJson)
    {
        ChatFunctionChoice choice;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($"\"auto\"");

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            choice = ModelReaderWriter.Read<ChatFunctionChoice>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            choice = ChatFunctionChoice.Auto;
        }

        BinaryData serializedChoice = ModelReaderWriter.Write(choice);
        using JsonDocument choiceAsJson = JsonDocument.Parse(serializedChoice);
        Assert.That(choiceAsJson.RootElement, Is.Not.Null);
        Assert.That(choiceAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(choiceAsJson.RootElement.ToString(), Is.EqualTo("auto"));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeChatFunctionChoiceAsObject(bool fromRawJson)
    {
        const string functionName = "my_function_name";
        ChatFunctionChoice choice;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($$"""
            {
                "name": "{{functionName}}",
                "additional_property": true
            }
            """);

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            choice = ModelReaderWriter.Read<ChatFunctionChoice>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
#pragma warning disable CS0618
            choice = new ChatFunctionChoice(new ChatFunction(functionName));
#pragma warning restore CS0618
        }

        BinaryData serializedChoice = ModelReaderWriter.Write(choice);
        using JsonDocument choiceAsJson = JsonDocument.Parse(serializedChoice);
        Assert.That(choiceAsJson.RootElement, Is.Not.Null);
        Assert.That(choiceAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(choiceAsJson.RootElement.TryGetProperty("name", out JsonElement nameProperty), Is.True);
        Assert.That(nameProperty, Is.Not.Null);
        Assert.That(nameProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(nameProperty.ToString(), Is.EqualTo(functionName));

        if (fromRawJson)
        {
            // Confirm that we also have the additional data.
            Assert.That(choiceAsJson.RootElement.TryGetProperty("additional_property", out JsonElement additionalPropertyProperty), Is.True);
            Assert.That(additionalPropertyProperty, Is.Not.Null);
            Assert.That(additionalPropertyProperty.ValueKind, Is.EqualTo(JsonValueKind.True));
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeChatMessageContentPartAsText(bool fromRawJson)
    {
        const string text = "Hello, world!";
        ChatMessageContentPart part;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($$"""
                {
                    "type": "text",
                    "text": "{{text}}",
                    "additional_property": true
                }
                """);

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            part = ModelReaderWriter.Read<ChatMessageContentPart>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            part = ChatMessageContentPart.CreateTextMessageContentPart(text);
        }

        BinaryData serializedPart = ModelReaderWriter.Write(part);
        using JsonDocument partAsJson = JsonDocument.Parse(serializedPart);
        Assert.That(partAsJson.RootElement, Is.Not.Null);
        Assert.That(partAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(partAsJson.RootElement.TryGetProperty("type", out JsonElement typeProperty), Is.True);
        Assert.That(typeProperty, Is.Not.Null);
        Assert.That(typeProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(typeProperty.ToString(), Is.EqualTo("text"));

        Assert.That(partAsJson.RootElement.TryGetProperty("text", out JsonElement textProperty), Is.True);
        Assert.That(textProperty, Is.Not.Null);
        Assert.That(textProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(textProperty.ToString(), Is.EqualTo(text));

        if (fromRawJson)
        {
            // Confirm that we also have the additional data.
            Assert.That(partAsJson.RootElement.TryGetProperty("additional_property", out JsonElement additionalPropertyProperty), Is.True);
            Assert.That(additionalPropertyProperty, Is.Not.Null);
            Assert.That(additionalPropertyProperty.ValueKind, Is.EqualTo(JsonValueKind.True));
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeChatMessageContentPartAsImageUri(bool fromRawJson)
    {
        const string uri = "https://avatars.githubusercontent.com/u/14957082";
        ChatMessageContentPart part;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($$"""
                {
                    "type": "image_url",
                    "image_url": {
                        "url": "{{uri}}",
                        "detail": "high"
                    },
                    "additional_property": true
                }
                """);

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            part = ModelReaderWriter.Read<ChatMessageContentPart>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            part = ChatMessageContentPart.CreateImageMessageContentPart(new Uri(uri), ImageChatMessageContentPartDetail.High);
        }

        BinaryData serializedPart = ModelReaderWriter.Write(part);
        using JsonDocument partAsJson = JsonDocument.Parse(serializedPart);
        Assert.That(partAsJson.RootElement, Is.Not.Null);
        Assert.That(partAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(partAsJson.RootElement.TryGetProperty("type", out JsonElement typeProperty), Is.True);
        Assert.That(typeProperty, Is.Not.Null);
        Assert.That(typeProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(typeProperty.ToString(), Is.EqualTo("image_url"));

        Assert.That(partAsJson.RootElement.TryGetProperty("image_url", out JsonElement imageUrlProperty), Is.True);
        Assert.That(imageUrlProperty, Is.Not.Null);
        Assert.That(imageUrlProperty.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(imageUrlProperty.TryGetProperty("url", out JsonElement imageUrlUrlProperty), Is.True);
        Assert.That(imageUrlUrlProperty, Is.Not.Null);
        Assert.That(imageUrlUrlProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(imageUrlUrlProperty.ToString(), Is.EqualTo(uri));

        Assert.That(imageUrlProperty.TryGetProperty("detail", out JsonElement imageUrlDetailProperty), Is.True);
        Assert.That(imageUrlDetailProperty, Is.Not.Null);
        Assert.That(imageUrlDetailProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(imageUrlDetailProperty.ToString(), Is.EqualTo("high"));

        if (fromRawJson)
        {
            // Confirm that we also have the additional data.
            Assert.That(partAsJson.RootElement.TryGetProperty("additional_property", out JsonElement additionalPropertyProperty), Is.True);
            Assert.That(additionalPropertyProperty, Is.Not.Null);
            Assert.That(additionalPropertyProperty.ValueKind, Is.EqualTo(JsonValueKind.True));
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeChatMessageContentPartAsImageBytes(bool fromRawJson)
    {
        string imageMediaType = "image/png";
        string imageFilename = "images_dog_and_cat.png";
        string imagePath = Path.Combine("Assets", imageFilename);
        using Stream image = File.OpenRead(imagePath);

        BinaryData imageData = BinaryData.FromStream(image);
        string base64EncodedData = Convert.ToBase64String(imageData.ToArray());
        string dataUri = $"data:{imageMediaType};base64,{base64EncodedData}";

        ChatMessageContentPart part;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($$"""
                {
                    "type": "image_url",
                    "image_url": {
                        "url": "{{dataUri}}",
                        "detail": "auto"
                    },
                    "additional_property": true
                }
                """);

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            part = ModelReaderWriter.Read<ChatMessageContentPart>(data);

            // Confirm that we parsed the data URI correctly.
            Assert.That(part.ImageBytesMediaType, Is.EqualTo(imageMediaType));
            Assert.That(part.ImageBytes.ToArray(), Is.EqualTo(imageData.ToArray()));
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            part = ChatMessageContentPart.CreateImageMessageContentPart(imageData, imageMediaType, ImageChatMessageContentPartDetail.Auto);
        }

        BinaryData serializedPart = ModelReaderWriter.Write(part);
        using JsonDocument partAsJson = JsonDocument.Parse(serializedPart);
        Assert.That(partAsJson.RootElement, Is.Not.Null);
        Assert.That(partAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(partAsJson.RootElement.TryGetProperty("type", out JsonElement typeProperty), Is.True);
        Assert.That(typeProperty, Is.Not.Null);
        Assert.That(typeProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(typeProperty.ToString(), Is.EqualTo("image_url"));

        Assert.That(partAsJson.RootElement.TryGetProperty("image_url", out JsonElement imageUrlProperty), Is.True);
        Assert.That(imageUrlProperty, Is.Not.Null);
        Assert.That(imageUrlProperty.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(imageUrlProperty.TryGetProperty("url", out JsonElement imageUrlUrlProperty), Is.True);
        Assert.That(imageUrlUrlProperty, Is.Not.Null);
        Assert.That(imageUrlUrlProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(imageUrlUrlProperty.ToString(), Is.EqualTo(dataUri));

        Assert.That(imageUrlProperty.TryGetProperty("detail", out JsonElement imageUrlDetailProperty), Is.True);
        Assert.That(imageUrlDetailProperty, Is.Not.Null);
        Assert.That(imageUrlDetailProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(imageUrlDetailProperty.ToString(), Is.EqualTo("auto"));

        if (fromRawJson)
        {
            // Confirm that we also have the additional data.
            Assert.That(partAsJson.RootElement.TryGetProperty("additional_property", out JsonElement additionalPropertyProperty), Is.True);
            Assert.That(additionalPropertyProperty, Is.Not.Null);
            Assert.That(additionalPropertyProperty.ValueKind, Is.EqualTo(JsonValueKind.True));
        }
    }

    [Test]
    public async Task JsonResult()
    {
        ChatClient client = GetTestClient<ChatClient>(TestScenario.Chat);
        IEnumerable<ChatMessage> messages = [
            new UserChatMessage("Give me a JSON object with the following properties: red, green, and blue. The value "
                + "of each property should be a string containing their RGB representation in hexadecimal.")
        ];
        ChatCompletionOptions options = new() { ResponseFormat = ChatResponseFormat.JsonObject };
        ClientResult<ChatCompletion> result = IsAsync
            ? await client.CompleteChatAsync(messages, options)
            : client.CompleteChat(messages, options);

        JsonDocument jsonDocument = JsonDocument.Parse(result.Value.Content[0].Text);

        Assert.That(jsonDocument.RootElement.TryGetProperty("red", out JsonElement redProperty));
        Assert.That(jsonDocument.RootElement.TryGetProperty("green", out JsonElement greenProperty));
        Assert.That(jsonDocument.RootElement.TryGetProperty("blue", out JsonElement blueProperty));
        Assert.That(redProperty.GetString().ToLowerInvariant(), Contains.Substring("ff0000"));
        Assert.That(greenProperty.GetString().ToLowerInvariant(), Contains.Substring("00ff00"));
        Assert.That(blueProperty.GetString().ToLowerInvariant(), Contains.Substring("0000ff"));
    }
}
