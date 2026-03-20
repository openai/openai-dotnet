using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Assistants;

internal partial class InternalAssistantResponseFormatPlainTextNoObject : IJsonModel<InternalAssistantResponseFormatPlainTextNoObject>
{
    internal static void SerializeInternalAssistantResponseFormatPlainTextNoObject(InternalAssistantResponseFormatPlainTextNoObject instance, Utf8JsonWriter writer, ModelReaderWriterOptions options = null)
    {
        writer.WriteStringValue(instance.Value);
    }

    internal static InternalAssistantResponseFormatPlainTextNoObject DeserializeInternalAssistantResponseFormatPlainTextNoObject(JsonElement element, ModelReaderWriterOptions options = null)
    {
        if (element.ValueKind == JsonValueKind.String)
        {
            return new(element.GetString());
        }
        return null;
    }

    void IJsonModel<InternalAssistantResponseFormatPlainTextNoObject>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeInternalAssistantResponseFormatPlainTextNoObject, writer, options);

    InternalAssistantResponseFormatPlainTextNoObject IJsonModel<InternalAssistantResponseFormatPlainTextNoObject>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeInternalAssistantResponseFormatPlainTextNoObject, ref reader, options);

    BinaryData IPersistableModel<InternalAssistantResponseFormatPlainTextNoObject>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    InternalAssistantResponseFormatPlainTextNoObject IPersistableModel<InternalAssistantResponseFormatPlainTextNoObject>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeInternalAssistantResponseFormatPlainTextNoObject, data, options);

    string IPersistableModel<InternalAssistantResponseFormatPlainTextNoObject>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
}
