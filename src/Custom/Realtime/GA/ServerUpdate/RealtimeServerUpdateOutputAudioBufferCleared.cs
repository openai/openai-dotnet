using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>output_audio_buffer.cleared</c> server event.
/// <b>WebRTC/SIP Only:</b> Emitted when the output audio buffer is cleared. This happens either in VAD
/// mode when the user has interrupted (<c>input_audio_buffer.speech_started</c>),
/// or when the client has emitted the <c>output_audio_buffer.clear</c> event to manually
/// cut off the current audio response.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventOutputAudioBufferClearedGA")]
public partial class GARealtimeServerUpdateOutputAudioBufferCleared
{
}
