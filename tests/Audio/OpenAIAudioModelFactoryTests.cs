using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenAI.Audio;

namespace OpenAI.Tests.Audio;

[Parallelizable(ParallelScope.All)]
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
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(long)));
        Assert.That(transcribedSegment.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.End, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds, Is.Not.Null.And.Empty);
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(double)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(double)));
    }

    [Test]
    public void TranscribedSegmentWithIdWorks()
    {
        int id = 10;
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(id: id);

        Assert.That(transcribedSegment.Id, Is.EqualTo(id));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(long)));
        Assert.That(transcribedSegment.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.End, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds, Is.Not.Null.And.Empty);
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(double)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(double)));
    }

    [Test]
    public void TranscribedSegmentWithSeekOffsetWorks()
    {
        long seekOffset = 9000000000;
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(seekOffset: seekOffset);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(seekOffset));
        Assert.That(transcribedSegment.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.End, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds, Is.Not.Null.And.Empty);
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(double)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(double)));
    }

    [Test]
    public void TranscribedSegmentWithStartWorks()
    {
        TimeSpan start = TimeSpan.FromSeconds(45);
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(start: start);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(long)));
        Assert.That(transcribedSegment.Start, Is.EqualTo(start));
        Assert.That(transcribedSegment.End, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds, Is.Not.Null.And.Empty);
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(double)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(double)));
    }

    [Test]
    public void TranscribedSegmentWithEndWorks()
    {
        TimeSpan end = TimeSpan.FromSeconds(45);
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(end: end);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(long)));
        Assert.That(transcribedSegment.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.End, Is.EqualTo(end));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds, Is.Not.Null.And.Empty);
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(double)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(double)));
    }

    [Test]
    public void TranscribedSegmentWithTextWorks()
    {
        string text = "A segment text";
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(text: text);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(long)));
        Assert.That(transcribedSegment.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.End, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.EqualTo(text));
        Assert.That(transcribedSegment.TokenIds, Is.Not.Null.And.Empty);
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(double)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(double)));
    }

    [Test]
    public void TranscribedSegmentWithTokenIdsWorks()
    {
        IEnumerable<long> tokenIds = [9000000000, 9000000010];
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(tokenIds: tokenIds);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(long)));
        Assert.That(transcribedSegment.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.End, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds.SequenceEqual(tokenIds), Is.True);
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(double)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(double)));
    }

    [Test]
    public void TranscribedSegmentWithTemperatureWorks()
    {
        float temperature = 0.232f;
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(temperature: temperature);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(long)));
        Assert.That(transcribedSegment.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.End, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds, Is.Not.Null.And.Empty);
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(temperature));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(double)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(double)));
    }

    [Test]
    public void TranscribedSegmentWithAverageLogProbabilityWorks()
    {
        double averageLogProbability = -3.1415;
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(averageLogProbability: averageLogProbability);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(long)));
        Assert.That(transcribedSegment.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.End, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds, Is.Not.Null.And.Empty);
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(averageLogProbability));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(double)));
    }

    [Test]
    public void TranscribedSegmentWithCompressionRatioWorks()
    {
        float compressionRatio = 1.33f;
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(compressionRatio: compressionRatio);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(long)));
        Assert.That(transcribedSegment.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.End, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds, Is.Not.Null.And.Empty);
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(double)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(compressionRatio));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(default(double)));
    }

    [Test]
    public void TranscribedSegmentWithNoSpeechProbabilityWorks()
    {
        double noSpeechProbability = 0.02;
        TranscribedSegment transcribedSegment = OpenAIAudioModelFactory.TranscribedSegment(noSpeechProbability: noSpeechProbability);

        Assert.That(transcribedSegment.Id, Is.EqualTo(default(int)));
        Assert.That(transcribedSegment.SeekOffset, Is.EqualTo(default(long)));
        Assert.That(transcribedSegment.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.End, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedSegment.Text, Is.Null);
        Assert.That(transcribedSegment.TokenIds, Is.Not.Null.And.Empty);
        Assert.That(transcribedSegment.Temperature, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.AverageLogProbability, Is.EqualTo(default(double)));
        Assert.That(transcribedSegment.CompressionRatio, Is.EqualTo(default(float)));
        Assert.That(transcribedSegment.NoSpeechProbability, Is.EqualTo(noSpeechProbability));
    }

    [Test]
    public void TranscribedWordWithNoPropertiesWorks()
    {
        TranscribedWord transcribedWord = OpenAIAudioModelFactory.TranscribedWord();

        Assert.That(transcribedWord.Word, Is.Null);
        Assert.That(transcribedWord.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedWord.End, Is.EqualTo(default(TimeSpan)));
    }

    [Test]
    public void TranscribedWordWithIdWorks()
    {
        string word = "croissant";
        TranscribedWord transcribedWord = OpenAIAudioModelFactory.TranscribedWord(word: word);

        Assert.That(transcribedWord.Word, Is.EqualTo(word));
        Assert.That(transcribedWord.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedWord.End, Is.EqualTo(default(TimeSpan)));
    }

    [Test]
    public void TranscribedWordWithStartWorks()
    {
        TimeSpan start = TimeSpan.FromSeconds(45);
        TranscribedWord transcribedWord = OpenAIAudioModelFactory.TranscribedWord(start: start);

        Assert.That(transcribedWord.Word, Is.Null);
        Assert.That(transcribedWord.Start, Is.EqualTo(start));
        Assert.That(transcribedWord.End, Is.EqualTo(default(TimeSpan)));
    }

    [Test]
    public void TranscribedWordWithEndWorks()
    {
        TimeSpan end = TimeSpan.FromSeconds(45);
        TranscribedWord transcribedWord = OpenAIAudioModelFactory.TranscribedWord(end: end);

        Assert.That(transcribedWord.Word, Is.Null);
        Assert.That(transcribedWord.Start, Is.EqualTo(default(TimeSpan)));
        Assert.That(transcribedWord.End, Is.EqualTo(end));
    }
}
