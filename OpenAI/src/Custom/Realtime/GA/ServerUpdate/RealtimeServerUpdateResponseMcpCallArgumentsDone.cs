using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.mcp_call_arguments.done</c> server event.
/// Returned when MCP tool call arguments are finalized during response generation.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseMCPCallArgumentsDoneGA")]
public partial class RealtimeServerUpdateResponseMcpCallArgumentsDone
{
    // CUSTOM:
    // - Renamed.
    // - Changed type.
    [CodeGenMember("Arguments")]
    public BinaryData ToolArguments { get; }
}
