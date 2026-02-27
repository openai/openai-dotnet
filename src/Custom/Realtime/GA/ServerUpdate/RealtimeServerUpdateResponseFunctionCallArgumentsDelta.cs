using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.function_call_arguments.delta</c> server event.
/// Returned when the model-generated function call arguments are updated.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseFunctionCallArgumentsDeltaGA")]
public partial class RealtimeServerUpdateResponseFunctionCallArgumentsDelta
{
}
