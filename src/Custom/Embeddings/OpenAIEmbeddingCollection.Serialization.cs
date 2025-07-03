using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Embeddings;

[CodeGenSuppress("global::System.ClientModel.Primitives.IJsonModel<OpenAI.Embeddings.OpenAIEmbeddingCollection>.Write", typeof(Utf8JsonWriter), typeof(ModelReaderWriterOptions))]
public partial class OpenAIEmbeddingCollection : IJsonModel<OpenAIEmbeddingCollection>
{
    // CUSTOM:
    // - Serialized the Items property.
    // - Recovered the deserialization of SerializedAdditionalRawData. See https://github.com/Azure/autorest.csharp/issues/4636.
    void IJsonModel<OpenAIEmbeddingCollection>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => CustomSerializationHelpers.SerializeInstance(this, SerializeOpenAIEmbeddingCollection, writer, options);

    internal static void SerializeOpenAIEmbeddingCollection(OpenAIEmbeddingCollection instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("data"u8);
        writer.WriteStartArray();
        foreach (var item in instance.Items)
        {
            writer.WriteObjectValue<OpenAIEmbedding>(item, options);
        }
        writer.WriteEndArray();
        writer.WritePropertyName("model"u8);
        writer.WriteStringValue(instance.Model);
        writer.WritePropertyName("object"u8);
        writer.WriteStringValue(instance.Object.ToString());
        writer.WritePropertyName("usage"u8);
        writer.WriteObjectValue<EmbeddingTokenUsage>(instance.Usage, options);
        writer.WriteSerializedAdditionalRawData(instance._additionalBinaryDataProperties, options);
        writer.WriteEndObject();
    }

    // CUSTOM: Recovered the deserialization of SerializedAdditionalRawData. See https://github.com/Azure/autorest.csharp/issues/4636.
    internal static OpenAIEmbeddingCollection DeserializeOpenAIEmbeddingCollection(JsonElement element, ModelReaderWriterOptions options = null)
    {
        options ??= new ModelReaderWriterOptions("W");

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        IReadOnlyList<OpenAIEmbedding> data = default;
        string model = default;
        string @object = default;
        EmbeddingTokenUsage usage = default;
        IDictionary<string, BinaryData> serializedAdditionalRawData = default;
        Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("data"u8))
            {
                List<OpenAIEmbedding> array = new List<OpenAIEmbedding>();
                foreach (var item in property.Value.EnumerateArray())
                {
                    array.Add(OpenAIEmbedding.DeserializeOpenAIEmbedding(item, options));
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
                @object = property.Value.GetString();
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
        return new OpenAIEmbeddingCollection(data, model, @object, usage, serializedAdditionalRawData);
    }
}
