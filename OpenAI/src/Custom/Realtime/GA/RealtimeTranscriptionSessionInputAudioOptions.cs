using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeTranscriptionSessionCreateRequestGAAudioInputGA")]
public partial class RealtimeTranscriptionSessionInputAudioOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("Format")]
    public RealtimeAudioFormat AudioFormat { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Transcription")]
    public RealtimeAudioTranscriptionOptions AudioTranscriptionOptions { get; set; }

    public void DisableTurnDetection()
    {
        Patch.SetNull("$.turn_detection"u8);
    }
}
