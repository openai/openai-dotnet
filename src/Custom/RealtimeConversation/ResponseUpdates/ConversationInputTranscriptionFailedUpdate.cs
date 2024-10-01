using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeResponseItemInputAudioTranscriptionFailedCommand")]
public partial class ConversationInputTranscriptionFailedUpdate
{
    [CodeGenMember("Error")]
    private readonly InternalRealtimeResponseApiError _error;

    public string ErrorCode => _error?.Code;
    public string ErrorMessage => _error?.Message;
    public string ErrorParameterName => _error?.Param;
}