using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.UserChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class UserChatMessage : IJsonModel<UserChatMessage>
{
    void IJsonModel<UserChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeUserChatMessage, writer, options);

    internal static void SerializeUserChatMessage(UserChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
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
            if (instance.Content.Count == 1 && !string.IsNullOrEmpty(instance.Content[0].Text))
            {
                writer.WriteStringValue(instance.Content[0].Text);
            }
            else
            {
                writer.WriteStartArray();
                foreach (var item in instance.Content)
                {
                    writer.WriteObjectValue(item, options);
                }
                writer.WriteEndArray();
            }
        }
        writer.WriteSerializedAdditionalRawData(instance._serializedAdditionalRawData, options);
        writer.WriteEndObject();
    }

    internal static UserChatMessage DeserializeUserChatMessage(JsonElement element, ModelReaderWriterOptions options = null)
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
                if (property.Value.ValueKind == JsonValueKind.String)
                {
                    List<ChatMessageContentPart> array = new List<ChatMessageContentPart>();
                    array.Add(ChatMessageContentPart.CreateTextMessageContentPart(property.Value.GetString()));
                    continue;
                }
                else
                {
                    List<ChatMessageContentPart> array = new List<ChatMessageContentPart>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(ChatMessageContentPart.DeserializeChatMessageContentPart(item, options));
                    }
                    content = array;
                    continue;
                }
            }
            if (true)
            {
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new UserChatMessage(role, content ?? new ChangeTrackingList<ChatMessageContentPart>(), serializedAdditionalRawData, name);
    }
}
