using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeResponseItem")]
internal partial class InternalRealtimeResponseItem
{
    public string ResponseId
        => (this as InternalRealtimeResponseMessageItem)?.ResponseId
        ?? (this as InternalRealtimeResponseFunctionCallItem)?.ResponseId
        ?? (this as InternalRealtimeResponseFunctionCallOutputItem)?.ResponseId;

    public ConversationMessageRole? MessageRole =>
        (this as InternalRealtimeResponseMessageItem)?.Role;

    public IReadOnlyList<ConversationContentPart> MessageContentParts =>
        (this as InternalRealtimeResponseMessageItem)?.Content;

    public string FunctionName
        => (this as InternalRealtimeResponseFunctionCallItem)?.Name;

    public string FunctionCallId =>
        (this as InternalRealtimeResponseFunctionCallItem)?.CallId
        ?? (this as InternalRealtimeResponseFunctionCallOutputItem)?.CallId;

    public string FunctionCallArguments =>
        (this as InternalRealtimeResponseFunctionCallItem)?.Arguments;

    public string FunctionCallOutput =>
        (this as InternalRealtimeResponseFunctionCallOutputItem)?.Output;
}
