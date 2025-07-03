using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Realtime;

public partial class OutputPartFinishedUpdate : IJsonModel<OutputPartFinishedUpdate>
{
    void IJsonModel<OutputPartFinishedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    => CustomSerializationHelpers.SerializeInstance(this, SerializeConversationContentPartDeltaUpdate, writer, options);

    OutputPartFinishedUpdate IJsonModel<OutputPartFinishedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeOutputPartFinishedUpdate, ref reader, options);

    BinaryData IPersistableModel<OutputPartFinishedUpdate>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    OutputPartFinishedUpdate IPersistableModel<OutputPartFinishedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeOutputPartFinishedUpdate, data, options);

    string IPersistableModel<OutputPartFinishedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeConversationContentPartDeltaUpdate(OutputPartFinishedUpdate instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        List<RealtimeUpdate> possibleUnionVariants =
            [
                instance._contentPartDone,
                instance._functionCallArgumentsDone,
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

    internal static OutputPartFinishedUpdate DeserializeOutputPartFinishedUpdate(JsonElement element, ModelReaderWriterOptions options = null)
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
                    StringComparer.OrdinalIgnoreCase.Equals(updateType, RealtimeUpdateKind.ItemContentPartFinished.ToSerialString())
                        ? InternalRealtimeServerEventResponseContentPartDone.DeserializeInternalRealtimeServerEventResponseContentPartDone(element, options)
                    : StringComparer.OrdinalIgnoreCase.Equals(updateType, RealtimeUpdateKind.ItemStreamingFunctionCallArgumentsFinished.ToSerialString())
                        ? InternalRealtimeServerEventResponseFunctionCallArgumentsDone.DeserializeInternalRealtimeServerEventResponseFunctionCallArgumentsDone(element, options)
                    : null;

                return new OutputPartFinishedUpdate(baseUpdate);
            }
        }
        return null;
    }
}
