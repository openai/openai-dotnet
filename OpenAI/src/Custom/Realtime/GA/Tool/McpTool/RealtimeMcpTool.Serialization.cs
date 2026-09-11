using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Realtime;

[CodeGenSerialization(nameof(AllowedTools), DeserializationValueHook = nameof(DeserializeAllowedToolsValue))]
public partial class RealtimeMcpTool : IJsonModel<RealtimeMcpTool>
{
    // CUSTOM: Accepts the MCP shorthand allowed_tools array form and normalizes it into RealtimeMcpToolFilter.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeAllowedToolsValue(JsonProperty property, ref RealtimeMcpToolFilter allowedTools, ModelReaderWriterOptions options = null)
    {
        if (property.Value.ValueKind == JsonValueKind.Null)
        {
            allowedTools = null;
            return;
        }

        if (property.Value.ValueKind == JsonValueKind.Object)
        {
            allowedTools = RealtimeMcpToolFilter.DeserializeRealtimeMcpToolFilter(property.Value, property.Value.GetUtf8Bytes(), options);
            return;
        }

        if (property.Value.ValueKind == JsonValueKind.Array)
        {
            allowedTools = new RealtimeMcpToolFilter();
            foreach (JsonElement item in property.Value.EnumerateArray())
            {
                allowedTools.ToolNames.Add(item.ValueKind == JsonValueKind.Null ? null : item.GetString());
            }
            return;
        }

        throw new JsonException($"Expected allowed_tools to be null, an object, or an array but found {property.Value.ValueKind}.");
    }
}
