using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.input_audio_transcription.completed</c> server event.
/// This event is the output of audio transcription for user audio written to the
/// user audio buffer. Transcription begins when the input audio buffer is
/// committed by the client or server (when VAD is enabled). Transcription runs
/// asynchronously with Response creation, so this event may come before or after
/// the Response events.
/// <para>
/// Realtime API models accept audio natively, and thus input transcription is a
/// separate process run on a separate ASR (Automatic Speech Recognition) model.
/// The transcript may diverge somewhat from the model's interpretation, and
/// should be treated as a rough guide.
/// </para>
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationItemInputAudioTranscriptionCompletedGA")]
public partial class RealtimeServerUpdateConversationItemInputAudioTranscriptionCompleted
{
}
