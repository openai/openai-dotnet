using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ChatMessageContentPart>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class ChatMessageContentPart : IJsonModel<ChatMessageContentPart>
{
    void IJsonModel<ChatMessageContentPart>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeChatMessageContentPart, writer, options);

    internal static void SerializeChatMessageContentPart(ChatMessageContentPart instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();

        if (instance._kind == ChatMessageContentPartKind.Text)
        {
            writer.WritePropertyName("type"u8);
            writer.WriteStringValue(instance._kind.ToString());
            writer.WritePropertyName("text"u8);
            writer.WriteStringValue(instance._text);
        }
        else if (instance._kind == ChatMessageContentPartKind.Image)
        {
            writer.WritePropertyName("type"u8);
            writer.WriteStringValue(instance._kind.ToString());
            writer.WritePropertyName("image_url"u8);
            writer.WriteObjectValue(instance._imageUrl, options);
        }
        writer.WriteSerializedAdditionalRawData(instance._serializedAdditionalRawData, options);
        writer.WriteEndObject();
    }

    internal static ChatMessageContentPart DeserializeChatMessageContentPart(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        string kind = default;
        string text = default;
        InternalChatCompletionRequestMessageContentPartImageImageUrl imageUrl = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("type"u8))
            {
                kind = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("text"u8))
            {
                text = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("image_url"u8))
            {
                imageUrl = InternalChatCompletionRequestMessageContentPartImageImageUrl.DeserializeInternalChatCompletionRequestMessageContentPartImageImageUrl(property.Value, options);
                continue;
            }
            if (true)
            {
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new ChatMessageContentPart(kind, text, imageUrl, serializedAdditionalRawData);
    }
}
