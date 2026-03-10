using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>input_audio_buffer.dtmf_event_received</c> server event.
/// <b>SIP Only:</b> Returned when an DTMF event is received. A DTMF event is a message that
/// represents a telephone keypad press (0–9, *, #, A–D). The <c>event</c> property
/// is the keypad that the user press. The <c>received_at</c> is the UTC Unix Timestamp
/// that the server received the event.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventInputAudioBufferDtmfEventReceivedGA")]
public partial class RealtimeServerUpdateInputAudioBufferDtmfEventReceived
{
}
