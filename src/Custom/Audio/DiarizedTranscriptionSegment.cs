using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Runtime.InteropServices;

namespace OpenAI.Audio;

// CUSTOM: Renamed.
[CodeGenType("TranscriptionDiarizedSegment")]
[StructLayout(LayoutKind.Auto)]
public readonly partial struct DiarizedTranscriptionSegment
{
    // CUSTOM: Made internal
    internal string Kind { get; } = "transcript.text.segment";

    // CUSTOM: Renamed.
    [CodeGenMember("Start")]
    public TimeSpan StartTime { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("End")]
    public TimeSpan EndTime { get; }

    // CUSTOM: Rename.
    [CodeGenMember("Speaker")]
    public string SpeakerLabel { get; }
}

