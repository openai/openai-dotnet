using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ChatToolChoice>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class ChatToolChoice : IJsonModel<ChatToolChoice>
{
    void IJsonModel<ChatToolChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeChatToolChoice, writer, options);

    internal static void SerializeChatToolChoice(ChatToolChoice instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (instance._isPlainString)
        {
            writer.WriteStringValue(instance._string);
        }
        else
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type"u8);
            writer.WriteStringValue(instance._type.ToString());
            writer.WritePropertyName("function"u8);
            writer.WriteObjectValue(instance._function, options);
            writer.WriteSerializedAdditionalRawData(instance._serializedAdditionalRawData, options);
            writer.WriteEndObject();
        }
    }

    internal static ChatToolChoice DeserializeChatToolChoice(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        else if (element.ValueKind == JsonValueKind.String)
        {
            return new ChatToolChoice(element.ToString());
        }
        else
        {
            InternalChatCompletionNamedToolChoiceType type = default;
            InternalChatCompletionNamedToolChoiceFunction function = default;
            IDictionary<string, BinaryData> serializedAdditionalRawData = default;
            Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("type"u8))
                {
                    type = new InternalChatCompletionNamedToolChoiceType(property.Value.GetString());
                    continue;
                }
                if (property.NameEquals("function"u8))
                {
                    function = InternalChatCompletionNamedToolChoiceFunction.DeserializeInternalChatCompletionNamedToolChoiceFunction(property.Value, options);
                    continue;
                }
                if (true)
                {
                    rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
                }
            }
            serializedAdditionalRawData = rawDataDictionary;
            return new ChatToolChoice(function.Name, serializedAdditionalRawData);
        }
    }
}