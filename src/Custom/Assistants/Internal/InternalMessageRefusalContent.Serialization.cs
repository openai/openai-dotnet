using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.InternalMessageRefusalContent>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
internal partial class InternalMessageRefusalContent : IJsonModel<InternalMessageRefusalContent>
{
    void IJsonModel<InternalMessageRefusalContent>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeInternalMessageRefusalContent, writer, options);

    internal static void SerializeInternalMessageRefusalContent(InternalMessageRefusalContent instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    protected override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("type"u8);
        writer.WriteStringValue(_type);
        writer.WritePropertyName("refusal"u8);
        writer.WriteStringValue(Refusal);
        writer.WriteEndObject();
    }
}