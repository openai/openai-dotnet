using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>conversation.created</c> server event.
/// Returned when a conversation is created. Emitted right after session creation.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventConversationCreatedGA")]
public partial class RealtimeServerUpdateConversationCreated
{
}
