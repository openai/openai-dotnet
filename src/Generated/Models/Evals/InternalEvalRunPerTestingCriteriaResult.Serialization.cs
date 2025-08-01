// <auto-generated/>

#nullable disable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Evals
{
    internal partial class InternalEvalRunPerTestingCriteriaResult : IJsonModel<InternalEvalRunPerTestingCriteriaResult>
    {
        internal InternalEvalRunPerTestingCriteriaResult()
        {
        }

        void IJsonModel<InternalEvalRunPerTestingCriteriaResult>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalEvalRunPerTestingCriteriaResult>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalEvalRunPerTestingCriteriaResult)} does not support writing '{format}' format.");
            }
            if (_additionalBinaryDataProperties?.ContainsKey("testing_criteria") != true)
            {
                writer.WritePropertyName("testing_criteria"u8);
                writer.WriteStringValue(TestingCriteria);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("passed") != true)
            {
                writer.WritePropertyName("passed"u8);
                writer.WriteNumberValue(Passed);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("failed") != true)
            {
                writer.WritePropertyName("failed"u8);
                writer.WriteNumberValue(Failed);
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

        InternalEvalRunPerTestingCriteriaResult IJsonModel<InternalEvalRunPerTestingCriteriaResult>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual InternalEvalRunPerTestingCriteriaResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalEvalRunPerTestingCriteriaResult>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalEvalRunPerTestingCriteriaResult)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeInternalEvalRunPerTestingCriteriaResult(document.RootElement, options);
        }

        internal static InternalEvalRunPerTestingCriteriaResult DeserializeInternalEvalRunPerTestingCriteriaResult(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            string testingCriteria = default;
            int passed = default;
            int failed = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("testing_criteria"u8))
                {
                    testingCriteria = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("passed"u8))
                {
                    passed = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("failed"u8))
                {
                    failed = prop.Value.GetInt32();
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new InternalEvalRunPerTestingCriteriaResult(testingCriteria, passed, failed, additionalBinaryDataProperties);
        }

        BinaryData IPersistableModel<InternalEvalRunPerTestingCriteriaResult>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalEvalRunPerTestingCriteriaResult>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(InternalEvalRunPerTestingCriteriaResult)} does not support writing '{options.Format}' format.");
            }
        }

        InternalEvalRunPerTestingCriteriaResult IPersistableModel<InternalEvalRunPerTestingCriteriaResult>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        protected virtual InternalEvalRunPerTestingCriteriaResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalEvalRunPerTestingCriteriaResult>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeInternalEvalRunPerTestingCriteriaResult(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(InternalEvalRunPerTestingCriteriaResult)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<InternalEvalRunPerTestingCriteriaResult>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
