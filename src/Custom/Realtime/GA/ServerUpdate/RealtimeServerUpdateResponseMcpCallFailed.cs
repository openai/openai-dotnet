using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

/// <summary>
/// Corresponds to the <c>response.mcp_call.failed</c> server event.
/// Returned when an MCP tool call has failed.
/// </summary>
// CUSTOM: Renamed.
[CodeGenType("RealtimeServerEventResponseMCPCallFailedGA")]
public partial class GARealtimeServerUpdateResponseMcpCallFailed
{
}
