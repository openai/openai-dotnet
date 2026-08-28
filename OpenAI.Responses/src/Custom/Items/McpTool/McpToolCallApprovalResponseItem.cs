using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("MCPApprovalResponseItemResource")]
[CodeGenSuppress("McpToolCallApprovalResponseItem")]
public partial class McpToolCallApprovalResponseItem
{
    public McpToolCallApprovalResponseItem() : this(ResponseItemKind.McpApprovalResponse, null, default, null, default, null)
    {
    }
}
