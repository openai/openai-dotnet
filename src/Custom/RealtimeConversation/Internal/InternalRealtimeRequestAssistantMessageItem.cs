using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[CodeGenModel("RealtimeRequestAssistantMessageItem")]
[Experimental("OPENAI002")]
internal partial class InternalRealtimeRequestAssistantMessageItem
{
    [CodeGenMember("Content")]
    public IList<ConversationContentPart> Content { get; }
}