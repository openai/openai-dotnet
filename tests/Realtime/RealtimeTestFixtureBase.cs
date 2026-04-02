using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Realtime;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel.Primitives;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;

namespace OpenAI.Tests.Realtime;

#pragma warning disable OPENAI002

[Category("Realtime")]
public class RealtimeTestFixtureBase : OpenAIRecordedTestBase
{
    public CancellationTokenSource CancellationTokenSource { get; private set; }
    public CancellationToken CancellationToken => CancellationTokenSource?.Token ?? default;
    public RequestOptions CancellationOptions => new() { CancellationToken = CancellationToken };

    public RealtimeTestFixtureBase(bool isAsync, RecordedTestMode mode = RecordedTestMode.Playback) : base(isAsync, mode)
    {
    }

    [SetUp]
    public void SetUpCancellationToken()
    {
        // Create a fresh CancellationTokenSource for each test to ensure each test
        // gets its own 15-second timeout, not a shared one from the fixture constructor
        CancellationTokenSource = new();
        if (!Debugger.IsAttached)
        {
            CancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
        }
    }

    [TearDown]
    public void TearDownCancellationToken()
    {
        CancellationTokenSource?.Dispose();
        CancellationTokenSource = null;
    }

    public static string GetTestModel() => TestModel.Realtime;

    public RealtimeClient GetTestClient()
    {
        // WebSocket connections cannot go through the test proxy, so for Live mode tests
        // we create a direct client instead of using GetProxiedOpenAIClient.
        // The proxy only works with HTTP/HTTPS requests, not WebSocket connections.
        RealtimeClient client = Mode switch
        {
            // Create a direct client without proxy for live WebSocket tests
            RecordedTestMode.Live => TestEnvironment.GetTestClient<RealtimeClient>(TestModel.Realtime),

            // Use proxied client for playback mode (recordings)
            _ => GetProxiedOpenAIClient<RealtimeClient>(TestModel.Realtime)
        };

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
