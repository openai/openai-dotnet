using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>input_audio_buffer.cleared</c> server event.
/// Returned when the input audio buffer is cleared by the client with a
/// <c>input_audio_buffer.clear</c> event.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventInputAudioBufferClearedGA")]
public partial class RealtimeServerUpdateInputAudioBufferCleared
{
}
