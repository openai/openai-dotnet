using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Tests.Utility;
using System;
using System.Text;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Audio;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Audio")]
public partial class GenerateSpeechTests : SyncAsyncTestBase
{
    public GenerateSpeechTests(bool isAsync) : base(isAsync)
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
    [TestCase("mp3")]
    [TestCase("opus")]
    [TestCase("aac")]
    [TestCase("flac")]
    [TestCase("wav")]
    [TestCase("pcm")]
    public async Task OutputFormatWorks(string responseFormat)
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_TTS);

        SpeechGenerationOptions options = new();

        if (!string.IsNullOrEmpty(responseFormat))
        {
            options.ResponseFormat = responseFormat switch
            {
                "mp3" => GeneratedSpeechFormat.Mp3,
                "opus" => GeneratedSpeechFormat.Opus,
                "aac" => GeneratedSpeechFormat.Aac,
                "flac" => GeneratedSpeechFormat.Flac,
                "wav" => GeneratedSpeechFormat.Wav,
                "pcm" => GeneratedSpeechFormat.Pcm,
                _ => throw new ArgumentException("Invalid response format")
            };
        }

        BinaryData audio = IsAsync
            ? await client.GenerateSpeechAsync("Hello, world!", GeneratedSpeechVoice.Alloy, options)
            : client.GenerateSpeech("Hello, world!", GeneratedSpeechVoice.Alloy, options);

        Assert.That(audio, Is.Not.Null);

        byte[] audioBytes = audio.ToArray();
        byte[] expectedFileHeader = responseFormat switch
        {
            "opus" => Encoding.ASCII.GetBytes("OggS"),
            "flac" => Encoding.ASCII.GetBytes("fLaC"),
            "wav" => Encoding.ASCII.GetBytes("RIFF"),
            _ => []
        };

        Assert.That(audioBytes.Length, Is.GreaterThanOrEqualTo(expectedFileHeader.Length));

        for (int i = 0; i < expectedFileHeader.Length; i++)
        {
            Assert.That(audioBytes[i], Is.EqualTo(expectedFileHeader[i]), $"File header differs on byte {i}. Expected: {expectedFileHeader[i]}. Actual: {audioBytes[i]}.");
        }
    }

    private void ValidateGeneratedAudio(BinaryData audio, string expectedSubstring)
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_Whisper);
        AudioTranscription transcription = client.TranscribeAudio(audio.ToStream(), "hello_world.wav");

        Assert.That(transcription.Text.ToLowerInvariant(), Contains.Substring(expectedSubstring));
    }
}
