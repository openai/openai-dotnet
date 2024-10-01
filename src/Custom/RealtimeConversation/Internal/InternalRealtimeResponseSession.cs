using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeResponseSession")]
internal partial class InternalRealtimeResponseSession
{
    [CodeGenMember("MaxResponseOutputTokens")]
    private BinaryData _maxResponseOutputTokens;

    public ConversationMaxTokensChoice MaxResponseOutputTokens
    {
        get => ConversationMaxTokensChoice.FromBinaryData(_maxResponseOutputTokens);
        set
        {
            _maxResponseOutputTokens = value == null ? null : ModelReaderWriter.Write(value);
        }
    }
}
