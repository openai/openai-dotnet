using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeResponseItemCreatedCommand")]
public partial class ConversationItemAcknowledgedUpdate
{
    [CodeGenMember("Item")]
    public ConversationItem Item { get; }
}
