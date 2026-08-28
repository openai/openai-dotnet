using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("MCPCallItemResource")]
[CodeGenSuppress("McpToolCallItem")]
public partial class McpToolCallItem
{
    public McpToolCallItem() : this(ResponseItemKind.McpCall, null, default, null, null, null, null, null)
    {
    }
}
