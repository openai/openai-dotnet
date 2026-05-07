using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>mcp_list_tools.in_progress</c> server event.
/// Returned when listing MCP tools is in progress for an item.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventMCPListToolsInProgressGA")]
public partial class RealtimeServerUpdateMcpListToolsInProgress
{
}
