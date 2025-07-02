using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Realtime;

internal partial class InternalRealtimeAudioDisabledNoiseReduction : IJsonModel<InternalRealtimeAudioDisabledNoiseReduction>
{
    void IJsonModel<InternalRealtimeAudioDisabledNoiseReduction>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeInternalRealtimeAudioDisabledNoiseReduction, writer, options);

    InternalRealtimeAudioDisabledNoiseReduction IJsonModel<InternalRealtimeAudioDisabledNoiseReduction>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeInternalRealtimeAudioDisabledNoiseReduction, ref reader, options);

    BinaryData IPersistableModel<InternalRealtimeAudioDisabledNoiseReduction>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    InternalRealtimeAudioDisabledNoiseReduction IPersistableModel<InternalRealtimeAudioDisabledNoiseReduction>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeInternalRealtimeAudioDisabledNoiseReduction, data, options);

    string IPersistableModel<InternalRealtimeAudioDisabledNoiseReduction>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeInternalRealtimeAudioDisabledNoiseReduction(InternalRealtimeAudioDisabledNoiseReduction instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteNullValue();
    }

    internal static InternalRealtimeAudioDisabledNoiseReduction DeserializeInternalRealtimeAudioDisabledNoiseReduction(JsonElement element, ModelReaderWriterOptions options = null)
    {
        if (element.ValueKind != JsonValueKind.Null)
        {
            throw new ArgumentException($"Unexpected deserialization of disabled input_audio_noise_reduction with non-null element kind: {element.ValueKind}");
        }
        return new();
    }
}