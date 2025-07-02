using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Realtime;

internal partial class InternalRealtimeNoTurnDetection : IJsonModel<InternalRealtimeNoTurnDetection>
{
    void IJsonModel<InternalRealtimeNoTurnDetection>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeInternalRealtimeNoTurnDetection, writer, options);

    InternalRealtimeNoTurnDetection IJsonModel<InternalRealtimeNoTurnDetection>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeInternalRealtimeNoTurnDetection, ref reader, options);

    BinaryData IPersistableModel<InternalRealtimeNoTurnDetection>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    InternalRealtimeNoTurnDetection IPersistableModel<InternalRealtimeNoTurnDetection>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeInternalRealtimeNoTurnDetection, data, options);

    string IPersistableModel<InternalRealtimeNoTurnDetection>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeInternalRealtimeNoTurnDetection(InternalRealtimeNoTurnDetection instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteNullValue();
    }

    internal static InternalRealtimeNoTurnDetection DeserializeInternalRealtimeNoTurnDetection(JsonElement element, ModelReaderWriterOptions options = null)
    {
        return new();
    }
}