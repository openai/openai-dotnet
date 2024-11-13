using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeRequestItem")]
public partial class ConversationItem
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

    public static ConversationItem CreateUserMessage(IEnumerable<ConversationContentPart> contentItems)
    {
        return new InternalRealtimeRequestUserMessageItem(contentItems);
    }

    public static ConversationItem CreateSystemMessage(string toolCallId, IEnumerable<ConversationContentPart> contentItems)
    {
        return new InternalRealtimeRequestSystemMessageItem(contentItems);
    }

    public static ConversationItem CreateAssistantMessage(IEnumerable<ConversationContentPart> contentItems)
    {
        return new InternalRealtimeRequestAssistantMessageItem(contentItems);
    }

    public static ConversationItem CreateFunctionCall(string name, string callId, string arguments)
    {
        return new InternalRealtimeRequestFunctionCallItem(name, callId, arguments);
    }

    public static ConversationItem CreateFunctionCallOutput(string callId, string output)
    {
        return new InternalRealtimeRequestFunctionCallOutputItem(callId, output);
    }
}