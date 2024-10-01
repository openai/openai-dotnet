using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeRequestSystemMessageItem")]
internal partial class InternalRealtimeRequestSystemMessageItem
{
    [CodeGenMember("Content")]
    public IList<ConversationContentPart> Content { get; }
}