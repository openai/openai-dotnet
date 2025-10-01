using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeResponseTextContentPart")]
public partial class InternalRealtimeResponseTextContentPart
{
    [CodeGenMember("Text")]
    public string InternalTextValue { get; }
}
