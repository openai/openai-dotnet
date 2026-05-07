using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.mcp_call_arguments.delta</c> server event.
/// Returned when MCP tool call arguments are updated during response generation.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseMCPCallArgumentsDeltaGA")]
public partial class RealtimeServerUpdateResponseMcpCallArgumentsDelta
{
}
