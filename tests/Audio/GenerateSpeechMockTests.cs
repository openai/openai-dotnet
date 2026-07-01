using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.ClientModel;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Audio;

[Parallelizable(ParallelScope.All)]
[Category("Audio")]
[Category("Smoke")]
internal class GenerateSpeechMockTests : ClientTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    public GenerateSpeechMockTests(bool isAsync)
        : base(isAsync)
    {
    }

    [Test]
    public void GenerateSpeechRespectsTheCancellationToken()
    {
        AudioClient client = CreateProxyFromClient(new AudioClient("model", s_fakeCredential));
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        Assert.That(async () => await client.GenerateSpeechAsync("text", GeneratedSpeechVoice.Echo, cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    [TestCase("tts-1")]
    [TestCase("tts-1-hd")]
    public void GenerateSpeechStreamingThrowsForUnsupportedModel(string model)
    {
        AudioClient client = new AudioClient(model, s_fakeCredential);

        Assert.That(
            () => client.GenerateSpeechStreaming("text", GeneratedSpeechVoice.Alloy),
            Throws.InstanceOf<NotSupportedException>()
                .With.Message.Contains(model)
                .And.Message.Contains("OPENAI_ENABLE_TTS_SSE_STREAMING"));
    }

    [Test]
    public async Task GenerateSpeechStreamingKeepsRequestContentAliveUntilEnumeration()
    {
        string requestBody = null;
        MockPipelineResponse response = CreateStreamingSpeechResponse();
        OpenAIClientOptions options = new()
        {
            Transport = new MockPipelineTransport(message =>
            {
                using MemoryStream stream = new();
                message.Request.Content.WriteTo(stream);
                requestBody = BinaryData.FromBytes(stream.ToArray()).ToString();
                return response;
            })
            {
                ExpectSyncPipeline = !IsAsync
            }
        };
        AudioClient client = CreateProxyFromClient(new AudioClient("gpt-4o-mini-tts", s_fakeCredential, options));

        if (IsAsync)
        {
            await foreach (StreamingSpeechUpdate _ in client.GenerateSpeechStreamingAsync("text", GeneratedSpeechVoice.Alloy))
            {
            }
        }
        else
        {
            foreach (StreamingSpeechUpdate _ in client.GenerateSpeechStreaming("text", GeneratedSpeechVoice.Alloy))
            {
            }
        }

        using JsonDocument requestDocument = JsonDocument.Parse(requestBody);
        Assert.That(requestDocument.RootElement.GetProperty("input").GetString(), Is.EqualTo("text"));
        Assert.That(requestDocument.RootElement.GetProperty("stream_format").GetString(), Is.EqualTo("sse"));
    }

    private static MockPipelineResponse CreateStreamingSpeechResponse()
        => new MockPipelineResponse(200).WithContent(
            """
            data: {"type":"speech.audio.delta","audio":"AQI="}

            data: {"type":"speech.audio.done"}

            data: [DONE]
            """);
}
