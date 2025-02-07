using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.SystemChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class SystemChatMessage : IJsonModel<SystemChatMessage>
{
    void IJsonModel<SystemChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeSystemChatMessage, writer, options);

    internal static void SerializeSystemChatMessage(SystemChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    internal override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        WriteRoleProperty(writer, options);
        WriteContentProperty(writer, options);
        writer.WriteOptionalProperty("name"u8, ParticipantName, options);
        writer.WriteSerializedAdditionalRawData(_additionalBinaryDataProperties, options);
        writer.WriteEndObject();
    }
}