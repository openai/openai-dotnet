using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[Flags]
public enum ConversationContentModalities : int
{
    Default = 0,
    Text = 1 << 0,
    Audio = 1 << 1,
}