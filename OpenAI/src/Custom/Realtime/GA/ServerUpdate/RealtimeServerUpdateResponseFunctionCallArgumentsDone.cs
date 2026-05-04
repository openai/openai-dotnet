using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.function_call_arguments.done</c> server event.
/// Returned when the model-generated function call arguments are done streaming.
/// Also emitted when a Response is interrupted, incomplete, or cancelled.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseFunctionCallArgumentsDoneGA")]
public partial class RealtimeServerUpdateResponseFunctionCallArgumentsDone
{
    // CUSTOM: Renamed.
    [CodeGenMember("Name")]
    public string FunctionName { get; }

    // CUSTOM:
    // - Renamed.
    // - Changed type.
    [CodeGenMember("Arguments")]
    public BinaryData FunctionArguments { get; }
}
