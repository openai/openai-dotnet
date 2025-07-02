using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeRequestAssistantMessageItem")]
[Experimental("OPENAI002")]
internal partial class InternalRealtimeRequestAssistantMessageItem
{
    [CodeGenMember("Content")]
    public IList<ConversationContentPart> Content { get; }

    public InternalRealtimeRequestAssistantMessageItem(IEnumerable<ConversationContentPart> content)
    {
        Argument.AssertNotNull(content, nameof(content));

        // CUSTOM: Add missing Type via doubly-discriminated hierarchy
        Kind = InternalRealtimeItemType.Message;
        Role = ConversationMessageRole.Assistant;

        // CUSTOM: Convert input_text to text
        Content = content
            .Select(item => item.Kind == ConversationContentPartKind.InputText ? ConversationContentPart.CreateOutputTextPart(item.Text) : item)
            .ToList();
    }
}