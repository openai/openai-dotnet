using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenAI.Audio;

/// <summary> A segment of the transcribed audio. </summary>
[CodeGenModel("TranscriptionSegment")]
[StructLayout(LayoutKind.Auto)]
public readonly partial struct TranscribedSegment
{
    // CUSTOM: Remove setter. Auto-implemented instance properties in readonly structs must be readonly.
    internal IDictionary<string, BinaryData> SerializedAdditionalRawData { get; }

    // CUSTOM: Renamed.
    /// <summary> The start time of the segment. </summary>
    [CodeGenMember("Start")]
    public TimeSpan StartTime { get; }

    // CUSTOM: Renamed.
    /// <summary> The end time of the segment. </summary>
    [CodeGenMember("End")]
    public TimeSpan EndTime { get; }

    // CUSTOM: Renamed.
    /// <summary> The seek offset of the segment. </summary>
    [CodeGenMember("Seek")]
    public int SeekOffset { get; }

    // CUSTOM: Renamed.
    /// <summary> The token IDs corresponding to the text content of the segment. </summary>
    [CodeGenMember("Tokens")]
    public ReadOnlyMemory<int> TokenIds { get; }

    // CUSTOM: Renamed.
    /// <summary> The average log probability of the segment. </summary>
    [CodeGenMember("AvgLogprob")]
    public float AverageLogProbability { get; }

    // CUSTOM: Renamed.
    /// <summary> The probability that the segment contains no speech. </summary>
    [CodeGenMember("NoSpeechProb")]
    public float NoSpeechProbability { get; }
}