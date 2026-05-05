using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.input_audio_transcription.delta</c> server event.
/// Returned when the text value of an input audio transcription content part is updated with incremental transcription results.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationItemInputAudioTranscriptionDeltaGA")]
public partial class RealtimeServerUpdateConversationItemInputAudioTranscriptionDelta
{
}
