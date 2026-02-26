using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.cancel</c> client event.
/// Send this event to cancel an in-progress response. The server will respond
/// with a <c>response.done</c> event with a status of <c>response.status=cancelled</c>. If
/// there is no response to cancel, the server will respond with an error. It's safe
/// to call <c>response.cancel</c> even if no response is in progress, an error will be
/// returned the session will remain unaffected.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeClientEventResponseCancelGA")]
public partial class GARealtimeClientCommandResponseCancel
{
}