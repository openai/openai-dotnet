using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

public partial class StreamingResponseStatusUpdate : IJsonModel<StreamingResponseStatusUpdate>
{
    void IJsonModel<StreamingResponseStatusUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeStreamingResponseStatusUpdate, writer, options);

    StreamingResponseStatusUpdate IJsonModel<StreamingResponseStatusUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeStreamingResponseStatusUpdate, ref reader, options);

    BinaryData IPersistableModel<StreamingResponseStatusUpdate>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    StreamingResponseStatusUpdate IPersistableModel<StreamingResponseStatusUpdate>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeStreamingResponseStatusUpdate, data, options);

    string IPersistableModel<StreamingResponseStatusUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeStreamingResponseStatusUpdate(
        StreamingResponseStatusUpdate instance,
        Utf8JsonWriter writer,
        ModelReaderWriterOptions options)
            => writer.WriteFirstObject<StreamingResponseUpdate>(
                options,
                instance._responseCompleted,
                instance._responseIncomplete,
                instance._responseInProgress,
                instance._responseFailed,
                instance._responseCreated);

    internal static StreamingResponseStatusUpdate DeserializeStreamingResponseStatusUpdate(
        JsonElement element,
        ModelReaderWriterOptions options)
            => DeserializeUpdateWithWrappers(element, options) as StreamingResponseStatusUpdate;
}
