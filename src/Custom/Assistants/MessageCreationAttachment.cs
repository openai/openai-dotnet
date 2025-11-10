using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenType("CreateMessageRequestAttachment")]
[CodeGenSerialization(nameof(Tools), "tools", SerializationValueHook = nameof(SerializeTools), DeserializationValueHook = nameof(DeserializeTools))]
public partial class MessageCreationAttachment
{
    /// <summary>
    /// The tools to which the attachment applies to.
    /// </summary>
    /// <remarks>
    /// These are <see cref="ToolDefinition"/> instances that can be checked via downcast, e.g.:
    /// <code>
    /// if (message.Attachments[0].Tools[0] is <see cref="CodeInterpreterToolDefinition"/>)
    /// {
    ///     // The attachment applies to the code interpreter tool
    /// }
    /// </code>
    /// </remarks>
    [CodeGenMember("Tools")]
    public IReadOnlyList<ToolDefinition> Tools { get; }

    private void SerializeTools(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (Tools is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        foreach (ToolDefinition tool in Tools)
        {
            using var ms = new System.IO.MemoryStream();
            using (var tempWriter = new Utf8JsonWriter(ms))
            {
                tempWriter.WriteObjectValue(tool, options);
                tempWriter.Flush();
            }

            using (JsonDocument doc = JsonDocument.Parse(ms.ToArray()))
            {
                JsonElement root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Object)
                {
                    writer.WriteStartObject();
                    foreach (var prop in root.EnumerateObject())
                    {
                        if (prop.NameEquals("file_search"u8))
                        {
                            continue;
                        }
                        prop.WriteTo(writer);
                    }
                    writer.WriteEndObject();
                }
                else
                {
                    root.WriteTo(writer);
                }
            }
        }
        writer.WriteEndArray();
    }

    private static void DeserializeTools(JsonProperty property, ref IReadOnlyList<ToolDefinition> tools)
    {
        if (property.Value.ValueKind == JsonValueKind.Null)
        {
            tools = null;
        }
        else
        {
            List<ToolDefinition> deserializedTools = [];
            foreach (JsonElement toolElement in property.Value.EnumerateArray())
            {
                deserializedTools.Add(ToolDefinition.DeserializeToolDefinition(toolElement, ModelSerializationExtensions.WireOptions));
            }
            tools = deserializedTools;
        }
    }
}
