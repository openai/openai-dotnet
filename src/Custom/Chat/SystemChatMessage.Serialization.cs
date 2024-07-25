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
            writer.WriteStringValue(Content[0].Text);
        }
        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}