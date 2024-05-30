using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Files;
using OpenAI.Images;
using System;
using System.ClientModel;

namespace OpenAI.Samples.Miscellaneous
{
    public partial class ClientSamples
    {
        [Test]
        [Ignore("Compilation validation only")]
        public void CreateAssistantAndFileClients()
        {
            OpenAIClient openAIClient = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            FileClient fileClient = openAIClient.GetFileClient();
#pragma warning disable OPENAI001
            AssistantClient assistantClient = openAIClient.GetAssistantClient();
#pragma warning restore OPENAI001
        }

        [Test]
        [Ignore("Compilation validation only")]
        public void CreateChatClient()
        {
            ChatClient client = new("gpt-3.5-turbo", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        }

        [Test]
        [Ignore("Compilation validation only")]
        public void CreateEmbeddingClient()
        {
            EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        }

        [Test]
        [Ignore("Compilation validation only")]
        public void CreateImageClient()
        {
            ImageClient client = new("dall-e-3", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        }

        [Test]
        [Ignore("Compilation validation only")]
        public void CreateMultipleAudioClients()
        {
            OpenAIClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            AudioClient ttsClient = client.GetAudioClient("tts-1");
            AudioClient whisperClient = client.GetAudioClient("whisper-1");
        }
    }
}
