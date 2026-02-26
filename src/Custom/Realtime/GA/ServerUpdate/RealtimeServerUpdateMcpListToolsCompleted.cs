using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>mcp_list_tools.completed</c> server event.
/// Returned when listing MCP tools has completed for an item.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventMCPListToolsCompletedGA")]
public partial class GARealtimeServerUpdateMcpListToolsCompleted
{
}
