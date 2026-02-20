using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventResponseCreateGA")]
public partial class GARealtimeClientCommandResponseCreate
{
    // CUSTOM: Renamed.
    [CodeGenMember("Response")]
    public GARealtimeResponseOptions ResponseOptions { get; set; }
}