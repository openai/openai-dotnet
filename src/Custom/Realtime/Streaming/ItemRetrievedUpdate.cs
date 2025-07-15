using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>conversation.item.retrieved</c>, which is received in response to a
/// <c>conversation.item.retrieve</c> client command sent via
/// <see cref="RealtimeSession.RequestItemRetrieval(string, System.Threading.CancellationToken)"/>.
/// </summary>
[CodeGenType("RealtimeServerEventConversationItemRetrieved")]
public partial class ItemRetrievedUpdate
{
    [CodeGenMember("Item")]
    public RealtimeItem Item { get; }
}
