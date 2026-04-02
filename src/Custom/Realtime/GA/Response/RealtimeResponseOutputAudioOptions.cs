using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseAudioOutputGA")]
public partial class RealtimeResponseOutputAudioOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Format")]
    public RealtimeAudioFormat AudioFormat { get; set; }
}