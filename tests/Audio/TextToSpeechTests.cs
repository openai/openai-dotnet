using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Audio;

[TestFixture(true)]
[TestFixture(false)]
public partial class TextToSpeechTests : SyncAsyncTestBase
{
    public TextToSpeechTests(bool isAsync)
        : base(isAsync)
    {
    }

    [Test]
    public async Task BasicTextToSpeechWorks()
    {
        AudioClient client = new("tts-1");

        BinaryData audio = IsAsync
            ? await client.GenerateSpeechFromTextAsync("Hello, world! This is a test.", GeneratedSpeechVoice.Shimmer)
            : client.GenerateSpeechFromText("Hello, world! This is a test.", GeneratedSpeechVoice.Shimmer);

        Assert.That(audio, Is.Not.Null);
        ValidateGeneratedAudio(audio, "hello");
    }

    [Test]
    [TestCase(null)]
    [TestCase(GeneratedSpeechFormat.Mp3)]
    [TestCase(GeneratedSpeechFormat.Opus)]
    [TestCase(GeneratedSpeechFormat.Aac)]
    [TestCase(GeneratedSpeechFormat.Flac)]
    [TestCase(GeneratedSpeechFormat.Wav)]
    [TestCase(GeneratedSpeechFormat.Pcm)]
    public async Task OutputFormatWorks(GeneratedSpeechFormat? responseFormat)
    {
        AudioClient client = new("tts-1");

        SpeechGenerationOptions options = responseFormat == null
            ? new()
            : new() { ResponseFormat = responseFormat };

        BinaryData audio = IsAsync
            ? await client.GenerateSpeechFromTextAsync("Hello, world!", GeneratedSpeechVoice.Alloy, options)
            : client.GenerateSpeechFromText("Hello, world!", GeneratedSpeechVoice.Alloy, options);

        Assert.That(audio, Is.Not.Null);
    }

    private void ValidateGeneratedAudio(BinaryData audio, string expectedSubstring)
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Transcription);
        AudioTranscription transcription = client.TranscribeAudio(audio.ToStream(), "hello_world.wav");

        Assert.That(transcription.Text.ToLowerInvariant(), Contains.Substring(expectedSubstring));
    }
}
