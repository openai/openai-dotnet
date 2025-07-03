using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OpenAI.Tests.Chat;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Chat")]
[Category("Smoke")]
public class ChatSmokeTests : SyncAsyncTestBase
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
            ? await client.CompleteChatAsync([new UserChatMessage("Mock me!")])
            : client.CompleteChat([new UserChatMessage("Mock me!")]);
        Assert.That(completionResult?.GetRawResponse(), Is.Not.Null);
        Assert.That(completionResult.GetRawResponse().Content?.ToString(), Does.Contain("additional world"));

        ChatCompletion completion = completionResult;

        Assert.That(completion.Id, Is.EqualTo(mockResponseId));
        Assert.That(completion.CreatedAt.ToUnixTimeSeconds, Is.EqualTo(mockCreated));
        Assert.That(completion.Role, Is.EqualTo(ChatMessageRole.Assistant));
        Assert.That(completion.Content[0].Text, Is.EqualTo("Hi there, user!"));
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
            choice = ChatToolChoice.CreateAutoChoice();
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
            choice = ChatToolChoice.CreateFunctionChoice(functionName);
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

#pragma warning disable CS0618
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
            choice = ChatFunctionChoice.CreateAutoChoice();
        }

        BinaryData serializedChoice = ModelReaderWriter.Write(choice);
        using JsonDocument choiceAsJson = JsonDocument.Parse(serializedChoice);
        Assert.That(choiceAsJson.RootElement, Is.Not.Null);
        Assert.That(choiceAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(choiceAsJson.RootElement.ToString(), Is.EqualTo("auto"));
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
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

            choice = ChatFunctionChoice.CreateNamedChoice(functionName);
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
#pragma warning restore CS0618

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
            part = ChatMessageContentPart.CreateTextPart(text);
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
            part = ChatMessageContentPart.CreateImagePart(new Uri(uri), ChatImageDetailLevel.High);
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
            part = ChatMessageContentPart.CreateImagePart(imageData, imageMediaType, ChatImageDetailLevel.Auto);
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
            ChatMessageContentPart.CreateTextPart("Describe this image for me:"),
            ChatMessageContentPart.CreateImagePart(new Uri("https://api.openai.com/test")));
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

        AssistantChatMessage manufacturedMessage = new([
            ChatToolCall.CreateFunctionToolCall("fake_tool_call_id", "fake_function_name", BinaryData.FromBytes("{}"u8.ToArray()))
        ]);
        manufacturedMessage.Refusal = "No!";
        string serialized = ModelReaderWriter.Write(manufacturedMessage).ToString();
        Assert.That(serialized, Does.Contain("refusal"));
        Assert.That(serialized, Does.Contain("No!"));
        Assert.That(serialized, Does.Contain("tool_calls"));
        Assert.That(serialized, Does.Not.Contain("content"));
    }

    [Test]
    public void SerializeAudioThings()
    {
        // User audio input: wire-correlated ("real") content parts should cleanly serialize/deserialize
        ChatMessageContentPart inputAudioContentPart = ChatMessageContentPart.CreateInputAudioPart(
            BinaryData.FromBytes([0x4, 0x2]),
            ChatInputAudioFormat.Mp3);
        Assert.That(inputAudioContentPart, Is.Not.Null);
        BinaryData serializedInputAudioContentPart = ModelReaderWriter.Write(inputAudioContentPart);
        Assert.That(serializedInputAudioContentPart.ToString(), Does.Contain(@"""format"":""mp3"""));
        ChatMessageContentPart deserializedInputAudioContentPart = ModelReaderWriter.Read<ChatMessageContentPart>(serializedInputAudioContentPart);
        Assert.That(deserializedInputAudioContentPart.InputAudioBytes.ToArray()[1], Is.EqualTo(0x2));

        AssistantChatMessage message = ModelReaderWriter.Read<AssistantChatMessage>(BinaryData.FromBytes("""
            {
                "role": "assistant",
                "audio": {
                    "id": "audio_correlated_id_1234"
                }
            }
            """u8.ToArray()));
        Assert.That(message.Content, Has.Count.EqualTo(0));
        Assert.That(message.OutputAudioReference, Is.Not.Null);
        Assert.That(message.OutputAudioReference.Id, Is.EqualTo("audio_correlated_id_1234"));
        string serializedMessage = ModelReaderWriter.Write(message).ToString();
        Assert.That(serializedMessage, Does.Contain(@"""audio"":{""id"":""audio_correlated_id_1234""}"));

        AssistantChatMessage ordinaryTextAssistantMessage = new(["This was a message from the assistant"]);
        ordinaryTextAssistantMessage.OutputAudioReference = new("extra-audio-id");
        BinaryData serializedLateAudioMessage = ModelReaderWriter.Write(ordinaryTextAssistantMessage);
        Assert.That(serializedLateAudioMessage.ToString(), Does.Contain("was a message"));
        Assert.That(serializedLateAudioMessage.ToString(), Does.Contain("extra-audio-id"));

        BinaryData rawAudioResponse = BinaryData.FromBytes("""
            {
              "id": "chatcmpl-AOqyHuhjVDeGVbCZXJZ8mCLyl5nBq",
              "object": "chat.completion",
              "created": 1730486857,
              "model": "gpt-4o-audio-preview-2024-10-01",
              "choices": [
                {
                  "index": 0,
                  "message": {
                    "role": "assistant",
                    "content": null,
                    "refusal": null,
                    "audio": {
                      "id": "audio_6725224ac62481908ab55dc283289d87",
                      "data": "dHJ1bmNhdGVk",
                      "expires_at": 1730490458,
                      "transcript": "Hello there! How can I assist you with your test today?"
                    }
                  },
                  "finish_reason": "stop"
                }
              ],
              "usage": {
                "prompt_tokens": 28,
                "completion_tokens": 97,
                "total_tokens": 125,
                "prompt_tokens_details": {
                  "cached_tokens": 0,
                  "text_tokens": 11,
                  "image_tokens": 0,
                  "audio_tokens": 17
                },
                "completion_tokens_details": {
                  "reasoning_tokens": 0,
                  "text_tokens": 23,
                  "audio_tokens": 74,
                  "accepted_prediction_tokens": 0,
                  "rejected_prediction_tokens": 0
                }
              },
              "system_fingerprint": "fp_49254d0e9b"
            }
            """u8.ToArray());
        ChatCompletion audioCompletion = ModelReaderWriter.Read<ChatCompletion>(rawAudioResponse);
        Assert.That(audioCompletion, Is.Not.Null);
        Assert.That(audioCompletion.Content, Has.Count.EqualTo(0));
        Assert.That(audioCompletion.OutputAudio, Is.Not.Null);
        Assert.That(audioCompletion.OutputAudio.Id, Is.EqualTo("audio_6725224ac62481908ab55dc283289d87"));
        Assert.That(audioCompletion.OutputAudio.AudioBytes, Is.Not.Null);
        Assert.That(audioCompletion.OutputAudio.Transcript, Is.Not.Null.And.Not.Empty);
        
        AssistantChatMessage audioHistoryMessage = new(audioCompletion);
        Assert.That(audioHistoryMessage.OutputAudioReference?.Id, Is.EqualTo(audioCompletion.OutputAudio.Id));

        foreach (KeyValuePair<ChatResponseModalities, (bool, bool, bool)> modalitiesValueToKeyTextAndAudioPresenceItem
            in new List<KeyValuePair<ChatResponseModalities, (bool, bool, bool)>>()
            {
                new(ChatResponseModalities.Default, (false, false, false)),
                new(ChatResponseModalities.Default | ChatResponseModalities.Text, (true, true, false)),
                new(ChatResponseModalities.Default | ChatResponseModalities.Audio, (true, false, true)),
                new(ChatResponseModalities.Default | ChatResponseModalities.Text | ChatResponseModalities.Audio, (true, true, true)),
                new(ChatResponseModalities.Text, (true, true, false)),
                new(ChatResponseModalities.Audio, (true, false, true)),
                new(ChatResponseModalities.Text | ChatResponseModalities.Audio, (true, true, true)),
            })
        {
            ChatResponseModalities modalitiesValue = modalitiesValueToKeyTextAndAudioPresenceItem.Key;
            (bool keyExpected, bool textExpected, bool audioExpected) = modalitiesValueToKeyTextAndAudioPresenceItem.Value;
            ChatCompletionOptions testOptions = new()
            {
                ResponseModalities = modalitiesValue,
            };
            string serializedOptions = ModelReaderWriter.Write(testOptions).ToString().ToLower();
            Assert.That(serializedOptions.Contains("modalities"), Is.EqualTo(keyExpected));
            Assert.That(serializedOptions.Contains("text"), Is.EqualTo(textExpected));
            Assert.That(serializedOptions.Contains("audio"), Is.EqualTo(audioExpected));
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeChatMessageWithSingleStringContent(bool fromRawJson)
    {
        const string text = "Hello, world!";
        AssistantChatMessage message;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($$"""
                {
                    "role": "assistant",
                    "content": "{{text}}",
                    "additional_property": true
                }
                """);

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            message = ModelReaderWriter.Read<AssistantChatMessage>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            message = new AssistantChatMessage([
                ChatMessageContentPart.CreateTextPart(text),
            ]);
        }

        BinaryData serializedMessage = ModelReaderWriter.Write(message);
        using JsonDocument messageAsJson = JsonDocument.Parse(serializedMessage);
        Assert.That(messageAsJson.RootElement, Is.Not.Null);
        Assert.That(messageAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(messageAsJson.RootElement.TryGetProperty("content", out JsonElement contentProperty), Is.True);
        Assert.That(contentProperty, Is.Not.Null);
        Assert.That(contentProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(contentProperty.ToString(), Is.EqualTo(text));

        if (fromRawJson)
        {
            // Confirm that we also have the additional data.
            Assert.That(messageAsJson.RootElement.TryGetProperty("additional_property", out JsonElement additionalPropertyProperty), Is.True);
            Assert.That(additionalPropertyProperty, Is.Not.Null);
            Assert.That(additionalPropertyProperty.ValueKind, Is.EqualTo(JsonValueKind.True));
        }
    }


    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeChatMessageWithEmptyStringContent(bool fromRawJson)
    {
        const string text = "";
        AssistantChatMessage message;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($$"""
                {
                    "role": "assistant",
                    "content": "{{text}}",
                    "additional_property": true
                }
                """);

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            message = ModelReaderWriter.Read<AssistantChatMessage>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            message = new AssistantChatMessage([
                ChatMessageContentPart.CreateTextPart(text),
            ]);
        }

        BinaryData serializedMessage = ModelReaderWriter.Write(message);
        using JsonDocument messageAsJson = JsonDocument.Parse(serializedMessage);
        Assert.That(messageAsJson.RootElement, Is.Not.Null);
        Assert.That(messageAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(messageAsJson.RootElement.TryGetProperty("content", out JsonElement contentProperty), Is.True);
        Assert.That(contentProperty, Is.Not.Null);
        Assert.That(contentProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
        Assert.That(contentProperty.ToString(), Is.EqualTo(text));

        if (fromRawJson)
        {
            // Confirm that we also have the additional data.
            Assert.That(messageAsJson.RootElement.TryGetProperty("additional_property", out JsonElement additionalPropertyProperty), Is.True);
            Assert.That(additionalPropertyProperty, Is.Not.Null);
            Assert.That(additionalPropertyProperty.ValueKind, Is.EqualTo(JsonValueKind.True));
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void SerializeChatMessageWithNoContent(bool fromRawJson)
    {
        string toolCallId = "fake_tool_call_id";
        string toolCallType = "function";
        string toolCallFunctionName = "fake_function_name";
        string toolCallFunctionArguments = "{}";
        AssistantChatMessage message;

        if (fromRawJson)
        {
            BinaryData data = BinaryData.FromString($$"""
                {
                    "role": "assistant",
                    "tool_calls": [{
                        "id": "{{toolCallId}}",
                        "type": "{{toolCallType}}",
                        "function": {
                            "name": "{{toolCallFunctionName}}",
                            "arguments": "{{toolCallFunctionArguments}}"
                        }
                    }],
                    "additional_property": true
                }
                """);

            // We deserialize the raw JSON. Later, we serialize it back and confirm nothing was lost in the process.
            message = ModelReaderWriter.Read<AssistantChatMessage>(data);
        }
        else
        {
            // We construct a new instance. Later, we serialize it and confirm it was constructed correctly.
            message = new AssistantChatMessage([
                ChatToolCall.CreateFunctionToolCall(toolCallId, toolCallFunctionName, BinaryData.FromBytes("{}"u8.ToArray()))
            ]);
        }

        BinaryData serializedMessage = ModelReaderWriter.Write(message);
        using JsonDocument messageAsJson = JsonDocument.Parse(serializedMessage);
        Assert.That(messageAsJson.RootElement, Is.Not.Null);
        Assert.That(messageAsJson.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Object));

        Assert.That(messageAsJson.RootElement.TryGetProperty("content", out JsonElement contentProperty), Is.False);

        Assert.That(messageAsJson.RootElement.TryGetProperty("tool_calls", out JsonElement toolCallsProperty), Is.True);
        Assert.That(toolCallsProperty, Is.Not.Null);
        Assert.That(toolCallsProperty.ValueKind, Is.EqualTo(JsonValueKind.Array));

        foreach (JsonElement toolCall in toolCallsProperty.EnumerateArray())
        {
            Assert.That(toolCall.TryGetProperty("id", out JsonElement toolCallIdProperty), Is.True);
            Assert.That(toolCallIdProperty, Is.Not.Null);
            Assert.That(toolCallIdProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(toolCallIdProperty.ToString(), Is.EqualTo(toolCallId));

            Assert.That(toolCall.TryGetProperty("type", out JsonElement toolCallTypeProperty), Is.True);
            Assert.That(toolCallTypeProperty, Is.Not.Null);
            Assert.That(toolCallTypeProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(toolCallTypeProperty.ToString(), Is.EqualTo(toolCallType));

            Assert.That(toolCall.TryGetProperty("function", out JsonElement toolCallFunctionProperty), Is.True);
            Assert.That(toolCallFunctionProperty, Is.Not.Null);
            Assert.That(toolCallFunctionProperty.ValueKind, Is.EqualTo(JsonValueKind.Object));

            Assert.That(toolCallFunctionProperty.TryGetProperty("name", out JsonElement toolCallFunctionNameProperty), Is.True);
            Assert.That(toolCallFunctionNameProperty, Is.Not.Null);
            Assert.That(toolCallFunctionNameProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(toolCallFunctionNameProperty.ToString(), Is.EqualTo(toolCallFunctionName));

            Assert.That(toolCallFunctionProperty.TryGetProperty("arguments", out JsonElement toolCallFunctionArgumentsProperty), Is.True);
            Assert.That(toolCallFunctionArgumentsProperty, Is.Not.Null);
            Assert.That(toolCallFunctionArgumentsProperty.ValueKind, Is.EqualTo(JsonValueKind.String));
            Assert.That(toolCallFunctionArgumentsProperty.ToString(), Is.EqualTo(toolCallFunctionArguments));
        }

        if (fromRawJson)
        {
            // Confirm that we also have the additional data.
            Assert.That(messageAsJson.RootElement.TryGetProperty("additional_property", out JsonElement additionalPropertyProperty), Is.True);
            Assert.That(additionalPropertyProperty, Is.Not.Null);
            Assert.That(additionalPropertyProperty.ValueKind, Is.EqualTo(JsonValueKind.True));
        }
    }

#pragma warning disable CS0618
    [Test]
    public void AssistantAndFunctionMessagesHandleNoContentCorrectly()
    {
        // AssistantChatMessage and FunctionChatMessage can both exist without content, but follow different rules:
        //   - AssistantChatMessage treats content as optional, as valid assistant message variants (e.g. for tool calls)
        //   - FunctionChatMessage meanwhile treats content as required and nullable.
        // This test validates that no-content assistant messages just don't serialize content, while no-content
        // function messages serialize content with an explicit null value.

        ChatToolCall fakeToolCall = ChatToolCall.CreateFunctionToolCall("call_abcd1234", "function_name", functionArguments: BinaryData.FromString("{}"));
        AssistantChatMessage assistantChatMessage = new([fakeToolCall]);
        string serializedAssistantChatMessage = ModelReaderWriter.Write(assistantChatMessage).ToString();
        Assert.That(serializedAssistantChatMessage, Does.Not.Contain("content"));

        FunctionChatMessage functionChatMessage = new("function_name", null);
        string serializedFunctionChatMessage = ModelReaderWriter.Write(functionChatMessage).ToString();
        Assert.That(serializedFunctionChatMessage, Does.Contain(@"""content"":null"));
    }
#pragma warning restore CS0618

#pragma warning disable CS0618
    [Test]
    public void SerializeMessagesWithNullProperties()
    {
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
        FunctionChatMessage functionMessage = new("my_function", null);
        BinaryData serializedMessage = ModelReaderWriter.Write(functionMessage);
        Console.WriteLine(serializedMessage.ToString());

        FunctionChatMessage deserializedMessage = ModelReaderWriter.Read<FunctionChatMessage>(serializedMessage);
    }
#pragma warning restore CS0618

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

        OpenAIClient topLevelClient = new(new ApiKeyCredential("mock-credential"), options);
        ChatClient firstClient = topLevelClient.GetChatClient("mock-model");
        ClientResult first = firstClient.CompleteChat(new UserChatMessage("Hello, world"));

        Assert.That(observedEndpoint, Is.Not.Null);
        Assert.That(observedEndpoint.AbsoluteUri, Does.Contain("my.custom.com/expected/test/endpoint"));
    }

    [Test]
    public void CanUseCollections()
    {
        ChatCompletionOptions options = new();
        Assert.That(options.Tools.Count, Is.EqualTo(0));
        Assert.That(options.Metadata.Count, Is.EqualTo(0));
        Assert.That(options.StopSequences.Count, Is.EqualTo(0));
    }

    [Test]
    public void IdempotentOptionsSerialization()
    {
        ChatCompletionOptions emptyOptions = new();
        BinaryData serializedEmptyOptions = ModelReaderWriter.Write(emptyOptions);
        Assert.That(serializedEmptyOptions.ToString(), Is.EqualTo("{}"));
        ChatCompletionOptions deserializedEmptyOptions = ModelReaderWriter.Read<ChatCompletionOptions>(serializedEmptyOptions);
        BinaryData reserializedEmptyOptions = ModelReaderWriter.Write(deserializedEmptyOptions);
        Assert.That(reserializedEmptyOptions.ToString(), Is.EqualTo("{}"));

        ChatCompletionOptions originalOptions = new()
        {
            IncludeLogProbabilities = true,
            FrequencyPenalty = 0.4f,
        };

        BinaryData serializedOptions = ModelReaderWriter.Write(originalOptions);

        string serializedOptionsText = serializedOptions.ToString();
        Assert.That(serializedOptionsText, Does.Contain("frequency_penalty"));
        Assert.That(serializedOptionsText, Does.Not.Contain("presence_penalty"));
        Assert.That(serializedOptionsText, Does.Not.Contain("stream_options"));

        ChatCompletionOptions deserializedOptions = ModelReaderWriter.Read<ChatCompletionOptions>(serializedOptions);
        BinaryData reserializedOptions = ModelReaderWriter.Write(deserializedOptions);

        string reserializedOptionsText = reserializedOptions.ToString();
        Assert.That(serializedOptions.ToString(), Is.EqualTo(reserializedOptionsText));
    }

    [Test]
    public void StableImageContentPartSerialization()
    {
        string base64HelloWorld = Convert.ToBase64String(Encoding.UTF8.GetBytes("hello world"));

        void AssertExpectedImagePart(ChatMessageContentPart imagePart)
        {
            Assert.That(imagePart.Kind, Is.EqualTo(ChatMessageContentPartKind.Image));
            Assert.That(imagePart.ImageBytesMediaType, Is.EqualTo("image/png"));
            Assert.That(imagePart.ImageDetailLevel, Is.EqualTo(ChatImageDetailLevel.High));
            Assert.That(Convert.FromBase64String(Convert.ToBase64String(imagePart.ImageBytes.ToArray())), Is.EqualTo("hello world"));
        }

        ChatMessageContentPart imagePart = ChatMessageContentPart.CreateImagePart(
            BinaryData.FromBytes(Encoding.UTF8.GetBytes("hello world")),
            "image/png",
            ChatImageDetailLevel.High);

        AssertExpectedImagePart(imagePart);

        BinaryData serializedImagePart = ModelReaderWriter.Write(imagePart);
        Assert.That(serializedImagePart, Is.Not.Null);

        ChatMessageContentPart deserializedImagePart = ModelReaderWriter.Read<ChatMessageContentPart>(serializedImagePart);

        AssertExpectedImagePart(deserializedImagePart);

        ChatMessageContentPart nonDataImagePart = ChatMessageContentPart.CreateImagePart(
            new Uri("https://test.openai.com/image.png"),
            ChatImageDetailLevel.High);

        Assert.That(nonDataImagePart.Kind, Is.EqualTo(ChatMessageContentPartKind.Image));
        Assert.That(nonDataImagePart.ImageUri?.AbsoluteUri, Is.EqualTo("https://test.openai.com/image.png"));

        serializedImagePart = ModelReaderWriter.Write(nonDataImagePart);
        Assert.That(serializedImagePart, Is.Not.Null);

        deserializedImagePart = ModelReaderWriter.Read<ChatMessageContentPart>(serializedImagePart);
        Assert.That(deserializedImagePart.Kind, Is.EqualTo(ChatMessageContentPartKind.Image));
        Assert.That(deserializedImagePart.ImageUri?.AbsoluteUri, Is.EqualTo("https://test.openai.com/image.png"));
    }

    [Test]
    public void StableFileContentPartSerialization()
    {
        string base64HelloWorld = Convert.ToBase64String(Encoding.UTF8.GetBytes("hello world"));

        void AssertExpectedFilePart(ChatMessageContentPart filePart)
        {
            Assert.That(filePart.Kind, Is.EqualTo(ChatMessageContentPartKind.File));
            Assert.That(filePart.FileBytesMediaType, Is.EqualTo("text/plain"));
            Assert.That(filePart.Filename, Is.EqualTo("test_content_part.txt"));
            Assert.That(Convert.FromBase64String(Convert.ToBase64String(filePart.FileBytes.ToArray())), Is.EqualTo("hello world"));
        }

        ChatMessageContentPart filePart = ChatMessageContentPart.CreateFilePart(
            BinaryData.FromBytes(Encoding.UTF8.GetBytes("hello world")),
            "text/plain",
            "test_content_part.txt");

        AssertExpectedFilePart(filePart);

        BinaryData serializedFilePart = ModelReaderWriter.Write(filePart);
        Assert.That(serializedFilePart, Is.Not.Null);

        ChatMessageContentPart deserializedFilePart = ModelReaderWriter.Read<ChatMessageContentPart>(serializedFilePart);

        AssertExpectedFilePart(deserializedFilePart);
    }
}
