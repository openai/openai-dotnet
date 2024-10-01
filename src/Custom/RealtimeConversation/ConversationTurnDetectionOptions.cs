using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeTurnDetection")]
public partial class ConversationTurnDetectionOptions
{
    [CodeGenMember("Kind")]
    public ConversationTurnDetectionKind Kind { get; internal protected set; }

    public static ConversationTurnDetectionOptions CreateDisabledTurnDetectionOptions()
    {
        return new InternalRealtimeNoTurnDetection();
    }

    public static ConversationTurnDetectionOptions CreateServerVoiceActivityTurnDetectionOptions(
        float? detectionThreshold = null,
        TimeSpan? prefixPaddingDuration = null,
        TimeSpan? silenceDuration = null)
    {
        return new InternalRealtimeServerVadTurnDetection()
        {
            Threshold = detectionThreshold,
            PrefixPaddingMs = prefixPaddingDuration,
            SilenceDurationMs = silenceDuration
        };
    }
}