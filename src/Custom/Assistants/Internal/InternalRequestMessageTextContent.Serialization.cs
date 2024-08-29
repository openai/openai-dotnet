using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.InternalRequestMessageTextContent>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
internal partial class InternalRequestMessageTextContent : IJsonModel<InternalRequestMessageTextContent>
{
    void IJsonModel<InternalRequestMessageTextContent>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeInternalRequestMessageTextContent, writer, options);

    internal static void SerializeInternalRequestMessageTextContent(InternalRequestMessageTextContent instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    protected override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("type"u8);
        writer.WriteStringValue(Type.ToString());
        writer.WritePropertyName("text"u8);
        writer.WriteStringValue(InternalText);
        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}
