using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>session.updated</c> server event.
/// Returned when a session is updated with a <c>session.update</c> event, unless
/// there is an error.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventSessionUpdatedGA")]
public partial class GARealtimeServerUpdateSessionUpdated
{
}
