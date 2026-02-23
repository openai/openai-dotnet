using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>conversation.item.deleted</c>, which is received in response to a
/// <c>conversation.item.delete</c> request command
/// (<see cref="RealtimeSessionClient.DeleteItemAsync(string, System.Threading.CancellationToken)"/>).
/// </summary>
[CodeGenType("RealtimeServerEventConversationItemDeleted")]
public partial class ItemDeletedUpdate
{ }
