using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Responses;

[CodeGenSerialization(nameof(AllowedTools), DeserializationValueHook = nameof(DeserializeAllowedToolsValue))]
public partial class McpTool : IJsonModel<McpTool>
{
    // CUSTOM: Accepts the MCP shorthand allowed_tools array form and normalizes it into McpToolFilter.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeAllowedToolsValue(JsonProperty property, ref McpToolFilter allowedTools, ModelReaderWriterOptions options = null)
    {
        if (property.Value.ValueKind == JsonValueKind.Null)
        {
            allowedTools = null;
            return;
        }

        if (property.Value.ValueKind == JsonValueKind.Object)
        {
            allowedTools = McpToolFilter.DeserializeMcpToolFilter(property.Value, property.Value.GetUtf8Bytes(), options);
            return;
        }

        if (property.Value.ValueKind == JsonValueKind.Array)
        {
            allowedTools = new McpToolFilter();
            foreach (JsonElement item in property.Value.EnumerateArray())
            {
                allowedTools.ToolNames.Add(item.ValueKind == JsonValueKind.Null ? null : item.GetString());
            }
            return;
        }

        throw new JsonException($"Expected allowed_tools to be null, an object, or an array but found {property.Value.ValueKind}.");
    }
}
