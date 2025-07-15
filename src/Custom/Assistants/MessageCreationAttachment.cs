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
        => writer.WriteObjectValue(Tools, options);

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
