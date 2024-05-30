using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.AssistantResponseFormat>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.AssistantResponseFormat>.Create", typeof(Utf8JsonReader), typeof(ModelReaderWriterOptions))]
[CodeGenSuppress("global::System.ClientModel.Primitives.IPersistableModel<OpenAI.Assistants.AssistantResponseFormat>.Write", typeof(ModelReaderWriterOptions))]
[CodeGenSuppress("global::System.ClientModel.Primitives.IPersistableModel<OpenAI.Assistants.AssistantResponseFormat>.Create", typeof(BinaryData), typeof(ModelReaderWriterOptions))]
public partial class AssistantResponseFormat : IJsonModel<AssistantResponseFormat>
{
    void IJsonModel<AssistantResponseFormat>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
         => CustomSerializationHelpers.SerializeInstance(this, SerializeAssistantResponseFormat, writer, options);

    AssistantResponseFormat IJsonModel<AssistantResponseFormat>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeAssistantResponseFormat, ref reader, options);

    BinaryData IPersistableModel<AssistantResponseFormat>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    AssistantResponseFormat IPersistableModel<AssistantResponseFormat>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeAssistantResponseFormat, data, options);

    internal static void SerializeAssistantResponseFormat(AssistantResponseFormat formatInstance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (formatInstance._plainTextValue is not null)
        {
            writer.WriteStringValue(formatInstance._plainTextValue);
        }
        else
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type"u8);
            writer.WriteStringValue(formatInstance._objectType);
            writer.WriteSerializedAdditionalRawData(formatInstance._serializedAdditionalRawData, options);
            writer.WriteEndObject();
        }
    }

    internal static AssistantResponseFormat DeserializeAssistantResponseFormat(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        string plainTextValue = null;
        string objectType = null;
        IDictionary<string, BinaryData> rawDataDictionary = new ChangeTrackingDictionary<string, BinaryData>();

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        else if (element.ValueKind == JsonValueKind.String)
        {
            plainTextValue = element.GetString();
        }
        else
        {
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("type"u8))
                {
                    objectType = property.Value.GetString();
                    continue;
                }
                if (true)
                {
                    rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
                }
            }
        }
        return new AssistantResponseFormat(plainTextValue, objectType, rawDataDictionary);
    }
}
