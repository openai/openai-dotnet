using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Audio;

[Parallelizable(ParallelScope.All)]
[Category("Audio")]
[Category("Smoke")]
public class SpeechOptionsReuseTests
{
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task StreamingThenBufferedCallsUseTheirOwnResponseFormat(bool streamAsync, bool bufferedAsync)
    {
        using RecordingHandler handler = new();
        using HttpClient http = new(handler);
        AudioClient client = new("gpt-4o-mini-tts", new ApiKeyCredential("synthetic-key"),
            new OpenAIClientOptions { Transport = new HttpClientPipelineTransport(http) });
        SpeechGenerationOptions options = new() { ResponseFormat = GeneratedSpeechFormat.Pcm, SpeedRatio = 1.25f };
        await ReadStream(client, options, streamAsync);
        BinaryData audio = bufferedAsync
            ? (await client.GenerateSpeechAsync("second", GeneratedSpeechVoice.Alloy, options)).Value
            : client.GenerateSpeech("second", GeneratedSpeechVoice.Alloy, options).Value;
        Assert.That(audio.ToArray(), Is.EqualTo(RecordingHandler.Audio));
        Assert.That(handler.Formats, Is.EqualTo(new[] { "sse", "audio" }));
        await ReadStream(client, options, streamAsync);
        Assert.That(handler.Formats, Is.EqualTo(new[] { "sse", "audio", "sse" }));
        Assert.That(options.ResponseFormat, Is.EqualTo(GeneratedSpeechFormat.Pcm));
        Assert.That(options.SpeedRatio, Is.EqualTo(1.25f));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task RepeatedBufferedCallsKeepReturningAudio(bool asynchronous)
    {
        using RecordingHandler handler = new();
        using HttpClient http = new(handler);
        AudioClient client = new("gpt-4o-mini-tts", new ApiKeyCredential("synthetic-key"),
            new OpenAIClientOptions { Transport = new HttpClientPipelineTransport(http) });
        SpeechGenerationOptions options = new() { ResponseFormat = GeneratedSpeechFormat.Pcm, SpeedRatio = 1.25f };
        for (int i = 0; i < 2; i++)
        {
            BinaryData audio = asynchronous
                ? (await client.GenerateSpeechAsync("second", GeneratedSpeechVoice.Alloy, options)).Value
                : client.GenerateSpeech("second", GeneratedSpeechVoice.Alloy, options).Value;
            Assert.That(audio.ToArray(), Is.EqualTo(RecordingHandler.Audio));
        }
        Assert.That(handler.Formats, Is.EqualTo(new[] { "audio", "audio" }));
    }

    private static async Task ReadStream(AudioClient client, SpeechGenerationOptions options, bool asynchronous)
    {
        int count = 0;
        if (asynchronous)
        {
            await foreach (StreamingSpeechUpdate update in client.GenerateSpeechStreamingAsync("first", GeneratedSpeechVoice.Alloy, options))
            {
                Assert.That(update, Is.TypeOf<StreamingSpeechAudioDoneUpdate>());
                count++;
            }
        }
        else
        {
            foreach (StreamingSpeechUpdate update in client.GenerateSpeechStreaming("first", GeneratedSpeechVoice.Alloy, options))
            {
                Assert.That(update, Is.TypeOf<StreamingSpeechAudioDoneUpdate>());
                count++;
            }
        }
        Assert.That(count, Is.EqualTo(1));
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        internal static readonly byte[] Audio = [0, 1, 2, 255];
        internal List<string> Formats { get; } = [];
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken token) => Respond(request);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) => Task.FromResult(Respond(request));
        private HttpResponseMessage Respond(HttpRequestMessage request)
        {
            using JsonDocument payload = JsonDocument.Parse(request.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            JsonElement body = payload.RootElement;
            Assert.That(body.GetProperty("response_format").GetString(), Is.EqualTo("pcm"));
            Assert.That(body.GetProperty("speed").GetSingle(), Is.EqualTo(1.25f));
            string format = body.TryGetProperty("stream_format", out JsonElement field) ? field.GetString() : "audio";
            Formats.Add(format);
            byte[] bytes = format == "sse"
                ? Encoding.UTF8.GetBytes("data: {\"type\":\"speech.audio.done\"}\n\ndata: [DONE]\n\n")
                : Audio;
            HttpResponseMessage response = new(HttpStatusCode.OK) { Content = new ByteArrayContent(bytes) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(format == "sse" ? "text/event-stream" : "application/octet-stream");
            return response;
        }
    }
}
