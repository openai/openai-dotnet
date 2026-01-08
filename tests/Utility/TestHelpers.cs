using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Batch;
using OpenAI.Chat;
using OpenAI.Containers;
using OpenAI.Conversations;
using OpenAI.Embeddings;
using OpenAI.Files;
using OpenAI.FineTuning;
using OpenAI.Images;
using OpenAI.Models;
using OpenAI.Moderations;
using OpenAI.Realtime;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[assembly: LevelOfParallelism(8)]

namespace OpenAI.Tests;

internal static partial class TestHelpers
{
    public partial class TestScenario
    {
        public string ModelId { get; }

        public TestScenario(string modelId)
        {
            ModelId = modelId;
        }

        public virtual object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
        {
            throw new NotImplementedException();
        }

        public static readonly TestScenario Assistants = new AssistantsScenario();
        public static readonly TestScenario Audio_TTS = new AudioTTSScenario();
        public static readonly TestScenario Audio_Whisper = new AudioWhisperScenario();
        public static readonly TestScenario Audio_Gpt_4o_Mini_Transcribe = new AudioGpt4oMiniTranscribeScenario();
        public static readonly TestScenario Batch = new BatchScenario();
        public static readonly TestScenario Chat = new ChatScenario();
        public static readonly TestScenario Containers = new ContainersScenario();
        public static readonly TestScenario Conversations = new ConversationsScenario();
        public static readonly TestScenario Embeddings = new EmbeddingsScenario();
        public static readonly TestScenario Files = new FilesScenario();
        public static readonly TestScenario FineTuning = new FineTuningScenario();
        public static readonly TestScenario Images = new ImagesScenario();
        public static readonly TestScenario LegacyCompletions = new LegacyCompletionsScenario();
        public static readonly TestScenario Models = new ModelsScenario();
        public static readonly TestScenario Moderations = new ModerationsScenario();
        public static readonly TestScenario Realtime = new RealtimeScenario();
        public static readonly TestScenario VectorStores = new VectorStoresScenario();
        public static readonly TestScenario TopLevel = new TopLevelScenario();

        private class AssistantsScenario : TestScenario
        {
            public AssistantsScenario() : base(null) { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
#pragma warning disable OPENAI001
                => new AssistantClient(credential, options);
#pragma warning restore OPENAI001
        }

        private class AudioTTSScenario : TestScenario
        {
            public AudioTTSScenario() : base("tts-1") { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
                => new AudioClient(model, credential, options);
        }

        private class AudioWhisperScenario : TestScenario
        {
            public AudioWhisperScenario() : base("whisper-1") { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
                => new AudioClient(model, credential, options);
        }

        private class AudioGpt4oMiniTranscribeScenario : TestScenario
        {
            public AudioGpt4oMiniTranscribeScenario() : base("gpt-4o-mini-transcribe") { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
                => new AudioClient(model, credential, options);
        }

        private class BatchScenario : TestScenario
        {
            public BatchScenario() : base(null) { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
                => new BatchClient(credential, options);
        }

        private class ChatScenario : TestScenario
        {
            public ChatScenario() : base("gpt-4o-mini") { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
                => new ChatClient(model, credential, options);
        }

        private class ContainersScenario : TestScenario
        {
            public ContainersScenario() : base("gpt-4o-mini") { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
#pragma warning disable OPENAI001
                => new ContainerClient(credential, options);
#pragma warning restore OPENAI001
        }

        private class ConversationsScenario : TestScenario
        {
            public ConversationsScenario() : base(null) { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
#pragma warning disable OPENAI001
                => new ConversationClient(credential, options);
#pragma warning restore OPENAI001
        }

        private class EmbeddingsScenario : TestScenario
        {
            public EmbeddingsScenario() : base("text-embedding-3-small") { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
                => new EmbeddingClient(model, credential, options);
        }

        private class FilesScenario : TestScenario
        {
            public FilesScenario() : base(null) { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
                => new OpenAIFileClient(credential, options);
        }

        private class FineTuningScenario : TestScenario
        {
            public FineTuningScenario() : base(null) { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
                => new FineTuningClient(credential, options);
        }

        private class ImagesScenario : TestScenario
        {
            public ImagesScenario() : base("gpt-image-1") { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
                => new ImageClient(model, credential, options);
        }

        private class LegacyCompletionsScenario : TestScenario
        {
            public LegacyCompletionsScenario() : base(null) { }
        }

        private class ModelsScenario : TestScenario
        {
            public ModelsScenario() : base(null) { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
                => new OpenAIModelClient(credential, options);
        }

        private class ModerationsScenario : TestScenario
        {
            public ModerationsScenario() : base("omni-moderation-latest") { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
                => new ModerationClient(model, credential, options);
        }

        private class RealtimeScenario : TestScenario
        {
            public RealtimeScenario() : base("gpt-realtime") { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
#pragma warning disable OPENAI002
                => new RealtimeClient(credential, options);
#pragma warning restore OPENAI002
        }

        private class VectorStoresScenario : TestScenario
        {
            public VectorStoresScenario() : base(null) { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
#pragma warning disable OPENAI001
                => new VectorStoreClient(credential, options);
#pragma warning restore OPENAI001
        }

        private class TopLevelScenario : TestScenario
        {
            public TopLevelScenario() : base(null) { }
            public override object CreateClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
                => new OpenAIClient(credential, options);
        }
    }

    public static string GetModelForScenario(TestScenario testScenario) => testScenario.ModelId;

    public static OpenAIClient GetTestTopLevelClient() => GetTestClient<OpenAIClient>(TestScenario.TopLevel);

    public static ApiKeyCredential GetTestApiKeyCredential()
        => new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

    public static T GetTestClient<T>(
        TestScenario scenario,
        string overrideModel = null,
        bool excludeDumpPolicy = false,
        OpenAIClientOptions options = default,
        ApiKeyCredential credential = default)
    {
        options ??= new();
        credential ??= GetTestApiKeyCredential();

        if (!excludeDumpPolicy)
        {
            options.AddPolicy(GetDumpPolicy(), PipelinePosition.BeforeTransport);
        }

        string model = overrideModel ?? GetModelForScenario(scenario);
        object clientObject = scenario.CreateClient(model, credential, options);
        return (T)clientObject;
    }

    public static async Task RetryWithExponentialBackoffAsync(Func<Task> action, int maxRetries = 5, int initialWaitMs = 750)
    {
        int waitDuration = initialWaitMs;
        int retryCount = 0;
        bool successful = false;

        while (retryCount < maxRetries && !successful)
        {
            try
            {
                await action();
                successful = true;
            }
            catch (ClientResultException ex) when (ex.Status == 404)
            {
                await Task.Delay(waitDuration);
                waitDuration *= 2;
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    throw;
                }
            }
        }
    }

    private static PipelinePolicy GetDumpPolicy()
    {
        return new TestPipelinePolicy((message) =>
        {
            if (message.Request is not null && message.Response is null)
            {
                Console.WriteLine($"--- New request ---");
                IEnumerable<string> headerPairs = message?.Request?.Headers?.Select(header => $"{header.Key}={(header.Key.ToLower().Contains("auth") ? "***" : header.Value)}");
                string headers = string.Join(',', headerPairs);
                Console.WriteLine($"Headers: {headers}");
                Console.WriteLine($"{message?.Request?.Method} URI: {message?.Request?.Uri}");
                if (message.Request?.Content != null)
                {
                    string contentType = "Unknown Content Type";
                    if (message.Request.Headers?.TryGetValue("Content-Type", out contentType) == true
                        && contentType == "application/json")
                    {
                        using MemoryStream stream = new();
                        message.Request.Content.WriteTo(stream, default);
                        stream.Position = 0;
                        using StreamReader reader = new(stream);
                        string requestDump = reader.ReadToEnd();
                        stream.Position = 0;
                        requestDump = Regex.Replace(requestDump, @"""data"":[\\w\\r\\n]*""[^""]*""", @"""data"":""...""");
                        Console.WriteLine(requestDump);
                    }
                    else
                    {
                        string length = message.Request.Content.TryComputeLength(out long numberLength)
                            ? $"{numberLength} bytes"
                            : "unknown length";
                        Console.WriteLine($"<< Non-JSON content: {contentType} >> {length}");
                    }
                }
            }
            if (message.Response != null)
            {
                if (message.BufferResponse)
                {
                    Console.WriteLine("--- Begin response content ---");
                    Console.WriteLine(message.Response.Content?.ToString());
                    Console.WriteLine("--- End of response content ---");
                }
                else
                {
                    Console.WriteLine("--- Response (unbuffered, content not rendered) ---");
                }
            }
        });
    }
}