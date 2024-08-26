using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Tests.Utility;
using System;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Audio;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Audio")]
public partial class TextToSpeechTests : SyncAsyncTestBase
{
    public TextToSpeechTests(bool isAsync) : base(isAsync)
    {
    }

    [Test]
    public async Task BasicTextToSpeechWorks()
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_TTS);

        BinaryData audio = IsAsync
            ? await client.GenerateSpeechAsync("Hello, world! This is a test.", GeneratedSpeechVoice.Shimmer)
            : client.GenerateSpeech("Hello, world! This is a test.", GeneratedSpeechVoice.Shimmer);

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
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_TTS);

        SpeechGenerationOptions options = responseFormat == null
            ? new()
            : new() { ResponseFormat = responseFormat };

        BinaryData audio = IsAsync
            ? await client.GenerateSpeechAsync("Hello, world!", GeneratedSpeechVoice.Alloy, options)
            : client.GenerateSpeech("Hello, world!", GeneratedSpeechVoice.Alloy, options);

        Assert.That(audio, Is.Not.Null);
    }

    private void ValidateGeneratedAudio(BinaryData audio, string expectedSubstring)
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_Whisper);
        AudioTranscription transcription = client.TranscribeAudio(audio.ToStream(), "hello_world.wav");

        Assert.That(transcription.Text.ToLowerInvariant(), Contains.Substring(expectedSubstring));
    }
}
