using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeRequestUserMessageItem")]
internal partial class InternalRealtimeRequestUserMessageItem
{
    public InternalRealtimeRequestUserMessageItem(IEnumerable<ConversationContentPart> content)
    {
        Argument.AssertNotNull(content, nameof(content));

        Type = InternalRealtimeItemType.Message;
        Role = ConversationMessageRole.User;
        Content = content.ToList();
    }

    [CodeGenMember("Content")]
    public IList<ConversationContentPart> Content { get; }
}