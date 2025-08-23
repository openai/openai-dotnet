namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("MCPTool")]
public partial class MCPTool
{
    // CUSTOM: Re-used MCPToolFilter as the type.
    [CodeGenMember("AllowedTools")]
    public MCPToolFilter AllowedTools { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type from BinaryData (generated from the original union) to a custom type.
    [CodeGenMember("RequireApproval")]
    public MCPToolCallApprovalPolicy ToolCallApprovalPolicy { get; set; }
}