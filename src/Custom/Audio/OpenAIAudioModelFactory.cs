using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Audio;

/// <summary> Model factory for models. </summary>
public static partial class OpenAIAudioModelFactory
{
    /// <summary> Initializes a new instance of <see cref="OpenAI.Audio.AudioTranscription"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Audio.AudioTranscription"/> instance for mocking. </returns>
    public static AudioTranscription AudioTranscription(string language = null, TimeSpan? duration = null, string text = null, IEnumerable<TranscribedWord> words = null, IEnumerable<TranscribedSegment> segments = null)
    {
        words ??= new List<TranscribedWord>();
        segments ??= new List<TranscribedSegment>();

        return new AudioTranscription(
            InternalCreateTranscriptionResponseVerboseJsonTask.Transcribe,
            language,
            duration,
            text,
            words.ToList(),
            segments.ToList(),
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Audio.AudioTranslation"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Audio.AudioTranslation"/> instance for mocking. </returns>
    public static AudioTranslation AudioTranslation(string language = null, TimeSpan? duration = null, string text = null, IEnumerable<TranscribedSegment> segments = null)
    {
        segments ??= new List<TranscribedSegment>();

        return new AudioTranslation(
            InternalCreateTranslationResponseVerboseJsonTask.Translate,
            language,
            duration,
            text,
            segments.ToList(),
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Audio.TranscribedSegment"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Audio.TranscribedSegment"/> instance for mocking. </returns>
    public static TranscribedSegment TranscribedSegment(int id = default, int seekOffset = default, TimeSpan startTime = default, TimeSpan endTime = default, string text = null, ReadOnlyMemory<int> tokenIds = default, float temperature = default, float averageLogProbability = default, float compressionRatio = default, float noSpeechProbability = default)
    {
        return new TranscribedSegment(
            id,
            seekOffset,
            startTime,
            endTime,
            text,
            tokenIds,
            temperature,
            averageLogProbability,
            compressionRatio,
            noSpeechProbability,
            serializedAdditionalRawData: null);
    }

    /// <summary> Initializes a new instance of <see cref="OpenAI.Audio.TranscribedWord"/>. </summary>
    /// <returns> A new <see cref="OpenAI.Audio.TranscribedWord"/> instance for mocking. </returns>
    public static TranscribedWord TranscribedWord(string word = null, TimeSpan startTime = default, TimeSpan endTime = default)
    {
        return new TranscribedWord(
            word,
            startTime,
            endTime,
            serializedAdditionalRawData: null);
    }
}
