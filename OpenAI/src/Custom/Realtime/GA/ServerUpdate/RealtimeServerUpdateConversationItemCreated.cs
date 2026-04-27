using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.item.created</c> server event.
/// Returned when a conversation item is created. There are several scenarios that produce this event:
/// <list type="bullet">
/// <item><description>The server is generating a Response, which if successful will produce either one or two Items, which will be of type <c>message</c> (role <c>assistant</c>) or type <c>function_call</c>.</description></item>
/// <item><description>The input audio buffer has been committed, either by the client or the server (in <c>server_vad</c> mode). The server will take the content of the input audio buffer and add it to a new user message Item.</description></item>
/// <item><description>The client has sent a <c>conversation.item.create</c> event to add a new Item to the Conversation.</description></item>
/// </list>
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationItemCreatedGA")]
public partial class RealtimeServerUpdateConversationItemCreated
{
}
