using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.create</c> client event.
/// Add a new Item to the Conversation's context, including messages, function
/// calls, and function call responses. This event can be used both to populate a
/// "history" of the conversation and to add new items mid-stream, but has the
/// current limitation that it cannot populate assistant audio messages.
/// If successful, the server will respond with a <c>conversation.item.created</c>
/// event, otherwise an <c>error</c> event will be sent.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventConversationItemCreateGA")]
public partial class GARealtimeClientCommandConversationItemCreate
{
}