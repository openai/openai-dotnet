using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenAI.Audio;

/// <summary> A word of the transcribed audio. </summary>
[CodeGenType("TranscriptionWord")]
[StructLayout(LayoutKind.Auto)]
public readonly partial struct TranscribedWord
{
    // CUSTOM: Renamed.
    /// <summary> The start time of the word. </summary>
    [CodeGenMember("Start")]
    public TimeSpan StartTime { get; }

    // CUSTOM: Renamed.
    /// <summary> The end time of the word. </summary>
    [CodeGenMember("End")]
    public TimeSpan EndTime { get; }
}