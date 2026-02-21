using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeSessionCreateRequestGAAudioOutputGA")]
public partial class GARealtimeConversationSessionOutputAudioOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Format")]
    public GARealtimeAudioFormat AudioFormat { get; set; }
}
