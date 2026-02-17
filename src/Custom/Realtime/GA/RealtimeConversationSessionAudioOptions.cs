using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeSessionCreateRequestGAAudioGA")]
public partial class GARealtimeConversationSessionAudioOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Input")]
    public GARealtimeConversationSessionInputAudioOptions InputAudioOptions { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Output")]
    public GARealtimeConversationSessionOutputAudioOptions OutputAudioOptions { get; set; }
}