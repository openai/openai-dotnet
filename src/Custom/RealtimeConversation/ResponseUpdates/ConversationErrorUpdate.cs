using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeResponseErrorCommand")]
public partial class ConversationErrorUpdate
{
    [CodeGenMember("Error")]
    private readonly InternalRealtimeResponseError _error;

    public string ErrorCode => _error?.Code;
    public string ErrorMessage => _error?.Message;
    public string ErrorParameterName => _error?.Param;
    public string ErrorEventId => _error?.EventId;
}