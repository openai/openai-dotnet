using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>output_audio_buffer.clear</c> client event.
/// <b>WebRTC/SIP Only:</b> Emit to cut off the current audio response. This will trigger the server to
/// stop generating audio and emit a <c>output_audio_buffer.cleared</c> event. This
/// event should be preceded by a <c>response.cancel</c> client event to stop the
/// generation of the current response.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventOutputAudioBufferClearGA")]
public partial class RealtimeClientCommandOutputAudioBufferClear
{
}