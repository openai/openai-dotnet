using NUnit.Framework;
using OpenAI.Realtime;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel.Primitives;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Realtime;

#pragma warning disable OPENAI002

[Parallelizable(ParallelScope.All)]
[Category("Conversation")]
public class RealtimeTestFixtureBase : SyncAsyncTestBase
{
    public CancellationTokenSource CancellationTokenSource { get; }
    public CancellationToken CancellationToken => CancellationTokenSource?.Token ?? default;
    public RequestOptions CancellationOptions => new() { CancellationToken = CancellationToken };

    public RealtimeTestFixtureBase(bool isAsync) : base(isAsync)
    {
        CancellationTokenSource = new();
        if (!Debugger.IsAttached)
        {
            CancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
        }
    }

    public static string GetTestModel() => GetModelForScenario(TestScenario.Realtime);

    public static RealtimeClient GetTestClient()
    {
        RealtimeClient client = GetTestClient<RealtimeClient>(TestScenario.Realtime);
        client.OnSendingCommand += (_, data) => PrintMessageData(data, "> ");
        client.OnReceivingCommand += (_, data) => PrintMessageData(data, "  < ");
        return client;
    }

    public static void PrintMessageData(BinaryData data, string prefix = "")
    {
        JsonNode jsonNode = JsonNode.Parse(data.ToString());

        foreach ((string labelKey, string labelValue, string dataKey) in new (string, string, string)[]
        {
            ("type", "input_audio_buffer.append", "audio"),
            ("type", "audio", "data"),
            ("event", "add_user_audio", "data"),
            ("type", "response.audio.delta", "delta")
        })
        {
            if (jsonNode[labelKey]?.GetValue<string>() == labelValue)
            {
                string rawBase64Data = jsonNode[dataKey]?.GetValue<string>();
                if (rawBase64Data is not null)
                {
                    byte[] base64Data = rawBase64Data == null ? [] : Convert.FromBase64String(rawBase64Data);
                    jsonNode[dataKey] = $"<{base64Data.Length} bytes>";
                }
            }
        }

        string rawMessage = jsonNode.ToJsonString(new JsonSerializerOptions());
        Console.WriteLine($"{prefix}{rawMessage}");
    }
}
