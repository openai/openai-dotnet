using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.content_part.done</c> server event.
/// Returned when a content part is done streaming in an assistant message item.
/// Also emitted when a Response is interrupted, incomplete, or cancelled.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseContentPartDoneGA")]
public partial class GARealtimeServerUpdateResponseContentPartDone
{
}
