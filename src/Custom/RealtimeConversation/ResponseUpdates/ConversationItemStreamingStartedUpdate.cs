using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

/// <summary>
/// The update (response command) of type <c>response.output_item.added</c>, which is received when a response turn
/// has begun generation of a new conversation item. This new item will have content streamed via <c>*delta</c>
/// commands and paired with an ending <c>response.output_item.done</c> update.
/// </summary>
[Experimental("OPENAI002")]
[CodeGenModel("RealtimeServerEventResponseOutputItemAdded")]
public partial class ConversationItemStreamingStartedUpdate
{
    [CodeGenMember("Item")]
    private readonly InternalRealtimeResponseItem _internalItem;

    public string ItemId => _internalItem.Id;

    [CodeGenMember("OutputIndex")]
    public int ItemIndex { get; }

    public ConversationMessageRole? MessageRole => _internalItem.MessageRole;

    public IReadOnlyList<ConversationContentPart> MessageContentParts => _internalItem.MessageContentParts;

    public string FunctionName => _internalItem.FunctionName;

    public string FunctionCallId => _internalItem.FunctionCallId;

    public string FunctionCallArguments => _internalItem.FunctionCallArguments;

    public string FunctionCallOutput => _internalItem.FunctionCallOutput;
}
