#pragma warning disable OPENAI001
using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.ClientModel;
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

        Assert.ThatAsync(
            () => client.GenerateSpeechStreamingAsync("text", GeneratedSpeechVoice.Alloy),
            Throws.InstanceOf<NotSupportedException>()
                .With.Message.Contains(model)
                .And.Message.Contains("OPENAI_ENABLE_TTS_SSE_STREAMING"));
    }

    [Test]
    public async Task GenerateSpeechStreamingUsesStreamingRequest()
    {
        string acceptHeader = null;
        bool? bufferResponse = null;
        OpenAIClientOptions clientOptions = new()
        {
            Transport = new MockPipelineTransport(message =>
            {
                message.Request.Headers.TryGetValue("Accept", out acceptHeader);
                bufferResponse = message.BufferResponse;
                return new MockPipelineResponse(200).WithContent(string.Empty);
            })
            {
                ExpectSyncPipeline = false
            }
        };
        AudioClient client = new("model", s_fakeCredential, clientOptions);

        await using var result = await client.GenerateSpeechStreamingAsync("text", GeneratedSpeechVoice.Alloy);

        Assert.That(acceptHeader, Is.EqualTo("text/event-stream"));
        Assert.That(bufferResponse, Is.False);
    }
}
