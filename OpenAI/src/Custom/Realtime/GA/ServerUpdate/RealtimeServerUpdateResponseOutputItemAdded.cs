using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.output_item.added</c> server event.
/// Returned when a new Item is created during Response generation.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseOutputItemAddedGA")]
public partial class RealtimeServerUpdateResponseOutputItemAdded
{
}
