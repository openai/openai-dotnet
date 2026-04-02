using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>output_audio_buffer.started</c> server event.
/// <b>WebRTC/SIP Only:</b> Emitted when the server begins streaming audio to the client. This event is
/// emitted after an audio content part has been added (<c>response.content_part.added</c>)
/// to the response.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventOutputAudioBufferStartedGA")]
public partial class RealtimeServerUpdateOutputAudioBufferStarted
{
}
