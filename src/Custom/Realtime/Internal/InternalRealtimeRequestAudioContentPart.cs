using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeRequestAudioContentPart")]
public partial class InternalRealtimeRequestAudioContentPart
{
    [CodeGenMember("Transcript")]
    public string InternalTranscriptValue { get; set; }
}
