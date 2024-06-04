using System.Collections.Generic;

namespace OpenAI.Audio;

[CodeGenModel("TranscriptionSegment")]
public readonly partial struct TranscribedSegment
{
    // CUSTOM: Rename.
    [CodeGenMember("Seek")]
    public long SeekOffset { get; }

    // CUSTOM: Rename.
    [CodeGenMember("Tokens")]
    public IReadOnlyList<long> TokenIds { get; }

    // CUSTOM: Rename.
    [CodeGenMember("AvgLogprob")]
    public double AverageLogProbability { get; }

    // CUSTOM: Rename.
    [CodeGenMember("NoSpeechProb")]
    public double NoSpeechProbability { get; }
}