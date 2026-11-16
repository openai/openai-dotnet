using NUnit.Framework;
using OpenAI.Responses;
using System;
using System.ClientModel.Primitives;

namespace OpenAI.Tests.Responses;

#pragma warning disable OPENAI001

[Category("Responses")]
public class StreamingResponseUpdateSerializationTests
{
    [Test]
    [TestCase("response.audio.transcript.delta")]
    [TestCase("response.audio_transcript.delta")]
    public void AudioTranscriptDelta_DeserializesCorrectly(string discriminator)
    {
        string json = $$"""
            {
                "type": "{{discriminator}}",
                "sequence_number": 3,
                "delta": "Hello world"
            }
            """;
        StreamingResponseUpdate update = ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(json));
        Assert.That(update, Is.Not.Null);
        Assert.That(update.Kind, Is.EqualTo(StreamingResponseUpdateKind.ResponseAudioTranscriptDelta));
        Assert.That(update.SequenceNumber, Is.EqualTo(3));
        Assert.That(update.GetType().Name, Is.EqualTo("StreamingResponseAudioTranscriptDeltaUpdate"));
    }

    [Test]
    [TestCase("response.audio.transcript.done")]
    [TestCase("response.audio_transcript.done")]
    public void AudioTranscriptDone_DeserializesCorrectly(string discriminator)
    {
        string json = $$"""
            {
                "type": "{{discriminator}}",
                "sequence_number": 5
            }
            """;
        StreamingResponseUpdate update = ModelReaderWriter.Read<StreamingResponseUpdate>(BinaryData.FromString(json));
        Assert.That(update, Is.Not.Null);
        Assert.That(update.Kind, Is.EqualTo(StreamingResponseUpdateKind.ResponseAudioTranscriptDone));
        Assert.That(update.SequenceNumber, Is.EqualTo(5));
        Assert.That(update.GetType().Name, Is.EqualTo("StreamingResponseAudioTranscriptDoneUpdate"));
    }

    [Test]
    public void AudioTranscriptKind_EqualitySupportsBothSpellingConventions()
    {
        StreamingResponseUpdateKind canonicalDelta = StreamingResponseUpdateKind.ResponseAudioTranscriptDelta;
        StreamingResponseUpdateKind dottedDelta = new("response.audio.transcript.delta");
        StreamingResponseUpdateKind legacyDelta = new("response.audio_transcript.delta");

        Assert.That(dottedDelta, Is.EqualTo(canonicalDelta));
        Assert.That(legacyDelta, Is.EqualTo(canonicalDelta));
        Assert.That(dottedDelta, Is.EqualTo(legacyDelta));
        Assert.That(dottedDelta == canonicalDelta, Is.True);
        Assert.That(legacyDelta == canonicalDelta, Is.True);
        Assert.That(dottedDelta.GetHashCode(), Is.EqualTo(canonicalDelta.GetHashCode()));
        Assert.That(legacyDelta.GetHashCode(), Is.EqualTo(canonicalDelta.GetHashCode()));

        StreamingResponseUpdateKind canonicalDone = StreamingResponseUpdateKind.ResponseAudioTranscriptDone;
        StreamingResponseUpdateKind dottedDone = new("response.audio.transcript.done");
        StreamingResponseUpdateKind legacyDone = new("response.audio_transcript.done");

        Assert.That(dottedDone, Is.EqualTo(canonicalDone));
        Assert.That(legacyDone, Is.EqualTo(canonicalDone));
        Assert.That(dottedDone, Is.EqualTo(legacyDone));
        Assert.That(dottedDone == canonicalDone, Is.True);
        Assert.That(legacyDone == canonicalDone, Is.True);
        Assert.That(dottedDone.GetHashCode(), Is.EqualTo(canonicalDone.GetHashCode()));
        Assert.That(legacyDone.GetHashCode(), Is.EqualTo(canonicalDone.GetHashCode()));
    }
}
