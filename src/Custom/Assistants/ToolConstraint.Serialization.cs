using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Assistants;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.ToolConstraint>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Assistants.ToolConstraint>.Create", typeof(Utf8JsonReader), typeof(ModelReaderWriterOptions))]
[CodeGenSuppress("global::System.ClientModel.Primitives.IPersistableModel<OpenAI.Assistants.ToolConstraint>.Write", typeof(ModelReaderWriterOptions))]
[CodeGenSuppress("global::System.ClientModel.Primitives.IPersistableModel<OpenAI.Assistants.ToolConstraint>.Create", typeof(BinaryData), typeof(ModelReaderWriterOptions))]
public partial class ToolConstraint : IJsonModel<ToolConstraint>
{
    void IJsonModel<ToolConstraint>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeToolConstraint, writer, options);

    ToolConstraint IJsonModel<ToolConstraint>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeToolConstraint, ref reader, options);

    BinaryData IPersistableModel<ToolConstraint>.Write(ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, options);

    ToolConstraint IPersistableModel<ToolConstraint>.Create(BinaryData data, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.DeserializeNewInstance(this, DeserializeToolConstraint, data, options);

    internal static void SerializeToolConstraint(ToolConstraint instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        if (instance._plainTextValue is not null)
        {
            writer.WriteStringValue(instance._plainTextValue);
        }
        else
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type"u8);
            writer.WriteStringValue(instance.Kind.ToString());
            if (Optional.IsDefined(instance._objectFunctionName))
            {
                writer.WritePropertyName("function"u8);
                writer.WriteStartObject();
                writer.WritePropertyName("name"u8);
                writer.WriteStringValue(instance._objectFunctionName);
                writer.WriteEndObject();
            }
            writer.WriteSerializedAdditionalRawData(instance.SerializedAdditionalRawData, options);
            writer.WriteEndObject();
        }
    }

    internal static ToolConstraint DeserializeToolConstraint(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        string plainTextValue = null;
        string objectType = null;
        string objectFunctionName = null;
        IDictionary<string, BinaryData> rawDataDictionary = new ChangeTrackingDictionary<string, BinaryData>();

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        if (element.ValueKind == JsonValueKind.String)
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
                if (property.NameEquals("function"u8))
                {
                    foreach (JsonProperty functionProperty in property.Value.EnumerateObject())
                    {
                        if (functionProperty.NameEquals("name"u8))
                        {
                            objectFunctionName = functionProperty.Value.GetString();
                            continue;
                        }
                    }
                    continue;
                }
                if (true)
                {
                    rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
                }
            }
        }

        return new ToolConstraint(plainTextValue, objectType, objectFunctionName, rawDataDictionary);
    }
}
