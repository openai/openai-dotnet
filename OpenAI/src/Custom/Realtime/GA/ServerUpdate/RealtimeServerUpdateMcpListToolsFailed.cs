using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>mcp_list_tools.failed</c> server event.
/// Returned when listing MCP tools has failed for an item.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventMCPListToolsFailedGA")]
public partial class RealtimeServerUpdateMcpListToolsFailed
{
}
