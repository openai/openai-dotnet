using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeConversationRequestItem")]
public partial class RealtimeItem
{
    public string FunctionCallId => (this as InternalRealtimeRequestFunctionCallItem)?.CallId;
    public string FunctionName => (this as InternalRealtimeRequestFunctionCallItem)?.Name;
    public string FunctionArguments => (this as InternalRealtimeRequestFunctionCallItem)?.Arguments;

    public IReadOnlyList<ConversationContentPart> MessageContentParts
        => (this as InternalRealtimeRequestAssistantMessageItem)?.Content.ToList().AsReadOnly()
        ?? (this as InternalRealtimeRequestSystemMessageItem)?.Content?.ToList().AsReadOnly()
        ?? (this as InternalRealtimeRequestUserMessageItem)?.Content?.ToList().AsReadOnly();
    public ConversationMessageRole? MessageRole
        => (this as InternalRealtimeRequestMessageItem)?.Role;

    public static RealtimeItem CreateUserMessage(IEnumerable<ConversationContentPart> contentItems)
    {
        return new InternalRealtimeRequestUserMessageItem(contentItems);
    }

    public static RealtimeItem CreateSystemMessage(IEnumerable<ConversationContentPart> contentItems)
    {
        return new InternalRealtimeRequestSystemMessageItem(contentItems);
    }

    public static RealtimeItem CreateAssistantMessage(IEnumerable<ConversationContentPart> contentItems)
    {
        return new InternalRealtimeRequestAssistantMessageItem(contentItems);
    }

    public static RealtimeItem CreateFunctionCall(string name, string callId, string arguments)
    {
        return new InternalRealtimeRequestFunctionCallItem(name, callId, arguments);
    }

    public static RealtimeItem CreateFunctionCallOutput(string callId, string output)
    {
        return new InternalRealtimeRequestFunctionCallOutputItem(callId, output);
    }
}