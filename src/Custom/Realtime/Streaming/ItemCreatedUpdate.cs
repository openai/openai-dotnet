using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>conversation.item.created</c>, which is received when a new item has
/// been emplaced into the conversation. In the case of model-generated items, this may be preceded by a
/// <see cref="OutputStreamingStartedUpdate"/> (<c>response.output_item.added</c>) command, in which case the newly-
/// created item associated with this update will not yet have content associated with it and will instead have
/// content streamed via <c>*delta</c> commands before finalization via a
/// <see cref="OutputStreamingFinishedUpdate"/> (<c>response.output_item.done</c>).
/// </summary>
[CodeGenType("RealtimeServerEventConversationItemCreated")]
public partial class ItemCreatedUpdate
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
