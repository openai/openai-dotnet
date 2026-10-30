#pragma warning disable OPENAI001
using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.Mocks;
using Moq;
using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.IO;
using System.Net.ServerSentEvents;
using System.Text;
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

    [Test]
    public async Task GenerateSpeechStreamingUsesProtocolOverride()
    {
        var clientMock = new Mock<AudioClient>("model", s_fakeCredential) { CallBase = true };
        clientMock
            .Setup(client => client.GenerateSpeechStreamingAsync(
                It.IsAny<BinaryContent>(),
                It.IsAny<RequestOptions>()))
            .ReturnsAsync(CreateProtocolStreamingResult());

        await using var result = await clientMock.Object.GenerateSpeechStreamingAsync(
            "text",
            GeneratedSpeechVoice.Alloy);
        await foreach (var _ in result)
        {
            // Consume the stream to verify the protocol override result is adapted.
        }

        clientMock.Verify(
            client => client.GenerateSpeechStreamingAsync(
                It.IsAny<BinaryContent>(),
                It.IsAny<RequestOptions>()),
            Times.Once);
    }

    private static AsyncStreamingResult<SseItem<BinaryData>> CreateProtocolStreamingResult()
    {
        MockPipelineResponse response = new(200)
        {
            ContentStream = new NonDisposingMemoryStream(Encoding.UTF8.GetBytes("data: [DONE]\n\n")),
        };

        return AsyncStreamingResult.CreateSse(response);
    }

    private sealed class NonDisposingMemoryStream(byte[] content) : MemoryStream(content)
    {
        protected override void Dispose(bool disposing)
        {
        }
    }
}
