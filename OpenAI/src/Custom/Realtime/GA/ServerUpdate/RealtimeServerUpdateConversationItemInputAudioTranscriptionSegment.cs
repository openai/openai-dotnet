using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.input_audio_transcription.segment</c> server event.
/// Returned when an input audio transcription segment is identified for an item.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationItemInputAudioTranscriptionSegmentGA")]
public partial class RealtimeServerUpdateConversationItemInputAudioTranscriptionSegment
{
}
