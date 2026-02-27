using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("MCPToolRequireApprovalGA")]
public partial class RealtimeCustomMcpToolCallApprovalPolicy
{
    // CUSTOM: Renamed.
    [CodeGenMember("Always")]
    public RealtimeMcpToolFilter ToolsAlwaysRequiringApproval { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Never")]
    public RealtimeMcpToolFilter ToolsNeverRequiringApproval { get; set; }
}