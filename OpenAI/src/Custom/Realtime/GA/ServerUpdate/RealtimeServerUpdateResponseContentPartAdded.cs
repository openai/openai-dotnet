using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.content_part.added</c> server event.
/// Returned when a new content part is added to an assistant message item during
/// response generation.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseContentPartAddedGA")]
public partial class RealtimeServerUpdateResponseContentPartAdded
{
}
