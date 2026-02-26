using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>error</c> server event.
/// Returned when an error occurs, which could be a client problem or a server
/// problem. Most errors are recoverable and the session will stay open, we
/// recommend to implementors to monitor and log error messages by default.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventErrorGA")]
public partial class GARealtimeServerUpdateError
{
}
