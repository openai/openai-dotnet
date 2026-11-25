using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Responses;
using System.ClientModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Tests.Responses;

[Parallelizable(ParallelScope.All)]
[Category("Responses")]
[Category("Smoke")]
public class ResponsesConvenienceOverloadTests
{
    private const string Model = "mock";
    private const string Text = "hello";

    public enum Overload { InputItems, UserInputText }

    public enum Mode { Sync, Async, Streaming }

    [Test]
    public async Task ConvenienceOverloadSendsSameBodyAsOptions(
        [Values] Overload overload,
        [Values] Mode mode,
        [Values(null, "resp_prev")] string previousResponseId)
    {
        CapturingTransport transport = new(expectSync: mode == Mode.Sync);
        ResponsesClient client = new(new ApiKeyCredential("key"), new ResponsesClientOptions { Transport = transport.Transport });

        await SendAsync(client, CreateExpectedOptions(mode == Mode.Streaming, previousResponseId), mode);
        string expected = transport.LastRequestText;

        await SendConvenienceAsync(client, overload, mode, previousResponseId);

        Assert.That(transport.LastRequestText, Is.EqualTo(expected));
    }

    // Built via property setters to stay independent of the constructor under test.
    private static CreateResponseOptions CreateExpectedOptions(bool streaming, string previousResponseId)
    {
        CreateResponseOptions options = new() { Model = Model };
        if (streaming)
        {
            options.StreamingEnabled = true;
        }
        if (previousResponseId is not null)
        {
            options.PreviousResponseId = previousResponseId;
        }
        options.InputItems.Add(ResponseItem.CreateUserMessageItem(Text));
        return options;
    }

    private static async Task SendAsync(ResponsesClient client, CreateResponseOptions options, Mode mode)
    {
        switch (mode)
        {
            case Mode.Sync:
                client.CreateResponse(options);
                break;
            case Mode.Async:
                await client.CreateResponseAsync(options);
                break;
            default:
                await using (await client.CreateResponseStreamingAsync(options)) { }
                break;
        }
    }

    private static async Task SendConvenienceAsync(ResponsesClient client, Overload overload, Mode mode, string previousResponseId)
    {
        ResponseItem[] items = [ResponseItem.CreateUserMessageItem(Text)];
        switch (mode, overload)
        {
            case (Mode.Sync, Overload.InputItems):
                client.CreateResponse(Model, items, previousResponseId);
                break;
            case (Mode.Sync, Overload.UserInputText):
                client.CreateResponse(Model, Text, previousResponseId);
                break;
            case (Mode.Async, Overload.InputItems):
                await client.CreateResponseAsync(Model, items, previousResponseId);
                break;
            case (Mode.Async, Overload.UserInputText):
                await client.CreateResponseAsync(Model, Text, previousResponseId);
                break;
            case (Mode.Streaming, Overload.InputItems):
                await using (await client.CreateResponseStreamingAsync(Model, items, previousResponseId)) { }
                break;
            default:
                await using (await client.CreateResponseStreamingAsync(Model, Text, previousResponseId)) { }
                break;
        }
    }

    private sealed class CapturingTransport
    {
        private const string ResponseJson = """{"id":"resp_1","object":"response","created_at":0,"status":"completed","model":"mock","output":[],"tools":[]}""";

        public CapturingTransport(bool expectSync)
        {
            Transport = new MockPipelineTransport(message =>
            {
                using MemoryStream content = new();
                message.Request.Content.WriteTo(content);
                LastRequestText = Encoding.UTF8.GetString(content.ToArray());

                bool streaming = JsonDocument.Parse(LastRequestText).RootElement.TryGetProperty("stream", out JsonElement stream)
                    && stream.GetBoolean();
                return new MockPipelineResponse(200)
                    .WithHeader("Content-Type", streaming ? "text/event-stream" : "application/json")
                    .WithContent(streaming ? "data: [DONE]\n\n" : ResponseJson);
            })
            {
                ExpectSyncPipeline = expectSync,
            };
        }

        public MockPipelineTransport Transport { get; }

        public string LastRequestText { get; private set; }
    }
}
