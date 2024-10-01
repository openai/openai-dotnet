using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeResponse")]
internal partial class InternalRealtimeResponse
{
    [CodeGenMember("Output")]
    public IReadOnlyList<ConversationItem> Output { get; }
}
