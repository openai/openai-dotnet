namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("MCPTool")]
public partial class McpTool
{
    // CUSTOM: Re-used MCPToolFilter as the type.
    [CodeGenMember("AllowedTools")]
    public McpToolFilter AllowedTools { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("RequireApproval")]
    public McpToolCallApprovalPolicy ToolCallApprovalPolicy { get; set; }
}