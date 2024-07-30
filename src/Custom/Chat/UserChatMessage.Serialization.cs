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
        => instance.WriteCore(writer, options);

    protected override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        if (Optional.IsDefined(ParticipantName))
        {
            writer.WritePropertyName("name"u8);
            writer.WriteStringValue(ParticipantName);
        }
        writer.WritePropertyName("role"u8);
        writer.WriteStringValue(Role);
        if (Optional.IsCollectionDefined(Content))
        {
            writer.WritePropertyName("content"u8);
            if (Content.Count == 1 && !string.IsNullOrEmpty(Content[0].Text))
            {
                writer.WriteStringValue(Content[0].Text);
            }
            else
            {
                writer.WriteStartArray();
                foreach (var item in Content)
                {
                    writer.WriteObjectValue(item, options);
                }
                writer.WriteEndArray();
            }
        }
        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}