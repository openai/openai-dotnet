using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Tests.Utility;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.Tests.Audio;

[Category("Audio")]
public partial class GenerateSpeechTests : OpenAIRecordedTestBase
{
    public GenerateSpeechTests(bool isAsync) : base(isAsync)
    {
    }

    [RecordedTest]
    public async Task BasicTextToSpeechWorks()
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_TTS);

        BinaryData audio = await client.GenerateSpeechAsync("Hello, world! This is a test.", GeneratedSpeechVoice.Shimmer);

        Assert.That(audio, Is.Not.Null);
        await ValidateGeneratedAudio(audio, "hello");
    }

    [RecordedTest]
    [TestCase(null)]
    [TestCase("mp3")]
    [TestCase("opus")]
    [TestCase("aac")]
    [TestCase("flac")]
    [TestCase("wav")]
    [TestCase("pcm")]
    public async Task OutputFormatWorks(string responseFormat)
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_TTS);

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

        BinaryData audio = await client.GenerateSpeechAsync("Hello, world!", GeneratedSpeechVoice.Alloy, options);

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

    private async Task ValidateGeneratedAudio(BinaryData audio, string expectedSubstring)
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Whisper);
        AudioTranscription transcription = await client.TranscribeAudioAsync(audio.ToStream(), "hello_world.wav");

        Assert.That(transcription.Text.ToLowerInvariant(), Contains.Substring(expectedSubstring));
    }
}
