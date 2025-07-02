using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Audio;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIAudioModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Audio.AudioTranscription"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Audio.AudioTranscription"/> instance for mocking. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static AudioTranscription AudioTranscription(string language, TimeSpan? duration, string text, IEnumerable<TranscribedWord> words, IEnumerable<TranscribedSegment> segments)
        => AudioTranscription(language, duration, text, words, segments, null);

    /// <summary> Initializes a new instance of <see cref="OpenAI.Audio.AudioTranscription"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Audio.AudioTranscription"/> instance for mocking. </returns>
    [Experimental("OPENAI001")]
    public static AudioTranscription AudioTranscription(string language = null, TimeSpan? duration = null, string text = null, IEnumerable<TranscribedWord> words = null, IEnumerable<TranscribedSegment> segments = null, IEnumerable<AudioTokenLogProbabilityDetails> transcriptionTokenLogProbabilities = null)
    {
        words ??= new List<TranscribedWord>();
        segments ??= new List<TranscribedSegment>();
        transcriptionTokenLogProbabilities ??= new List<AudioTokenLogProbabilityDetails>();

        return new AudioTranscription(
            language,
            text,
            words.ToList(),
            segments.ToList(),
            "transcribe",
            duration,
            transcriptionTokenLogProbabilities.ToList(),
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Audio.AudioTranslation"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Audio.AudioTranslation"/> instance for mocking. </returns>
    public static AudioTranslation AudioTranslation(string language = null, TimeSpan? duration = null, string text = null, IEnumerable<TranscribedSegment> segments = null)
    {
        segments ??= new List<TranscribedSegment>();

        return new AudioTranslation(
            language,
            text,
            segments: segments.ToList(),
            task: "translate",
            duration,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Audio.TranscribedSegment"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Audio.TranscribedSegment"/> instance for mocking. </returns>
    public static TranscribedSegment TranscribedSegment(int id = default, int seekOffset = default, TimeSpan startTime = default, TimeSpan endTime = default, string text = null, ReadOnlyMemory<int> tokenIds = default, float temperature = default, float averageLogProbability = default, float compressionRatio = default, float noSpeechProbability = default)
    {
        return new TranscribedSegment(
            id,
            text,
            temperature,
            compressionRatio,
            startTime,
            endTime,
            seekOffset,
            tokenIds,
            averageLogProbability,
            noSpeechProbability,
            additionalBinaryDataProperties: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Audio.TranscribedWord"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Audio.TranscribedWord"/> instance for mocking. </returns>
    public static TranscribedWord TranscribedWord(string word = null, TimeSpan startTime = default, TimeSpan endTime = default)
    {
        return new TranscribedWord(
            word,
            startTime,
            endTime,
            additionalBinaryDataProperties: null);
    }
}
