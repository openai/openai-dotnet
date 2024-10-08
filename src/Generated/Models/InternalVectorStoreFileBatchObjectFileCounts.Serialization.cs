// <auto-generated/>

#nullable disable

using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.VectorStores
{
    internal partial class InternalVectorStoreFileBatchObjectFileCounts : IJsonModel<InternalVectorStoreFileBatchObjectFileCounts>
    {
        void IJsonModel<InternalVectorStoreFileBatchObjectFileCounts>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<InternalVectorStoreFileBatchObjectFileCounts>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalVectorStoreFileBatchObjectFileCounts)} does not support writing '{format}' format.");
            }

            writer.WriteStartObject();
            if (SerializedAdditionalRawData?.ContainsKey("in_progress") != true)
            {
                writer.WritePropertyName("in_progress"u8);
                writer.WriteNumberValue(InProgress);
            }
            if (SerializedAdditionalRawData?.ContainsKey("completed") != true)
            {
                writer.WritePropertyName("completed"u8);
                writer.WriteNumberValue(Completed);
            }
            if (SerializedAdditionalRawData?.ContainsKey("failed") != true)
            {
                writer.WritePropertyName("failed"u8);
                writer.WriteNumberValue(Failed);
            }
            if (SerializedAdditionalRawData?.ContainsKey("cancelled") != true)
            {
                writer.WritePropertyName("cancelled"u8);
                writer.WriteNumberValue(Cancelled);
            }
            if (SerializedAdditionalRawData?.ContainsKey("total") != true)
            {
                writer.WritePropertyName("total"u8);
                writer.WriteNumberValue(Total);
            }
            if (SerializedAdditionalRawData != null)
            {
                foreach (var item in SerializedAdditionalRawData)
                {
                    if (ModelSerializationExtensions.IsSentinelValue(item.Value))
                    {
                        continue;
                    }
                    writer.WritePropertyName(item.Key);
#if NET6_0_OR_GREATER
				writer.WriteRawValue(item.Value);
#else
                    using (JsonDocument document = JsonDocument.Parse(item.Value))
                    {
                        JsonSerializer.Serialize(writer, document.RootElement);
                    }
#endif
                }
            }
            writer.WriteEndObject();
        }

        InternalVectorStoreFileBatchObjectFileCounts IJsonModel<InternalVectorStoreFileBatchObjectFileCounts>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<InternalVectorStoreFileBatchObjectFileCounts>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalVectorStoreFileBatchObjectFileCounts)} does not support reading '{format}' format.");
            }

            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeInternalVectorStoreFileBatchObjectFileCounts(document.RootElement, options);
        }

        internal static InternalVectorStoreFileBatchObjectFileCounts DeserializeInternalVectorStoreFileBatchObjectFileCounts(JsonElement element, ModelReaderWriterOptions options = null)
        {
            options ??= ModelSerializationExtensions.WireOptions;

            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            int inProgress = default;
            int completed = default;
            int failed = default;
            int cancelled = default;
            int total = default;
            IDictionary<string, BinaryData> serializedAdditionalRawData = default;
            Dictionary<string, BinaryData> rawDataDictionary = new Dictionary<string, BinaryData>();
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("in_progress"u8))
                {
                    inProgress = property.Value.GetInt32();
                    continue;
                }
                if (property.NameEquals("completed"u8))
                {
                    completed = property.Value.GetInt32();
                    continue;
                }
                if (property.NameEquals("failed"u8))
                {
                    failed = property.Value.GetInt32();
                    continue;
                }
                if (property.NameEquals("cancelled"u8))
                {
                    cancelled = property.Value.GetInt32();
                    continue;
                }
                if (property.NameEquals("total"u8))
                {
                    total = property.Value.GetInt32();
                    continue;
                }
                if (true)
                {
                    rawDataDictionary ??= new Dictionary<string, BinaryData>();
                    rawDataDictionary.Add(property.Name, BinaryData.FromString(property.Value.GetRawText()));
                }
            }
            serializedAdditionalRawData = rawDataDictionary;
            return new InternalVectorStoreFileBatchObjectFileCounts(
                inProgress,
                completed,
                failed,
                cancelled,
                total,
                serializedAdditionalRawData);
        }

        BinaryData IPersistableModel<InternalVectorStoreFileBatchObjectFileCounts>.Write(ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<InternalVectorStoreFileBatchObjectFileCounts>)this).GetFormatFromOptions(options) : options.Format;

            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options);
                default:
                    throw new FormatException($"The model {nameof(InternalVectorStoreFileBatchObjectFileCounts)} does not support writing '{options.Format}' format.");
            }
        }

        InternalVectorStoreFileBatchObjectFileCounts IPersistableModel<InternalVectorStoreFileBatchObjectFileCounts>.Create(BinaryData data, ModelReaderWriterOptions options)
        {
            var format = options.Format == "W" ? ((IPersistableModel<InternalVectorStoreFileBatchObjectFileCounts>)this).GetFormatFromOptions(options) : options.Format;

            switch (format)
            {
                case "J":
                    {
                        using JsonDocument document = JsonDocument.Parse(data);
                        return DeserializeInternalVectorStoreFileBatchObjectFileCounts(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(InternalVectorStoreFileBatchObjectFileCounts)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<InternalVectorStoreFileBatchObjectFileCounts>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

        internal static InternalVectorStoreFileBatchObjectFileCounts FromResponse(PipelineResponse response)
        {
            using var document = JsonDocument.Parse(response.Content);
            return DeserializeInternalVectorStoreFileBatchObjectFileCounts(document.RootElement);
        }

        internal virtual BinaryContent ToBinaryContent()
        {
            return BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
        }
    }
}
