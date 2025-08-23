namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("DotNetCustomToolCallApprovalPolicy")]
public partial class CustomMCPToolCallApprovalPolicy
{
    // CUSTOM:
    // - Renamed.
    // - Re-used MCPToolFilter as the type.
    [CodeGenMember("Always")]
    public MCPToolFilter ToolsAlwaysRequiringApproval { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Re-used MCPToolFilter as the type.
    [CodeGenMember("Never")]
    public MCPToolFilter ToolsNeverRequiringApproval { get; set; }
}
