using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeResponseSession")]
internal partial class InternalRealtimeResponseSession
{
    // CUSTOM: Use a scenario-specific copy of the voice ID collection.
    [CodeGenMember("Voice")]
    internal ConversationVoice Voice { get; set; }

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
