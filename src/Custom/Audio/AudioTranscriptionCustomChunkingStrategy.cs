using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Audio;

// CUSTOM: Renamed.
[CodeGenType("VadConfig")]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Internal)]
public partial class AudioTranscriptionCustomChunkingStrategy
{
    // CUSTOM: Renamed.
    [CodeGenMember("PrefixPaddingMs")]
    public TimeSpan? PrefixPadding { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("SilenceDurationMs")]
    public TimeSpan? SilenceDuration { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Threshold")]
    public float? ChunkingStrategyThreshold { get; set; }
}
