using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.retrieve</c> client event.
/// Send this event when you want to retrieve the server's representation of a specific item in the conversation history.
/// This is useful, for example, to inspect user audio after noise cancellation and VAD.
/// <para>
/// The server will respond with a <c>conversation.item.retrieved</c> event,
/// unless the item does not exist in the conversation history, in which case the
/// server will respond with an error.
/// </para>
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventConversationItemRetrieveGA")]
public partial class GARealtimeClientCommandConversationItemRetrieve
{
}