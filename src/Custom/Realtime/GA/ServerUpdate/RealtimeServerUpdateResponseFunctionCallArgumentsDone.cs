using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseFunctionCallArgumentsDoneGA")]
public partial class GARealtimeServerUpdateResponseFunctionCallArgumentsDone
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
