using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.InternalMessageImageUrlContent>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
internal partial class InternalMessageImageUrlContent : IJsonModel<InternalMessageImageUrlContent>
{
    void IJsonModel<InternalMessageImageUrlContent>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeInternalMessageImageUrlContent, writer, options);

    internal static void SerializeInternalMessageImageUrlContent(InternalMessageImageUrlContent instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => instance.WriteCore(writer, options);

    protected override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("type"u8);
        writer.WriteStringValue(_type);
        writer.WritePropertyName("image_url"u8);
        writer.WriteObjectValue<InternalMessageContentImageUrlObjectImageUrl>(_imageUrl, options);
        writer.WriteSerializedAdditionalRawData(SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }
}