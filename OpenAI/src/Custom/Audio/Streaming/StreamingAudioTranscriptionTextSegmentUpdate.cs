using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;

namespace OpenAI.Audio;

// CUSTOM: Renamed.
[CodeGenType("DotNetTranscriptTextSegmentEvent")]
public partial class StreamingAudioTranscriptionTextSegmentUpdate
{
    // CUSTOM: Rename.
    [CodeGenMember("Id")]
    public string SegmentId { get; }

    // CUSTOM: Rename.
    [CodeGenMember("Start")]
    public TimeSpan StartTime { get; }

    // CUSTOM: Rename.
    [CodeGenMember("End")]
    public TimeSpan EndTime { get; }

    // CUSTOM: Rename.
    [CodeGenMember("Speaker")]
    public string SpeakerLabel { get; }
}
