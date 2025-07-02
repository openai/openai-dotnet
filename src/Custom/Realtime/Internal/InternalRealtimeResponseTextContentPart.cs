using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeResponseTextContentPart")]
internal partial class InternalRealtimeResponseTextContentPart
{
    [CodeGenMember("Text")]
    public string InternalTextValue { get; }
}
