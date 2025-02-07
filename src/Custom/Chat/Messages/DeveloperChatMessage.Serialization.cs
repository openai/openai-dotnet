using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Chat;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Chat.DeveloperChatMessage>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class DeveloperChatMessage : IJsonModel<DeveloperChatMessage>
{
    void IJsonModel<DeveloperChatMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeDeveloperChatMessage, writer, options);

    internal static void SerializeDeveloperChatMessage(DeveloperChatMessage instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
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