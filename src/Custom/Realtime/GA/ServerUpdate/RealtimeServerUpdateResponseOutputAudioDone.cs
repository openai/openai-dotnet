using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.output_audio.done</c> server event.
/// Returned when the model-generated audio is done. Also emitted when a Response
/// is interrupted, incomplete, or cancelled.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseOutputAudioDoneGA")]
public partial class RealtimeServerUpdateResponseOutputAudioDone
{
}
