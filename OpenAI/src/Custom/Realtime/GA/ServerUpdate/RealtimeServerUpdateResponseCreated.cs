using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.created</c> server event.
/// Returned when a new Response is created. The first event of response creation,
/// where the response is in an initial state of <c>in_progress</c>.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseCreatedGA")]
public partial class RealtimeServerUpdateResponseCreated
{
}