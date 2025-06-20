# Guide for migrating to OpenAI 2.0.0-beta.1 or higher from OpenAI 1.11.0

This guide is intended to assist in the migration to the official OpenAI library (2.0.0-beta.1 or higher) from [OpenAI 1.11.0][openai_1110], focusing on side-by-side comparisons for similar operations between libraries. Version 2.0.0-beta.1 will be used for comparison with 1.11.0 but this guide can still be safely used when migrating to higher versions.

Prior to 2.0.0-beta.1, the OpenAI package was a community library not officially supported by OpenAI. See the [CHANGELOG][changelog] for more details.

Familiarity with the OpenAI 1.11.0 package is assumed. For those new to any OpenAI library for .NET, see the [README][readme] rather than this guide.

## Table of contents
- [Client usage](#client-usage)
- [Authentication](#authentication)
- [Highlighted scenarios](#highlighted-scenarios)
    - [Chat Completions: Text generation](#chat-completions-text-generation)
    - [Chat Completions: Streaming](#chat-completions-streaming)
    - [Chat Completions: JSON mode](#chat-completions-json-mode)
    - [Chat Completions: Vision](#chat-completions-vision)
    - [Audio: Speech-to-text](#audio-speech-to-text)
    - [Audio: Text-to-speech](#audio-text-to-speech)
    - [Image: Image generation](#image-image-generation)
- [Additional examples](#additional-examples)

## Client usage

The client usage has considerably changed between libraries. While the OpenAI 1.11.0 had a single client, `OpenAIAPI`, from which multiple APIs could be accessed, OpenAI 2.0.0-beta.1 keeps a separate client per API. The following snippets illustrate this difference when invoking the image generation capability from the Image API:

OpenAI 1.11.0:
```cs
OpenAIAPI api = new OpenAIAPI("<api-key>");
ImageResult result = await api.ImageGenerations.CreateImageAsync("Draw a quick brown fox jumping over a lazy dog.", Model.DALLE3);
```

OpenAI 2.0.0-beta.1:
```cs
ImageClient client = new ImageClient("dall-e-3", "<api-key>");
ClientResult<GeneratedImage> result = await client.GenerateImageAsync("Draw a quick brown fox jumping over a lazy dog.");
```

Another major difference highlighted in the snippets above is that OpenAI 2.0.0-beta.1 requires the model to be explicitly set during client instantiation, while the `OpenAIAPI` client allows a model to be specified per call.

The table below illustrates to which client each endpoint of `OpenAIAPI` was ported. Note that the deprecated Completions API is not supported in 2.0.0-beta.1:

Old library's endpoint|New library's client
|-|-
|Chat | ChatClient
|ImageGenerations | ImageClient
|TextToSpeech | AudioClient
|Transcriptions | AudioClient
|Translations | AudioClient
|Moderation | ModerationClient
|Embeddings | EmbeddingClient
|Files | OpenAIFileClient
|Models | OpenAIModelClient
|Completions | Not supported

## Authentication

To authenticate to OpenAI, you must set an API key when creating a client.

OpenAI 1.11.0 allowed setting the API key in 3 different ways:
- Directly from a string
- From an environment variable
- From a configuration file

```cs
OpenAIAPI api;

// Sets the API key directly from a string.
api = new OpenAIAPI("<api-key>");

// Attempts to load the API key from environment variables OPENAI_KEY and OPENAI_API_KEY.
api = new OpenAIAPI(APIAuthentication.LoadFromEnv());

// Attempts to load the API key from a configuration file.
api = new OpenAIAPI(APIAuthentication.LoadFromPath("<directory>", "<filename>"));
```

OpenAI 2.0.0-beta.1 only supports setting it from a string or from an environment variable. The following snippet illustrates the behavior with the `ChatClient`, but other clients behave the same:

```cs
ChatClient client;

// Sets the API key directly from a string.
client = new ChatClient("gpt-3.5-turbo", "<api-key>");

// When no API key string is specified, attempts to load the API key from the environment variable OPENAI_API_KEY.
client = new ChatClient("gpt-3.5-turbo");
```

Note that, unlike the OpenAI 1.11.0, OpenAI 2.0.0-beta.1 will never attempt to load the API key from the `OPENAI_KEY` environment variable. Only `OPENAI_API_KEY` is supported.

## Highlighted scenarios

The following sections illustrate side-by-side comparisons for similar operations between the two libraries, highlighting common scenarios.

### Chat Completions: Text generation

OpenAI 1.11.0:
```cs
OpenAIAPI api = new OpenAIAPI("<api-key>");
Conversation conversation = api.Chat.CreateConversation();

conversation.Model = Model.ChatGPTTurbo;
conversation.AppendSystemMessage("You are a helpful assistant.");
conversation.AppendUserInput("When was the Nobel Prize founded?");

await conversation.GetResponseFromChatbotAsync();

conversation.AppendUserInput("Who was the first person to be awarded one?");

await conversation.GetResponseFromChatbotAsync();

foreach (ChatMessage message in conversation.Messages)
{
    Console.WriteLine($"{message.Role}: {message.TextContent}");
}
```

OpenAI 2.0.0-beta.1:
```cs
ChatClient client = new ChatClient("gpt-3.5-turbo", "<api-key>");
List<ChatMessage> messages = new List<ChatMessage>()
{
    new SystemChatMessage("You are a helpful assistant."),
    new UserChatMessage("When was the Nobel Prize founded?")
};

ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);

messages.Add(new AssistantChatMessage(result));
messages.Add(new UserChatMessage("Who was the first person to be awarded one?"));

result = await client.CompleteChatAsync(messages);

messages.Add(new AssistantChatMessage(result));

foreach (ChatMessage message in messages)
{
    string role = message.GetType().Name;
    string text = message.Content[0].Text;

    Console.WriteLine($"{role}: {text}");
}
```

### Chat Completions: Streaming

OpenAI 1.11.0:
```cs
OpenAIAPI api = new OpenAIAPI("<api-key>");
Conversation conversation = api.Chat.CreateConversation();

conversation.Model = Model.ChatGPTTurbo;
conversation.AppendUserInput("Give me a list of Nobel Prize winners of the last 5 years.");

await foreach (string response in conversation.StreamResponseEnumerableFromChatbotAsync())
{
    Console.Write(response);
}
```

OpenAI 2.0.0-beta.1:
```cs
ChatClient client = new ChatClient("gpt-3.5-turbo", "<api-key>");
List<ChatMessage> messages = new List<ChatMessage>()
{
    new UserChatMessage("Give me a list of Nobel Prize winners of the last 5 years.")
};

await foreach (StreamingChatCompletionUpdate chatUpdate in client.CompleteChatStreamingAsync(messages))
{
    if (chatUpdate.ContentUpdate.Count > 0)
    {
        Console.Write(chatUpdate.ContentUpdate[0].Text);
    }
}
```

### Chat Completions: JSON mode

OpenAI 1.11.0:
```cs
OpenAIAPI api = new OpenAIAPI("<api-key>");
ChatRequest request = new ChatRequest()
{
    Model = Model.ChatGPTTurbo,
    ResponseFormat = request.ResponseFormats.JsonObject,
    Messages = new List<ChatMessage>()
    {
        new ChatMessage(ChatMessageRole.System, "You are a helpful assistant designed to output JSON."),
        new ChatMessage(ChatMessageRole.User, "Give me a JSON object listing Nobel Prize winners of the last 5 years.")
    }
};

ChatResult result = await api.Chat.CreateChatCompletionAsync(request);

Console.WriteLine(result);
```

OpenAI 2.0.0-beta.1:
```cs
ChatClient client = new ChatClient("gpt-3.5-turbo", "<api-key>");
List<ChatMessage> messages = new List<ChatMessage>()
{
    new SystemChatMessage("You are a helpful assistant designed to output JSON."),
    new UserChatMessage("Give me a JSON object listing Nobel Prize winners of the last 5 years.")
};
ChatCompletionOptions options = new ChatCompletionOptions()
{
    ResponseFormat = ChatResponseFormat.JsonObject
};

ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages, options);
string text = result.Value.Content[0].Text;

Console.WriteLine(text);
```

### Chat Completions: Vision

OpenAI 1.11.0:
```cs
OpenAIAPI api = new OpenAIAPI("<api-key>");
Conversation conversation = api.Chat.CreateConversation();
byte[] imageData = await File.ReadAllBytesAsync("<file-path>");

conversation.Model = Model.GPT4_Vision;
conversation.AppendUserInput("Describe this image.", ImageInput.FromImageBytes(imageData));

string response = await conversation.GetResponseFromChatbotAsync();

Console.WriteLine(response);
```

OpenAI 2.0.0-beta.1:
```cs
ChatClient client = new ChatClient("gpt-4-vision-preview", "<api-key>");
using FileStream file = File.OpenRead("<file-path>");
BinaryData imageData = await BinaryData.FromStreamAsync(file);
List<ChatMessage> messages = new List<ChatMessage>()
{
    new UserChatMessage(
        ChatMessageContentPart.CreateTextMessageContentPart("Describe this image."),
        ChatMessageContentPart.CreateImageMessageContentPart(imageData, "image/png"))
};

ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);
string text = result.Value.Content[0].Text;

Console.WriteLine(text);
```

### Audio: Speech-to-text

OpenAI 1.11.0:
```cs
OpenAIAPI api = new OpenAIAPI("<api-key>");
string result = await api.Transcriptions.GetTextAsync("<file-path>", "fr");

Console.WriteLine(result);
```

OpenAI 2.0.0-beta.1:
```cs
AudioClient client = new AudioClient("whisper-1", "<api-key>");
AudioTranscriptionOptions options = new AudioTranscriptionOptions()
{
    Language = "fr"
};

ClientResult<AudioTranscription> result = await client.TranscribeAudioAsync("<file-path>", options);
string text = result.Value.Text;

Console.WriteLine(text);
```

### Audio: Text-to-speech

OpenAI 1.11.0:
```cs
OpenAIAPI api = new OpenAIAPI("<api-key>");
TextToSpeechRequest request = new TextToSpeechRequest()
{
    Input = "Hasta la vista, baby.",
    Model = Model.TTS_Speed,
    Voice = "alloy"
};

await api.TextToSpeech.SaveSpeechToFileAsync(request, "<file-path>");
```

OpenAI 2.0.0-beta.1:
```cs
AudioClient client = new AudioClient("tts-1", "<api-key>");

ClientResult<BinaryData> result = await client.GenerateSpeechFromTextAsync("Hasta la vista, baby.", GeneratedSpeechVoice.Alloy);
BinaryData data = result.Value;

await File.WriteAllBytesAsync("<file-path>", data.ToArray());
```

### Image: Image generation

OpenAI 1.11.0:
```cs
OpenAIAPI api = new OpenAIAPI("<api-key>");
ImageGenerationRequest request = new ImageGenerationRequest()
{
    Prompt = "Draw a quick brown fox jumping over a lazy dog.",
    Model = Model.DALLE3,
    Quality = "standard",
    Size = ImageSize._1024
};

ImageResult result = await api.ImageGenerations.CreateImageAsync(request);

Console.WriteLine(result.Data[0].Url);
```

OpenAI 2.0.0-beta.1:
```cs
ImageClient client = new ImageClient("dall-e-3", "<api-key>");
ImageGenerationOptions options = new ImageGenerationOptions()
{
    Quality = GeneratedImageQuality.Standard,
    Size = GeneratedImageSize.W1024xH1024
};

ClientResult<GeneratedImage> result = await client.GenerateImageAsync("Draw a quick brown fox jumping over a lazy dog.", options);
Uri imageUri = result.Value.ImageUri;

Console.WriteLine(imageUri.AbsoluteUri);
```

## Additional examples

For additional examples, see [OpenAI Examples][examples].

[readme]: https://github.com/openai/openai-dotnet/blob/main/README.md
[changelog]: https://github.com/openai/openai-dotnet/blob/main/CHANGELOG.md
[examples]: https://github.com/openai/openai-dotnet/tree/main/examples
[openai_1110]: https://aka.ms/openai1110
