using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.input_audio_transcription.delta</c> server event.
/// Returned when the text value of an input audio transcription content part is updated with incremental transcription results.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationItemInputAudioTranscriptionDeltaGA")]
public partial class RealtimeServerUpdateConversationItemInputAudioTranscriptionDelta
{
    // CUSTOM: Renamed.
    [CodeGenMember("Logprobs")]
    public IList<RealtimeTokenLogProbabilityDetails> TranscriptionTokenLogProbabilities { get; }
}
