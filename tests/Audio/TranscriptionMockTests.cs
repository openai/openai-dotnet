using System;
using System.ClientModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Tests.Utility;

namespace OpenAI.Tests.Audio;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Audio")]
[Category("Smoke")]
public partial class TranscriptionMockTests : SyncAsyncTestBase
{
    private static readonly ApiKeyCredential s_fakeCredential = new ApiKeyCredential("key");

    public TranscriptionMockTests(bool isAsync)
        : base(isAsync)
    {
    }

    public enum AudioSourceKind
    {
        UsingStream,
        UsingFilePath
    }

    [Test]
    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public async Task TranscribeAudioDeserializesLanguage(AudioSourceKind audioSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "language": "la"
        }
        """);
        AudioTranscription transcription = await InvokeTranscribeAudioSyncOrAsync(clientOptions, audioSourceKind);

        Assert.That(transcription.Language, Is.EqualTo("la"));
    }

    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public async Task TranscribeAudioDeserializesDuration(AudioSourceKind audioSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "duration": 185
        }
        """);
        AudioTranscription transcription = await InvokeTranscribeAudioSyncOrAsync(clientOptions, audioSourceKind);

        Assert.That(transcription.Duration, Is.EqualTo(TimeSpan.FromSeconds(185)));
    }

    [Test]
    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public async Task TranscribeAudioDeserializesText(AudioSourceKind audioSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "text": "The quick brown fox got lost."
        }
        """);
        AudioTranscription transcription = await InvokeTranscribeAudioSyncOrAsync(clientOptions, audioSourceKind);

        Assert.That(transcription.Text, Is.EqualTo("The quick brown fox got lost."));
    }

    [Test]
    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public async Task TranscribeAudioDeserializesWord(AudioSourceKind audioSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "words": [
                {
                    "word": "pneumonoultramicroscopicsilicovolcanoconiosis",
                    "start": 2.5,
                    "end": 7.5
                }
            ]
        }
        """);
        AudioTranscription transcription = await InvokeTranscribeAudioSyncOrAsync(clientOptions, audioSourceKind);
        TranscribedWord word = transcription.Words.Single();

        Assert.That(word.Word, Is.EqualTo("pneumonoultramicroscopicsilicovolcanoconiosis"));
        Assert.That(word.StartTime, Is.EqualTo(TimeSpan.FromSeconds(2.5)));
        Assert.That(word.EndTime, Is.EqualTo(TimeSpan.FromSeconds(7.5)));
    }

    [Test]
    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public async Task TranscribeAudioDeserializesSegment(AudioSourceKind audioSourceKind)
    {
        OpenAIClientOptions clientOptions = GetClientOptionsWithMockResponse(200, """
        {
            "segments": [
                {
                    "id": 15,
                    "seek": 50,
                    "start": 2.5,
                    "end": 7.5,
                    "text": "The quick brown fox got lost.",
                    "tokens": [
                        255, 305, 678
                    ],
                    "temperature": 0.8,
                    "avg_logprob": -0.3,
                    "compression_ratio": 1.5,
                    "no_speech_prob": 0.2
                }
            ]
        }
        """);
        AudioTranscription transcription = await InvokeTranscribeAudioSyncOrAsync(clientOptions, audioSourceKind);
        TranscribedSegment segment = transcription.Segments.Single();

        Assert.That(segment.Id, Is.EqualTo(15));
        Assert.That(segment.SeekOffset, Is.EqualTo(50));
        Assert.That(segment.StartTime, Is.EqualTo(TimeSpan.FromSeconds(2.5)));
        Assert.That(segment.EndTime, Is.EqualTo(TimeSpan.FromSeconds(7.5)));
        Assert.That(segment.Text, Is.EqualTo("The quick brown fox got lost."));
        Assert.That(segment.TokenIds.Span.SequenceEqual([255, 305, 678]), Is.True);
        Assert.That(segment.Temperature, Is.EqualTo(0.8f));
        Assert.That(segment.AverageLogProbability, Is.EqualTo(-0.3f));
        Assert.That(segment.CompressionRatio, Is.EqualTo(1.5f));
        Assert.That(segment.NoSpeechProbability, Is.EqualTo(0.2f));
    }

    [Test]
    public void TranscribeAudioFromStreamRespectsTheCancellationToken()
    {
        AudioClient client = new AudioClient("model", s_fakeCredential);
        using Stream stream = new MemoryStream();
        using CancellationTokenSource cancellationSource = new();
        cancellationSource.Cancel();

        if (IsAsync)
        {
            Assert.That(async () => await client.TranscribeAudioAsync(stream, "filename", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
        else
        {
            Assert.That(() => client.TranscribeAudio(stream, "filename", cancellationToken: cancellationSource.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }

    private OpenAIClientOptions GetClientOptionsWithMockResponse(int status, string content)
    {
        MockPipelineResponse response = new MockPipelineResponse(status);
        response.SetContent(content);

        return new OpenAIClientOptions()
        {
            Transport = new MockPipelineTransport(response)
        };
    }

    private async ValueTask<AudioTranscription> InvokeTranscribeAudioSyncOrAsync(OpenAIClientOptions clientOptions, AudioSourceKind audioSourceKind)
    {
        AudioClient client = new AudioClient("model", s_fakeCredential, clientOptions);
        string filename = "audio_french.wav";
        string path = Path.Combine("Assets", filename);

        if (audioSourceKind == AudioSourceKind.UsingStream)
        {
            using FileStream audio = File.OpenRead(path);

            return IsAsync
                ? await client.TranscribeAudioAsync(audio, filename)
                : client.TranscribeAudio(audio, filename);
        }
        else if (audioSourceKind == AudioSourceKind.UsingFilePath)
        {
            return IsAsync
                ? await client.TranscribeAudioAsync(path)
                : client.TranscribeAudio(path);
        }

        Assert.Fail("Invalid source kind.");
        return null;
    }
}
