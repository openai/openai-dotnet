using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
[CodeGenModel("RealtimeResponseOutputItemDoneCommand")]
public partial class ConversationItemFinishedUpdate
{
    [CodeGenMember("Item")]
    private readonly InternalRealtimeResponseItem _internalItem;

    public ConversationMessageRole? MessageRole => _internalItem.MessageRole;

    public IReadOnlyList<ConversationContentPart> MessageContentParts => _internalItem.MessageContentParts;

    public string FunctionName => _internalItem.FunctionName;

    public string FunctionCallId => _internalItem.FunctionCallId;

    public string FunctionCallArguments => _internalItem.FunctionCallArguments;

    public string FunctionCallOutput => _internalItem.FunctionCallOutput;
}