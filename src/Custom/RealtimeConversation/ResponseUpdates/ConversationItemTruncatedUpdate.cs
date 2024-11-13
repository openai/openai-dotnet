using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

/// <summary>
/// The update (response command) of type <c>conversation.item.truncated</c>, which is received in response to a
/// <c>conversation.item.truncate</c> request command
/// (<see cref="RealtimeConversationSession.TruncateItemAsync(string, int, TimeSpan, System.Threading.CancellationToken)"/>).
/// </summary>
[Experimental("OPENAI002")]
[CodeGenModel("RealtimeServerEventConversationItemTruncated")]
public partial class ConversationItemTruncatedUpdate
{ }
