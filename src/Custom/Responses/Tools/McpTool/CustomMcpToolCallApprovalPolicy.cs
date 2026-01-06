using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("DotNetCustomToolCallApprovalPolicy")]
public partial class CustomMcpToolCallApprovalPolicy
{
    // CUSTOM: Renamed.
    [CodeGenMember("Always")]
    public McpToolFilter ToolsAlwaysRequiringApproval { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Never")]
    public McpToolFilter ToolsNeverRequiringApproval { get; set; }
}
