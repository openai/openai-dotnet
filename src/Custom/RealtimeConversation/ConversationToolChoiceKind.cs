using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeToolChoiceLiteral")]
public enum ConversationToolChoiceKind
{
    [CodeGenMember("Auto")]
    Auto,
    [CodeGenMember("None")]
    None,
    [CodeGenMember("Required")]
    Required,
    Function,
}