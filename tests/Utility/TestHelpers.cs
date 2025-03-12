﻿using NUnit.Framework;
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
using OpenAI.RealtimeConversation;
using OpenAI.Responses;
using OpenAI.VectorStores;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

[assembly: LevelOfParallelism(8)]

namespace OpenAI.Tests;

internal static class TestHelpers
{
    public enum TestScenario
    {
        Assistants,
        Audio_TTS,
        Audio_Whisper,
        Batch,
        Chat,
        Embeddings,
        Files,
        FineTuning,
        Images,
        LegacyCompletions,
        Models,
        Moderations,
        RealtimeConversation,
        Responses,
        VectorStores,
        TopLevel,
    }

    public static OpenAIClient GetTestTopLevelClient() => GetTestClient<OpenAIClient>(TestScenario.TopLevel);

    public static T GetTestClient<T>(TestScenario scenario, string overrideModel = null, OpenAIClientOptions options = default)
    {
        options ??= new();
        ApiKeyCredential credential = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        options.AddPolicy(GetDumpPolicy(), PipelinePosition.BeforeTransport);
        object clientObject = scenario switch
        {
#pragma warning disable OPENAI001
            TestScenario.Assistants => new AssistantClient(credential, options),
#pragma warning restore OPENAI001
            TestScenario.Audio_TTS => new AudioClient(overrideModel ?? "tts-1", credential, options),
            TestScenario.Audio_Whisper => new AudioClient(overrideModel ?? "whisper-1", credential, options),
            TestScenario.Batch => new BatchClient(credential, options),
            TestScenario.Chat => new ChatClient(overrideModel ?? "gpt-4o-mini", credential, options),
            TestScenario.Embeddings => new EmbeddingClient(overrideModel ?? "text-embedding-3-small", credential, options),
            TestScenario.Files => new OpenAIFileClient(credential, options),
            TestScenario.FineTuning => new FineTuningClient(credential, options),
            TestScenario.Images => new ImageClient(overrideModel ?? "dall-e-3", credential, options),
            TestScenario.Models => new OpenAIModelClient(credential, options),
            TestScenario.Moderations => new ModerationClient(overrideModel ?? "text-moderation-stable", credential, options),
#pragma warning disable OPENAI001
            TestScenario.VectorStores => new VectorStoreClient(credential, options),
#pragma warning restore OPENAI001
            TestScenario.TopLevel => new OpenAIClient(credential, options),
#pragma warning disable OPENAI002
            TestScenario.RealtimeConversation => new RealtimeConversationClient(overrideModel ?? "gpt-4o-realtime-preview-2024-10-01", credential, options),
#pragma warning restore
#pragma warning disable OPENAI003
            TestScenario.Responses => new OpenAIResponseClient(overrideModel ?? "gpt-4o-mini", credential, options),
#pragma warning restore
            _ => throw new NotImplementedException(),
        };
        return (T)clientObject;
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