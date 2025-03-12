using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeTurnDetectionType")]
public enum ConversationTurnDetectionKind
{
    [CodeGenMember("ServerVad")]
    ServerVoiceActivityDetection,
    Disabled,
}