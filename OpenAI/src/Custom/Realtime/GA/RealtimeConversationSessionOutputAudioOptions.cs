using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeSessionCreateRequestGAAudioOutputGA")]
public partial class RealtimeConversationSessionOutputAudioOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Format")]
    public RealtimeAudioFormat AudioFormat { get; set; }
}
