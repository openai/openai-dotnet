// <auto-generated/>

#nullable disable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.FineTuning
{
    public partial class FineTuningCheckpointMetrics : IJsonModel<FineTuningCheckpointMetrics>
    {
        void IJsonModel<FineTuningCheckpointMetrics>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<FineTuningCheckpointMetrics>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(FineTuningCheckpointMetrics)} does not support writing '{format}' format.");
            }
            if (Optional.IsDefined(TrainLoss) && _additionalBinaryDataProperties?.ContainsKey("train_loss") != true)
            {
                writer.WritePropertyName("train_loss"u8);
                writer.WriteNumberValue(TrainLoss.Value);
            }
            if (Optional.IsDefined(TrainMeanTokenAccuracy) && _additionalBinaryDataProperties?.ContainsKey("train_mean_token_accuracy") != true)
            {
                writer.WritePropertyName("train_mean_token_accuracy"u8);
                writer.WriteNumberValue(TrainMeanTokenAccuracy.Value);
            }
            if (Optional.IsDefined(ValidLoss) && _additionalBinaryDataProperties?.ContainsKey("valid_loss") != true)
            {
                writer.WritePropertyName("valid_loss"u8);
                writer.WriteNumberValue(ValidLoss.Value);
            }
            if (Optional.IsDefined(ValidMeanTokenAccuracy) && _additionalBinaryDataProperties?.ContainsKey("valid_mean_token_accuracy") != true)
            {
                writer.WritePropertyName("valid_mean_token_accuracy"u8);
                writer.WriteNumberValue(ValidMeanTokenAccuracy.Value);
            }
            if (Optional.IsDefined(FullValidLoss) && _additionalBinaryDataProperties?.ContainsKey("full_valid_loss") != true)
            {
                writer.WritePropertyName("full_valid_loss"u8);
                writer.WriteNumberValue(FullValidLoss.Value);
            }
            if (Optional.IsDefined(FullValidMeanTokenAccuracy) && _additionalBinaryDataProperties?.ContainsKey("full_valid_mean_token_accuracy") != true)
            {
                writer.WritePropertyName("full_valid_mean_token_accuracy"u8);
                writer.WriteNumberValue(FullValidMeanTokenAccuracy.Value);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("step") != true)
            {
                writer.WritePropertyName("step"u8);
                writer.WriteNumberValue(StepNumber);
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

        FineTuningCheckpointMetrics IJsonModel<FineTuningCheckpointMetrics>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual FineTuningCheckpointMetrics JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<FineTuningCheckpointMetrics>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(FineTuningCheckpointMetrics)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeFineTuningCheckpointMetrics(document.RootElement, options);
        }

        internal static FineTuningCheckpointMetrics DeserializeFineTuningCheckpointMetrics(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            float? trainLoss = default;
            float? trainMeanTokenAccuracy = default;
            float? validLoss = default;
            float? validMeanTokenAccuracy = default;
            float? fullValidLoss = default;
            float? fullValidMeanTokenAccuracy = default;
            int stepNumber = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("train_loss"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    trainLoss = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("train_mean_token_accuracy"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    trainMeanTokenAccuracy = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("valid_loss"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    validLoss = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("valid_mean_token_accuracy"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    validMeanTokenAccuracy = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("full_valid_loss"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    fullValidLoss = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("full_valid_mean_token_accuracy"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    fullValidMeanTokenAccuracy = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("step"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    stepNumber = prop.Value.GetInt32();
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new FineTuningCheckpointMetrics(
                trainLoss,
                trainMeanTokenAccuracy,
                validLoss,
                validMeanTokenAccuracy,
                fullValidLoss,
                fullValidMeanTokenAccuracy,
                stepNumber,
                additionalBinaryDataProperties);
        }

        BinaryData IPersistableModel<FineTuningCheckpointMetrics>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<FineTuningCheckpointMetrics>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(FineTuningCheckpointMetrics)} does not support writing '{options.Format}' format.");
            }
        }

        FineTuningCheckpointMetrics IPersistableModel<FineTuningCheckpointMetrics>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        protected virtual FineTuningCheckpointMetrics PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<FineTuningCheckpointMetrics>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeFineTuningCheckpointMetrics(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(FineTuningCheckpointMetrics)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<FineTuningCheckpointMetrics>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
