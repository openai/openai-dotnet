using OpenAI.Files;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ChatMessageContentPart>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class ChatMessageContentPart : IJsonModel<ChatMessageContentPart>
{
    void IJsonModel<ChatMessageContentPart>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, WriteCoreContentPart, writer, options);

    internal static void WriteCoreContentPart(ChatMessageContentPart instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("type"u8);
        writer.WriteStringValue(instance._kind.ToSerialString());

        if (instance._kind == ChatMessageContentPartKind.Text)
        {
            writer.WritePropertyName("text"u8);
            writer.WriteStringValue(instance._text);
        }
        else if (instance._kind == ChatMessageContentPartKind.Refusal)
        {
            writer.WritePropertyName("refusal"u8);
            writer.WriteStringValue(instance._refusal);
        }
        else if (instance._kind == ChatMessageContentPartKind.Image)
        {
            writer.WritePropertyName("image_url"u8);
            writer.WriteObjectValue(instance._imageUri, options);
        }
        writer.WriteSerializedAdditionalRawData(instance._additionalBinaryDataProperties, options);
        writer.WriteEndObject();
    }

    internal static ChatMessageContentPart DeserializeChatMessageContentPart(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        ChatMessageContentPartKind kind = default;
        string text = default;
        string refusal = default;
        InternalChatCompletionRequestMessageContentPartImageImageUrl imageUri = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("type"u8))
            {
                kind = property.Value.GetString().ToChatMessageContentPartKind();
                continue;
            }
            if (property.NameEquals("text"u8))
            {
                text = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("image_url"u8))
            {
                imageUri = InternalChatCompletionRequestMessageContentPartImageImageUrl.DeserializeInternalChatCompletionRequestMessageContentPartImageImageUrl(property.Value, options);
                continue;
            }
            if (property.NameEquals("refusal"u8))
            {
                refusal = property.Value.GetString();
                continue;
            }
            if (true)
            {
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new ChatMessageContentPart(kind, text, imageUri, refusal, serializedAdditionalRawData);
    }
}
