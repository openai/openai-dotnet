using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OpenAI.Responses;

public partial class McpToolFilter
{
    // CUSTOM: Overridden to handle deserialization of both the object format
    // (e.g. {"tool_names": ["tool1"], "read_only": true}) and the string array format
    // (e.g. ["tool1", "tool2"]), which is a valid representation per the API specification.
    internal static McpToolFilter DeserializeMcpToolFilter(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        // CUSTOM: When the element is a JSON array, treat it as a list of tool names.
        if (element.ValueKind == JsonValueKind.Array)
        {
            List<string> toolNames = new List<string>();
            foreach (var item in element.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Null)
                {
                    toolNames.Add(null);
                }
                else
                {
                    toolNames.Add(item.GetString());
                }
            }
            return new McpToolFilter(toolNames, default, default);
        }

        IList<string> objectToolNames = default;
        bool? isReadOnly = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        foreach (var prop in element.EnumerateObject())
        {
            if (prop.NameEquals("tool_names"u8))
            {
                if (prop.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                List<string> array = new List<string>();
                foreach (var item in prop.Value.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Null)
                    {
                        array.Add(null);
                    }
                    else
                    {
                        array.Add(item.GetString());
                    }
                }
                objectToolNames = array;
                continue;
            }
            if (prop.NameEquals("read_only"u8))
            {
                if (prop.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                isReadOnly = prop.Value.GetBoolean();
                continue;
            }
            patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(prop.Name)], prop.Value.GetUtf8Bytes());
        }
        return new McpToolFilter(objectToolNames ?? new ChangeTrackingList<string>(), isReadOnly, patch);
    }
}
