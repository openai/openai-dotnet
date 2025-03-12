using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

public partial class StreamingResponseItemUpdate : IJsonModel<StreamingResponseItemUpdate>
{
    void IJsonModel<StreamingResponseItemUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeStreamingResponseItemUpdate, writer, options);

    StreamingResponseItemUpdate IJsonModel<StreamingResponseItemUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeStreamingResponseItemUpdate, ref reader, options);

    BinaryData IPersistableModel<StreamingResponseItemUpdate>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    StreamingResponseItemUpdate IPersistableModel<StreamingResponseItemUpdate>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeStreamingResponseItemUpdate, data, options);

    string IPersistableModel<StreamingResponseItemUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeStreamingResponseItemUpdate(
        StreamingResponseItemUpdate instance,
        Utf8JsonWriter writer,
        ModelReaderWriterOptions options)
            => writer.WriteFirstObject<StreamingResponseUpdate>(
                options,
                instance._outputItemAdded,
                instance._outputItemDone);

    internal static StreamingResponseItemUpdate DeserializeStreamingResponseItemUpdate(
        JsonElement element,
        ModelReaderWriterOptions options)
            => DeserializeUpdateWithWrappers(element, options) as StreamingResponseItemUpdate;
}
