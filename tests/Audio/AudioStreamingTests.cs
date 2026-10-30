using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Audio;

[Parallelizable(ParallelScope.All)]
[Category("Audio")]
[Category("Smoke")]
public class AudioStreamingTests
{
    private const string SpeechEvents = """
        data: {"type":"speech.audio.delta","audio":"AAECAw=="}

        data: {"type":"speech.audio.done","usage":{"input_tokens":3,"output_tokens":5,"total_tokens":8}}


        """;

    private const string TranscriptionEvents = """
        data: {"type":"transcript.text.delta","delta":"Hello"}

        data: {"type":"transcript.text.done","text":"Hello"}


        """;

    [Test]
    public async Task GenerateSpeechStreamingAsyncReturnsAnEstablishedResult()
    {
        StreamingTestServer server = new(SpeechEvents);
        AudioClient client = new("gpt-4o-mini-tts", new ApiKeyCredential("key"), new OpenAIClientOptions { Transport = server.Transport });

        await using AsyncStreamingResult<StreamingSpeechUpdate> result = await client.GenerateSpeechStreamingAsync("hello", GeneratedSpeechVoice.Alloy);

        Assert.That(server.RequestCount, Is.EqualTo(1));
        Assert.That(server.LastBufferResponse, Is.False);
        Assert.That(server.LastRequestJson.GetProperty("stream_format").GetString(), Is.EqualTo("sse"));
        AssertEstablished(result.Status, result.Headers, server);

        List<StreamingSpeechUpdate> updates = [];

        await foreach (StreamingSpeechUpdate update in result)
        {
            updates.Add(update);
        }

        Assert.That(updates, Has.Count.EqualTo(2));
        Assert.That(((StreamingSpeechAudioDeltaUpdate)updates[0]).AudioBytes.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3 }));
        Assert.That(((StreamingSpeechAudioDoneUpdate)updates[1]).Usage, Is.Not.Null);
        Assert.That(server.RequestCount, Is.EqualTo(1));
        Assert.That(server.LastResponseStream.IsDisposed, Is.True);
    }

    [Test]
    public async Task TranscribeAudioStreamingAsyncReturnsAnEstablishedResult([Values] bool usingFilePath)
    {
        StreamingTestServer server = new(TranscriptionEvents);
        AudioClient client = new("gpt-4o-transcribe", new ApiKeyCredential("key"), new OpenAIClientOptions { Transport = server.Transport });
        string path = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"streaming-{Guid.NewGuid():N}.mp3");
        using MemoryStream audio = new([0x01, 0x02, 0x03, 0x04]);
        File.WriteAllBytes(path, audio.ToArray());

        try
        {
            await using AsyncStreamingResult<StreamingAudioTranscriptionUpdate> result = usingFilePath
                ? await client.TranscribeAudioStreamingAsync(path)
                : await client.TranscribeAudioStreamingAsync(audio, "audio.mp3");

            Assert.That(server.RequestCount, Is.EqualTo(1));
            Assert.That(server.LastBufferResponse, Is.False);
            Assert.That(server.LastRequestText, Does.Contain("stream"));
            AssertEstablished(result.Status, result.Headers, server);

            if (usingFilePath)
            {
                // The request has been sent, so the client must no longer hold the input file open.
                using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                }
            }

            List<StreamingAudioTranscriptionUpdate> updates = [];

            await foreach (StreamingAudioTranscriptionUpdate update in result)
            {
                updates.Add(update);
            }

            Assert.That(updates, Has.Count.EqualTo(2));
            Assert.That(((StreamingAudioTranscriptionTextDeltaUpdate)updates[0]).Delta, Is.EqualTo("Hello"));
            Assert.That(((StreamingAudioTranscriptionTextDoneUpdate)updates[1]).Text, Is.EqualTo("Hello"));
            Assert.That(server.RequestCount, Is.EqualTo(1));
            Assert.That(server.LastResponseStream.IsDisposed, Is.True);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task TranscribeAudioStreamingAsyncReleasesTheAudioOnceTheRequestIsSent([Values] bool httpClientTransport)
    {
        StreamingTestServer server = new(TranscriptionEvents);
        using HttpClient httpClient = new(new SseHandler(TranscriptionEvents));
        PipelineTransport transport = httpClientTransport ? new HttpClientPipelineTransport(httpClient) : server.Transport;
        AudioClient client = new("gpt-4o-transcribe", new ApiKeyCredential("key"), new OpenAIClientOptions { Transport = transport });
        using MemoryStream audio = new([0x01, 0x02, 0x03, 0x04]);

        await using AsyncStreamingResult<StreamingAudioTranscriptionUpdate> result = await client.TranscribeAudioStreamingAsync(audio, "audio.mp3");

        // Like the non-streaming overloads, the call releases the request content, including the audio, whatever the transport.
        Assert.That(audio.CanRead, Is.False);

        List<StreamingAudioTranscriptionUpdate> updates = [];

        await foreach (StreamingAudioTranscriptionUpdate update in result)
        {
            updates.Add(update);
        }

        Assert.That(updates, Has.Count.EqualTo(2));
    }

    private static void AssertEstablished(int status, PipelineResponseHeaders headers, StreamingTestServer server)
    {
        Assert.That(status, Is.EqualTo(200));
        Assert.That(headers.TryGetValue("x-request-id", out string requestId), Is.True);
        Assert.That(requestId, Is.EqualTo(StreamingTestServer.RequestId));
        Assert.That(server.LastResponseStream.BytesRead, Is.Zero);
    }

    private sealed class SseHandler(string content) : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await request.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);

            HttpResponseMessage response = new(HttpStatusCode.OK)
            {
                RequestMessage = request,
                Content = new StringContent(content, Encoding.UTF8, "text/event-stream"),
            };
            response.Headers.Add("x-request-id", StreamingTestServer.RequestId);
            return response;
        }
    }
}
