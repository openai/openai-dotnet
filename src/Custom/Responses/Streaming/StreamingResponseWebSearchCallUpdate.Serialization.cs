using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

public partial class StreamingResponseWebSearchCallUpdate : IJsonModel<StreamingResponseWebSearchCallUpdate>
{
    void IJsonModel<StreamingResponseWebSearchCallUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    => CustomSerializationHelpers.SerializeInstance(this, SerializeStreamingResponseWebSearchCallUpdate, writer, options);

    StreamingResponseWebSearchCallUpdate IJsonModel<StreamingResponseWebSearchCallUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeResponsesItemStreamingPartDeltaUpdate, ref reader, options);

    BinaryData IPersistableModel<StreamingResponseWebSearchCallUpdate>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    StreamingResponseWebSearchCallUpdate IPersistableModel<StreamingResponseWebSearchCallUpdate>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeResponsesItemStreamingPartDeltaUpdate, data, options);

    string IPersistableModel<StreamingResponseWebSearchCallUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeStreamingResponseWebSearchCallUpdate(
        StreamingResponseWebSearchCallUpdate instance,
        Utf8JsonWriter writer,
        ModelReaderWriterOptions options)
            => writer.WriteFirstObject<StreamingResponseUpdate>(
                options,
                instance._webSearchCallCompleted,
                instance._webSearchCallInProgress,
                instance._webSearchCallSearching);

    internal static StreamingResponseWebSearchCallUpdate DeserializeResponsesItemStreamingPartDeltaUpdate(JsonElement element, ModelReaderWriterOptions options = null)
        => DeserializeUpdateWithWrappers(element, options) as StreamingResponseWebSearchCallUpdate;
}
