using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

/// <summary>
/// The update (response command) of type <c>conversation.item.input_audio_transcription.completed</c>, which is
/// received when a configured <c>input_audio_transcription</c> has completed its parallel processing of the user
/// audio input buffer.
/// </summary>
[Experimental("OPENAI002")]
[CodeGenModel("RealtimeServerEventConversationItemInputAudioTranscriptionCompleted")]
public partial class ConversationInputTranscriptionFinishedUpdate
{ }
