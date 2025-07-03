using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Realtime;

public partial class OutputDeltaUpdate : IJsonModel<OutputDeltaUpdate>
{
    void IJsonModel<OutputDeltaUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    => CustomSerializationHelpers.SerializeInstance(this, SerializeOutputDeltaUpdate, writer, options);

    OutputDeltaUpdate IJsonModel<OutputDeltaUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeOutputDeltaUpdate, ref reader, options);

    BinaryData IPersistableModel<OutputDeltaUpdate>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    OutputDeltaUpdate IPersistableModel<OutputDeltaUpdate>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeOutputDeltaUpdate, data, options);

    string IPersistableModel<OutputDeltaUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeOutputDeltaUpdate(OutputDeltaUpdate instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        List<RealtimeUpdate> possibleUnionVariants =
            [
                instance._contentPartAdded,
                instance._audioDelta,
                instance._outputTranscriptionDelta,
                instance._textDelta,
                instance._functionArgumentsDelta
            ];

        foreach (RealtimeUpdate unionVariant in possibleUnionVariants)
        {
            if (unionVariant is not null)
            {
                writer.WriteObjectValue(unionVariant, options);
                break;
            }
        }
    }

    internal static OutputDeltaUpdate DeserializeOutputDeltaUpdate(JsonElement element, ModelReaderWriterOptions options = null)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }
        foreach (JsonProperty elementProperty in element.EnumerateObject())
        {
            if (elementProperty.NameEquals("type"u8))
            {
                string updateType = elementProperty.Value.ToString();

                RealtimeUpdate baseUpdate =
                    StringComparer.OrdinalIgnoreCase.Equals(updateType, RealtimeUpdateKind.ItemContentPartStarted.ToSerialString())
                        ? InternalRealtimeServerEventResponseContentPartAdded.DeserializeInternalRealtimeServerEventResponseContentPartAdded(element, options)
                    : StringComparer.OrdinalIgnoreCase.Equals(updateType, RealtimeUpdateKind.ItemStreamingPartAudioDelta.ToSerialString())
                        ? InternalRealtimeServerEventResponseAudioDelta.DeserializeInternalRealtimeServerEventResponseAudioDelta(element, options)
                    : StringComparer.OrdinalIgnoreCase.Equals(updateType, RealtimeUpdateKind.ItemStreamingPartAudioTranscriptionDelta.ToSerialString())
                        ? InternalRealtimeServerEventResponseAudioTranscriptDelta.DeserializeInternalRealtimeServerEventResponseAudioTranscriptDelta(element, options)
                    : StringComparer.OrdinalIgnoreCase.Equals(updateType, RealtimeUpdateKind.ItemStreamingPartTextDelta.ToSerialString())
                        ? InternalRealtimeServerEventResponseTextDelta.DeserializeInternalRealtimeServerEventResponseTextDelta(element, options)
                    : StringComparer.OrdinalIgnoreCase.Equals(updateType, RealtimeUpdateKind.ItemStreamingFunctionCallArgumentsDelta.ToSerialString())
                        ? InternalRealtimeServerEventResponseFunctionCallArgumentsDelta.DeserializeInternalRealtimeServerEventResponseFunctionCallArgumentsDelta(element, options)
                    : null;

                return new OutputDeltaUpdate(baseUpdate);
            }
        }
        return null;
    }
}
