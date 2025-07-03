using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeAudioNoiseReductionType")]
public enum InputNoiseReductionKind
{
    Unknown,
    [CodeGenMember("NearField")]
    NearField,
    [CodeGenMember("SemanticVad")]
    FarField,
    Disabled,
}