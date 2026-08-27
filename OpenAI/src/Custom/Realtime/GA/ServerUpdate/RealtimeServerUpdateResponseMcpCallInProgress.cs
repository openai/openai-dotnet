using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.mcp_call.in_progress</c> server event.
/// Returned when an MCP tool call has started and is in progress.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseMCPCallInProgressGA")]
public partial class RealtimeServerUpdateResponseMcpCallInProgress
{
}
