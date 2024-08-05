using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenAI.Audio;

[CodeGenModel("TranscriptionSegment")]
[StructLayout(LayoutKind.Auto)]
public readonly partial struct TranscribedSegment
{
    // CUSTOM: Remove setter.
    internal IDictionary<string, BinaryData> SerializedAdditionalRawData { get; }

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