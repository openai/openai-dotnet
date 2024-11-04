using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeToolChoiceLiteral")]
public enum ConversationToolChoiceKind
{
    Unknown,
    [CodeGenMember("Auto")]
    Auto,
    [CodeGenMember("None")]
    None,
    [CodeGenMember("Required")]
    Required,
    [CodeGenMember("Function")]
    Function,
}