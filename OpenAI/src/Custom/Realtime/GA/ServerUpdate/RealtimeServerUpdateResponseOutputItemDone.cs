using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.output_item.done</c> server event.
/// Returned when an Item is done streaming. Also emitted when a Response is
/// interrupted, incomplete, or cancelled.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseOutputItemDoneGA")]
public partial class RealtimeServerUpdateResponseOutputItemDone
{
}
