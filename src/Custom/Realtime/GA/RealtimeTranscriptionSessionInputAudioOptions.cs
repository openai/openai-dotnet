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

    public void DisableTurnDetection()
    {
        Patch.SetNull("$.turn_detection"u8);
    }
}
