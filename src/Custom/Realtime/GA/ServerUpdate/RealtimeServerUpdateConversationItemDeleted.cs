using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.deleted</c> server event.
/// Returned when an item in the conversation is deleted by the client with a
/// <c>conversation.item.delete</c> event. This event is used to synchronize the
/// server's understanding of the conversation history with the client's view.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationItemDeletedGA")]
public partial class GARealtimeServerUpdateConversationItemDeleted
{
}
