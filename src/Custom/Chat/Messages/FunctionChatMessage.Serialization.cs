using System.ClientModel.Primitives;
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

    internal override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        WriteRoleProperty(writer, options);
        WriteContentProperty(writer, options);
        writer.WritePropertyName("name"u8);
        writer.WriteStringValue(FunctionName);
        writer.WriteSerializedAdditionalRawData(_additionalBinaryDataProperties, options);
        writer.WriteEndObject();
    }
}