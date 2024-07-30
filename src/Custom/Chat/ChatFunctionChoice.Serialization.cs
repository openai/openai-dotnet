using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.ChatFunctionChoice>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class ChatFunctionChoice : IJsonModel<ChatFunctionChoice>
{
    void IJsonModel<ChatFunctionChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeChatFunctionChoice, writer, options);

    internal static void SerializeChatFunctionChoice(ChatFunctionChoice instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (instance._isPlainString)
        {
            writer.WriteStringValue(instance._string);
        }
        else
        {
            writer.WriteStartObject();
            writer.WritePropertyName("name"u8);
            writer.WriteStringValue(instance._function.Name);
            writer.WriteSerializedAdditionalRawData(instance.SerializedAdditionalRawData, options);
            writer.WriteEndObject();
        }
    }

    internal static ChatFunctionChoice DeserializeChatFunctionChoice(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        else if (element.ValueKind == JsonValueKind.String)
        {
            return new ChatFunctionChoice(element.ToString());
        }
        else
        {
            string name = default;
            IDictionary<string, BinaryData> serializedAdditionalRawData = default;
            Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("name"u8))
                {
                    name = property.Value.GetString();
                    continue;
                }
                if (true)
                {
                    rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
                }
            }
            serializedAdditionalRawData = rawDataDictionary;
            return new ChatFunctionChoice(name, serializedAdditionalRawData);
        }
    }
}
