using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Files;
using OpenAI.Images;
using OpenAI.Responses;

#pragma warning disable OPENAI001
#pragma warning disable OPENAI002
#pragma warning disable SCME0001

namespace OpenAI.Tests.Snippets;

[Explicit("These tests are used for compile-time verification of code snippets used in the README.  There is little value in executing them for test runs.")]
[Category("Snippets")]
[TestFixture]
public class ReadMeSnippets
{
    private string _originalApiKey;
    private string _originalAzureOpenAIEndpoint;

    [OneTimeSetUp]
    public void Setup()
    {
        _originalApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        _originalAzureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");

        Environment.SetEnvironmentVariable("OPENAI_API_KEY", nameof(ReadMeSnippets));
        Environment.SetEnvironmentVariable("AZURE_OPENAI_ENDPOINT", "https://my-azure-openai-endpoint.contoso.com/");
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", _originalApiKey);
        Environment.SetEnvironmentVariable("AZURE_OPENAI_ENDPOINT", _originalAzureOpenAIEndpoint);
    }

    [Test]
    public void ChatCompletion_Basic()
    {
        var clientMock = new Mock<ChatClient>();

        clientMock
            .Setup(c => c.CompleteChat(It.IsAny<ChatMessage[]>()))
            .Returns(ClientResult.FromValue(CreateChatCompletionMock(), new MockPipelineResponse()));

        #region Snippet:ReadMe_ChatCompletion_Basic
#if SNIPPET
        ChatClient client = new(model: "gpt-5.1", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
#else
        ChatClient client = clientMock.Object;
#endif

        ChatCompletion completion = client.CompleteChat("Say 'this is a test.'");
        Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
        #endregion
    }

    [Test]
    public async Task ChatCompletion_Async()
    {
        var clientMock = new Mock<ChatClient>();
        var client = clientMock.Object;

        clientMock
            .Setup(c => c.CompleteChatAsync(It.IsAny<ChatMessage[]>()))
            .ReturnsAsync(ClientResult.FromValue(CreateChatCompletionMock(), new MockPipelineResponse()));

        #region Snippet:ReadMe_ChatCompletion_Async
        ChatCompletion completion = await client.CompleteChatAsync("Say 'this is a test.'");
        #endregion
    }

    [Test]
    public void CustomUrl()
    {
        #region Snippet:ReadMe_CustomUrl
        ChatClient client = new(
            model: "MODEL_NAME",
            credential: new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAI_API_KEY")),
            options: new OpenAIClientOptions()
            {
                Endpoint = new Uri("https://YOUR_BASE_URL")
            });
        #endregion
    }

    [Test]
    public void OpenAIClient_Create()
    {
        #region Snippet:ReadMe_OpenAIClient_Create
        OpenAIClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        #endregion
    }

    [Test]
    public void OpenAIClient_GetAudioClient()
    {
        OpenAIClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        #region Snippet:ReadMe_OpenAIClient_GetAudioClient
        AudioClient ttsClient = client.GetAudioClient("tts-1");
        AudioClient whisperClient = client.GetAudioClient("whisper-1");
        #endregion
    }

    [Test]
    public void DependencyInjection_Register()
    {
        var builderMock = new Mock<ApplicationBuilder>();
        var builder = builderMock.Object;

        builderMock
            .SetupGet(b => b.Services)
            .Returns(new ServiceCollection());

        #region Snippet:ReadMe_DependencyInjection_Register
        builder.Services.AddSingleton<ChatClient>(serviceProvider =>
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            var model = "gpt-5.1";

            return new ChatClient(model, apiKey);
        });
        #endregion
    }

    [Test]
    public void Streaming_Sync()
    {
        var client = Mock.Of<ChatClient>();

        #region Snippet:ReadMe_Streaming_Sync
        CollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreaming("Say 'this is a test.'");
        #endregion
    }

    [Test]
    public void Streaming_Enumerate()
    {
         var resultMock = new Mock<CollectionResult<StreamingChatCompletionUpdate>>();
         var clientMock = new Mock<ChatClient>();

         resultMock
            .Protected()
            .Setup<IEnumerable<StreamingChatCompletionUpdate>>("GetValuesFromPage", ItExpr.IsAny<ClientResult>())
            .Returns(Enumerable.Empty<StreamingChatCompletionUpdate>());

         clientMock
            .Setup(c => c.CompleteChatStreaming(It.IsAny<ChatMessage[]>()))
            .Returns(resultMock.Object);

#if SNIPPET
        ChatClient client = new(model: "gpt-5.1", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
#else
        ChatClient client = clientMock.Object;
#endif

        #region Snippet:ReadMe_Streaming_Enumerate
        CollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreaming("Say 'this is a test.'");

        Console.Write($"[ASSISTANT]: ");
        foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
        {
            if (completionUpdate.ContentUpdate.Count > 0)
            {
                Console.Write(completionUpdate.ContentUpdate[0].Text);
            }
        }
        #endregion
    }

    [Test]
    public async Task Streaming_Async()
    {
        var resultMock = new Mock<AsyncCollectionResult<StreamingChatCompletionUpdate>>();
        var clientMock = new Mock<ChatClient>();

        resultMock
            .Setup(r => r.GetRawPagesAsync())
            .Returns(AsyncEnumerable.Empty<ClientResult>());

        clientMock
            .Setup(c => c.CompleteChatStreamingAsync(It.IsAny<ChatMessage[]>()))
            .Returns(resultMock.Object);

#if SNIPPET
        ChatClient client = new(model: "gpt-5.1", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
#else
        ChatClient client = clientMock.Object;
#endif

        #region Snippet:ReadMe_Streaming_Async
        AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreamingAsync("Say 'this is a test.'");

        Console.Write($"[ASSISTANT]: ");
        await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
        {
            if (completionUpdate.ContentUpdate.Count > 0)
            {
                Console.Write(completionUpdate.ContentUpdate[0].Text);
            }
        }
        #endregion
    }

    [Test]
    public void Tools_Definitions()
    {
        var mockClient = new Mock<ChatClient>();
        var client = mockClient.Object;

        mockClient
            .Setup(c => c.CompleteChat(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatCompletionOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(ClientResult.FromValue(
                OpenAIChatModelFactory.ChatCompletion(finishReason: ChatFinishReason.Stop, role: ChatMessageRole.Assistant),
                new MockPipelineResponse()));

        #region Snippet:ReadMe_Tools_Functions
        static string GetCurrentLocation()
        {
            // Call the location API here.
            return "San Francisco";
        }

        static string GetCurrentWeather(string location, string unit = "celsius")
        {
            // Call the weather API here.
            return $"31 {unit}";
        }
        #endregion

        #region Snippet:ReadMe_Tools_Definitions
        ChatTool getCurrentLocationTool = ChatTool.CreateFunctionTool(
            functionName: nameof(GetCurrentLocation),
            functionDescription: "Get the user's current location"
        );

        ChatTool getCurrentWeatherTool = ChatTool.CreateFunctionTool(
            functionName: nameof(GetCurrentWeather),
            functionDescription: "Get the current weather in a given location",
            functionParameters: BinaryData.FromBytes("""
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
                """u8.ToArray())
        );
        #endregion

        #region Snippet:ReadMe_Tools_Options
        List<ChatMessage> messages =
        [
            new UserChatMessage("What's the weather like today?"),
        ];

        ChatCompletionOptions options = new()
        {
            Tools = { getCurrentLocationTool, getCurrentWeatherTool },
        };
        #endregion

        #region Snippet:ReadMe_Tools_Loop
        bool requiresAction;

        do
        {
            requiresAction = false;
            ChatCompletion completion = client.CompleteChat(messages, options);

            switch (completion.FinishReason)
            {
                case ChatFinishReason.Stop:
                {
                    // Add the assistant message to the conversation history.
                    messages.Add(new AssistantChatMessage(completion));
                    break;
                }

                case ChatFinishReason.ToolCalls:
                {
                    // First, add the assistant message with tool calls to the conversation history.
                    messages.Add(new AssistantChatMessage(completion));

                    // Then, add a new tool message for each tool call that is resolved.
                    foreach (ChatToolCall toolCall in completion.ToolCalls)
                    {
                        switch (toolCall.FunctionName)
                        {
                            case nameof(GetCurrentLocation):
                                {
                                    string toolResult = GetCurrentLocation();
                                    messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                    break;
                                }

                            case nameof(GetCurrentWeather):
                                {
                                    // The arguments that the model wants to use to call the function are specified as a
                                    // stringified JSON object based on the schema defined in the tool definition. Note that
                                    // the model may hallucinate arguments too. Consequently, it is important to do the
                                    // appropriate parsing and validation before calling the function.
                                    using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                                    bool hasLocation = argumentsJson.RootElement.TryGetProperty("location", out JsonElement location);
                                    bool hasUnit = argumentsJson.RootElement.TryGetProperty("unit", out JsonElement unit);

                                    if (!hasLocation)
                                    {
                                        throw new ArgumentNullException(nameof(location), "The location argument is required.");
                                    }

                                    string toolResult = hasUnit
                                        ? GetCurrentWeather(location.GetString(), unit.GetString())
                                        : GetCurrentWeather(location.GetString());
                                    messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                    break;
                                }

                            default:
                                {
                                    // Handle other unexpected calls.
                                    throw new NotImplementedException();
                                }
                        }
                    }

                    requiresAction = true;
                    break;
                }

                case ChatFinishReason.Length:
                    throw new NotImplementedException("Incomplete model output due to MaxTokens parameter or token limit exceeded.");

                case ChatFinishReason.ContentFilter:
                    throw new NotImplementedException("Omitted content due to a content filter flag.");

                case ChatFinishReason.FunctionCall:
                    throw new NotImplementedException("Deprecated in favor of tool calls.");

                default:
                    throw new NotImplementedException(completion.FinishReason.ToString());
            }
        } while (requiresAction);
        #endregion
    }

    [Test]
    public void StructuredOutputs()
    {
        var clientMock = new Mock<ChatClient>();

        clientMock
            .Setup(c => c.CompleteChat(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatCompletionOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(ClientResult.FromValue(
                OpenAIChatModelFactory.ChatCompletion(content: new ChatMessageContent("""{"steps":[],"final_answer":"x = -3.75"}""")),
                new MockPipelineResponse()));

#if SNIPPET
        ChatClient client = new(model: "gpt-5.1", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
#else
        ChatClient client = clientMock.Object;
#endif

        #region Snippet:ReadMe_StructuredOutputs
        List<ChatMessage> messages =
        [
            new UserChatMessage("How can I solve 8x + 7 = -23?"),
        ];

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "math_reasoning",
                jsonSchema: BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "steps": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                        "explanation": { "type": "string" },
                                        "output": { "type": "string" }
                                    },
                                    "required": ["explanation", "output"],
                                    "additionalProperties": false
                                }
                            },
                            "final_answer": { "type": "string" }
                        },
                        "required": ["steps", "final_answer"],
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
                jsonSchemaIsStrict: true)
        };

        ChatCompletion completion = client.CompleteChat(messages, options);

        using JsonDocument structuredJson = JsonDocument.Parse(completion.Content[0].Text);

        Console.WriteLine($"Final answer: {structuredJson.RootElement.GetProperty("final_answer")}");
        Console.WriteLine("Reasoning steps:");

        foreach (JsonElement stepElement in structuredJson.RootElement.GetProperty("steps").EnumerateArray())
        {
            Console.WriteLine($"  - Explanation: {stepElement.GetProperty("explanation")}");
            Console.WriteLine($"    Output: {stepElement.GetProperty("output")}");
        }
        #endregion
    }

    [Test]
    public void ChatAudio()
    {
        var clientMock = new Mock<ChatClient>();

        clientMock
            .Setup(c => c.CompleteChat(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatCompletionOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(ClientResult.FromValue(
                OpenAIChatModelFactory.ChatCompletion(
                    content: new ChatMessageContent("The weather is sunny."),
                    role: ChatMessageRole.Assistant,
                    outputAudio: OpenAIChatModelFactory.ChatOutputAudio(
                        id: "audio-123",
                        audioBytes: BinaryData.FromBytes(new byte[] { 0x00 }),
                        transcript: "The weather is sunny.",
                        expiresAt: DateTimeOffset.UtcNow.AddHours(1))),
                new MockPipelineResponse()));

        #region Snippet:ReadMe_ChatAudio
#if SNIPPET
        // Chat audio input and output is only supported on specific models, beginning with gpt-4o-audio-preview
        ChatClient client = new("gpt-5.1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        // Input audio is provided to a request by adding an audio content part to a user message
        string audioFilePath = Path.Combine("Assets", "realtime_whats_the_weather_pcm16_24khz_mono.wav");
        byte[] audioFileRawBytes = File.ReadAllBytes(audioFilePath);
        BinaryData audioData = BinaryData.FromBytes(audioFileRawBytes);
#else

        ChatClient client = clientMock.Object;
        byte[] audioFileRawBytes = new byte[] { 0x00 };
        BinaryData audioData = BinaryData.FromBytes(audioFileRawBytes);
#endif

        List<ChatMessage> messages =
        [
            new UserChatMessage(ChatMessageContentPart.CreateInputAudioPart(audioData, ChatInputAudioFormat.Wav)),
        ];

        // Output audio is requested by configuring ChatCompletionOptions to include the appropriate
        // ResponseModalities values and corresponding AudioOptions.
        ChatCompletionOptions options = new()
        {
            ResponseModalities = ChatResponseModalities.Text | ChatResponseModalities.Audio,
            AudioOptions = new(ChatOutputAudioVoice.Alloy, ChatOutputAudioFormat.Mp3),
        };

        ChatCompletion completion = client.CompleteChat(messages, options);

        void PrintAudioContent()
        {
            if (completion.OutputAudio is ChatOutputAudio outputAudio)
            {
                Console.WriteLine($"Response audio transcript: {outputAudio.Transcript}");
                string outputFilePath = $"{outputAudio.Id}.mp3";
#if SNIPPET
                using (FileStream outputFileStream = File.OpenWrite(outputFilePath))
                {
                    outputFileStream.Write(outputAudio.AudioBytes);
                }
#endif

                Console.WriteLine($"Response audio written to file: {outputFilePath}");
                Console.WriteLine($"Valid on followup requests until: {outputAudio.ExpiresAt}");
            }
        }

        PrintAudioContent();

        // To refer to past audio output, create an assistant message from the earlier ChatCompletion, use the earlier
        // response content part, or use ChatMessageContentPart.CreateAudioPart(string) to manually instantiate a part.
        messages.Add(new AssistantChatMessage(completion));
        messages.Add("Can you say that like a pirate?");

        completion = client.CompleteChat(messages, options);

        PrintAudioContent();
        #endregion
    }

    [Test]
    public async Task ResponsesStreaming()
    {
        var clientMock = new Mock<ResponsesClient>();
        var streamingResultMock = new Mock<AsyncCollectionResult<StreamingResponseUpdate>>();

        clientMock
            .Setup(c => c.CreateResponseAsync(
                It.IsAny<CreateResponseOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientResult.FromValue(new ResponseResult(), new MockPipelineResponse()));

        streamingResultMock
            .Setup(r => r.GetRawPagesAsync())
            .Returns(AsyncEnumerable.Empty<ClientResult>());

        clientMock
            .Setup(c => c.CreateResponseStreamingAsync(
                It.IsAny<CreateResponseOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(streamingResultMock.Object);

        #region Snippet:ReadMe_ResponsesStreaming
#if SNIPPET
        ResponsesClient client = new(
            apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
#else
        ResponsesClient client = clientMock.Object;
#endif

        CreateResponseOptions options = new()
        {
            Model = "gpt-5.1",
            ReasoningOptions = new ResponseReasoningOptions()
            {
                ReasoningEffortLevel = ResponseReasoningEffortLevel.High,
            },
        };

        options.InputItems.Add(ResponseItem.CreateUserMessageItem("What's the optimal strategy to win at poker?"));
        ResponseResult response = await client.CreateResponseAsync(options);

        CreateResponseOptions streamingOptions = new()
        {
            Model = "gpt-5.1",
            ReasoningOptions = new ResponseReasoningOptions()
            {
                ReasoningEffortLevel = ResponseReasoningEffortLevel.High,
            },
            StreamingEnabled = true,
        };

        streamingOptions.InputItems.Add(ResponseItem.CreateUserMessageItem("What's the optimal strategy to win at poker?"));

        await foreach (StreamingResponseUpdate update
            in client.CreateResponseStreamingAsync(streamingOptions))
        {
            if (update is StreamingResponseOutputItemAddedUpdate itemUpdate
                && itemUpdate.Item is ReasoningResponseItem reasoningItem)
            {
                Console.WriteLine($"[Reasoning] ({reasoningItem.Status})");
            }
            else if (update is StreamingResponseOutputItemAddedUpdate itemDone
                && itemDone.Item is ReasoningResponseItem reasoningDone)
            {
                Console.WriteLine($"[Reasoning DONE] ({reasoningDone.Status})");
            }
            else if (update is StreamingResponseOutputTextDeltaUpdate delta)
            {
                Console.Write(delta.Delta);
            }
        }
        #endregion
    }

    [Test]
    public async Task ResponsesFileSearch()
    {
        var clientMock = new Mock<ResponsesClient>();

        clientMock
            .Setup(c => c.CreateResponseAsync(
                It.IsAny<CreateResponseOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientResult.FromValue(new ResponseResult(), new MockPipelineResponse()));

        #region Snippet:ReadMe_ResponsesFileSearch
#if SNIPPET
        ResponsesClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
#else
        ResponsesClient client = clientMock.Object;

#endif
        string vectorStoreId = "vs-123";

        ResponseTool fileSearchTool
            = ResponseTool.CreateFileSearchTool(vectorStoreIds: [vectorStoreId]);

        CreateResponseOptions options = new()
        {
            Model = "gpt-5.1",
            Tools = { fileSearchTool }
        };

        options.InputItems.Add(ResponseItem.CreateUserMessageItem("According to available files, what's the secret number?"));
        ResponseResult response = await client.CreateResponseAsync(options);

        foreach (ResponseItem outputItem in response.OutputItems)
        {
            if (outputItem is FileSearchCallResponseItem fileSearchCall)
            {
                Console.WriteLine($"[file_search] ({fileSearchCall.Status}): {fileSearchCall.Id}");
                foreach (string query in fileSearchCall.Queries)
                {
                    Console.WriteLine($"  - {query}");
                }
            }
            else if (outputItem is MessageResponseItem message)
            {
                Console.WriteLine($"[{message.Role}] {message.Content.FirstOrDefault()?.Text}");
            }
        }
        #endregion
    }

    [Test]
    public async Task ResponsesWebSearch()
    {
        var clientMock = new Mock<ResponsesClient>();

        clientMock
            .Setup(c => c.CreateResponseAsync(
                It.IsAny<CreateResponseOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ClientResult.FromValue(new ResponseResult(), new MockPipelineResponse()));

        #region Snippet:ReadMe_ResponsesWebSearch
#if SNIPPET
        ResponsesClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
#else
        ResponsesClient client = clientMock.Object;
#endif

        CreateResponseOptions options = new()
        {
            Model = "gpt-5.1",
            Tools = { ResponseTool.CreateWebSearchTool() },
        };

        options.InputItems.Add(ResponseItem.CreateUserMessageItem("What's a happy news headline from today?"));
        ResponseResult response = await client.CreateResponseAsync(options);

        foreach (ResponseItem item in response.OutputItems)
        {
            if (item is WebSearchCallResponseItem webSearchCall)
            {
                Console.WriteLine($"[Web search invoked]({webSearchCall.Status}) {webSearchCall.Id}");
            }
            else if (item is MessageResponseItem message)
            {
                Console.WriteLine($"[{message.Role}] {message.Content?.FirstOrDefault()?.Text}");
            }
        }
        #endregion
    }

    [Test]
    public void Embeddings()
    {
        var clientMock = new Mock<EmbeddingClient>();

        clientMock
            .Setup(c => c.GenerateEmbedding(
                It.IsAny<string>(),
                It.IsAny<EmbeddingGenerationOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(ClientResult.FromValue(
                OpenAIEmbeddingsModelFactory.OpenAIEmbedding(vector: new float[] { 0.1f, 0.2f, 0.3f }),
                new MockPipelineResponse()));

#if SNIPPET
        EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
#else
        EmbeddingClient client = clientMock.Object;
#endif

        #region Snippet:ReadMe_Embeddings
        string description = "Best hotel in town if you like luxury hotels. They have an amazing infinity pool, a spa,"
            + " and a really helpful concierge. The location is perfect -- right downtown, close to all the tourist"
            + " attractions. We highly recommend this hotel.";

        OpenAIEmbedding embedding = client.GenerateEmbedding(description);
        ReadOnlyMemory<float> vector = embedding.ToFloats();
        #endregion
    }

    [Test]
    public void Embeddings_WithDimensions()
    {
        var clientMock = new Mock<EmbeddingClient>();

        clientMock
            .Setup(c => c.GenerateEmbedding(
                It.IsAny<string>(),
                It.IsAny<EmbeddingGenerationOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(ClientResult.FromValue(
                OpenAIEmbeddingsModelFactory.OpenAIEmbedding(vector: new float[] { 0.1f, 0.2f, 0.3f }),
                new MockPipelineResponse()));

#if SNIPPET
        EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
#else
        EmbeddingClient client = clientMock.Object;
#endif

        #region Snippet:ReadMe_Embeddings_WithDimensions
        string description = "Best hotel in town if you like luxury hotels.";
        EmbeddingGenerationOptions options = new() { Dimensions = 512 };

        OpenAIEmbedding embedding = client.GenerateEmbedding(description, options);
        #endregion
    }

    [Test]
    public void Images()
    {
        var clientMock = new Mock<ImageClient>();

        clientMock
            .Setup(c => c.GenerateImage(
                It.IsAny<string>(),
                It.IsAny<ImageGenerationOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(ClientResult.FromValue(
                OpenAIImagesModelFactory.GeneratedImage(imageBytes: BinaryData.FromBytes(new byte[] { 0x89, 0x50, 0x4E, 0x47 })),
                new MockPipelineResponse()));

        #region Snippet:ReadMe_Images_CreateClient
#if SNIPPET
        ImageClient client = new("dall-e-3", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
#else
        ImageClient client = clientMock.Object;
#endif
        #endregion

        #region Snippet:ReadMe_Images_Options
        string prompt = "The concept for a living room that blends Scandinavian simplicity with Japanese minimalism for"
            + " a serene and cozy atmosphere. It's a space that invites relaxation and mindfulness, with natural light"
            + " and fresh air. Using neutral tones, including colors like white, beige, gray, and black, that create a"
            + " sense of harmony. Featuring sleek wood furniture with clean lines and subtle curves to add warmth and"
            + " elegance. Plants and flowers in ceramic pots adding color and life to a space. They can serve as focal"
            + " points, creating a connection with nature. Soft textiles and cushions in organic fabrics adding comfort"
            + " and softness to a space. They can serve as accents, adding contrast and texture.";

        ImageGenerationOptions options = new()
        {
            Quality = GeneratedImageQuality.High,
            Size = GeneratedImageSize.W1792xH1024,
            Style = GeneratedImageStyle.Vivid,
            ResponseFormat = GeneratedImageFormat.Bytes
        };
        #endregion

        #region Snippet:ReadMe_Images_Generate
        GeneratedImage image = client.GenerateImage(prompt, options);
        BinaryData bytes = image.ImageBytes;
        #endregion

        #region Snippet:ReadMe_Images_Save
        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);
        #endregion
    }

    [Test]
    public void Audio_Transcribe()
    {
        var clientMock = new Mock<AudioClient>();

        clientMock
            .Setup(c => c.TranscribeAudio(
                It.IsAny<string>(),
                It.IsAny<AudioTranscriptionOptions>()))
            .Returns(ClientResult.FromValue(
                OpenAIAudioModelFactory.AudioTranscription(
                    text: "Sample transcription text.",
                    words: new[] { OpenAIAudioModelFactory.TranscribedWord("Sample", TimeSpan.Zero, TimeSpan.FromMilliseconds(500)) },
                    segments: new[] { OpenAIAudioModelFactory.TranscribedSegment(0, 0, TimeSpan.Zero, TimeSpan.FromSeconds(5), "Sample transcription text.") }),
                new MockPipelineResponse()));

        #region Snippet:ReadMe_Audio_Transcribe
        #if SNIPPET
        AudioClient client = new("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
#else
        AudioClient client = clientMock.Object;
#endif
        string audioFilePath = Path.Combine("Assets", "audio_houseplant_care.mp3");

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Verbose,
            TimestampGranularities = AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment,
        };

        AudioTranscription transcription = client.TranscribeAudio(audioFilePath, options);

        Console.WriteLine("Transcription:");
        Console.WriteLine($"{transcription.Text}");
        Console.WriteLine();
        Console.WriteLine($"Words:");

        foreach (TranscribedWord word in transcription.Words)
        {
            Console.WriteLine($"  {word.Word,15} : {word.StartTime.TotalMilliseconds,5:0} - {word.EndTime.TotalMilliseconds,5:0}");
        }

        Console.WriteLine();
        Console.WriteLine($"Segments:");
        foreach (TranscribedSegment segment in transcription.Segments)
        {
            Console.WriteLine($"  {segment.Text,90} : {segment.StartTime.TotalMilliseconds,5:0} - {segment.EndTime.TotalMilliseconds,5:0}");
        }
        #endregion
    }

    [Test]
    public void Assistants_RAG()
    {
        // Setup mocks
        var threadMessageCollectionMock = new Mock<CollectionResult<ThreadMessage>>();
        var fileClientMock = new Mock<OpenAIFileClient>();
        var assistantClientMock = new Mock<AssistantClient>();

        threadMessageCollectionMock
            .Protected()
            .Setup<IEnumerable<ThreadMessage>>("GetValuesFromPage", ItExpr.IsAny<ClientResult>())
            .Returns(Enumerable.Empty<ThreadMessage>());

        fileClientMock
            .Setup(c => c.UploadFile(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<FileUploadPurpose>(),
                It.IsAny<CancellationToken>()))
            .Returns(ClientResult.FromValue(
                OpenAIFilesModelFactory.OpenAIFileInfo(id: "file-123", filename: "monthly_sales.json"),
                new MockPipelineResponse()));

         assistantClientMock
             .Setup(c => c.CreateAssistant(
                 It.IsAny<string>(),
                 It.IsAny<AssistantCreationOptions>(),
                 It.IsAny<CancellationToken>()))
             .Returns(ClientResult.FromValue(
                 CreateAssistantMock("asst-123"),
                 new MockPipelineResponse()));

         assistantClientMock
             .Setup(c => c.CreateThreadAndRun(
                 It.IsAny<string>(),
                 It.IsAny<ThreadCreationOptions>(),
                 It.IsAny<RunCreationOptions>(),
                 It.IsAny<CancellationToken>()))
             .Returns(ClientResult.FromValue(
                 CreateThreadRunMock("123", "asst-123"),
                 new MockPipelineResponse()));

         assistantClientMock
             .Setup(c => c.GetMessages(
                 It.IsAny<string>(),
                 It.IsAny<MessageCollectionOptions>(),
                 It.IsAny<CancellationToken>()))
             .Returns(threadMessageCollectionMock.Object);

         assistantClientMock
             .Setup(c => c.GetRun(
                 It.IsAny<string>(),
                 It.IsAny<string>(),
                 It.IsAny<CancellationToken>()))
             .Returns(ClientResult.FromValue(
                 CreateThreadRunMock("123", "asst-123", RunStatus.Completed),
                 new MockPipelineResponse()));

        #region Snippet:ReadMe_Assistants_CreateClients
#if SNIPPET
        OpenAIClient openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
        AssistantClient assistantClient = openAIClient.GetAssistantClient();
#else
        OpenAIFileClient fileClient = fileClientMock.Object;
        AssistantClient assistantClient = assistantClientMock.Object;
#endif
        #endregion

        #region Snippet:ReadMe_Assistants_Document
        Stream document = BinaryData.FromBytes("""
            {
                "description": "This document contains the sale history data for Contoso products.",
                "sales": [
                    {
                        "month": "January",
                        "by_product": {
                            "113043": 15,
                            "113045": 12,
                            "113049": 2
                        }
                    },
                    {
                        "month": "February",
                        "by_product": {
                            "113045": 22
                        }
                    },
                    {
                        "month": "March",
                        "by_product": {
                            "113045": 16,
                            "113055": 5
                        }
                    }
                ]
            }
            """u8.ToArray()).ToStream();
        #endregion

        #region Snippet:ReadMe_Assistants_UploadFile
        OpenAIFile salesFile = fileClient.UploadFile(
            document,
            "monthly_sales.json",
            FileUploadPurpose.Assistants);
        #endregion

        #region Snippet:ReadMe_Assistants_CreateAssistant
        AssistantCreationOptions assistantOptions = new()
        {
            Name = "Example: Contoso sales RAG",
            Instructions =
                "You are an assistant that looks up sales data and helps visualize the information based"
                + " on user queries. When asked to generate a graph, chart, or other visualization, use"
                + " the code interpreter tool to do so.",
            Tools =
            {
                new FileSearchToolDefinition(),
                new CodeInterpreterToolDefinition(),
            },
            ToolResources = new()
            {
                FileSearch = new()
                {
                    NewVectorStores =
                    {
                        new VectorStoreCreationHelper([salesFile.Id]),
                    }
                }
            },
        };

        Assistant assistant = assistantClient.CreateAssistant("gpt-5.1", assistantOptions);
        #endregion

        #region Snippet:ReadMe_Assistants_CreateThreadAndRun
        ThreadCreationOptions threadOptions = new()
        {
            InitialMessages = { "How well did product 113045 sell in February? Graph its trend over time." }
        };

        ThreadRun threadRun = assistantClient.CreateThreadAndRun(assistant.Id, threadOptions);
        #endregion

        #region Snippet:ReadMe_Assistants_Poll
        do
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            threadRun = assistantClient.GetRun(threadRun.ThreadId, threadRun.Id);
        } while (!threadRun.Status.IsTerminal);
        #endregion

        #region Snippet:ReadMe_Assistants_GetMessages
        CollectionResult<ThreadMessage> messages
            = assistantClient.GetMessages(threadRun.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending });

        foreach (ThreadMessage message in messages)
        {
            Console.Write($"[{message.Role.ToString().ToUpper()}]: ");
            foreach (MessageContent contentItem in message.Content)
            {
                if (!string.IsNullOrEmpty(contentItem.Text))
                {
                    Console.WriteLine($"{contentItem.Text}");

                    if (contentItem.TextAnnotations.Count > 0)
                    {
                        Console.WriteLine();
                    }

                    // Include annotations, if any.
                    foreach (TextAnnotation annotation in contentItem.TextAnnotations)
                    {
                        if (!string.IsNullOrEmpty(annotation.InputFileId))
                        {
                            Console.WriteLine($"* File citation, file ID: {annotation.InputFileId}");
                        }
                        if (!string.IsNullOrEmpty(annotation.OutputFileId))
                        {
                            Console.WriteLine($"* File output, new file ID: {annotation.OutputFileId}");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(contentItem.ImageFileId))
                {
                    OpenAIFile imageInfo = fileClient.GetFile(contentItem.ImageFileId);
                    BinaryData imageBytes = fileClient.DownloadFile(contentItem.ImageFileId);
                    using FileStream stream = File.OpenWrite($"{imageInfo.Filename}.png");
                    imageBytes.ToStream().CopyTo(stream);

                    Console.WriteLine($"<image: {imageInfo.Filename}.png>");
                }
            }
            Console.WriteLine();
        }
        #endregion
    }

    [Test]
    public void Assistants_Vision()
    {
        // Setup mocks
        var streamingUpdatesMock = new Mock<CollectionResult<StreamingUpdate>>();
        var fileClientMock = new Mock<OpenAIFileClient>();
        var assistantClientMock = new Mock<AssistantClient>();

        streamingUpdatesMock
            .Protected()
            .Setup<IEnumerable<StreamingUpdate>>("GetValuesFromPage", ItExpr.IsAny<ClientResult>())
            .Returns(Enumerable.Empty<StreamingUpdate>());

        fileClientMock
            .Setup(c => c.UploadFile(
                It.IsAny<string>(),
                It.IsAny<FileUploadPurpose>()))
            .Returns(ClientResult.FromValue(
                OpenAIFilesModelFactory.OpenAIFileInfo(id: "file-apple-123", filename: "images_apple.png"),
                new MockPipelineResponse()));

        assistantClientMock
            .Setup(c => c.CreateAssistant(
                It.IsAny<string>(),
                It.IsAny<AssistantCreationOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(ClientResult.FromValue(
                CreateAssistantMock("asst-vision-123"),
                new MockPipelineResponse()));

        assistantClientMock
            .Setup(c => c.CreateThread(
                It.IsAny<ThreadCreationOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(ClientResult.FromValue(
                CreateAssistantThreadMock("thread-vision-123"),
                new MockPipelineResponse()));

        assistantClientMock
            .Setup(c => c.CreateRunStreaming(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<RunCreationOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(streamingUpdatesMock.Object);

        #region Snippet:ReadMe_Assistants_Vision_CreateClients
#if SNIPPET
        OpenAIClient openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
        AssistantClient assistantClient = openAIClient.GetAssistantClient();
#else
        OpenAIFileClient fileClient = fileClientMock.Object;
        AssistantClient assistantClient = assistantClientMock.Object;
#endif
        #endregion

        #region Snippet:ReadMe_Assistants_Vision_UploadImage
        OpenAIFile pictureOfAppleFile = fileClient.UploadFile(
            Path.Combine("Assets", "images_apple.png"),
            FileUploadPurpose.Vision);

        Uri linkToPictureOfOrange = new("https://raw.githubusercontent.com/openai/openai-dotnet/refs/heads/main/examples/Assets/images_orange.png");
        #endregion

        #region Snippet:ReadMe_Assistants_Vision_CreateAssistantAndThread
        Assistant assistant = assistantClient.CreateAssistant(
            "gpt-5.1",
            new AssistantCreationOptions()
            {
                Instructions = "When asked a question, attempt to answer very concisely. "
                    + "Prefer one-sentence answers whenever feasible."
            });

        AssistantThread thread = assistantClient.CreateThread(new ThreadCreationOptions()
        {
            InitialMessages =
                {
                    new ThreadInitializationMessage(
                        OpenAI.Assistants.MessageRole.User,
                        [
                            "Hello, assistant! Please compare these two images for me:",
                            MessageContent.FromImageFileId(pictureOfAppleFile.Id),
                            MessageContent.FromImageUri(linkToPictureOfOrange),
                        ]),
                }
        });
        #endregion

        #region Snippet:ReadMe_Assistants_Vision_CreateRunStreaming
        CollectionResult<StreamingUpdate> streamingUpdates = assistantClient.CreateRunStreaming(
            thread.Id,
            assistant.Id,
            new RunCreationOptions()
            {
                AdditionalInstructions = "When possible, try to sneak in puns if you're asked to compare things.",
            });
        #endregion

        #region Snippet:ReadMe_Assistants_Vision_HandleStreamingUpdates
        foreach (StreamingUpdate streamingUpdate in streamingUpdates)
        {
            if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCreated)
            {
                Console.WriteLine($"--- Run started! ---");
            }
            if (streamingUpdate is MessageContentUpdate contentUpdate)
            {
                Console.Write(contentUpdate.Text);
            }
        }
        #endregion
    }

    [Test]
    public async Task AzureOpenAI()
    {
        #region Snippet:ReadMe_AzureOpenAI

        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
            ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is required.");

        var client = new ResponsesClient(
            new BearerTokenPolicy(new DefaultAzureCredential(), "https://ai.azure.com/.default"),
            new OpenAIClientOptions { Endpoint = new Uri($"{endpoint}/openai/v1/") }
        );

#if SNIPPET
        var response = await client.CreateResponseAsync("gpt-5-mini", "Hello world!");
        Console.WriteLine(response.Value.GetOutputText());
#endif
        #endregion
    }

    [Test]
    public void ProtocolMethods()
    {
        var clientMock = new Mock<ChatClient>();
        var responseMock = new MockPipelineResponse(200).WithContent("""{"choices":[{"message":{"content":"this is a test."}}]}""");

        clientMock
            .Setup(c => c.CompleteChat(It.IsAny<BinaryContent>(), It.IsAny<RequestOptions>()))
            .Returns(ClientResult.FromResponse(responseMock));

#if SNIPPET
        ChatClient client = new("gpt-5.1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
#else
        ChatClient client = clientMock.Object;
#endif

        #region Snippet:ReadMe_ProtocolMethods
        BinaryData input = BinaryData.FromBytes("""
            {
               "model": "gpt-5.1",
               "messages": [
                   {
                       "role": "user",
                       "content": "Say 'this is a test.'"
                   }
               ]
            }
            """u8.ToArray());

        using BinaryContent content = BinaryContent.Create(input);
        ClientResult result = client.CompleteChat(content);
        BinaryData output = result.GetRawResponse().Content;

        using JsonDocument outputAsJson = JsonDocument.Parse(output.ToString());
        string message = outputAsJson.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        Console.WriteLine($"[ASSISTANT]: {message}");
        #endregion
    }

    [Test]
    public void Mocking_Test()
    {
        #region Snippet:ReadMe_Mocking_MethodUnderTest
        bool ContainsSecretWord(AudioClient client, string audioFilePath, string secretWord)
        {
            AudioTranscription transcription = client.TranscribeAudio(audioFilePath);
            return transcription.Text.Contains(secretWord);
        }
        #endregion

        #region Snippet:ReadMe_Mocking_Test
        // Instantiate mocks and the AudioTranscription object.
        Mock<AudioClient> mockClient = new();
        AudioTranscription transcription = OpenAIAudioModelFactory.AudioTranscription(text: "I swear I saw an apple flying yesterday!");

        // Set up mocks' properties and methods.
        mockClient
            .Setup(client => client.TranscribeAudio(It.IsAny<string>()))
            .Returns(ClientResult.FromValue(transcription, Mock.Of<PipelineResponse>()));

        // Perform validation.
        AudioClient client = mockClient.Object;
        bool containsSecretWord = ContainsSecretWord(client, "<audioFilePath>", "apple");

        Assert.That(containsSecretWord, Is.True);
        #endregion
    }

    private static ChatCompletion CreateChatCompletionMock() =>
        OpenAIChatModelFactory.ChatCompletion(
            id: "chatcmpl-123",
            finishReason: ChatFinishReason.Stop,
            content: new ChatMessageContent("result"));

    private static ThreadRun CreateThreadRunMock(string id, string threadId = default, RunStatus runStatus = default) =>
        ModelReaderWriter.Read<ThreadRun>(BinaryData.FromObjectAsJson(new
        {
          id,
          thread_id = threadId ?? "thread-123",
          status = (runStatus == default) ? RunStatus.Completed.ToString() : runStatus.ToString(),
        }));

    private static Assistant CreateAssistantMock(string id) =>
        ModelReaderWriter.Read<Assistant>(BinaryData.FromObjectAsJson(new
        {
          id,
        }));

    private static AssistantThread CreateAssistantThreadMock(string id) =>
        ModelReaderWriter.Read<AssistantThread>(BinaryData.FromObjectAsJson(new
        {
          id,
        }));

    #region Snippet:ReadMe_DependencyInjection_Controller
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatClient _chatClient;

        public ChatController(ChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteChat([FromBody] string message)
        {
            ChatCompletion completion = await _chatClient.CompleteChatAsync(message);
            return Ok(new { response = completion.Content[0].Text });
        }
    }
    #endregion

    public class ApplicationBuilder
    {
        public virtual IServiceCollection Services { get; }
    }
}
