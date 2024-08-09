using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Images;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Images.GeneratedImageCollection>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class GeneratedImageCollection : IJsonModel<GeneratedImageCollection>
{
    // CUSTOM:
    // - Serialized the Items property.
    // - Recovered the serialization of SerializedAdditionalRawData. See https://github.com/Azure/autorest.csharp/issues/4636.
    void IJsonModel<GeneratedImageCollection>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeGeneratedImageCollection, writer, options);

    internal static void SerializeGeneratedImageCollection(GeneratedImageCollection instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("created"u8);
        writer.WriteNumberValue(instance.CreatedAt, "U");
        writer.WritePropertyName("data"u8);
        writer.WriteStartArray();
        foreach (var item in instance.Items)
        {
            writer.WriteObjectValue<GeneratedImage>(item, options);
        }
        writer.WriteEndArray();
        writer.WriteSerializedAdditionalRawData(instance.SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }

    // CUSTOM: Recovered the deserialization of SerializedAdditionalRawData. See https://github.com/Azure/autorest.csharp/issues/4636.
    internal static GeneratedImageCollection DeserializeGeneratedImageCollection(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= ModelSerializationExtensions.WireOptions;

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        DateTimeOffset created = default;
        IReadOnlyList<GeneratedImage> data = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("created"u8))
            {
                created = DateTimeOffset.FromUnixTimeSeconds(property.Value.GetInt64());
                continue;
            }
            if (property.NameEquals("data"u8))
            {
                List<GeneratedImage> array = new List<GeneratedImage>();
                foreach (var item in property.Value.EnumerateArray())
                {
                    array.Add(GeneratedImage.DeserializeGeneratedImage(item, options));
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
        return new GeneratedImageCollection(created, data, serializedAdditionalRawData);
    }
}
