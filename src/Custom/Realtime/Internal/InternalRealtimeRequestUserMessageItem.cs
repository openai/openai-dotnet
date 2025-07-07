using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeRequestUserMessageItem")]
internal partial class InternalRealtimeRequestUserMessageItem
{
    [CodeGenMember("Content")]
    public IList<ConversationContentPart> Content { get; }

    public InternalRealtimeRequestUserMessageItem(IEnumerable<ConversationContentPart> content)
    {
        Argument.AssertNotNull(content, nameof(content));

        // CUSTOM: Add missing Type via doubly-discriminated hierarchy
        Kind = InternalRealtimeItemType.Message;
        Role = ConversationMessageRole.User;
        Content = content.ToList();
    }
}