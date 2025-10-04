using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;
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
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (instance.Patch.Contains("$"u8))
        {
            writer.WriteRawValue(instance.Patch.GetJson("$"u8));
            return;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

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
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        instance.Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        writer.WriteEndObject();
    }

    // CUSTOM: Recovered the deserialization of SerializedAdditionalRawData. See https://github.com/Azure/autorest.csharp/issues/4636.
    internal static OpenAIEmbeddingCollection DeserializeOpenAIEmbeddingCollection(JsonElement element, BinaryData data, ModelReaderWriterOptions options = null)
    {
        options ??= new ModelReaderWriterOptions("W");

        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        IReadOnlyList<OpenAIEmbedding> embedding = default;
        string model = default;
        string @object = default;
        EmbeddingTokenUsage usage = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("data"u8))
            {
                List<OpenAIEmbedding> array = new List<OpenAIEmbedding>();
                foreach (var item in property.Value.EnumerateArray())
                {
                    array.Add(OpenAIEmbedding.DeserializeOpenAIEmbedding(item, item.GetUtf8Bytes(), options));
                }
                embedding = array;
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
                usage = EmbeddingTokenUsage.DeserializeEmbeddingTokenUsage(property.Value, property.Value.GetUtf8Bytes(), options);
                continue;
            }
            if (true)
            {
                patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(property.Name)], property.Value.GetUtf8Bytes());
            }
        }
        return new OpenAIEmbeddingCollection(embedding, model, @object, usage, patch);
    }
}
