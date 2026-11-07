using Moq;
using Moq.Protected;
using NUnit.Framework;
using OpenAI.Audio;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Tests.Audio;

[Parallelizable(ParallelScope.All)]
[Category("Audio")]
[Category("Smoke")]
public class TranscriptionTextCastTests
{
    [TestCase("text/plain", "Synthetic transcription.")]
    [TestCase("text/plain; charset=utf-8", "café 東\nSecond line.")]
    [TestCase("text/plain", "1\n00:00:00,000 --> 00:00:01,000\nSynthetic subtitle.\n")]
    [TestCase("text/plain", "WEBVTT\n\n00:00.000 --> 00:01.000\nSynthetic caption.\n")]
    [TestCase("text/plain", "")]
    [TestCase("text/plain", "{\"text\":\"literal text, not a JSON response\"}")]
    public void ExplicitCastPreservesTextResponse(string contentType, string content)
    {
        Mock<PipelineResponse> response = MakeResponse(contentType, content);
        AudioTranscription actual = (AudioTranscription)ClientResult.FromResponse(response.Object);
        Assert.That(actual.Text, Is.EqualTo(content));
        Assert.That(actual.Language, Is.Null);
        Assert.That(actual.Duration, Is.Null);
        Assert.That(actual.Words, Is.Empty);
        Assert.That(actual.Segments, Is.Empty);
        response.Verify(value => value.Dispose(), Times.Once);
    }

    [TestCase("application/json")]
    [TestCase("application/json; charset=utf-8")]
    [TestCase(null)]
    public void ExplicitCastRetainsJsonDeserialization(string contentType)
    {
        Mock<PipelineResponse> response = MakeResponse(contentType, "{\"text\":\"Synthetic\",\"language\":\"en\",\"duration\":1.25}");
        AudioTranscription actual = (AudioTranscription)ClientResult.FromResponse(response.Object);
        Assert.That(actual.Text, Is.EqualTo("Synthetic"));
        Assert.That(actual.Language, Is.EqualTo("en"));
        Assert.That(actual.Duration, Is.EqualTo(TimeSpan.FromSeconds(1.25)));
        response.Verify(value => value.Dispose(), Times.Once);
    }

    [Test]
    public void InvalidJsonStillFailsAndDisposesTheResponse()
    {
        Mock<PipelineResponse> response = MakeResponse("application/json", "not JSON");
        Assert.Catch<JsonException>(() => { _ = (AudioTranscription)ClientResult.FromResponse(response.Object); });
        response.Verify(value => value.Dispose(), Times.Once);
    }

    private static Mock<PipelineResponse> MakeResponse(string contentType, string content)
    {
        Mock<PipelineResponseHeaders> headers = new();
        headers.Setup(value => value.TryGetValue("Content-Type", out contentType)).Returns(contentType is not null);
        Mock<PipelineResponse> response = new();
        response.SetupGet(value => value.Status).Returns(200);
        response.SetupGet(value => value.Content).Returns(BinaryData.FromString(content));
        response.Protected().SetupGet<PipelineResponseHeaders>("HeadersCore").Returns(headers.Object);
        return response;
    }
}
