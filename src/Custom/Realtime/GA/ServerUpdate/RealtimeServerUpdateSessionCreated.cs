using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>session.created</c> server event.
/// Returned when a Session is created. Emitted automatically when a new
/// connection is established as the first server event. This event will contain
/// the default Session configuration.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventSessionCreatedGA")]
public partial class GARealtimeServerUpdateSessionCreated
{
}
