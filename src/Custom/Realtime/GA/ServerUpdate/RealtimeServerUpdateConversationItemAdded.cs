using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.added</c> server event.
/// Sent by the server when an Item is added to the default Conversation. This can happen in several cases:
/// <list type="bullet">
/// <item><description>When the client sends a <c>conversation.item.create</c> event.</description></item>
/// <item><description>When the input audio buffer is committed. In this case the item will be a user message containing the audio from the buffer.</description></item>
/// <item><description>When the model is generating a Response. In this case the <c>conversation.item.added</c> event will be sent when the model starts generating a specific Item, and thus it will not yet have any content (and <c>status</c> will be <c>in_progress</c>).</description></item>
/// </list>
/// The event will include the full content of the Item (except when model is generating a Response) except for audio data, which can be retrieved separately with a <c>conversation.item.retrieve</c> event if necessary.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationItemAddedGA")]
public partial class GARealtimeServerUpdateConversationItemAdded
{
}
