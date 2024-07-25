using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.FunctionChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class FunctionChatMessage : IJsonModel<FunctionChatMessage>
{
    void IJsonModel<FunctionChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeFunctionChatMessage, writer, options);

    internal static void SerializeFunctionChatMessage(FunctionChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        instance.WriteCore(writer, options);
    }

    protected override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("name"u8);
        writer.WriteStringValue(FunctionName);
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(Role);
        if (Optional.IsCollectionDefined(Content))
        {
            if (Content[0] != null)
            {
                writer.WritePropertyName("content"u8);
                writer.WriteStringValue(Content[0].Text);
            }
            else
            {
                writer.WriteNull("content");
            }
        }
        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }

    internal static FunctionChatMessage DeserializeFunctionChatMessage(JsonElement element, ModelReaderWriterOptions options = null)
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
                if (property.Value.ValueKind == JsonValueKind.Null)
                {
                    continue;
                }
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
        return new FunctionChatMessage(role, content ?? new ChangeTrackingList<ChatMessageContentPart>(), serializedAdditionalRawData, name);
    }
}
