using Microsoft.ClientModel.TestFramework;
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

namespace OpenAI.Tests.Audio;

[Category("Audio")]
public partial class TranscriptionTests : OpenAIRecordedTestBase
{
    public TranscriptionTests(bool isAsync) : base(isAsync)
    {
    }

    public enum AudioSourceKind
    {
        UsingStream,
        UsingFilePath,
    }

    [RecordedTest]
    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public async Task TranscriptionWorks(AudioSourceKind audioSourceKind)
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Whisper);
        string filename = "audio_hello_world.mp3";
        string path = Path.Combine("Assets", filename);
        AudioTranscription transcription = null;

        if (audioSourceKind == AudioSourceKind.UsingStream)
        {
            using FileStream inputStream = File.OpenRead(path);

            transcription = await client.TranscribeAudioAsync(inputStream, filename);
        }
        else if (audioSourceKind == AudioSourceKind.UsingFilePath)
        {
            transcription = await client.TranscribeAudioAsync(path);
        }

        Assert.That(transcription, Is.Not.Null);
        Assert.That(transcription.Text.ToLowerInvariant(), Contains.Substring("hello"));
    }

    [RecordedTest]
    [TestCase(AudioTimestampGranularities.Default)]
    [TestCase(AudioTimestampGranularities.Word)]
    [TestCase(AudioTimestampGranularities.Segment)]
    [TestCase(AudioTimestampGranularities.Word | AudioTimestampGranularities.Segment)]
    public async Task TimestampsWork(AudioTimestampGranularities granularityFlags)
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Whisper);

        using FileStream inputStream = File.OpenRead(Path.Combine("Assets", "audio_hello_world.mp3"));

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Verbose,
            Temperature = 0.4f,
            TimestampGranularities = granularityFlags,
        };

        ClientResult<AudioTranscription> transcriptionResult = await client.TranscribeAudioAsync(inputStream, "audio_hello_world.mp3", options);

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

    [RecordedTest]
    [TestCase("text")]
    [TestCase("json")]
    [TestCase("verbose_json")]
    [TestCase("srt")]
    [TestCase("vtt")]
    [TestCase(null)]
    public async Task TranscriptionFormatsWork(string responseFormat)
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Whisper);
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

        AudioTranscription transcription = await client.TranscribeAudioAsync(path, options);

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

    [RecordedTest]
    public async Task TranscriptionUsageWorks()
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Gpt_4o_Mini_Transcribe);
        string path = Path.Combine("Assets", "audio_hello_world.mp3");

        AudioTranscription transcription = await client.TranscribeAudioAsync(path);

        Assert.That(transcription, Is.Not.Null);
        Assert.That(transcription.Text.ToLowerInvariant(), Contains.Substring("hello"));
        Assert.That(transcription.Usage, Is.Not.Null);

        if (transcription.Usage is AudioTranscriptionTokenUsage tokenUsage)
        {
            Assert.That(tokenUsage.TotalTokenCount, Is.GreaterThan(0));
            Assert.That(tokenUsage.InputTokenCount, Is.GreaterThanOrEqualTo(0));
            Assert.That(tokenUsage.OutputTokenCount, Is.GreaterThanOrEqualTo(0));
        }
        else if (transcription.Usage is AudioTranscriptionDurationUsage durationUsage)
        {
            Assert.That(durationUsage.Duration, Is.GreaterThan(TimeSpan.Zero));
        }
    }

    [RecordedTest]
    public async Task IncludesWork()
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Gpt_4o_Mini_Transcribe);
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

    [RecordedTest]
    public async Task StreamingIncludesWork()
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Gpt_4o_Mini_Transcribe);
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

    [RecordedTest]
    public async Task BadTranscriptionRequest()
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Whisper);

        string path = Path.Combine("Assets", "audio_hello_world.mp3");

        AudioTranscriptionOptions options = new AudioTranscriptionOptions()
        {
            Language = "this should cause an error"
        };

        Exception caughtException = null;

        try
        {
            await client.TranscribeAudioAsync(path, options);
        }
        catch (Exception ex)
        {
            caughtException = ex;
        }

        Assert.That(caughtException, Is.InstanceOf<ClientResultException>());
        Assert.That(caughtException.Message?.ToLower(), Contains.Substring("invalid language"));
    }

    [RecordedTest]
    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public async Task StreamingTranscriptionWorks(AudioSourceKind audioSourceKind)
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Gpt_4o_Mini_Transcribe);
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

        Assert.That(deltaBuilder.ToString().ToLower(), Does.Contain("this is a test"));

        inputStream?.Dispose();
    }

    [RecordedTest]
    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public void StreamingTranscriptionThrowsForWhisperModel(AudioSourceKind audioSourceKind)
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Whisper);
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

    [RecordedTest]
    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public async Task DiarizedTranscriptionWorks(AudioSourceKind audioSourceKind)
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Gpt_4o_Transcribe_Diarize);
        string filename = "audio_meeting.wav";
        string path = Path.Combine("Assets", filename);

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.DiarizedJson,
        };

        DiarizedAudioTranscription transcription = null;

        if (audioSourceKind == AudioSourceKind.UsingStream)
        {
            using FileStream inputStream = File.OpenRead(path);
            transcription = await client.TranscribeAudioDiarizedAsync(inputStream, filename, options);
        }
        else if (audioSourceKind == AudioSourceKind.UsingFilePath)
        {
            transcription = await client.TranscribeAudioDiarizedAsync(path, options);
        }

        Assert.That(transcription, Is.Not.Null);
        Assert.That(transcription.Text, Is.Not.Null.And.Not.Empty);
        Assert.That(transcription.Segments, Is.Not.Null);
        Assert.That(transcription.Segments.Count, Is.GreaterThan(0));

        foreach (DiarizedTranscriptionSegment segment in transcription.Segments)
        {
            Assert.That(segment.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(segment.Text, Is.Not.Null.And.Not.Empty);
            Assert.That(segment.Speaker, Is.Not.Null.And.Not.Empty);
            Assert.That(segment.EndTime, Is.GreaterThanOrEqualTo(segment.StartTime));
        }
    }

    [RecordedTest]
    public async Task DiarizedTranscriptionWithKnownSpeakersWorks()
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Gpt_4o_Transcribe_Diarize);
        string audioFilePath = Path.Combine("Assets", "audio_meeting.wav");
        string speakerRefPath = Path.Combine("Assets", "audio_agent_reference.wav");

        byte[] speakerRefBytes = File.ReadAllBytes(speakerRefPath);
        string speakerRefBase64 = Convert.ToBase64String(speakerRefBytes);

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.DiarizedJson,
            KnownSpeakerNames = { "agent" },
            KnownSpeakerReferences = { $"data:audio/wav;base64,{speakerRefBase64}" },
        };

        DiarizedAudioTranscription transcription = await client.TranscribeAudioDiarizedAsync(audioFilePath, options);

        Assert.That(transcription, Is.Not.Null);
        Assert.That(transcription.Text, Is.Not.Null.And.Not.Empty);
        Assert.That(transcription.Segments, Is.Not.Null);
        Assert.That(transcription.Segments.Count, Is.GreaterThan(0));

        bool foundKnownSpeaker = false;

        foreach (DiarizedTranscriptionSegment segment in transcription.Segments)
        {
            Assert.That(segment.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(segment.Text, Is.Not.Null.And.Not.Empty);
            Assert.That(segment.Speaker, Is.Not.Null.And.Not.Empty);
            Assert.That(segment.EndTime, Is.GreaterThanOrEqualTo(segment.StartTime));

            if (segment.Speaker == "agent")
            {
                foundKnownSpeaker = true;
            }
        }

        Assert.That(foundKnownSpeaker, Is.True, "Expected at least one segment attributed to the known speaker 'agent'.");
    }

    [RecordedTest]
    public async Task DiarizedTranscriptionHasUsage()
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Gpt_4o_Transcribe_Diarize);
        string path = Path.Combine("Assets", "audio_meeting.wav");

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.DiarizedJson,
        };

        DiarizedAudioTranscription transcription = await client.TranscribeAudioDiarizedAsync(path, options);

        Assert.That(transcription, Is.Not.Null);
        Assert.That(transcription.Usage, Is.Not.Null);

        if (transcription.Usage is AudioTranscriptionTokenUsage tokenUsage)
        {
            Assert.That(tokenUsage.TotalTokenCount, Is.GreaterThan(0));
            Assert.That(tokenUsage.InputTokenCount, Is.GreaterThanOrEqualTo(0));
            Assert.That(tokenUsage.OutputTokenCount, Is.GreaterThanOrEqualTo(0));
        }
        else if (transcription.Usage is AudioTranscriptionDurationUsage durationUsage)
        {
            Assert.That(durationUsage.Duration, Is.GreaterThan(TimeSpan.Zero));
        }
    }

    [RecordedTest]
    public async Task DiarizedTranscriptionSegmentsAreOrdered()
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Gpt_4o_Transcribe_Diarize);
        string path = Path.Combine("Assets", "audio_meeting.wav");

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.DiarizedJson,
        };

        DiarizedAudioTranscription transcription = await client.TranscribeAudioDiarizedAsync(path, options);

        Assert.That(transcription, Is.Not.Null);
        Assert.That(transcription.Segments, Is.Not.Null);
        Assert.That(transcription.Segments.Count, Is.GreaterThan(1));

        for (int i = 1; i < transcription.Segments.Count; i++)
        {
            Assert.That(
                transcription.Segments[i].StartTime,
                Is.GreaterThanOrEqualTo(transcription.Segments[i - 1].StartTime),
                $"Segment {i} starts before segment {i - 1}.");
        }
    }

    [RecordedTest]
    [TestCase(AudioSourceKind.UsingStream)]
    [TestCase(AudioSourceKind.UsingFilePath)]
    public async Task StreamingDiarizedTranscriptionWorks(AudioSourceKind audioSourceKind)
    {
        AudioClient client = GetProxiedOpenAIClient<AudioClient>(TestModel.Audio_Gpt_4o_Transcribe_Diarize);
        string filename = "audio_meeting.wav";
        string path = Path.Combine("Assets", filename);

        FileStream inputStream = null;

        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.DiarizedJson,
        };

        AsyncCollectionResult<StreamingAudioTranscriptionUpdate> streamingUpdates = null;

        if (audioSourceKind == AudioSourceKind.UsingStream)
        {
            inputStream = File.OpenRead(path);
            streamingUpdates = client.TranscribeAudioStreamingAsync(inputStream, filename, options);
        }
        else if (audioSourceKind == AudioSourceKind.UsingFilePath)
        {
            streamingUpdates = client.TranscribeAudioStreamingAsync(path, options);
        }

        string doneText = null;
        List<StreamingAudioTranscriptionTextSegmentUpdate> segments = [];

        await foreach (StreamingAudioTranscriptionUpdate update in streamingUpdates)
        {
            if (update is StreamingAudioTranscriptionTextDoneUpdate doneUpdate)
            {
                Assert.That(doneUpdate.Text, Is.Not.Null.And.Not.Empty);
                doneText = doneUpdate.Text;
            }
            else if (update is StreamingAudioTranscriptionTextSegmentUpdate segmentUpdate)
            {
                segments.Add(segmentUpdate);
            }
        }

        Assert.That(doneText, Is.Not.Null.And.Not.Empty);
        Assert.That(segments, Has.Count.GreaterThan(0));

        foreach (StreamingAudioTranscriptionTextSegmentUpdate segment in segments)
        {
            Assert.That(segment.SegmentId, Is.Not.Null.And.Not.Empty);
            Assert.That(segment.Text, Is.Not.Null.And.Not.Empty);
            Assert.That(segment.SpeakerLabel, Is.Not.Null.And.Not.Empty);
            Assert.That(segment.EndTime, Is.GreaterThanOrEqualTo(segment.StartTime));
        }

        inputStream?.Dispose();
    }
}
