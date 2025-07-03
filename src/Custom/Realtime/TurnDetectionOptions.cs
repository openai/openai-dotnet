using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeTurnDetection")]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class TurnDetectionOptions
{
    [CodeGenMember("CreateResponse")]
    internal bool? ResponseCreationEnabled { get; set; }

    [CodeGenMember("InterruptResponse")]
    internal bool? ResponseInterruptionEnabled { get; set; }

    public static TurnDetectionOptions CreateDisabledTurnDetectionOptions()
    {
        return new InternalRealtimeNoTurnDetection();
    }

    public static TurnDetectionOptions CreateServerVoiceActivityTurnDetectionOptions(
        float? detectionThreshold = null,
        TimeSpan? prefixPaddingDuration = null,
        TimeSpan? silenceDuration = null,
        bool? enableAutomaticResponseCreation = null,
        bool? enableResponseInterruption = null)
    {
        return new InternalRealtimeServerVadTurnDetection()
        {
            Threshold = detectionThreshold,
            PrefixPaddingMs = prefixPaddingDuration,
            SilenceDurationMs = silenceDuration,
            ResponseCreationEnabled = enableAutomaticResponseCreation,
            ResponseInterruptionEnabled = enableResponseInterruption,
        };
    }

    internal static TurnDetectionOptions CreateSemanticVoiceActivityTurnDetectionOptions(
        InternalRealtimeSemanticVadTurnDetectionEagerness? eagernessLevel = null,
        bool? enableAutomaticResponseCreation = null,
        bool? enableResponseInterruption = null)
    {
        return new InternalRealtimeSemanticVadTurnDetection()
        {
            Eagerness = eagernessLevel,
            ResponseInterruptionEnabled = enableResponseInterruption,
            ResponseCreationEnabled = enableAutomaticResponseCreation,
        };
    }
}