using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("MCPListToolsItemResource")]
[CodeGenSuppress("McpToolDefinitionListItem")]
public partial class McpToolDefinitionListItem
{
    public McpToolDefinitionListItem() : this(ResponseItemKind.McpListTools, null, default, null, null, null)
    {
    }
}
