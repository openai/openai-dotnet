using OpenAI.Models;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

namespace OpenAI.Models;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Models.OpenAIModelInfoCollection>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class OpenAIModelInfoCollection : IJsonModel<OpenAIModelInfoCollection>
{
    // CUSTOM:
    // - Serialized the Items property.
    // - Recovered the serialization of SerializedAdditionalRawData. See https://github.com/Azure/autorest.csharp/issues/4636.
    void IJsonModel<OpenAIModelInfoCollection>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeOpenAIModelInfoCollection, writer, options);

    internal static void SerializeOpenAIModelInfoCollection(OpenAIModelInfoCollection instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("object"u8);
        writer.WriteStringValue(instance.Object.ToString());
        writer.WritePropertyName("data"u8);
        writer.WriteStartArray();
        foreach (var item in instance.Items)
        {
            writer.WriteObjectValue<OpenAIModelInfo>(item, options);
        }
        writer.WriteEndArray();
        writer.WriteSerializedAdditionalRawData(instance.SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }

    // CUSTOM: Recovered the deserialization of SerializedAdditionalRawData. See https://github.com/Azure/autorest.csharp/issues/4636.
    internal static OpenAIModelInfoCollection DeserializeOpenAIModelInfoCollection(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        InternalListModelsResponseObject @object = default;
        IReadOnlyList<OpenAIModelInfo> data = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("object"u8))
            {
                @object = new InternalListModelsResponseObject(property.Value.GetString());
                continue;
            }
            if (property.NameEquals("data"u8))
            {
                List<OpenAIModelInfo> array = new List<OpenAIModelInfo>();
                foreach (var item in property.Value.EnumerateArray())
                {
                    array.Add(OpenAIModelInfo.DeserializeOpenAIModelInfo(item, options));
                }
                data = array;
                continue;
            }
            if (true)
            {
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new OpenAIModelInfoCollection(@object, data, serializedAdditionalRawData);
    }
}