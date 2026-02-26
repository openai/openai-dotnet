using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>output_audio_buffer.stopped</c> server event.
/// <b>WebRTC/SIP Only:</b> Emitted when the output audio buffer has been completely drained on the server,
/// and no more audio is forthcoming. This event is emitted after the full response
/// data has been sent to the client (<c>response.done</c>).
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventOutputAudioBufferStoppedGA")]
public partial class GARealtimeServerUpdateOutputAudioBufferStopped
{
}
