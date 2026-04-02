using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeServerVADTurnDetectionGA")]
public partial class RealtimeServerVadTurnDetection
{
    // CUSTOM: Renamed.
    [CodeGenMember("Threshold")]
    public float? DetectionThreshold { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("CreateResponse")]
    public bool? CreateResponseEnabled { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("InterruptResponse")]
    public bool? InterruptResponseEnabled { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("IdleTimeoutMs")]
    public TimeSpan? IdleTimeout { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("PrefixPaddingMs")]
    public TimeSpan? PrefixPadding { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("SilenceDurationMs")]
    public TimeSpan? SilenceDuration { get; set; }
}
