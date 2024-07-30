using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Embeddings;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Embeddings.EmbeddingCollection>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class EmbeddingCollection : IJsonModel<EmbeddingCollection>
{
    // CUSTOM:
    // - Serialized the Items property.
    // - Recovered the deserialization of SerializedAdditionalRawData. See https://github.com/Azure/autorest.csharp/issues/4636.
    void IJsonModel<EmbeddingCollection>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeEmbeddingCollection, writer, options);

    internal static void SerializeEmbeddingCollection(EmbeddingCollection instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("data"u8);
        writer.WriteStartArray();
        foreach (var item in instance.Items)
        {
            writer.WriteObjectValue<Embedding>(item, options);
        }
        writer.WriteEndArray();
        writer.WritePropertyName("model"u8);
        writer.WriteStringValue(instance.Model);
        writer.WritePropertyName("object"u8);
        writer.WriteStringValue(instance.Object.ToString());
        writer.WritePropertyName("usage"u8);
        writer.WriteObjectValue<EmbeddingTokenUsage>(instance.Usage, options);
        writer.WriteSerializedAdditionalRawData(instance.SerializedAdditionalRawData, options);
        writer.WriteEndObject();
    }

    // CUSTOM: Recovered the deserialization of SerializedAdditionalRawData. See https://github.com/Azure/autorest.csharp/issues/4636.
    internal static EmbeddingCollection DeserializeEmbeddingCollection(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= new ModelReaderWriterOptions("W");

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        IReadOnlyList<Embedding> data = default;
        string model = default;
        InternalCreateEmbeddingResponseObject @object = default;
        EmbeddingTokenUsage usage = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("data"u8))
            {
                List<Embedding> array = new List<Embedding>();
                foreach (var item in property.Value.EnumerateArray())
                {
                    array.Add(Embedding.DeserializeEmbedding(item, options));
                }
                data = array;
                continue;
            }
            if (property.NameEquals("model"u8))
            {
                model = property.Value.GetString();
                continue;
            }
            if (property.NameEquals("object"u8))
            {
                @object = new InternalCreateEmbeddingResponseObject(property.Value.GetString());
                continue;
            }
            if (property.NameEquals("usage"u8))
            {
                usage = EmbeddingTokenUsage.DeserializeEmbeddingTokenUsage(property.Value, options);
                continue;
            }
            if (true)
            {
                rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
            }
        }
        serializedAdditionalRawData = rawDataDictionary;
        return new EmbeddingCollection(data, model, @object, usage, serializedAdditionalRawData);
    }
}
