using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.retrieved</c> server event.
/// Returned when a conversation item is retrieved with <c>conversation.item.retrieve</c>.
/// <para>
/// This is provided as a way to fetch the server's representation of an item, for example to get access to the post-processed audio data after noise cancellation and VAD.
/// It includes the full content of the Item, including audio data.
/// </para>
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationItemRetrievedGA")]
public partial class RealtimeServerUpdateConversationItemRetrieved
{
}
