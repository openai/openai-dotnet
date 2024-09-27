using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenAI.Audio;

namespace OpenAI.Tests.Audio;

[Parallelizable(ParallelScope.All)]
[Category("Audio")]
[Category("Smoke")]
public partial class OpenAIAudioModelFactoryTests
{
    [Test]
    public void AudioTranscriptionWithNoPropertiesWorks()
    {
        AudioTranscription audioTranscription = OpenAIAudioModelFactory.AudioTranscription();

        Assert.That(audioTranscription.Language, Is.Null);
        Assert.That(audioTranscription.Duration, Is.Null);
        Assert.That(audioTranscription.Text, Is.Null);
        Assert.That(audioTranscription.Words, Is.Not.Null.And.Empty);
        Assert.That(audioTranscription.Segments, Is.Not.Null.And.Empty);
    }

    [Test]
    public void AudioTranscriptionWithLanguageWorks()
    {
        string language = "esperanto";
        AudioTranscription audioTranscription = OpenAIAudioModelFactory.AudioTranscription(language: language);

        Assert.That(audioTranscription.Language, Is.EqualTo(language));
        Assert.That(audioTranscription.Duration, Is.Null);
        Assert.That(audioTranscription.Text, Is.Null);
        Assert.That(audioTranscription.Words, Is.Not.Null.And.Empty);
        Assert.That(audioTranscription.Segments, Is.Not.Null.And.Empty);
    }

    [Test]
    public void AudioTranscriptionWithDurationWorks()
    {
        TimeSpan duration = TimeSpan.FromSeconds(45);
        AudioTranscription audioTranscription = OpenAIAudioModelFactory.AudioTranscription(duration: duration);

        Assert.That(audioTranscription.Language, Is.Null);
        Assert.That(audioTranscription.Duration, Is.EqualTo(duration));
        Assert.That(audioTranscription.Text, Is.Null);
        Assert.That(audioTranscription.Words, Is.Not.Null.And.Empty);
        Assert.That(audioTranscription.Segments, Is.Not.Null.And.Empty);
    }

    [Test]
    public void AudioTranscriptionWithTextWorks()
    {
        string text = "I've been transcripted.";
        AudioTranscription audioTranscription = OpenAIAudioModelFactory.AudioTranscription(text: text);

        Assert.That(audioTranscription.Language, Is.Null);
        Assert.That(audioTranscription.Duration, Is.Null);
        Assert.That(audioTranscription.Text, Is.EqualTo(text));
        Assert.That(audioTranscription.Words, Is.Not.Null.And.Empty);
        Assert.That(audioTranscription.Segments, Is.Not.Null.And.Empty);
    }

    [Test]
    public void AudioTranscriptionWithWordsWorks()
    {
        IEnumerable<TranscribedWord> words = [
            OpenAIAudioModelFactory.TranscribedWord(word: "Apple"),
            OpenAIAudioModelFactory.TranscribedWord(word: "pie")
        ];
        AudioTranscription audioTranscription = OpenAIAudioModelFactory.AudioTranscription(words: words);

        Assert.That(audioTranscription.Language, Is.Null);
        Assert.That(audioTranscription.Duration, Is.Null);
        Assert.That(audioTranscription.Text, Is.Null);
        Assert.That(audioTranscription.Words.SequenceEqual(words), Is.True);
        Assert.That(audioTranscription.Segments, Is.Not.Null.And.Empty);
    }

    [Test]
    public void AudioTranscriptionWithSegmentsWorks()
    {
        IEnumerable<TranscribedSegment> segments = [
            OpenAIAudioModelFactory.TranscribedSegment(id: 1),
            OpenAIAudioModelFactory.TranscribedSegment(id: 2)
        ];
        AudioTranscription audioTranscription = OpenAIAudioModelFactory.AudioTranscription(segments: segments);

        Assert.That(audioTranscription.Language, Is.Null);
        Assert.That(audioTranscription.Duration, Is.Null);
        Assert.That(audioTranscription.Text, Is.Null);
        Assert.That(audioTranscription.Words, Is.Not.Null.And.Empty);
        Assert.That(audioTranscription.Segments.SequenceEqual(segments), Is.True);
    }

    [Test]
    public void AudioTranslationWithNoPropertiesWorks()
    {
        AudioTranslation audioTranslation = OpenAIAudioModelFactory.AudioTranslation();

        Assert.That(audioTranslation.Language, Is.Null);
        Assert.That(audioTranslation.Duration, Is.Null);
        Assert.That(audioTranslation.Text, Is.Null);
        Assert.That(audioTranslation.Segments, Is.Not.Null.And.Empty);
    }

    [Test]
    public void AudioTranslationWithLanguageWorks()
    {
        string language = "esperanto";
        AudioTranslation audioTranslation = OpenAIAudioModelFactory.AudioTranslation(language: language);

        Assert.That(audioTranslation.Language, Is.EqualTo(language));
        Assert.That(audioTranslation.Duration, Is.Null);
        Assert.That(audioTranslation.Text, Is.Null);
        Assert.That(audioTranslation.Segments, Is.Not.Null.And.Empty);
    }

    [Test]
    public void AudioTranslationWithDurationWorks()
    {
        TimeSpan duration = TimeSpan.FromSeconds(45);
        AudioTranslation audioTranslation = OpenAIAudioModelFactory.AudioTranslation(duration: duration);

        Assert.That(audioTranslation.Language, Is.Null);
        Assert.That(audioTranslation.Duration, Is.EqualTo(duration));
        Assert.That(audioTranslation.Text, Is.Null);
        Assert.That(audioTranslation.Segments, Is.Not.Null.And.Empty);
    }

    [Test]
    public void AudioTranslationWithTextWorks()
    {
        string text = "I've been transcripted.";
        AudioTranslation audioTranslation = OpenAIAudioModelFactory.AudioTranslation(text: text);

        Assert.That(audioTranslation.Language, Is.Null);
        Assert.That(audioTranslation.Duration, Is.Null);
        Assert.That(audioTranslation.Text, Is.EqualTo(text));
        Assert.That(audioTranslation.Segments, Is.Not.Null.And.Empty);
    }

    [Test]
    public void AudioTranslationWithSegmentsWorks()
    {
        IEnumerable<TranscribedSegment> segments = [
            OpenAIAudioModelFactory.TranscribedSegment(id: 1),
            OpenAIAudioModelFactory.TranscribedSegment(id: 2)
        ];
        AudioTranslation audioTranslation = OpenAIAudioModelFactory.AudioTranslation(segments: segments);

        Assert.That(audioTranslation.Language, Is.Null);
        Assert.That(audioTranslation.Duration, Is.Null);
        Assert.That(audioTranslation.Text, Is.Null);
        Assert.That(audioTranslation.Segments.SequenceEqual(segments), Is.True);
    }

    [Test]
    public void TranscribedSegmentWithNoPropertiesWorks()
    {
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment();

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.EndTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds.Span.Length, Is.GreaterThanOrEqualTo(0));
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(float)));
    }

    [Test]
    public void TranscribedSegmentWithIdWorks()
    {
        int id = 10;
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(id: id);

        Assert.That(transcribedSegment.Id, Is.EqualTo(id));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.EndTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds.Span.Length, Is.GreaterThanOrEqualTo(0));
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(float)));
    }

    [Test]
    public void TranscribedSegmentWithSeekOffsetWorks()
    {
        int seekOffset = 900000000;
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(seekOffset: seekOffset);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(seekOffset));
        Assert.That(transcribedSegment.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.EndTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds.Span.Length, Is.GreaterThanOrEqualTo(0));
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(float)));
    }

    [Test]
    public void TranscribedSegmentWithStartWorks()
    {
        TimeSpan startTime = TimeSpan.FromSeconds(45);
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(startTime: startTime);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.StartTime, Is.EqualTo(startTime));
        Assert.That(transcribedSegment.EndTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds.Span.Length, Is.GreaterThanOrEqualTo(0));
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(float)));
    }

    [Test]
    public void TranscribedSegmentWithEndWorks()
    {
        TimeSpan endTime = TimeSpan.FromSeconds(45);
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(endTime: endTime);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.EndTime, Is.EqualTo(endTime));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds.Span.Length, Is.GreaterThanOrEqualTo(0));
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(float)));
    }

    [Test]
    public void TranscribedSegmentWithTextWorks()
    {
        string text = "A segment text";
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(text: text);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.EndTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.EqualTo(text));
        Assert.That(transcribedSegment.TokenIds.Span.Length, Is.GreaterThanOrEqualTo(0));
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(float)));
    }

    [Test]
    public void TranscribedSegmentWithTokenIdsWorks()
    {
        ReadOnlyMemory<int> tokenIds = new[] { 900000000, 900000001 };
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(tokenIds: tokenIds);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.EndTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds.Span.SequenceEqual(tokenIds.Span), Is.True);
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(float)));
    }

    [Test]
    public void TranscribedSegmentWithTemperatureWorks()
    {
        float temperature = 0.232f;
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(temperature: temperature);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.EndTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds.Span.Length, Is.GreaterThanOrEqualTo(0));
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(temperature));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(float)));
    }

    [Test]
    public void TranscribedSegmentWithAverageLogProbabilityWorks()
    {
        float averageLogProbability = -3.1415f;
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(averageLogProbability: averageLogProbability);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.EndTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds.Span.Length, Is.GreaterThanOrEqualTo(0));
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(averageLogProbability));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(float)));
    }

    [Test]
    public void TranscribedSegmentWithCompressionRatioWorks()
    {
        float compressionRatio = 1.33f;
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(compressionRatio: compressionRatio);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.EndTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds.Span.Length, Is.GreaterThanOrEqualTo(0));
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(compressionRatio));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(float)));
    }

    [Test]
    public void TranscribedSegmentWithNoSpeechProbabilityWorks()
    {
        float noSpeechProbability = 0.02f;
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(noSpeechProbability: noSpeechProbability);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.EndTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds.Span.Length, Is.GreaterThanOrEqualTo(0));
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(noSpeechProbability));
    }

    [Test]
    public void TranscribedWordWithNoPropertiesWorks()
    {
        TranscribedWord transcribedWord = OpenAIAudioModelFactory.TranscribedWord();

        Assert.That(transcribedWord.Word, Is.Null);
        Assert.That(transcribedWord.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedWord.EndTime, Is.EqualTo(default(TimeSpan)));
    }

    [Test]
    public void TranscribedWordWithIdWorks()
    {
        string word = "croissant";
        TranscribedWord transcribedWord = OpenAIAudioModelFactory.TranscribedWord(word: word);

        Assert.That(transcribedWord.Word, Is.EqualTo(word));
        Assert.That(transcribedWord.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedWord.EndTime, Is.EqualTo(default(TimeSpan)));
    }

    [Test]
    public void TranscribedWordWithStartWorks()
    {
        TimeSpan startTime = TimeSpan.FromSeconds(45);
        TranscribedWord transcribedWord = OpenAIAudioModelFactory.TranscribedWord(startTime: startTime);

        Assert.That(transcribedWord.Word, Is.Null);
        Assert.That(transcribedWord.StartTime, Is.EqualTo(startTime));
        Assert.That(transcribedWord.EndTime, Is.EqualTo(default(TimeSpan)));
    }

    [Test]
    public void TranscribedWordWithEndWorks()
    {
        TimeSpan endTime = TimeSpan.FromSeconds(45);
        TranscribedWord transcribedWord = OpenAIAudioModelFactory.TranscribedWord(endTime: endTime);

        Assert.That(transcribedWord.Word, Is.Null);
        Assert.That(transcribedWord.StartTime, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedWord.EndTime, Is.EqualTo(endTime));
    }
}
