using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.output_audio.delta</c> server event.
/// Returned when the model-generated audio is updated.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseOutputAudioDeltaGA")]
public partial class RealtimeServerUpdateResponseOutputAudioDelta
{
}
