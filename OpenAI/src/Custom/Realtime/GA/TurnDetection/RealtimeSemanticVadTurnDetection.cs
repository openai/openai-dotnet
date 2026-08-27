using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeSemanticVADTurnDetectionGA")]
public partial class RealtimeSemanticVadTurnDetection
{
    // CUSTOM: Renamed.
    [CodeGenMember("Eagerness")]
    public RealtimeSemanticVadEagernessLevel? EagernessLevel { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("CreateResponse")]
    public bool? CreateResponseEnabled { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("InterruptResponse")]
    public bool? InterruptResponseEnabled { get; set; }
}
