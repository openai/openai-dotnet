using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
[CodeGenType("RealtimeRequestSystemMessageItem")]
internal partial class InternalRealtimeRequestSystemMessageItem
{
    [CodeGenMember("Content")]
    public IList<ConversationContentPart> Content { get; }

    public InternalRealtimeRequestSystemMessageItem(IEnumerable<ConversationContentPart> content)
    {
        Argument.AssertNotNull(content, nameof(content));

        // CUSTOM: Add missing Type via doubly-discriminated hierarchy
        Kind = InternalRealtimeItemType.Message;
        Role = ConversationMessageRole.System;
        Content = content.ToList();
    }
}