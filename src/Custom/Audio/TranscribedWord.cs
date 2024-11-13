using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenAI.Audio;

/// <summary> A word of the transcribed audio. </summary>
[CodeGenModel("TranscriptionWord")]
[StructLayout(LayoutKind.Auto)]
public readonly partial struct TranscribedWord
{
    // CUSTOM: Remove setter. Auto-implemented instance properties in readonly structs must be readonly.
    internal IDictionary<string, BinaryData> SerializedAdditionalRawData { get; }

    // CUSTOM: Renamed.
    /// <summary> The start time of the word. </summary>
    [CodeGenMember("Start")]
    public TimeSpan StartTime { get; }

    // CUSTOM: Renamed.
    /// <summary> The end time of the word. </summary>
    [CodeGenMember("End")]
    public TimeSpan EndTime { get; }
}