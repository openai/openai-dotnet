using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseAudioOutputGA")]
public partial class GARealtimeResponseOutputAudioOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Format")]
    public GARealtimeAudioFormat AudioFormat { get; set; }
}