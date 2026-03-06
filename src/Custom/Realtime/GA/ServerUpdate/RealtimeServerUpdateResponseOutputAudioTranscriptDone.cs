using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.output_audio_transcript.done</c> server event.
/// Returned when the model-generated transcription of audio output is done
/// streaming. Also emitted when a Response is interrupted, incomplete, or
/// cancelled.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseOutputAudioTranscriptDoneGA")]
public partial class RealtimeServerUpdateResponseOutputAudioTranscriptDone
{
}
