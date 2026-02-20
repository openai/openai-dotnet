using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventSessionUpdateGA")]
public partial class GARealtimeClientCommandSessionUpdate
{
    // CUSTOM: Renamed.
    [CodeGenMember("Session")]
    public GARealtimeSessionOptions SessionOptions { get; }
}