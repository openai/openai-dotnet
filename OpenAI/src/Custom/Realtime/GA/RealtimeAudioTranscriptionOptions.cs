using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("AudioTranscriptionGA")]
public partial class RealtimeAudioTranscriptionOptions
{
    // CUSTOM: Changed type.
    [CodeGenMember("Model")]
    public string Model { get; set; }
}