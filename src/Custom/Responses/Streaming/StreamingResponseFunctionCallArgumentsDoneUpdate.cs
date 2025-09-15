using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseFunctionCallArgumentsDoneEvent")]
public partial class StreamingResponseFunctionCallArgumentsDoneUpdate
{
    // CUSTOM: Renamed.
    [CodeGenMember("Arguments")]
    public BinaryData FunctionArguments { get; }
}
