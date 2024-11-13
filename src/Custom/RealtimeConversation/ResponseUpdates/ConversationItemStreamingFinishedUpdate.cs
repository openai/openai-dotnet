using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

/// <summary>
/// The update (response command) of type <c>response.output_item.done</c>, which is received when a new item that was
/// added to a conversation by the model has finished streaming all of its content parts and other data. This update
/// is preceded by a <see cref="ConversationItemStreamingStartedUpdate"/> and some number of <c>*delta</c> commands (such as
/// <see cref="InternalConversationTextContentDeltaUpdate"/>).
/// </summary>
[Experimental("OPENAI002")]
[CodeGenModel("RealtimeServerEventResponseOutputItemDone")]
public partial class ConversationItemStreamingFinishedUpdate
{
    [CodeGenMember("Item")]
    private readonly InternalRealtimeResponseItem _internalItem;

    public string ItemId => _internalItem.Id;

    public ConversationMessageRole? MessageRole => _internalItem.MessageRole;

    public IReadOnlyList<ConversationContentPart> MessageContentParts => _internalItem.MessageContentParts;

    public string FunctionName => _internalItem.FunctionName;

    public string FunctionCallId => _internalItem.FunctionCallId;

    public string FunctionCallArguments => _internalItem.FunctionCallArguments;

    public string FunctionCallOutput => _internalItem.FunctionCallOutput;
}