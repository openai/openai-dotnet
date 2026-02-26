using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.output_text.done</c> server event.
/// Returned when the text value of an "output_text" content part is done streaming. Also
/// emitted when a Response is interrupted, incomplete, or cancelled.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseOutputTextDoneGA")]
public partial class GARealtimeServerUpdateResponseOutputTextDone
{
}
