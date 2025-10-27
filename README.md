# OpenAI .NET API library

[![NuGet stable version](https://img.shields.io/nuget/v/openai.svg)](https://www.nuget.org/packages/OpenAI) [![NuGet preview version](https://img.shields.io/nuget/vpre/openai.svg)](https://www.nuget.org/packages/OpenAI/absoluteLatest)

The OpenAI .NET library provides convenient access to the OpenAI REST API from .NET applications.

It is generated from our [OpenAPI specification](https://github.com/openai/openai-openapi) in collaboration with Microsoft.

## Table of Contents

- [Getting started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Install the NuGet package](#install-the-nuget-package)
- [Using the client library](#using-the-client-library)
  - [Namespace organization](#namespace-organization)
  - [Using the async API](#using-the-async-api)
  - [Using the `OpenAIClient` class](#using-the-openaiclient-class)
- [How to use dependency injection](#how-to-use-dependency-injection)
- [How to use chat completions with streaming](#how-to-use-chat-completions-with-streaming)
- [How to use chat completions with tools and function calling](#how-to-use-chat-completions-with-tools-and-function-calling)
- [How to use chat completions with structured outputs](#how-to-use-chat-completions-with-structured-outputs)
- [How to use chat completions with audio](#how-to-use-chat-completions-with-audio)
- [How to use responses with streaming and reasoning](#how-to-use-responses-with-streaming-and-reasoning)
- [How to use responses with file search](#how-to-use-responses-with-file-search)
- [How to use responses with web search](#how-to-use-responses-with-web-search)
- [How to generate text embeddings](#how-to-generate-text-embeddings)
- [How to generate images](#how-to-generate-images)
- [How to transcribe audio](#how-to-transcribe-audio)
- [How to use assistants with retrieval augmented generation (RAG)](#how-to-use-assistants-with-retrieval-augmented-generation-rag)
- [How to use assistants with streaming and vision](#how-to-use-assistants-with-streaming-and-vision)
- [How to work with Azure OpenAI](#how-to-work-with-azure-openai)
- [Advanced scenarios](#advanced-scenarios)
  - [Using protocol methods](#using-protocol-methods)
  - [Mock a client for testing](#mock-a-client-for-testing)
  - [Automatically retrying errors](#automatically-retrying-errors)
  - [Observability](#observability)

## Getting started

### Prerequisites

To call the OpenAI REST API, you will need an API key. To obtain one, first [create a new OpenAI account](https://platform.openai.com/signup) or [log in](https://platform.openai.com/login). Next, navigate to the [API key page](https://platform.openai.com/account/api-keys) and select "Create new secret key", optionally naming the key. Make sure to save your API key somewhere safe and do not share it with anyone.

### Install the NuGet package

Add the client library to your .NET project by installing the [NuGet](https://www.nuget.org/) package via your IDE or by running the following command in the .NET CLI:

```cli
dotnet add package OpenAI
```

If you would like to try the latest preview version, remember to append the `--prerelease` command option.

Note that the code examples included below were written using [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0). The OpenAI .NET library is compatible with all .NET Standard 2.0 applications, but the syntax used in some of the code examples in this document may depend on newer language features.

## Using the client library

The full API of this library can be found in the [OpenAI.netstandard2.0.cs](https://github.com/openai/openai-dotnet/blob/main/api/OpenAI.netstandard2.0.cs) file, and there are many [code examples](https://github.com/openai/openai-dotnet/tree/main/examples) to help. For instance, the following snippet illustrates the basic use of the chat completions API:

```csharp
using OpenAI.Chat;

ChatClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

ChatCompletion completion = client.CompleteChat("Say 'this is a test.'");

Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
```

While you can pass your API key directly as a string, it is highly recommended that you keep it in a secure location and instead access it via an environment variable or configuration file as shown above to avoid storing it in source control.

### Using a custom base URL and API key

If you need to connect to an alternative API endpoint (for example, a proxy or self-hosted OpenAI-compatible LLM), you can specify a custom base URL and API key using the `ApiKeyCredential` and `OpenAIClientOptions`:

```csharp
using OpenAI;
using OpenAI.Chat;

ChatClient client = new(
    model: "MODEL_NAME",
    credential: new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAI_API_KEY")),
    options: new OpenAIClientOptions() 
    { 
        Endpoint = new Uri("BASE_URL")
    }
);
```

Replace `CHAT_MODEL` with your model name and `BASE_URL` with your endpoint URI. This is useful when working with OpenAI-compatible APIs or custom deployments.

### Namespace organization

The library is organized into namespaces by feature areas in the OpenAI REST API. Each namespace contains a corresponding client class.

| Namespace                     | Client class                 |
| ------------------------------|------------------------------|
| `OpenAI.Assistants`           | `AssistantClient`            |
| `OpenAI.Audio`                | `AudioClient`                |
| `OpenAI.Batch`                | `BatchClient`                |
| `OpenAI.Chat`                 | `ChatClient`                 |
| `OpenAI.Embeddings`           | `EmbeddingClient`            |
| `OpenAI.Evals`                | `EvaluationClient`           |
| `OpenAI.FineTuning`           | `FineTuningClient`           |
| `OpenAI.Files`                | `OpenAIFileClient`           |
| `OpenAI.Images`               | `ImageClient`                |
| `OpenAI.Models`               | `OpenAIModelClient`          |
| `OpenAI.Moderations`          | `ModerationClient`           |
| `OpenAI.Realtime`             | `RealtimeClient`             |
| `OpenAI.Responses`            | `OpenAIResponseClient`       |
| `OpenAI.VectorStores`         | `VectorStoreClient`          |

### Using the async API

Every client method that performs a synchronous API call has an asynchronous variant in the same client class. For instance, the asynchronous variant of the `ChatClient`'s `CompleteChat` method is `CompleteChatAsync`. To rewrite the call above using the asynchronous counterpart, simply `await` the call to the corresponding async variant:

```csharp
ChatCompletion completion = await client.CompleteChatAsync("Say 'this is a test.'");
```

### Using the `OpenAIClient` class

In addition to the namespaces mentioned above, there is also the parent `OpenAI` namespace itself:

```csharp
using OpenAI;
```

This namespace contains the `OpenAIClient` class, which offers certain conveniences when you need to work with multiple feature area clients. Specifically, you can use an instance of this class to create instances of the other clients and have them share the same implementation details, which might be more efficient.

You can create an `OpenAIClient` by specifying the API key that all clients will use for authentication:

```csharp
OpenAIClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
```

Next, to create an instance of an `AudioClient`, for example, you can call the `OpenAIClient`'s `GetAudioClient` method by passing the OpenAI model that the `AudioClient` will use, just as if you were using the `AudioClient` constructor directly. If necessary, you can create additional clients of the same type to target different models.

```csharp
AudioClient ttsClient = client.GetAudioClient("tts-1");
AudioClient whisperClient = client.GetAudioClient("whisper-1");
```

## How to use dependency injection

The OpenAI clients are **thread-safe** and can be safely registered as **singletons** in ASP.NET Core's Dependency Injection container. This maximizes resource efficiency and HTTP connection reuse.

Register the `ChatClient` as a singleton in your `Program.cs`:

```csharp
builder.Services.AddSingleton<ChatClient>(serviceProvider =>
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    var model = "gpt-4o";

    return new ChatClient(model, apiKey);
});
```

Then inject and use the client in your controllers or services:

```csharp
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
```

## How to use chat completions with streaming

When you request a chat completion, the default behavior is for the server to generate it in its entirety before sending it back in a single response. Consequently, long chat completions can require waiting for several seconds before hearing back from the server. To mitigate this, the OpenAI REST API supports the ability to stream partial results back as they are being generated, allowing you to start processing the beginning of the completion before it is finished.

The client library offers a convenient approach to working with streaming chat completions. If you wanted to re-write the example from the previous section using streaming, rather than calling the `ChatClient`'s `CompleteChat` method, you would call its `CompleteChatStreaming` method instead:

```csharp
CollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreaming("Say 'this is a test.'");
```

Notice that the returned value is a `CollectionResult<StreamingChatCompletionUpdate>` instance, which can be enumerated to process the streaming response chunks as they arrive:

```csharp
Console.Write($"[ASSISTANT]: ");
foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
{
    if (completionUpdate.ContentUpdate.Count > 0)
    {
        Console.Write(completionUpdate.ContentUpdate[0].Text);
    }
}
```

Alternatively, you can do this asynchronously by calling the `CompleteChatStreamingAsync` method to get an `AsyncCollectionResult<StreamingChatCompletionUpdate>` and enumerate it using `await foreach`:

```csharp
AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreamingAsync("Say 'this is a test.'");

Console.Write($"[ASSISTANT]: ");
await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
{
    if (completionUpdate.ContentUpdate.Count > 0)
    {
        Console.Write(completionUpdate.ContentUpdate[0].Text);
    }
}
```

## How to use chat completions with tools and function calling

In this example, you have two functions. The first function can retrieve a user's current geographic location (e.g., by polling the location service APIs of the user's device), while the second function can query the weather in a given location (e.g., by making an API call to some third-party weather service). You want the model to be able to call these functions if it deems it necessary to have this information in order to respond to a user request as part of generating a chat completion. For illustrative purposes, consider the following:

```csharp
private static string GetCurrentLocation()
{
    // Call the location API here.
    return "San Francisco";
}

private static string GetCurrentWeather(string location, string unit = "celsius")
{
    // Call the weather API here.
    return $"31 {unit}";
}
```

Start by creating two `ChatTool` instances using the static `CreateFunctionTool` method to describe each function:

```csharp
private static readonly ChatTool getCurrentLocationTool = ChatTool.CreateFunctionTool(
    functionName: nameof(GetCurrentLocation),
    functionDescription: "Get the user's current location"
);

private static readonly ChatTool getCurrentWeatherTool = ChatTool.CreateFunctionTool(
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
```

Next, create a `ChatCompletionOptions` instance and add both to its `Tools` property. You will pass the `ChatCompletionOptions` as an argument in your calls to the `ChatClient`'s `CompleteChat` method.

```csharp
List<ChatMessage> messages = 
[
    new UserChatMessage("What's the weather like today?"),
];

ChatCompletionOptions options = new()
{
    Tools = { getCurrentLocationTool, getCurrentWeatherTool },
};
```

When the resulting `ChatCompletion` has a `FinishReason` property equal to `ChatFinishReason.ToolCalls`, it means that the model has determined that one or more tools must be called before the assistant can respond appropriately. In those cases, you must first call the function specified in the `ChatCompletion`'s `ToolCalls` and then call the `ChatClient`'s `CompleteChat` method again while passing the function's result as an additional `ChatRequestToolMessage`. Repeat this process as needed.

```csharp
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
```

## How to use chat completions with structured outputs

Beginning with the `gpt-4o-mini`, `gpt-4o-mini-2024-07-18`, and `gpt-4o-2024-08-06` model snapshots, structured outputs are available for both top-level response content and tool calls in the chat completion and assistants APIs. For information about the feature, see [the Structured Outputs guide](https://platform.openai.com/docs/guides/structured-outputs/introduction).

To use structured outputs to constrain chat completion content, set an appropriate `ChatResponseFormat` as in the following example:

```csharp
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
```

## How to use chat completions with audio

Starting with the `gpt-4o-audio-preview` model, chat completions can process audio input and output.

This example demonstrates:
  1. Configuring the client with the supported `gpt-4o-audio-preview` model
  1. Supplying user audio input on a chat completion request
  1. Requesting model audio output from the chat completion operation
  1. Retrieving audio output from a `ChatCompletion` instance
  1. Using past audio output as `ChatMessage` conversation history

```csharp
// Chat audio input and output is only supported on specific models, beginning with gpt-4o-audio-preview
ChatClient client = new("gpt-4o-audio-preview", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

// Input audio is provided to a request by adding an audio content part to a user message
string audioFilePath = Path.Combine("Assets", "realtime_whats_the_weather_pcm16_24khz_mono.wav");
byte[] audioFileRawBytes = File.ReadAllBytes(audioFilePath);
BinaryData audioData = BinaryData.FromBytes(audioFileRawBytes);
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
        using (FileStream outputFileStream = File.OpenWrite(outputFilePath))
        {
            outputFileStream.Write(outputAudio.AudioBytes);
        }
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
```

Streaming is highly parallel: `StreamingChatCompletionUpdate` instances can include a `OutputAudioUpdate` that may
contain any of:

- The `Id` of the streamed audio content, which can be referenced by subsequent `AssistantChatMessage` instances via `ChatAudioReference` once the streaming response is complete; this may appear across multiple `StreamingChatCompletionUpdate` instances but will always be the same value when present
- The `ExpiresAt` value that describes when the `Id` will no longer be valid for use with `ChatAudioReference` in subsequent requests; this typically appears once and only once, in the final `StreamingOutputAudioUpdate`
- Incremental `TranscriptUpdate` and/or `AudioBytesUpdate` values, which can incrementally consumed and, when concatenated, form the complete audio transcript and audio output for the overall response; many of these typically appear

## How to use responses with streaming and reasoning

```csharp
OpenAIResponseClient client = new(
    model: "o3-mini",
    apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

OpenAIResponse response = await client.CreateResponseAsync(
    userInputText: "What's the optimal strategy to win at poker?",
    new ResponseCreationOptions()
    {
        ReasoningOptions = new ResponseReasoningOptions()
        {
            ReasoningEffortLevel = ResponseReasoningEffortLevel.High,
        },
    });

await foreach (StreamingResponseUpdate update
    in client.CreateResponseStreamingAsync(
        userInputText: "What's the optimal strategy to win at poker?",
        new ResponseCreationOptions()
        {
            ReasoningOptions = new ResponseReasoningOptions()
            {
                ReasoningEffortLevel = ResponseReasoningEffortLevel.High,
            },
        }))
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
```

## How to use responses with file search

```csharp
OpenAIResponseClient client = new(
    model: "gpt-4o-mini",
    apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

ResponseTool fileSearchTool
    = ResponseTool.CreateFileSearchTool(
        vectorStoreIds: [ExistingVectorStoreForTest.Id]);
OpenAIResponse response = await client.CreateResponseAsync(
    userInputText: "According to available files, what's the secret number?",
    new ResponseCreationOptions()
    {
        Tools = { fileSearchTool }
    });

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
```

## How to use responses with web search

```csharp
OpenAIResponseClient client = new(
    model: "gpt-4o-mini",
    apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

OpenAIResponse response = await client.CreateResponseAsync(
    userInputText: "What's a happy news headline from today?",
    new ResponseCreationOptions()
    {
        Tools = { ResponseTool.CreateWebSearchTool() },
    });

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
```

## How to generate text embeddings

In this example, you want to create a trip-planning website that allows customers to write a prompt describing the kind of hotel that they are looking for and then offers hotel recommendations that closely match this description. To achieve this, it is possible to use text embeddings to measure the relatedness of text strings. In summary, you can get embeddings of the hotel descriptions, store them in a vector database, and use them to build a search index that you can query using the embedding of a given customer's prompt.

To generate a text embedding, use `EmbeddingClient` from the `OpenAI.Embeddings` namespace:

```csharp
using OpenAI.Embeddings;

EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

string description = "Best hotel in town if you like luxury hotels. They have an amazing infinity pool, a spa,"
    + " and a really helpful concierge. The location is perfect -- right downtown, close to all the tourist"
    + " attractions. We highly recommend this hotel.";

OpenAIEmbedding embedding = client.GenerateEmbedding(description);
ReadOnlyMemory<float> vector = embedding.ToFloats();
```

Notice that the resulting embedding is a list (also called a vector) of floating point numbers represented as an instance of `ReadOnlyMemory<float>`. By default, the length of the embedding vector will be 1536 when using the `text-embedding-3-small` model or 3072 when using the `text-embedding-3-large` model. Generally, larger embeddings perform better, but using them also tends to cost more in terms of compute, memory, and storage. You can reduce the dimensions of the embedding by creating an instance of the `EmbeddingGenerationOptions` class, setting the `Dimensions` property, and passing it as an argument in your call to the `GenerateEmbedding` method:

```csharp
EmbeddingGenerationOptions options = new() { Dimensions = 512 };

OpenAIEmbedding embedding = client.GenerateEmbedding(description, options);
```

## How to generate images

In this example, you want to build an app to help interior designers prototype new ideas based on the latest design trends. As part of the creative process, an interior designer can use this app to generate images for inspiration simply by describing the scene in their head as a prompt. As expected, high-quality, strikingly dramatic images with finer details deliver the best results for this application.

To generate an image, use `ImageClient` from the `OpenAI.Images` namespace:

```csharp
using OpenAI.Images;

ImageClient client = new("dall-e-3", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
```

Generating an image always requires a `prompt` that describes what should be generated. To further tailor the image generation to your specific needs, you can create an instance of the `ImageGenerationOptions` class and set the `Quality`, `Size`, and `Style` properties accordingly. Note that you can also set the `ResponseFormat` property of `ImageGenerationOptions` to `GeneratedImageFormat.Bytes` in order to receive the resulting PNG as `BinaryData` (instead of the default remote `Uri`) if this is convenient for your use case.

```csharp
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
```

Finally, call the `ImageClient`'s `GenerateImage` method by passing the prompt and the `ImageGenerationOptions` instance as arguments:

```csharp
GeneratedImage image = client.GenerateImage(prompt, options);
BinaryData bytes = image.ImageBytes;
```

For illustrative purposes, you could then save the generated image to local storage:

```csharp
using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
bytes.ToStream().CopyTo(stream);
```

## How to transcribe audio

In this example, an audio file is transcribed using the Whisper speech-to-text model, including both word- and audio-segment-level timestamp information.

```csharp
using OpenAI.Audio;

AudioClient client = new("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

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
```

## How to use assistants with retrieval augmented generation (RAG)

In this example, you have a JSON document with the monthly sales information of different products, and you want to build an assistant capable of analyzing it and answering questions about it.

To achieve this, use both `OpenAIFileClient` from the `OpenAI.Files` namespace and `AssistantClient` from the `OpenAI.Assistants` namespace.

Important: The Assistants REST API is currently in beta. As such, the details are subject to change, and correspondingly the `AssistantClient` is attributed as `[Experimental]`. To use it, you must suppress the `OPENAI001` warning first.

```csharp
using OpenAI.Assistants;
using OpenAI.Files;

OpenAIClient openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
AssistantClient assistantClient = openAIClient.GetAssistantClient();
```

Here is an example of what the JSON document might look like:

```csharp
using Stream document = BinaryData.FromBytes("""
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
```

Upload this document to OpenAI using the `OpenAIFileClient`'s `UploadFile` method, ensuring that you use `FileUploadPurpose.Assistants` to allow your assistant to access it later:

```csharp
OpenAIFile salesFile = fileClient.UploadFile(
    document,
    "monthly_sales.json",
    FileUploadPurpose.Assistants);
```

Create a new assistant using an instance of the `AssistantCreationOptions` class to customize it. Here, we use:

- A friendly `Name` for the assistant, as will display in the Playground
- Tool definition instances for the tools that the assistant should have access to; here, we use `FileSearchToolDefinition` to process the sales document we just uploaded and `CodeInterpreterToolDefinition` so we can analyze and visualize the numeric data
- Resources for the assistant to use with its tools, here using the `VectorStoreCreationHelper` type to automatically make a new vector store that indexes the sales file; alternatively, you could use `VectorStoreClient` to manage the vector store separately

```csharp
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

Assistant assistant = assistantClient.CreateAssistant("gpt-4o", assistantOptions);
```

Next, create a new thread. For illustrative purposes, you could include an initial user message asking about the sales information of a given product and then use the `AssistantClient`'s `CreateThreadAndRun` method to get it started:

```csharp
ThreadCreationOptions threadOptions = new()
{
    InitialMessages = { "How well did product 113045 sell in February? Graph its trend over time." }
};

ThreadRun threadRun = assistantClient.CreateThreadAndRun(assistant.Id, threadOptions);
```

Poll the status of the run until it is no longer queued or in progress:

```csharp
do
{
    Thread.Sleep(TimeSpan.FromSeconds(1));
    threadRun = assistantClient.GetRun(threadRun.ThreadId, threadRun.Id);
} while (!threadRun.Status.IsTerminal);
```

If everything went well, the terminal status of the run will be `RunStatus.Completed`.

Finally, you can use the `AssistantClient`'s `GetMessages` method to retrieve the messages associated with this thread, which now include the responses from the assistant to the initial user message.

For illustrative purposes, you could print the messages to the console and also save any images produced by the assistant to local storage:

```csharp
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
```

And it would yield something like this:

```text
[USER]: How well did product 113045 sell in February? Graph its trend over time.

[ASSISTANT]: Product 113045 sold 22 units in February【4:0†monthly_sales.json】.

Now, I will generate a graph to show its sales trend over time.

* File citation, file ID: file-hGOiwGNftMgOsjbynBpMCPFn

[ASSISTANT]: <image: 015d8e43-17fe-47de-af40-280f25452280.png>
The sales trend for Product 113045 over the past three months shows that:

- In January, 12 units were sold.
- In February, 22 units were sold, indicating significant growth.
- In March, sales dropped slightly to 16 units.

The graph above visualizes this trend, showing a peak in sales during February.
```

## How to use assistants with streaming and vision

This example shows how to use the v2 Assistants API to provide image data to an assistant and then stream the run's response.

As before, you will use a `OpenAIFileClient` and an `AssistantClient`:

```csharp
OpenAIClient openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
AssistantClient assistantClient = openAIClient.GetAssistantClient();
```

For this example, we will use both image data from a local file as well as an image located at a URL. For the local data, we upload the file with the `Vision` upload purpose, which would also allow it to be downloaded and retrieved later.

```csharp
OpenAIFile pictureOfAppleFile = fileClient.UploadFile(
    Path.Combine("Assets", "images_apple.png"),
    FileUploadPurpose.Vision);

Uri linkToPictureOfOrange = new("https://raw.githubusercontent.com/openai/openai-dotnet/refs/heads/main/examples/Assets/images_orange.png");
```

Next, create a new assistant with a vision-capable model like `gpt-4o` and a thread with the image information referenced:

```csharp
Assistant assistant = assistantClient.CreateAssistant(
    "gpt-4o",
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
                MessageRole.User,
                [
                    "Hello, assistant! Please compare these two images for me:",
                    MessageContent.FromImageFileId(pictureOfAppleFile.Id),
                    MessageContent.FromImageUri(linkToPictureOfOrange),
                ]),
        }
});
```

With the assistant and thread prepared, use the `CreateRunStreaming` method to get an enumerable `CollectionResult<StreamingUpdate>`. You can then iterate over this collection with `foreach`. For async calling patterns, use `CreateRunStreamingAsync` and iterate over the `AsyncCollectionResult<StreamingUpdate>` with `await foreach`, instead. Note that streaming variants also exist for `CreateThreadAndRunStreaming` and `SubmitToolOutputsToRunStreaming`.

```csharp
CollectionResult<StreamingUpdate> streamingUpdates = assistantClient.CreateRunStreaming(
    thread.Id,
    assistant.Id,
    new RunCreationOptions()
    {
        AdditionalInstructions = "When possible, try to sneak in puns if you're asked to compare things.",
    });
```

Finally, to handle the `StreamingUpdates` as they arrive, you can use the `UpdateKind` property on the base `StreamingUpdate` and/or downcast to a specifically desired update type, like `MessageContentUpdate` for `thread.message.delta` events or `RequiredActionUpdate` for streaming tool calls.

```csharp
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
```

This will yield streamed output from the run like the following:

```text
--- Run started! ---
The first image depicts a multicolored apple with a blend of red and green hues, while the second image shows an orange with a bright, textured orange peel; one might say it’s comparing apples to oranges!
```

## How to work with Azure OpenAI

For Azure OpenAI scenarios use the [Azure SDK](https://github.com/Azure/azure-sdk-for-net) and more specifically the [Azure OpenAI client library for .NET](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/openai/Azure.AI.OpenAI/README.md). 

The Azure OpenAI client library for .NET is a companion to this library and all common capabilities between OpenAI and Azure OpenAI share the same scenario clients, methods, and request/response types. It is designed to make Azure specific scenarios straightforward, with extensions for Azure-specific concepts like Responsible AI content filter results and On Your Data integration.

```c#
AzureOpenAIClient azureClient = new(
    new Uri("https://your-azure-openai-resource.com"),
    new DefaultAzureCredential());
ChatClient chatClient = azureClient.GetChatClient("my-gpt-35-turbo-deployment");

ChatCompletion completion = chatClient.CompleteChat(
    [
        // System messages represent instructions or other guidance about how the assistant should behave
        new SystemChatMessage("You are a helpful assistant that talks like a pirate."),
        // User messages represent user input, whether historical or the most recen tinput
        new UserChatMessage("Hi, can you help me?"),
        // Assistant messages in a request represent conversation history for responses
        new AssistantChatMessage("Arrr! Of course, me hearty! What can I do for ye?"),
        new UserChatMessage("What's the best way to train a parrot?"),
    ]);

Console.WriteLine($"{completion.Role}: {completion.Content[0].Text}");
```

## Advanced scenarios

### Using protocol methods

In addition to the client methods that use strongly-typed request and response objects, the .NET library also provides _protocol methods_ that enable more direct access to the REST API. Protocol methods are "binary in, binary out" accepting `BinaryContent` as request bodies and providing `BinaryData` as response bodies.

For example, to use the protocol method variant of the `ChatClient`'s `CompleteChat` method, pass the request body as `BinaryContent`:

```csharp
ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

BinaryData input = BinaryData.FromBytes("""
    {
       "model": "gpt-4o",
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
    .GetProperty("choices"u8)[0]
    .GetProperty("message"u8)
    .GetProperty("content"u8)
    .GetString();

Console.WriteLine($"[ASSISTANT]: {message}");
```

Notice how you can then call the resulting `ClientResult`'s `GetRawResponse` method and retrieve the response body as `BinaryData` via the `PipelineResponse`'s `Content` property.

### Mock a client for testing

The OpenAI .NET library has been designed to support mocking, providing key features such as:

- Client methods made virtual to allow overriding.
- Model factories to assist in instantiating API output models that lack public constructors.

To illustrate how mocking works, suppose you want to validate the behavior of the following method using the [Moq](https://github.com/devlooped/moq) library. Given the path to an audio file, it determines whether it contains a specified secret word:

```csharp
public bool ContainsSecretWord(AudioClient client, string audioFilePath, string secretWord)
{
    AudioTranscription transcription = client.TranscribeAudio(audioFilePath);
    return transcription.Text.Contains(secretWord);
}
```

Create mocks of `AudioClient` and `ClientResult<AudioTranscription>`, set up methods and properties that will be invoked, then test the behavior of the `ContainsSecretWord` method. Since the `AudioTranscription` class does not provide public constructors, it must be instantiated by the `OpenAIAudioModelFactory` static class:

```csharp
// Instantiate mocks and the AudioTranscription object.

Mock<AudioClient> mockClient = new();
Mock<ClientResult<AudioTranscription>> mockResult = new(null, Mock.Of<PipelineResponse>());
AudioTranscription transcription = OpenAIAudioModelFactory.AudioTranscription(text: "I swear I saw an apple flying yesterday!");

// Set up mocks' properties and methods.

mockResult
    .SetupGet(result => result.Value)
    .Returns(transcription);

mockClient.Setup(client => client.TranscribeAudio(
        It.IsAny<string>(),
        It.IsAny<AudioTranscriptionOptions>()))
    .Returns(mockResult.Object);

// Perform validation.

AudioClient client = mockClient.Object;
bool containsSecretWord = ContainsSecretWord(client, "<audioFilePath>", "apple");

Assert.That(containsSecretWord, Is.True);
```

All namespaces have their corresponding model factory to support mocking with the exception of the `OpenAI.Assistants` and `OpenAI.VectorStores` namespaces, for which model factories are coming soon.

### Automatically retrying errors

By default, the client classes will automatically retry the following errors up to three additional times using exponential backoff:

- 408 Request Timeout
- 429 Too Many Requests
- 500 Internal Server Error
- 502 Bad Gateway
- 503 Service Unavailable
- 504 Gateway Timeout

### Observability

OpenAI .NET library supports experimental distributed tracing and metrics with OpenTelemetry. Check out [Observability with OpenTelemetry](./docs/Observability.md) for more details.
