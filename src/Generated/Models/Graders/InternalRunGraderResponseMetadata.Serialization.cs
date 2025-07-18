// <auto-generated/>

#nullable disable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Graders
{
    internal partial class InternalRunGraderResponseMetadata : IJsonModel<InternalRunGraderResponseMetadata>
    {
        internal InternalRunGraderResponseMetadata()
        {
        }

        void IJsonModel<InternalRunGraderResponseMetadata>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalRunGraderResponseMetadata>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalRunGraderResponseMetadata)} does not support writing '{format}' format.");
            }
            if (_additionalBinaryDataProperties?.ContainsKey("name") != true)
            {
                writer.WritePropertyName("name"u8);
                writer.WriteStringValue(Name);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("type") != true)
            {
                writer.WritePropertyName("type"u8);
                writer.WriteStringValue(Kind);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("errors") != true)
            {
                writer.WritePropertyName("errors"u8);
                writer.WriteObjectValue(Errors, options);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("execution_time") != true)
            {
                writer.WritePropertyName("execution_time"u8);
                writer.WriteNumberValue(ExecutionTime);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("scores") != true)
            {
                writer.WritePropertyName("scores"u8);
#if NET6_0_OR_GREATER
                writer.WriteRawValue(Scores);
#else
                using (JsonDocument document = JsonDocument.Parse(Scores))
                {
                    JsonSerializer.Serialize(writer, document.RootElement);
                }
#endif
            }
            if (_additionalBinaryDataProperties?.ContainsKey("token_usage") != true)
            {
                if (Optional.IsDefined(TokenUsage))
                {
                    writer.WritePropertyName("token_usage"u8);
                    writer.WriteNumberValue(TokenUsage.Value);
                }
                else
                {
                    writer.WriteNull("token_usage"u8);
                }
            }
            if (_additionalBinaryDataProperties?.ContainsKey("sampled_model_name") != true)
            {
                if (Optional.IsDefined(SampledModelName))
                {
                    writer.WritePropertyName("sampled_model_name"u8);
                    writer.WriteStringValue(SampledModelName);
                }
                else
                {
                    writer.WriteNull("sampled_model_name"u8);
                }
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

        InternalRunGraderResponseMetadata IJsonModel<InternalRunGraderResponseMetadata>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual InternalRunGraderResponseMetadata JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalRunGraderResponseMetadata>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalRunGraderResponseMetadata)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeInternalRunGraderResponseMetadata(document.RootElement, options);
        }

        internal static InternalRunGraderResponseMetadata DeserializeInternalRunGraderResponseMetadata(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            string name = default;
            string kind = default;
            InternalRunGraderResponseMetadataErrors errors = default;
            float executionTime = default;
            BinaryData scores = default;
            int? tokenUsage = default;
            string sampledModelName = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("name"u8))
                {
                    name = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("type"u8))
                {
                    kind = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("errors"u8))
                {
                    errors = InternalRunGraderResponseMetadataErrors.DeserializeInternalRunGraderResponseMetadataErrors(prop.Value, options);
                    continue;
                }
                if (prop.NameEquals("execution_time"u8))
                {
                    executionTime = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("scores"u8))
                {
                    scores = BinaryData.FromString(prop.Value.GetRawText());
                    continue;
                }
                if (prop.NameEquals("token_usage"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        tokenUsage = null;
                        continue;
                    }
                    tokenUsage = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("sampled_model_name"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        sampledModelName = null;
                        continue;
                    }
                    sampledModelName = prop.Value.GetString();
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new InternalRunGraderResponseMetadata(
                name,
                kind,
                errors,
                executionTime,
                scores,
                tokenUsage,
                sampledModelName,
                additionalBinaryDataProperties);
        }

        BinaryData IPersistableModel<InternalRunGraderResponseMetadata>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalRunGraderResponseMetadata>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(InternalRunGraderResponseMetadata)} does not support writing '{options.Format}' format.");
            }
        }

        InternalRunGraderResponseMetadata IPersistableModel<InternalRunGraderResponseMetadata>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        protected virtual InternalRunGraderResponseMetadata PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalRunGraderResponseMetadata>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeInternalRunGraderResponseMetadata(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(InternalRunGraderResponseMetadata)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<InternalRunGraderResponseMetadata>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
