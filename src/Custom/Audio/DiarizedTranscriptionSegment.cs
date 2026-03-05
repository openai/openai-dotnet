using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Audio;

// CUSTOM: Renamed.
[CodeGenType("TranscriptionDiarizedSegment")]
public partial class DiarizedTranscriptionSegment
{
    // CUSTOM: Made internal
    internal string Kind { get; } = "transcript.text.segment";

    // CUSTOM: Renamed.
    [CodeGenMember("Start")]
    public TimeSpan StartTime { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("End")]
    public TimeSpan EndTime { get; }
}

