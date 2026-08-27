using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Audio;

// CUSTOM: Renamed.
[CodeGenType("VadConfig")]
public partial class AudioTranscriptionCustomServerVadChunkingStrategy
{
    // CUSTOM: Renamed.
    [CodeGenMember("PrefixPaddingMs")]
    public TimeSpan? PrefixPadding { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("SilenceDurationMs")]
    public TimeSpan? SilenceDuration { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Threshold")]
    public float? DetectionThreshold { get; set; }
}
