using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseAudioGA")]
public partial class GARealtimeResponseAudioOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Output")]
    public GARealtimeResponseOutputAudioOptions OutputAudioOptions { get; }
}