using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeTranscriptionSessionCreateRequestGAAudioGA")]
public partial class GARealtimeTranscriptionSessionAudioOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Input")]
    public GARealtimeTranscriptionSessionInputAudioOptions InputAudioOptions { get; set; }
}