// <auto-generated/>

#nullable disable

using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Chat
{
    public partial class ChatOutputTokenUsageDetails : IJsonModel<ChatOutputTokenUsageDetails>
    {
        void IJsonModel<ChatOutputTokenUsageDetails>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatOutputTokenUsageDetails>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatOutputTokenUsageDetails)} does not support writing '{format}' format.");
            }
            if (_additionalBinaryDataProperties?.ContainsKey("reasoning_tokens") != true)
            {
                writer.WritePropertyName("reasoning_tokens"u8);
                writer.WriteNumberValue(ReasoningTokenCount);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("audio_tokens") != true)
            {
                writer.WritePropertyName("audio_tokens"u8);
                writer.WriteNumberValue(AudioTokenCount);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("accepted_prediction_tokens") != true)
            {
                writer.WritePropertyName("accepted_prediction_tokens"u8);
                writer.WriteNumberValue(AcceptedPredictionTokenCount);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("rejected_prediction_tokens") != true)
            {
                writer.WritePropertyName("rejected_prediction_tokens"u8);
                writer.WriteNumberValue(RejectedPredictionTokenCount);
            }
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

        ChatOutputTokenUsageDetails IJsonModel<ChatOutputTokenUsageDetails>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual ChatOutputTokenUsageDetails JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatOutputTokenUsageDetails>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatOutputTokenUsageDetails)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeChatOutputTokenUsageDetails(document.RootElement, options);
        }

        internal static ChatOutputTokenUsageDetails DeserializeChatOutputTokenUsageDetails(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            int reasoningTokenCount = default;
            int audioTokenCount = default;
            int acceptedPredictionTokenCount = default;
            int rejectedPredictionTokenCount = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("reasoning_tokens"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    reasoningTokenCount = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("audio_tokens"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    audioTokenCount = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("accepted_prediction_tokens"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    acceptedPredictionTokenCount = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("rejected_prediction_tokens"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    rejectedPredictionTokenCount = prop.Value.GetInt32();
                    continue;
                }
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new ChatOutputTokenUsageDetails(reasoningTokenCount, audioTokenCount, acceptedPredictionTokenCount, rejectedPredictionTokenCount, additionalBinaryDataProperties);
        }

        BinaryData IPersistableModel<ChatOutputTokenUsageDetails>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatOutputTokenUsageDetails>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options);
                default:
                    throw new FormatException($"The model {nameof(ChatOutputTokenUsageDetails)} does not support writing '{options.Format}' format.");
            }
        }

        ChatOutputTokenUsageDetails IPersistableModel<ChatOutputTokenUsageDetails>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        protected virtual ChatOutputTokenUsageDetails PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatOutputTokenUsageDetails>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeChatOutputTokenUsageDetails(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(ChatOutputTokenUsageDetails)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<ChatOutputTokenUsageDetails>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

        public static implicit operator BinaryContent(ChatOutputTokenUsageDetails chatOutputTokenUsageDetails)
        {
            if (chatOutputTokenUsageDetails == null)
            {
                return null;
            }
            return BinaryContent.Create(chatOutputTokenUsageDetails, ModelSerializationExtensions.WireOptions);
        }

        public static explicit operator ChatOutputTokenUsageDetails(ClientResult result)
        {
            using PipelineResponse response = result.GetRawResponse();
            using JsonDocument document = JsonDocument.Parse(response.Content);
            return DeserializeChatOutputTokenUsageDetails(document.RootElement, ModelSerializationExtensions.WireOptions);
        }
    }
}
