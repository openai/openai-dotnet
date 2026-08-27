using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.done</c> server event.
/// Returned when a conversation item is finalized.
/// <para>
/// The event will include the full content of the Item except for audio data, which can be retrieved separately with a <c>conversation.item.retrieve</c> event if needed.
/// </para>
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationItemDoneGA")]
public partial class RealtimeServerUpdateConversationItemDone
{
}
