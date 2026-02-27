using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.output_audio_transcript.delta</c> server event.
/// Returned when the model-generated transcription of audio output is updated.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseOutputAudioTranscriptDeltaGA")]
public partial class RealtimeServerUpdateResponseOutputAudioTranscriptDelta
{
}
