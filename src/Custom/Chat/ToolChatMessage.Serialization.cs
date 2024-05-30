using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ToolChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class ToolChatMessage : IJsonModel<ToolChatMessage>
{
    void IJsonModel<ToolChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeToolChatMessage, writer, options);

    internal static void SerializeToolChatMessage(ToolChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("tool_call_id"u8);
        writer.WriteStringValue(instance.ToolCallId);
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(instance.Role);
        if (Optional.IsCollectionDefined(instance.Content))
        {
            writer.WritePropertyName("content"u8);
            writer.WriteStringValue(instance.Content[0].Text);
        }
        writer.WriteSerializedAdditionalRawData(instance._serializedAdditionalRawData, options);
        writer.WriteEndObject();
    }

    internal static ToolChatMessage DeserializeToolChatMessage(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        string toolCallId = default;
        string role = default;
        IList<ChatMessageContentPart> content = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("tool_call_id"u8))
            {
                toolCallId = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("role"u8))
            {
                role = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("content"u8))
            {
                List<ChatMessageContentPart> array = new List<ChatMessageContentPart>();
                array.Add(ChatMessageContentPart.CreateTextMessageContentPart(property.Value.GetString()));
                content = array;
                continue;
            }
            if (true)
            {
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new ToolChatMessage(role, content ?? new ChangeTrackingList<ChatMessageContentPart>(), serializedAdditionalRawData, toolCallId);
    }
}
