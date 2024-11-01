using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.RealtimeConversation;

public partial class ConversationItemStreamingPartFinishedUpdate : IJsonModel<ConversationItemStreamingPartFinishedUpdate>
{
    void IJsonModel<ConversationItemStreamingPartFinishedUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    => CustomSerializationHelpers.SerializeInstance(this, SerializeConversationContentPartDeltaUpdate, writer, options);

    ConversationItemStreamingPartFinishedUpdate IJsonModel<ConversationItemStreamingPartFinishedUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeItemStreamingPartFinishedUpdate, ref reader, options);

    BinaryData IPersistableModel<ConversationItemStreamingPartFinishedUpdate>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    ConversationItemStreamingPartFinishedUpdate IPersistableModel<ConversationItemStreamingPartFinishedUpdate>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeItemStreamingPartFinishedUpdate, data, options);

    string IPersistableModel<ConversationItemStreamingPartFinishedUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeConversationContentPartDeltaUpdate(ConversationItemStreamingPartFinishedUpdate instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        List<ConversationUpdate> possibleUnionVariants =
            [
                instance._contentPartDone,
                instance._functionCallArgumentsDone,
            ];

        foreach (ConversationUpdate unionVariant in possibleUnionVariants)
        {
            if (unionVariant is not null)
            {
                writer.WriteObjectValue(unionVariant, options);
                break;
            }
        }
    }

    internal static ConversationItemStreamingPartFinishedUpdate DeserializeItemStreamingPartFinishedUpdate(JsonElement element, ModelReaderWriterOptions options = null)
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

                ConversationUpdate baseUpdate =
                    StringComparer.OrdinalIgnoreCase.Equals(updateType, ConversationUpdateKind.ItemContentPartFinished.ToSerialString())
                        ? InternalRealtimeServerEventResponseContentPartDone.DeserializeInternalRealtimeServerEventResponseContentPartDone(element, options)
                    : StringComparer.OrdinalIgnoreCase.Equals(updateType, ConversationUpdateKind.ItemStreamingFunctionCallArgumentsFinished.ToSerialString())
                        ? InternalRealtimeServerEventResponseFunctionCallArgumentsDone.DeserializeInternalRealtimeServerEventResponseFunctionCallArgumentsDone(element, options)
                    : null;

                return new ConversationItemStreamingPartFinishedUpdate(baseUpdate);
            }
        }
        return null;
    }
}
