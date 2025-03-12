using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

public partial class StreamingResponseFileSearchCallUpdate : IJsonModel<StreamingResponseFileSearchCallUpdate>
{
    void IJsonModel<StreamingResponseFileSearchCallUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    => CustomSerializationHelpers.SerializeInstance(this, SerializeStreamingResponseFileSearchCallUpdate, writer, options);

    StreamingResponseFileSearchCallUpdate IJsonModel<StreamingResponseFileSearchCallUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeResponsesItemStreamingPartDeltaUpdate, ref reader, options);

    BinaryData IPersistableModel<StreamingResponseFileSearchCallUpdate>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    StreamingResponseFileSearchCallUpdate IPersistableModel<StreamingResponseFileSearchCallUpdate>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeResponsesItemStreamingPartDeltaUpdate, data, options);

    string IPersistableModel<StreamingResponseFileSearchCallUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeStreamingResponseFileSearchCallUpdate(
        StreamingResponseFileSearchCallUpdate instance,
        Utf8JsonWriter writer,
        ModelReaderWriterOptions options)
            => writer.WriteFirstObject<StreamingResponseUpdate>(
                options,
                instance._fileSearchCallCompleted,
                instance._fileSearchCallInProgress,
                instance._fileSearchCallSearching);

    internal static StreamingResponseFileSearchCallUpdate DeserializeResponsesItemStreamingPartDeltaUpdate(JsonElement element, ModelReaderWriterOptions options = null)
        => DeserializeUpdateWithWrappers(element, options) as StreamingResponseFileSearchCallUpdate;
}
