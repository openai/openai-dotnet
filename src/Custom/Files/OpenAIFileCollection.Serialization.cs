using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Files;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Files.OpenAIFileCollection>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class OpenAIFileCollection : IJsonModel<OpenAIFileCollection>
{
    // CUSTOM:
    // - Serialized the Items property.
    // - Recovered the serialization of SerializedAdditionalRawData. See https://github.com/Azure/autorest.csharp/issues/4636.
    void IJsonModel<OpenAIFileCollection>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeOpenAIFileCollection, writer, options);

    internal static void SerializeOpenAIFileCollection(OpenAIFileCollection instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("data"u8);
        writer.WriteStartArray();
        foreach (var item in instance.Items)
        {
            writer.WriteObjectValue<OpenAIFile>(item, options);
        }
        writer.WriteEndArray();
        writer.WritePropertyName("object"u8);
        writer.WriteStringValue(instance.Object.ToString());
        writer.WriteSerializedAdditionalRawData(instance.SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }

    // CUSTOM: Recovered the deserialization of SerializedAdditionalRawData. See https://github.com/Azure/autorest.csharp/issues/4636.
    internal static OpenAIFileCollection DeserializeOpenAIFileCollection(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        IReadOnlyList<OpenAIFile> data = default;
        InternalListFilesResponseObject @object = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("data"u8))
            {
                List<OpenAIFile> array = new List<OpenAIFile>();
                foreach (var item in property.Value.EnumerateArray())
                {
                    array.Add(OpenAIFile.DeserializeOpenAIFile(item, options));
                }
                data = array;
                continue;
            }
            if (property.NameEquals("object"u8))
            {
                @object = new InternalListFilesResponseObject(property.Value.GetString());
                continue;
            }
            if (true)
            {
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new OpenAIFileCollection(data, @object, serializedAdditionalRawData);
    }

}
