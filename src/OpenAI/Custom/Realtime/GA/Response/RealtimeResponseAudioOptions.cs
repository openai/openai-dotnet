using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeResponseAudioGA")]
public partial class RealtimeResponseAudioOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Output")]
    public RealtimeResponseOutputAudioOptions OutputAudioOptions { get; }
}