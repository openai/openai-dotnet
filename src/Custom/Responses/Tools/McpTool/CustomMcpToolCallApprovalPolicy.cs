namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("DotNetCustomToolCallApprovalPolicy")]
public partial class CustomMcpToolCallApprovalPolicy
{
    // CUSTOM:
    // - Renamed.
    // - Re-used McpToolFilter as the type.
    [CodeGenMember("Always")]
    public McpToolFilter ToolsAlwaysRequiringApproval { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Re-used McpToolFilter as the type.
    [CodeGenMember("Never")]
    public McpToolFilter ToolsNeverRequiringApproval { get; set; }
}
