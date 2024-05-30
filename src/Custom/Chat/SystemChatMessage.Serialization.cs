using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.SystemChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class SystemChatMessage : IJsonModel<SystemChatMessage>
{
    void IJsonModel<SystemChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeSystemChatMessage, writer, options);

    internal static void SerializeSystemChatMessage(SystemChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        if (Optional.IsDefined(instance.ParticipantName))
        {
            writer.WritePropertyName("name"u8);
            writer.WriteStringValue(instance.ParticipantName);
        }
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

    internal static SystemChatMessage DeserializeSystemChatMessage(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        string name = default;
        string role = default;
        IList<ChatMessageContentPart> content = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("name"u8))
            {
                name = property.Value.GetString();
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
        return new SystemChatMessage(role, content ?? new ChangeTrackingList<ChatMessageContentPart>(), serializedAdditionalRawData, name);
    }
}