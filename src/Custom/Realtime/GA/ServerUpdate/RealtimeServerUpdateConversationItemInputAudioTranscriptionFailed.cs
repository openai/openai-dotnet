using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.input_audio_transcription.failed</c> server event.
/// Returned when input audio transcription is configured, and a transcription
/// request for a user message failed. These events are separate from other
/// <c>error</c> events so that the client can identify the related Item.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationItemInputAudioTranscriptionFailedGA")]
public partial class GARealtimeServerUpdateConversationItemInputAudioTranscriptionFailed
{ 
}
