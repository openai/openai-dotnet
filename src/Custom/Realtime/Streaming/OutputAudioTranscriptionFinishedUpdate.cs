using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>response.audio_transcript.done</c>, which is received after all
/// <see cref="InternalConversationOutputTranscriptionDeltaUpdate"/> commands for an output audio content part have been
/// received.
/// </summary>
[CodeGenType("RealtimeServerEventResponseAudioTranscriptDone")]
public partial class OutputAudioTranscriptionFinishedUpdate
{ }
