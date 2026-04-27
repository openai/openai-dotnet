using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.mcp_call.completed</c> server event.
/// Returned when an MCP tool call has completed successfully.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseMCPCallCompletedGA")]
public partial class RealtimeServerUpdateResponseMcpCallCompleted
{
}
