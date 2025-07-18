// <auto-generated/>

#nullable disable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.VectorStores
{
    internal partial class InternalCreateVectorStoreFileRequest : IJsonModel<InternalCreateVectorStoreFileRequest>
    {
        internal InternalCreateVectorStoreFileRequest() : this(null, null, null, null)
        {
        }

        void IJsonModel<InternalCreateVectorStoreFileRequest>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalCreateVectorStoreFileRequest>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalCreateVectorStoreFileRequest)} does not support writing '{format}' format.");
            }
            if (_additionalBinaryDataProperties?.ContainsKey("file_id") != true)
            {
                writer.WritePropertyName("file_id"u8);
                writer.WriteStringValue(FileId);
            }
            if (Optional.IsCollectionDefined(Attributes) && _additionalBinaryDataProperties?.ContainsKey("attributes") != true)
            {
                writer.WritePropertyName("attributes"u8);
                writer.WriteStartObject();
                foreach (var item in Attributes)
                {
                    writer.WritePropertyName(item.Key);
                    if (item.Value == null)
                    {
                        writer.WriteNullValue();
                        continue;
                    }
#if NET6_0_OR_GREATER
                    writer.WriteRawValue(item.Value);
#else
                    using (JsonDocument document = JsonDocument.Parse(item.Value))
                    {
                        JsonSerializer.Serialize(writer, document.RootElement);
                    }
#endif
                }
                writer.WriteEndObject();
            }
            if (Optional.IsDefined(ChunkingStrategy) && _additionalBinaryDataProperties?.ContainsKey("chunking_strategy") != true)
            {
                writer.WritePropertyName("chunking_strategy"u8);
                writer.WriteObjectValue(ChunkingStrategy, options);
            }
            // Plugin customization: remove options.Format != "W" check
            if (_additionalBinaryDataProperties != null)
            {
                foreach (var item in _additionalBinaryDataProperties)
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
        }

        InternalCreateVectorStoreFileRequest IJsonModel<InternalCreateVectorStoreFileRequest>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual InternalCreateVectorStoreFileRequest JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalCreateVectorStoreFileRequest>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalCreateVectorStoreFileRequest)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeInternalCreateVectorStoreFileRequest(document.RootElement, options);
        }

        internal static InternalCreateVectorStoreFileRequest DeserializeInternalCreateVectorStoreFileRequest(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            string fileId = default;
            IDictionary<string, BinaryData> attributes = default;
            FileChunkingStrategy chunkingStrategy = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("file_id"u8))
                {
                    fileId = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("attributes"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    Dictionary<string, BinaryData> dictionary = new Dictionary<string, BinaryData>();
                    foreach (var prop0 in prop.Value.EnumerateObject())
                    {
                        if (prop0.Value.ValueKind == JsonValueKind.Null)
                        {
                            dictionary.Add(prop0.Name, null);
                        }
                        else
                        {
                            dictionary.Add(prop0.Name, BinaryData.FromString(prop0.Value.GetRawText()));
                        }
                    }
                    attributes = dictionary;
                    continue;
                }
                if (prop.NameEquals("chunking_strategy"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    chunkingStrategy = FileChunkingStrategy.DeserializeFileChunkingStrategy(prop.Value, options);
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new InternalCreateVectorStoreFileRequest(fileId, attributes ?? new ChangeTrackingDictionary<string, BinaryData>(), chunkingStrategy, additionalBinaryDataProperties);
        }

        BinaryData IPersistableModel<InternalCreateVectorStoreFileRequest>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalCreateVectorStoreFileRequest>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(InternalCreateVectorStoreFileRequest)} does not support writing '{options.Format}' format.");
            }
        }

        InternalCreateVectorStoreFileRequest IPersistableModel<InternalCreateVectorStoreFileRequest>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        protected virtual InternalCreateVectorStoreFileRequest PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalCreateVectorStoreFileRequest>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeInternalCreateVectorStoreFileRequest(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(InternalCreateVectorStoreFileRequest)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<InternalCreateVectorStoreFileRequest>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
