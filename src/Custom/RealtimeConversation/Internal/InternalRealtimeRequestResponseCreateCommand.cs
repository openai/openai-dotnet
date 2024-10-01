using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeRequestResponseCreateCommand")]
internal partial class InternalRealtimeRequestResponseCreateCommand
{
    [CodeGenMember("MaxOutputTokens")]
    private BinaryData _maxOutputTokens;

    public ConversationMaxTokensChoice MaxResponseOutputTokens
    {
        get => ConversationMaxTokensChoice.FromBinaryData(_maxOutputTokens);
        set
        {
            _maxOutputTokens = value == null ? null : ModelReaderWriter.Write(value);
        }
    }
}
