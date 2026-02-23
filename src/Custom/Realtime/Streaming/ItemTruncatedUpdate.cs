using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>conversation.item.truncated</c>, which is received in response to a
/// <c>conversation.item.truncate</c> request command
/// (<see cref="RealtimeSessionClient.TruncateItemAsync(string, int, TimeSpan, System.Threading.CancellationToken)"/>).
/// </summary>
[CodeGenType("RealtimeServerEventConversationItemTruncated")]
public partial class ItemTruncatedUpdate
{ }
