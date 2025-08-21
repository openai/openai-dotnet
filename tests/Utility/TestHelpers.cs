using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Batch;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Files;
using OpenAI.FineTuning;
using OpenAI.Images;
using OpenAI.Models;
using OpenAI.Moderations;
using OpenAI.Realtime;
using OpenAI.Responses;
using OpenAI.Tests.Utility;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;

[assembly: LevelOfParallelism(8)]

namespace OpenAI.Tests;

internal static class TestHelpers
{
    public static string GetModelForScenario(TestScenario testScenario) => testScenario switch
    {
        TestScenario.Assistants => null,
        TestScenario.Audio_TTS => "tts-1",
        TestScenario.Audio_Whisper => "whisper-1",
        TestScenario.Audio_Gpt_4o_Mini_Transcribe => "gpt-4o-mini-transcribe",
        TestScenario.Batch => null,
        TestScenario.Chat => "gpt-4o-mini",
        TestScenario.Embeddings => "text-embedding-3-small",
        TestScenario.Files => null,
        TestScenario.FineTuning => null,
        TestScenario.Images => "gpt-image-1",
        TestScenario.Models => null,
        TestScenario.Moderations => "text-moderation-stable",
        TestScenario.VectorStores => null,
        TestScenario.TopLevel => null,
        TestScenario.Realtime => "gpt-4o-realtime-preview-2024-10-01",
        TestScenario.Responses => "gpt-4o-mini",
        _ => throw new NotImplementedException(),
    };

    public static OpenAIClient GetTestTopLevelClient() => GetTestClient<OpenAIClient>(TestScenario.TopLevel);

    public static ApiKeyCredential GetTestApiKeyCredential()
        => new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

    public static T GetTestClient<T>(TestScenario scenario, string overrideModel = null, OpenAIClientOptions options = default)
    {
        options ??= new();
        ApiKeyCredential credential = GetTestApiKeyCredential();
        options.AddPolicy(new DumpPolicy(), PipelinePosition.BeforeTransport);
        string model = overrideModel ?? GetModelForScenario(scenario);
        object clientObject = scenario switch
        {
#pragma warning disable OPENAI001
            TestScenario.Assistants => new AssistantClient(credential, options),
#pragma warning restore OPENAI001
            TestScenario.Audio_TTS => new AudioClient(model, credential, options),
            TestScenario.Audio_Whisper => new AudioClient(model, credential, options),
            TestScenario.Audio_Gpt_4o_Mini_Transcribe => new AudioClient(model, credential, options),
            TestScenario.Batch => new BatchClient(credential, options),
            TestScenario.Chat => new ChatClient(model, credential, options),
            TestScenario.Embeddings => new EmbeddingClient(model, credential, options),
            TestScenario.Files => new OpenAIFileClient(credential, options),
            TestScenario.FineTuning => new FineTuningClient(credential, options),
            TestScenario.Images => new ImageClient(model, credential, options),
            TestScenario.Models => new OpenAIModelClient(credential, options),
            TestScenario.Moderations => new ModerationClient(model, credential, options),
#pragma warning disable OPENAI001
            TestScenario.VectorStores => new VectorStoreClient(credential, options),
#pragma warning restore OPENAI001
            TestScenario.TopLevel => new OpenAIClient(credential, options),
#pragma warning disable OPENAI002
            TestScenario.Realtime => new RealtimeClient(credential, options),
#pragma warning restore
#pragma warning disable OPENAI003
            TestScenario.Responses => new OpenAIResponseClient(model, credential, options),
#pragma warning restore
            _ => throw new NotImplementedException(),
        };
        return (T)clientObject;
    }
}