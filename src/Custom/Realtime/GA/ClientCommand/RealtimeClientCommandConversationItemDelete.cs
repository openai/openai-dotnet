using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.delete</c> client event.
/// Send this event when you want to remove any item from the conversation
/// history. The server will respond with a <c>conversation.item.deleted</c> event,
/// unless the item does not exist in the conversation history, in which case the
/// server will respond with an error.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventConversationItemDeleteGA")]
public partial class GARealtimeClientCommandConversationItemDelete
{
}