using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeTurnDetectionType")]
public enum TurnDetectionKind
{
    Unknown,
    [CodeGenMember("ServerVad")]
    ServerVoiceActivityDetection,
    [CodeGenMember("SemanticVad")]
    SemanticVoiceActivityDetection,
    Disabled,
}