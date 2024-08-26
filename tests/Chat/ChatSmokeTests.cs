using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Tests.Telemetry;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using static OpenAI.Tests.Telemetry.TestMeterListener;
using static OpenAI.Tests.TestHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OpenAI.Tests.Chat;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Chat")]
[Category("Smoke")]
public partial class ChatSmokeTests : SyncAsyncTestBase
{
    public ChatSmokeTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task SmokeTest()
    {
        string mockResponseId = Guid.NewGuid().ToString();
        long mockCreated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        BinaryData mockRequest = BinaryData.FromString($$"""
        {
            "model": "gpt-4o-mini",
            "messages": [
            { "role": "user", "content": "Hello, assistant!" }
            ]
        }
        """);
        BinaryData mockResponse = BinaryData.FromString($$"""
        {
            "id": "{{mockResponseId}}",
            "created": {{mockCreated}},
            "choices": [
                {
                "finish_reason": "stop",
                "message": { "role": "assistant", "content": "Hi there, user!" }
                }
            ],
            "additional_property": "hello, additional world!"
        }
        """);
        MockPipelineTransport mockTransport = new(mockRequest, mockResponse);

        OpenAIClientOptions options = new()
        {
            Transport = mockTransport
        };
        ChatClient client = new("model_name_replaced", new ApiKeyCredential("sk-not-a-real-key"), options);

        ClientResult<ChatCompletion> completionResult = IsAsync
            ? await client.CompleteChatAsync(["Mock me!"])
            : client.CompleteChat(["Mock me!"]);
        Assert.That(completionResult?.GetRawResponse(), Is.Not.Null);
        Assert.That(completionResult.GetRawResponse().Content?.ToString(), Does.Contain("additional world"));

        ChatCompletion completion = completionResult;

        Assert.That(completion.Id, Is.EqualTo(mockResponseId));
        Assert.That(completion.CreatedAt.ToUnixTimeSeconds, Is.EqualTo(mockCreated));
        Assert.That(completion.Role, Is.EqualTo(ChatMessageRole.Assistant));
        Assert.That(completion.Content[0].Text, Is.EqualTo("Hi there, user!"));

        var data = (IDictionary<string, BinaryData>)
            typeof(ChatCompletion)
            .GetProperty("SerializedAdditionalRawData", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetValue(completion);
        Assert.That(data, Is.Not.Null);
        Assert.That(data.Count, Is.GreaterThan(0));
    }

    [Test]
    public void CanCreateClients()
    {
        Uri fakeUri = new("https://127.0.0.1");
        ApiKeyCredential fakeCredential = new("sk-not-a-real-credential");

        {
            OpenAIClient topLevelClient = new(fakeCredential);
            Assert.That(topLevelClient, Is.Not.Null);
            ChatClient chatClient = topLevelClient.GetChatClient("model");
            Assert.That(chatClient, Is.Not.Null);
        }
        {
            OpenAIClient topLevelClient = new(fakeCredential, new OpenAIClientOptions()
            {
                Endpoint = fakeUri
            });
            Assert.That(topLevelClient, Is.Not.Null);
            ChatClient chatClient = topLevelClient.GetChatClient("model");
            Assert.That(chatClient, Is.Not.Null);
        }
        {
            ChatClient chatClient = new("model", fakeCredential);
            Assert.That(chatClient, Is.Not.Null);
        }
        {
            ChatClient chatClient = new("model", fakeCredential, new OpenAIClientOptions()
            {
                Endpoint = fakeUri
            });
            Assert.That(chatClient, Is.Not.Null);
        }
    }

    [Test]
    public void AuthFailureStreaming()
    {
        string fakeApiKey = "not-a-real-key-but-should-be-sanitized";
        ChatClient client = new("gpt-4o-mini", new ApiKeyCredential(fakeApiKey));
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
    public void SerializeCompoundContent()
    {
        UserChatMessage message = new(
            ChatMessageContentPart.CreateTextMessageContentPart("Describe this image for me:"),
            ChatMessageContentPart.CreateImageMessageContentPart(new Uri("https://api.openai.com/test")));
        string serializedMessage = ModelReaderWriter.Write(message).ToString();
        Assert.That(serializedMessage, Does.Contain("this image"));
        Assert.That(serializedMessage, Does.Contain("openai.com/test"));
    }

    [Test]
    public void SerializeRefusalMessages()
    {
        AssistantChatMessage message = ModelReaderWriter.Read<AssistantChatMessage>(BinaryData.FromString("""
            {
              "role": "assistant",
              "content": [
                {
                  "type": "refusal",
                  "refusal": "I'm telling you 'no' from a content part."
                }
              ],
              "refusal": "I'm telling you 'no' from the message refusal."
            }
            """));
        Assert.That(message.Content, Has.Count.EqualTo(1));
        Assert.That(message.Content[0].Refusal, Is.EqualTo("I'm telling you 'no' from a content part."));
        Assert.That(message.Refusal, Is.EqualTo("I'm telling you 'no' from the message refusal."));
        string reserialized = ModelReaderWriter.Write(message).ToString();
        Assert.That(reserialized, Does.Contain("from a content part"));
        Assert.That(reserialized, Does.Contain("from the message refusal"));

        AssistantChatMessage manufacturedMessage = new(toolCalls: []);
        manufacturedMessage.Refusal = "No!";
        string serialized = ModelReaderWriter.Write(manufacturedMessage).ToString();
        Assert.That(serialized, Does.Contain("refusal"));
        Assert.That(serialized, Does.Contain("No!"));
        Assert.That(serialized, Does.Not.Contain("tool"));
        Assert.That(serialized, Does.Not.Contain("content"));
    }

    [Test]
    public void SerializeMessagesWithNullProperties()
    {
#pragma warning disable CS0618 // FunctionChatMessage is deprecated
        AssistantChatMessage assistantMessage = ModelReaderWriter.Read<AssistantChatMessage>(BinaryData.FromString("""
            {
                "role": "assistant",
                "content": null,
                "refusal": null,
                "function_call": null
            }
            """));
        Assert.That(assistantMessage.Content, Has.Count.EqualTo(0));
        Assert.That(assistantMessage.Refusal, Is.Null);
        Assert.That(assistantMessage.FunctionCall, Is.Null);

        foreach ((string role, Type messageType) in new List<(string, Type)>()
            {
                ("assistant", typeof(AssistantChatMessage)),
                ("function", typeof(FunctionChatMessage)),
                ("tool", typeof(ToolChatMessage)),
                ("system", typeof(SystemChatMessage)),
                ("user", typeof(UserChatMessage))
            })
        {
            ChatMessage message = (ChatMessage)((object)ModelReaderWriter.Read(
                BinaryData.FromString($$"""
                    {
                      "role": "{{role}}",
                      "content": [null]
                    }
                    """),
                messageType));
            Assert.That(message, Is.Not.Null);
            Assert.That(message.Content, Has.Count.EqualTo(1));
            Assert.That(message.Content[0], Is.Null);
        }

        assistantMessage = ModelReaderWriter.Read<AssistantChatMessage>(BinaryData.FromString("""
            {
                "role": "assistant",
                "content": [null]
            }
            """));
        Assert.That(assistantMessage.Content, Has.Count.EqualTo(1));
        Assert.That(assistantMessage.Content[0], Is.Null);
        FunctionChatMessage functionMessage = new("my_function");
        functionMessage.Content.Add(null);
        BinaryData serializedMessage = ModelReaderWriter.Write(functionMessage);
        Console.WriteLine(serializedMessage.ToString());

        FunctionChatMessage deserializedMessage = ModelReaderWriter.Read<FunctionChatMessage>(serializedMessage);
#pragma warning restore
    }

    [Test]
    public void TopLevelClientOptionsPersistence()
    {
        MockPipelineTransport mockTransport = new(BinaryData.FromString("{}"), BinaryData.FromString("{}"));
        OpenAIClientOptions options = new()
        {
            Transport = mockTransport,
            Endpoint = new Uri("https://my.custom.com/expected/test/endpoint"),
        };
        Uri observedEndpoint = null;
        options.AddPolicy(new TestPipelinePolicy(message =>
        {
            observedEndpoint = message?.Request?.Uri;
        }),
        PipelinePosition.PerCall);

        OpenAIClient topLevelClient = new(new("mock-credential"), options);
        ChatClient firstClient = topLevelClient.GetChatClient("mock-model");
        ClientResult first = firstClient.CompleteChat("Hello, world");

        Assert.That(observedEndpoint, Is.Not.Null);
        Assert.That(observedEndpoint.AbsoluteUri, Does.Contain("my.custom.com/expected/test/endpoint"));
    }
}
