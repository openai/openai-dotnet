using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeSessionCreateRequestGAAudioGA")]
public partial class RealtimeConversationSessionAudioOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Input")]
    public RealtimeConversationSessionInputAudioOptions InputAudioOptions { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Output")]
    public RealtimeConversationSessionOutputAudioOptions OutputAudioOptions { get; set; }
}