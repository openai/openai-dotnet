using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeTranscriptionSessionCreateRequestGAAudioInputGA")]
public partial class GARealtimeTranscriptionSessionInputAudioOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Format")]
    public GARealtimeAudioFormat AudioFormat { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Transcription")]
    public GARealtimeAudioTranscriptionOptions AudioTranscriptionOptions { get; set; }

    // CUSTOM: Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("TurnDetection")]
    public GARealtimeTurnDetection TurnDetection { get; set; }
}
