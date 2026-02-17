using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("MCPToolRequireApprovalGA")]
public partial class GARealtimeCustomMcpToolCallApprovalPolicy
{
    // CUSTOM: Renamed.
    [CodeGenMember("Always")]
    public GARealtimeMcpToolFilter ToolsAlwaysRequiringApproval { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Never")]
    public GARealtimeMcpToolFilter ToolsNeverRequiringApproval { get; set; }
}