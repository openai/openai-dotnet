using NUnit.Framework;
using OpenAI.Audio;
using OpenAI.Tests.Utility;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Audio;

[TestFixture(true)]
[TestFixture(false)]
[Parallelizable(ParallelScope.All)]
[Category("Audio")]
public partial class TranscriptionTests : SyncAsyncTestBase
{
    public TranscriptionTests(bool isAsync) : base(isAsync)
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
    public async Task TranscriptionWorks(AudioSourceKind audioSourceKind)
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_Whisper);
        string filename = "audio_hello_world.mp3";
        string path = Path.Combine("Assets", filename);
        AudioTranscription transcription = null;

        if (audioSourceKind == AudioSourceKind.UsingStream)
        {
            using FileStream inputStream = File.OpenRead(path);

            transcription = IsAsync
                ? await client.TranscribeAudioAsync(inputStream, filename)
                : client.TranscribeAudio(inputStream, filename);
        }
        else if (audioSourceKind == AudioSourceKind.UsingFilePath)
        {
            transcription = IsAsync
                ? await client.TranscribeAudioAsync(path)
                : client.TranscribeAudio(path);
        }

        Assert.That(transcription, Is.Not.Null);
        Assert.That(transcription.Text.ToLowerInvariant(), Contains.Substring("hello"));
    }

    [Test]
    [TestCase(AudioTimestampGranularities.Default)]
    [TestCase(AudioTimestampGranularities.Word)]
    [TestCase(AudioTimestampGranularities.Segment)]
    [TestCase(AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment)]
    public async Task TimestampsWork(AudioTimestampGranularities granularityFlags)
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_Whisper);

        using FileStream inputStream = File.OpenRead(Path.Combine("Assets", "audio_hello_world.mp3"));

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Verbose,
            Temperature = 0.4f,
            TimestampGranularities = granularityFlags,
        };

        ClientResult<AudioTranscription> transcriptionResult = IsAsync
            ? await client.TranscribeAudioAsync(inputStream, "audio_hello_world.mp3", options)
            : client.TranscribeAudio(inputStream, "audio_hello_world.mp3", options);

        PipelineResponse rawResponse = transcriptionResult.GetRawResponse();
        Assert.That(rawResponse.Content.ToString(), Is.Not.Null.And.Not.Empty);

        AudioTranscription transcription = transcriptionResult;
        Assert.That(transcription, Is.Not.Null);

        IReadOnlyList<TranscribedWord> words = transcription.Words;
        IReadOnlyList<TranscribedSegment> segments = transcription.Segments;

        bool wordTimestampsPresent = words?.Count > 0;
        bool segmentTimestampsPresent = segments?.Count > 0;

        bool wordTimestampsExpected = granularityFlags.HasFlag(AudioTimestampGranularities.Word);
        bool segmentTimestampsExpected = granularityFlags.HasFlag(AudioTimestampGranularities.Segment)
            || granularityFlags == AudioTimestampGranularities.Default;

        Assert.That(wordTimestampsPresent, Is.EqualTo(wordTimestampsExpected));
        Assert.That(segmentTimestampsPresent, Is.EqualTo(segmentTimestampsExpected));

        for (int i = 0; i < (words?.Count ?? 0); i++)
        {
            if (i > 0)
            {
                Assert.That(words[i].StartTime, Is.GreaterThanOrEqualTo(words[i - 1].EndTime));
            }
            Assert.That(words[i].EndTime, Is.GreaterThan(words[i].StartTime));
            Assert.That(string.IsNullOrEmpty(words[i].Word), Is.False);
        }

        for (int i = 0; i < (segments?.Count ?? 0); i++)
        {
            if (i > 0)
            {
                Assert.That(segments[i].Id, Is.GreaterThan(segments[i - 1].Id));
                Assert.That(segments[i].SeekOffset, Is.GreaterThan(0));
                Assert.That(segments[i].StartTime, Is.GreaterThanOrEqualTo(segments[i - 1].EndTime));
            }
            Assert.That(segments[i].EndTime, Is.GreaterThan(segments[i].StartTime));
            Assert.That(string.IsNullOrEmpty(segments[i].Text), Is.False);
            Assert.That(segments[i].TokenIds.Span.Length, Is.GreaterThan(0));
            foreach (int tokenId in segments[i].TokenIds.ToArray())
            {
                Assert.That(tokenId, Is.GreaterThanOrEqualTo(0));
            }
            Assert.That(segments[i].Temperature, Is.LessThan(-0.001f).Or.GreaterThan(0.001f));
            Assert.That(segments[i].AverageLogProbability, Is.LessThan(-0.001f).Or.GreaterThan(0.001f));
            Assert.That(segments[i].CompressionRatio, Is.LessThan(-0.001f).Or.GreaterThan(0.001f));
            Assert.That(segments[i].NoSpeechProbability, Is.LessThan(-0.001f).Or.GreaterThan(0.001f));
        }
    }

    [Test]
    [TestCase("text")]
    [TestCase("json")]
    [TestCase("verbose_json")]
    [TestCase("srt")]
    [TestCase("vtt")]
    [TestCase(null)]
    public async Task TranscriptionFormatsWork(string responseFormat)
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_Whisper);
        string path = Path.Combine("Assets", "audio_hello_world.mp3");

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = responseFormat switch
            {
                "text" => AudioTranscriptionFormat.Text,
                "json" => AudioTranscriptionFormat.Simple,
                "verbose_json" => AudioTranscriptionFormat.Verbose,
                "srt" => AudioTranscriptionFormat.Srt,
                "vtt" => AudioTranscriptionFormat.Vtt,
                _ => (AudioTranscriptionFormat?)null
            }
        };

        AudioTranscription transcription = IsAsync
            ? await client.TranscribeAudioAsync(path, options)
            : client.TranscribeAudio(path, options);

        Assert.That(transcription?.Text?.ToLowerInvariant(), Does.Contain("hello"));


        if (options.ResponseFormat == AudioTranscriptionFormat.Verbose)
        {
            Assert.That(transcription.Language, Is.EqualTo("english"));
            Assert.That(transcription.Duration, Is.GreaterThan(TimeSpan.Zero));
            Assert.That(transcription.Segments, Is.Not.Empty);

            for (int i = 0; i < transcription.Segments.Count; i++)
            {
                TranscribedSegment segment = transcription.Segments[i];

                if (i > 0)
                {
                    Assert.That(segment.StartTime, Is.GreaterThanOrEqualTo(transcription.Segments[i - 1].EndTime));
                }

                Assert.That(segment.Id, Is.EqualTo(i));
                Assert.That(segment.EndTime, Is.GreaterThanOrEqualTo(segment.StartTime));
                Assert.That(segment.TokenIds, Is.Not.Null);
                Assert.That(segment.TokenIds.Length, Is.GreaterThan(0));

                Assert.That(segment.AverageLogProbability, Is.LessThan(-0.001f).Or.GreaterThan(0.001f));
                Assert.That(segment.CompressionRatio, Is.LessThan(-0.001f).Or.GreaterThan(0.001f));
                Assert.That(segment.NoSpeechProbability, Is.LessThan(-0.001f).Or.GreaterThan(0.001f));
            }
        }
        else
        {
            Assert.That(transcription.Duration, Is.Null);
            Assert.That(transcription.Language, Is.Null);
            Assert.That(transcription.Segments, Is.Not.Null.And.Empty);
            Assert.That(transcription.Words, Is.Not.Null.And.Empty);
        }
    }

    [Test]
    public async Task IncludesWork()
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_Gpt_4o_Mini_Transcribe);
        string filename = "audio_hello_world.mp3";
        string path = Path.Combine("Assets", filename);

        AudioTranscription transcription = await client.TranscribeAudioAsync(path, new AudioTranscriptionOptions()
        {
            Includes = AudioTranscriptionIncludes.Logprobs,
        });

        Assert.That(transcription.TranscriptionTokenLogProbabilities, Has.Count.GreaterThan(0));
        Assert.That(transcription.TranscriptionTokenLogProbabilities[0].Token, Is.Not.Null.And.Not.Empty);
        Assert.That(transcription.TranscriptionTokenLogProbabilities[0].LogProbability, Is.Not.EqualTo(0));
        Assert.That(transcription.TranscriptionTokenLogProbabilities[0].Utf8Bytes.ToArray(), Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task StreamingIncludesWork()
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_Gpt_4o_Mini_Transcribe);
        string filename = "audio_hello_world.mp3";
        string path = Path.Combine("Assets", filename);

        List<AudioTokenLogProbabilityDetails> streamedDeltaLogProbs = [];

        await foreach (StreamingAudioTranscriptionUpdate update
            in client.TranscribeAudioStreamingAsync(
                path,
                new AudioTranscriptionOptions()
                {
                    Includes = AudioTranscriptionIncludes.Logprobs,
                }))
        {
            if (update is StreamingAudioTranscriptionTextDeltaUpdate deltaUpdate)
            {
                Assert.That(deltaUpdate.TranscriptionTokenLogProbabilities, Is.Not.Null);
                streamedDeltaLogProbs.AddRange(deltaUpdate.TranscriptionTokenLogProbabilities);
            }
            else if (update is StreamingAudioTranscriptionTextDoneUpdate doneUpdate)
            {
                Assert.That(doneUpdate.TranscriptionTokenLogProbabilities, Has.Count.GreaterThan(0));
                Assert.That(doneUpdate.TranscriptionTokenLogProbabilities.Count, Is.EqualTo(streamedDeltaLogProbs.Count));
                Assert.That(doneUpdate.TranscriptionTokenLogProbabilities[0].Token, Is.Not.Null.And.Not.Empty);
                Assert.That(doneUpdate.TranscriptionTokenLogProbabilities[0].Token, Is.EqualTo(streamedDeltaLogProbs[0].Token));
            }
        }

        Assert.That(streamedDeltaLogProbs, Has.Count.GreaterThan(0));
    }

    [Test]
    public async Task BadTranscriptionRequest()
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_Whisper);

        string path = Path.Combine("Assets", "audio_hello_world.mp3");

        AudioTranscriptionOptions options = new AudioTranscriptionOptions()
        {
            Language = "this should cause an error"
        };

        Exception caughtException = null;

        try
        {
            _ = IsAsync
                ? await client.TranscribeAudioAsync(path, options)
                : client.TranscribeAudio(path, options);
        }
        catch (Exception ex)
        {
            caughtException = ex;
        }

        Assert.That(caughtException, Is.InstanceOf<ClientResultException>());
        Assert.That(caughtException.Message?.ToLower(), Contains.Substring("invalid language"));
    }

    [Test]
    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public async Task StreamingTranscriptionWorks(AudioSourceKind audioSourceKind)
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_Gpt_4o_Mini_Transcribe);
        string filename = "audio_hello_world.mp3";
        string path = Path.Combine("Assets", filename);

        FileStream inputStream = null;

        AsyncCollectionResult<StreamingAudioTranscriptionUpdate> streamingUpdates = null;

        if (audioSourceKind == AudioSourceKind.UsingStream)
        {
            inputStream = File.OpenRead(path);
            streamingUpdates = client.TranscribeAudioStreamingAsync(inputStream, filename);
        }
        else if (audioSourceKind == AudioSourceKind.UsingFilePath)
        {
            streamingUpdates = client.TranscribeAudioStreamingAsync(path);
        }

        StringBuilder deltaBuilder = new();

        await foreach (StreamingAudioTranscriptionUpdate update in streamingUpdates)
        {
            if (update is StreamingAudioTranscriptionTextDeltaUpdate deltaUpdate)
            {
                deltaBuilder.Append(deltaUpdate.Delta);
                Assert.That(deltaUpdate.TranscriptionTokenLogProbabilities, Has.Count.EqualTo(0));
            }
            else if (update is StreamingAudioTranscriptionTextDoneUpdate doneUpdate)
            {
                Assert.That(doneUpdate.Text, Is.Not.Null.And.Not.Empty);
                Assert.That(doneUpdate.Text, Is.EqualTo(deltaBuilder.ToString()));
                Assert.That(doneUpdate.TranscriptionTokenLogProbabilities, Has.Count.EqualTo(0));
            }
        }

        Assert.That(deltaBuilder.ToString().ToLower(), Does.Contain("hello world"));

        inputStream?.Dispose();
    }

    [Test]
    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public void StreamingTranscriptionThrowsForWhisperModel(AudioSourceKind audioSourceKind)
    {
        AudioClient client = GetTestClient<AudioClient>(TestScenario.Audio_Whisper);
        string filename = "audio_hello_world.mp3";
        string path = Path.Combine("Assets", filename);

        if (audioSourceKind == AudioSourceKind.UsingStream)
        {
            using FileStream inputStream = File.OpenRead(path);
            Assert.Throws<NotSupportedException>(() =>
            {
                _ = client.TranscribeAudioStreamingAsync(inputStream, filename);
            });
        }
        else if (audioSourceKind == AudioSourceKind.UsingFilePath)
        {
            Assert.Throws<NotSupportedException>(() =>
            {
                _ = client.TranscribeAudioStreamingAsync(path);
            });
        }
    }
}
