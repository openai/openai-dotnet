using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseMCPCallArgumentsDoneEvent")]
public partial class StreamingResponseMcpCallArgumentsDoneUpdate
{
    // CUSTOM: Renamed.
    [CodeGenMember("Arguments")]
    public BinaryData ToolArguments { get; }
}
