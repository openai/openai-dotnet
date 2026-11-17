using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
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
            // Initialize the collection to respect an empty array during normalization.
            allowedTools = new RealtimeMcpToolFilter(new List<string>(), default, default);
            foreach (JsonElement item in property.Value.EnumerateArray())
            {
                allowedTools.ToolNames.Add(item.ValueKind == JsonValueKind.Null ? null : item.GetString());
            }
            return;
        }

        throw new InvalidOperationException($"Expected allowed_tools to be null, an object, or an array but found {property.Value.ValueKind}.");
    }
}
