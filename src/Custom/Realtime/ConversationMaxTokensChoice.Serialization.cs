using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Realtime;

public partial class ConversationMaxTokensChoice : IJsonModel<ConversationMaxTokensChoice>
{
    void IJsonModel<ConversationMaxTokensChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeConversationMaxTokensChoice, writer, options);

    ConversationMaxTokensChoice IJsonModel<ConversationMaxTokensChoice>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeConversationMaxTokensChoice, ref reader, options);

    BinaryData IPersistableModel<ConversationMaxTokensChoice>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    ConversationMaxTokensChoice IPersistableModel<ConversationMaxTokensChoice>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeConversationMaxTokensChoice, data, options);

    string IPersistableModel<ConversationMaxTokensChoice>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

    internal static void SerializeConversationMaxTokensChoice(ConversationMaxTokensChoice instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (instance._isDefaultNullValue == true)
        {
            writer.WriteNullValue();
        }
        else if (instance._stringValue is not null)
        {
            writer.WriteStringValue(instance._stringValue);
        }
        else if (instance.NumericValue.HasValue)
        {
            writer.WriteNumberValue(instance.NumericValue.Value);
        }
    }

    internal static ConversationMaxTokensChoice DeserializeConversationMaxTokensChoice(JsonElement element, ModelReaderWriterOptions options = null)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return new ConversationMaxTokensChoice(isDefaultNullValue: true);
        }
        if (element.ValueKind == JsonValueKind.String)
        {
            return new ConversationMaxTokensChoice(stringValue: element.GetString());
        }
        if (element.ValueKind == JsonValueKind.Number)
        {
            return new ConversationMaxTokensChoice(numberValue: element.GetInt32());
        }
        return null;
    }

    internal static ConversationMaxTokensChoice FromBinaryData(BinaryData bytes)
    {
        if (bytes is null)
        {
            return new ConversationMaxTokensChoice(isDefaultNullValue: true);
        }
        using JsonDocument document = JsonDocument.Parse(bytes);
        return DeserializeConversationMaxTokensChoice(document.RootElement);
    }
}