using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Files;
using OpenAI.Images;
using System;

namespace OpenAI.Examples.Miscellaneous;

public partial class ClientExamples
{
    [Test]
    public void CreateChatClient()
    {
        ChatClient client = new("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
    }

    [Test]
    public void CreateEmbeddingClient()
    {
        EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
    }

    [Test]
    public void CreateImageClient()
    {
        ImageClient client = new("dall-e-3", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
    }

    [Test]
    public void CreateMultipleAudioClients()
    {
        OpenAIClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        AudioClient ttsClient = client.GetAudioClient("tts-1");
        AudioClient whisperClient = client.GetAudioClient("whisper-1");
    }

    [Test]
    public void CreateAssistantAndFileClients()
    {
        OpenAIClient openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        FileClient fileClient = openAIClient.GetFileClient();
        AssistantClient assistantClient = openAIClient.GetAssistantClient();
    }
}
