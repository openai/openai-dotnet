using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.output_text.delta</c> server event.
/// Returned when the text value of an "output_text" content part is updated.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseOutputTextDeltaGA")]
public partial class RealtimeServerUpdateResponseOutputTextDelta
{
}
