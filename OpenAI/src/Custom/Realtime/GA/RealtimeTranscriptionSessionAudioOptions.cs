using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeTranscriptionSessionCreateRequestGAAudioGA")]
public partial class RealtimeTranscriptionSessionAudioOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Input")]
    public RealtimeTranscriptionSessionInputAudioOptions InputAudioOptions { get; set; }
}