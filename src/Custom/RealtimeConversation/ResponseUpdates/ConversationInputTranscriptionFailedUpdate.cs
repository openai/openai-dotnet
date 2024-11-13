using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

/// <summary>
/// The update (response command) of type <c>conversation.item.input_audio_transcription.failed</c>, which is received
/// when a problem is encountered while processing transcription of the user audio input buffer via configured
/// <see cref="ConversationInputTranscriptionOptions"/> (<c>input_audio_transcription</c> in <c>session.update</c>)
/// settings.
/// </summary>
[Experimental("OPENAI002")]
[CodeGenModel("RealtimeServerEventConversationItemInputAudioTranscriptionFailed")]
public partial class ConversationInputTranscriptionFailedUpdate
{
    [CodeGenMember("Error")]
    private readonly InternalRealtimeServerEventConversationItemInputAudioTranscriptionFailedError _error;

    public string ErrorCode => _error?.Code;
    public string ErrorMessage => _error?.Message;
    public string ErrorParameterName => _error?.Param;
}