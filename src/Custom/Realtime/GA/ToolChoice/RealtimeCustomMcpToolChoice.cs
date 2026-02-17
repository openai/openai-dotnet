using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("ToolChoiceMCPGA")]
public partial class GARealtimeCustomMcpToolChoice
{
    // CUSTOM: Renamed.
    [CodeGenMember("Name")]
    public string McpToolName { get; set; }
}