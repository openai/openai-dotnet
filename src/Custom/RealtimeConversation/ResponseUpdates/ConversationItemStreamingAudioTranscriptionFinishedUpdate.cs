using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

/// <summary>
/// The update (response command) of type <c>response.audio_transcript.done</c>, which is received after all
/// <see cref="InternalConversationOutputTranscriptionDeltaUpdate"/> commands for an output audio content part have been
/// received.
/// </summary>
[Experimental("OPENAI002")]
[CodeGenModel("RealtimeServerEventResponseAudioTranscriptDone")]
public partial class ConversationItemStreamingAudioTranscriptionFinishedUpdate
{ }
