﻿using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Tests.Utility;
using System.IO;
using System.Threading.Tasks;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Audio;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Audio")]
public partial class TranslationTests : SyncAsyncTestBase
{
    public TranslationTests(bool isAsync) : base(isAsync)
    {
    }

    public enum AudioSourceKind
    {
        UsingStream,
        UsingFilePath,
    }

    [Test]
    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public async Task TranslationWorks(AudioSourceKind audioSourceKind)
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_Whisper);

        string filename = "audio_french.wav";
        string path = Path.Combine("Assets", filename);
        AudioTranslation translation = null;

        if (audioSourceKind == AudioSourceKind.UsingStream)
        {
            using FileStream audio = File.OpenRead(path);

            translation = IsAsync
                ? await client.TranslateAudioAsync(audio, filename)
                : client.TranslateAudio(audio, filename);
        }
        else if (audioSourceKind == AudioSourceKind.UsingFilePath)
        {
            translation = IsAsync
                ? await client.TranslateAudioAsync(path)
                : client.TranslateAudio(path);
        }
        Assert.That(translation?.Text, Is.Not.Null);
        Assert.That(translation.Text.ToLowerInvariant(), Contains.Substring("whisper"));
    }

    [Test]
    [TestCase(AudioTranslationFormat.Simple)]
    [TestCase(AudioTranslationFormat.Verbose)]
    [TestCase(AudioTranslationFormat.Srt)]
    [TestCase(AudioTranslationFormat.Vtt)]
    public async Task TranslationFormatsWork(AudioTranslationFormat formatToTest)
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_Whisper);
        string path = Path.Combine("Assets", "audio_french.wav");

        AudioTranslationOptions options = new()
        {
            ResponseFormat = formatToTest,
        };

        AudioTranslation translation = IsAsync
            ? await client.TranslateAudioAsync(path, options)
            : client.TranslateAudio(path, options);

        Assert.That(translation?.Text?.ToLowerInvariant(), Does.Contain("recognition"));
    }
}
