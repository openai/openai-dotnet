using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.RealtimeConversation;

public partial class ConversationItemStreamingPartDeltaUpdate : IJsonModel<ConversationItemStreamingPartDeltaUpdate>
{
    void IJsonModel<ConversationItemStreamingPartDeltaUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    => CustomSerializationHelpers.SerializeInstance(this, SerializeConversationContentPartDeltaUpdate, writer, options);

    ConversationItemStreamingPartDeltaUpdate IJsonModel<ConversationItemStreamingPartDeltaUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeItemStreamingPartDeltaUpdate, ref reader, options);

    BinaryData IPersistableModel<ConversationItemStreamingPartDeltaUpdate>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    ConversationItemStreamingPartDeltaUpdate IPersistableModel<ConversationItemStreamingPartDeltaUpdate>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeItemStreamingPartDeltaUpdate, data, options);

    string IPersistableModel<ConversationItemStreamingPartDeltaUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeConversationContentPartDeltaUpdate(ConversationItemStreamingPartDeltaUpdate instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        List<ConversationUpdate> possibleUnionVariants =
            [
                instance._contentPartAdded,
                instance._audioDelta,
                instance._outputTranscriptionDelta,
                instance._textDelta,
                instance._functionArgumentsDelta
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

    internal static ConversationItemStreamingPartDeltaUpdate DeserializeItemStreamingPartDeltaUpdate(JsonElement element, ModelReaderWriterOptions options = null)
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
                    StringComparer.OrdinalIgnoreCase.Equals(updateType, ConversationUpdateKind.ItemContentPartStarted.ToSerialString())
                        ? InternalRealtimeServerEventResponseContentPartAdded.DeserializeInternalRealtimeServerEventResponseContentPartAdded(element, options)
                    : StringComparer.OrdinalIgnoreCase.Equals(updateType, ConversationUpdateKind.ItemStreamingPartAudioDelta.ToSerialString())
                        ? InternalRealtimeServerEventResponseAudioDelta.DeserializeInternalRealtimeServerEventResponseAudioDelta(element, options)
                    : StringComparer.OrdinalIgnoreCase.Equals(updateType, ConversationUpdateKind.ItemStreamingPartAudioTranscriptionDelta.ToSerialString())
                        ? InternalRealtimeServerEventResponseAudioTranscriptDelta.DeserializeInternalRealtimeServerEventResponseAudioTranscriptDelta(element, options)
                    : StringComparer.OrdinalIgnoreCase.Equals(updateType, ConversationUpdateKind.ItemStreamingPartTextDelta.ToSerialString())
                        ? InternalRealtimeServerEventResponseTextDelta.DeserializeInternalRealtimeServerEventResponseTextDelta(element, options)
                    : StringComparer.OrdinalIgnoreCase.Equals(updateType, ConversationUpdateKind.ItemStreamingFunctionCallArgumentsDelta.ToSerialString())
                        ? InternalRealtimeServerEventResponseFunctionCallArgumentsDelta.DeserializeInternalRealtimeServerEventResponseFunctionCallArgumentsDelta(element, options)
                    : null;

                return new ConversationItemStreamingPartDeltaUpdate(baseUpdate);
            }
        }
        return null;
    }
}
