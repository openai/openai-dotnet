using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.InternalMessageImageFileContent>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
internal partial class InternalMessageImageFileContent : IJsonModel<InternalMessageImageFileContent>
{
    void IJsonModel<InternalMessageImageFileContent>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeInternalMessageImageFileContent, writer, options);

    internal static void SerializeInternalMessageImageFileContent(InternalMessageImageFileContent instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    protected override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("type"u8);
        writer.WriteStringValue(_type);
        writer.WritePropertyName("image_file"u8);
        writer.WriteObjectValue<InternalMessageContentItemFileObjectImageFile>(_imageFile, options);
        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}
