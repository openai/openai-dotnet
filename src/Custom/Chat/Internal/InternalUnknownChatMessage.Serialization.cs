using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
internal partial class InternalUnknownChatMessage : IJsonModel<ChatMessage>
{
    void IJsonModel<ChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance<ChatMessage, InternalUnknownChatMessage>(this, WriteCore, writer, options);

    internal static void WriteCore(InternalUnknownChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        instance.WriteCore(writer, options);
    }

    protected internal override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(Role.ToSerialString());
        ChatMessageContentPart.WriteCoreContentPartList(Content, writer, options);
        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }

    internal static InternalUnknownChatMessage DeserializeUnknownChatMessage(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        ChatMessageRole? role = null;
        IList<ChatMessageContentPart> content = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("role"u8))
            {
                role = property.Value.GetString().ToChatMessageRole();
                continue;
            }
            if (property.NameEquals("content"u8))
            {
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
                List<ChatMessageContentPart> array = new List<ChatMessageContentPart>();
                foreach (var item in property.Value.EnumerateArray())
                {
                    array.Add(ChatMessageContentPart.DeserializeChatMessageContentPart(item, options));
                }
                content = array;
                continue;
            }
            if (true)
            {
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new InternalUnknownChatMessage(role.Value, content ?? new ChangeTrackingList<ChatMessageContentPart>(), serializedAdditionalRawData);
    }
}
