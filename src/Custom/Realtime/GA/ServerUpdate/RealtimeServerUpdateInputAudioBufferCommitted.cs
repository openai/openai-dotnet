using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>input_audio_buffer.committed</c> server event.
/// Returned when an input audio buffer is committed, either by the client or
/// automatically in server VAD mode. The <c>item_id</c> property is the ID of the user
/// message item that will be created, thus a <c>conversation.item.created</c> event
/// will also be sent to the client.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventInputAudioBufferCommittedGA")]
public partial class GARealtimeServerUpdateInputAudioBufferCommitted
{
}
