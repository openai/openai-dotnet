using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

public partial class StreamingResponseContentPartDeltaUpdate : IJsonModel<StreamingResponseContentPartDeltaUpdate>
{
    void IJsonModel<StreamingResponseContentPartDeltaUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    => CustomSerializationHelpers.SerializeInstance(this, SerializeStreamingResponseContentPartDeltaUpdate, writer, options);

    StreamingResponseContentPartDeltaUpdate IJsonModel<StreamingResponseContentPartDeltaUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeResponsesItemStreamingPartDeltaUpdate, ref reader, options);

    BinaryData IPersistableModel<StreamingResponseContentPartDeltaUpdate>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    StreamingResponseContentPartDeltaUpdate IPersistableModel<StreamingResponseContentPartDeltaUpdate>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeResponsesItemStreamingPartDeltaUpdate, data, options);

    string IPersistableModel<StreamingResponseContentPartDeltaUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeStreamingResponseContentPartDeltaUpdate(
        StreamingResponseContentPartDeltaUpdate instance,
        Utf8JsonWriter writer,
        ModelReaderWriterOptions options)
            => writer.WriteFirstObject<StreamingResponseUpdate>(
                options,
                instance._contentPartAdded,
                instance._outputTextDelta,
                instance._functionArgumentsDelta,
                instance._refusalDelta);

    internal static StreamingResponseContentPartDeltaUpdate DeserializeResponsesItemStreamingPartDeltaUpdate(JsonElement element, ModelReaderWriterOptions options = null)
        => DeserializeUpdateWithWrappers(element, options) as StreamingResponseContentPartDeltaUpdate;
}
