using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.done</c> server event.
/// Returned when a Response is done streaming. Always emitted, no matter the
/// final state. The Response object included in the <c>response.done</c> event will
/// include all output Items in the Response but will omit the raw audio data.
/// Clients should check the <c>status</c> field of the Response to determine if it was successful
/// (<c>completed</c>) or if there was another outcome: <c>cancelled</c>, <c>failed</c>, or <c>incomplete</c>.
/// A response will contain all output items that were generated during the response, excluding
/// any audio content.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseDoneGA")]
public partial class GARealtimeServerUpdateResponseDone
{
}