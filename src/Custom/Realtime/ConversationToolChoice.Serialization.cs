using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Realtime;

public partial class ConversationToolChoice : IJsonModel<ConversationToolChoice>
{
    void IJsonModel<ConversationToolChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeConversationToolChoice, writer, options);

    ConversationToolChoice IJsonModel<ConversationToolChoice>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeConversationToolChoice, ref reader, options);

    BinaryData IPersistableModel<ConversationToolChoice>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    ConversationToolChoice IPersistableModel<ConversationToolChoice>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeConversationToolChoice, data, options);

    string IPersistableModel<ConversationToolChoice>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeConversationToolChoice(ConversationToolChoice instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (instance._objectToolChoice is not null)
        {
            writer.WriteObjectValue(instance._objectToolChoice, options);
        }
        else
        {
            writer.WriteStringValue(instance.Kind.ToSerialString());
        }
    }

    internal static ConversationToolChoice DeserializeConversationToolChoice(JsonElement element, ModelReaderWriterOptions options = null)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        if (element.ValueKind == JsonValueKind.String)
        {
            return new ConversationToolChoice(element.GetString().ToConversationToolChoiceKind(), null);
        }
        if (element.ValueKind == JsonValueKind.Object)
        {
            InternalRealtimeToolChoiceObject choiceObject = InternalRealtimeToolChoiceObject
                .DeserializeInternalRealtimeToolChoiceObject(element, options);
            return choiceObject switch
            {
                InternalRealtimeToolChoiceFunctionObject => new(ConversationToolChoiceKind.Function, choiceObject),
                _ => new(ConversationToolChoiceKind.Unknown, choiceObject),
            };
        }
        return null;
    }

    internal static ConversationToolChoice FromBinaryData(BinaryData bytes)
    {
        if (bytes is null)
        {
            return null;
        }
        using JsonDocument document = JsonDocument.Parse(bytes);
        return DeserializeConversationToolChoice(document.RootElement);
    }
}