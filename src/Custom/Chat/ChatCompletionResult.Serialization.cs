using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OpenAI.Chat
{
    public partial class ChatCompletionResult : IJsonModel<ChatCompletionResult>
    {
        internal ChatCompletionResult() : this(null, null, default, null, default, null, null, null, default)
        {
        }

        void IJsonModel<ChatCompletionResult>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionResult>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatCompletionResult)} does not support writing '{format}' format.");
            }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            if (!Patch.Contains("$.id"u8))
            {
                writer.WritePropertyName("id"u8);
                writer.WriteStringValue(Id);
            }
            if (Patch.Contains("$.choices"u8))
            {
                if (!Patch.IsRemoved("$.choices"u8))
                {
                    writer.WritePropertyName("choices"u8);
                    writer.WriteRawValue(Patch.GetJson("$.choices"u8));
                }
            }
            else
            {
                writer.WritePropertyName("choices"u8);
                writer.WriteStartArray();
                for (int i = 0; i < Choices.Count; i++)
                {
                    if (Choices[i].Patch.IsRemoved("$"u8))
                    {
                        continue;
                    }
                    writer.WriteObjectValue(Choices[i], options);
                }
                Patch.WriteTo(writer, "$.choices"u8);
                writer.WriteEndArray();
            }
            if (!Patch.Contains("$.created"u8))
            {
                writer.WritePropertyName("created"u8);
                writer.WriteNumberValue(Created, "U");
            }
            if (!Patch.Contains("$.model"u8))
            {
                writer.WritePropertyName("model"u8);
                writer.WriteStringValue(Model);
            }
            if (Optional.IsDefined(ServiceTier) && !Patch.Contains("$.service_tier"u8))
            {
                writer.WritePropertyName("service_tier"u8);
                writer.WriteStringValue(ServiceTier.Value.ToString());
            }
            if (Optional.IsDefined(SystemFingerprint) && !Patch.Contains("$.system_fingerprint"u8))
            {
                writer.WritePropertyName("system_fingerprint"u8);
                writer.WriteStringValue(SystemFingerprint);
            }
            if (!Patch.Contains("$.object"u8))
            {
                writer.WritePropertyName("object"u8);
                writer.WriteStringValue(Object);
            }
            if (Optional.IsDefined(Usage) && !Patch.Contains("$.usage"u8))
            {
                writer.WritePropertyName("usage"u8);
                writer.WriteObjectValue(Usage, options);
            }

            Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        }

        ChatCompletionResult IJsonModel<ChatCompletionResult>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual ChatCompletionResult JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionResult>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatCompletionResult)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeCreateChatCompletionResponse(document.RootElement, null, options);
        }

        internal static ChatCompletionResult DeserializeCreateChatCompletionResponse(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            string id = default;
            IList<CreateChatCompletionResponseChoice> choices = default;
            DateTimeOffset created = default;
            string model = default;
            ChatServiceTier? serviceTier = default;
            string systemFingerprint = default;
            string @object = default;
            ChatTokenUsage usage = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("id"u8))
                {
                    id = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("choices"u8))
                {
                    List<CreateChatCompletionResponseChoice> array = new List<CreateChatCompletionResponseChoice>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(CreateChatCompletionResponseChoice.DeserializeCreateChatCompletionResponseChoice(prop.Value, prop.Value.GetUtf8Bytes(), options));
                    }
                    choices = array;
                    continue;
                }
                if (prop.NameEquals("created"u8))
                {
                    created = DateTimeOffset.FromUnixTimeSeconds(prop.Value.GetInt64());
                    continue;
                }
                if (prop.NameEquals("model"u8))
                {
                    model = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("service_tier"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    serviceTier = new ChatServiceTier(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("system_fingerprint"u8))
                {
                    systemFingerprint = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("object"u8))
                {
                    @object = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("usage"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    usage = ChatTokenUsage.DeserializeChatTokenUsage(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(prop.Name)], prop.Value.GetUtf8Bytes());
            }
            return new ChatCompletionResult(
                id,
                choices,
                created,
                model,
                serviceTier,
                systemFingerprint,
                @object,
                usage,
                patch);
        }

        BinaryData IPersistableModel<ChatCompletionResult>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionResult>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(ChatCompletionResult)} does not support writing '{options.Format}' format.");
            }
        }

        ChatCompletionResult IPersistableModel<ChatCompletionResult>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        protected virtual ChatCompletionResult PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionResult>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeCreateChatCompletionResponse(document.RootElement, data, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(ChatCompletionResult)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<ChatCompletionResult>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

        public static explicit operator ChatCompletionResult(ClientResult result)
        {
            using PipelineResponse response = result.GetRawResponse();
            BinaryData data = response.Content;
            using JsonDocument document = JsonDocument.Parse(data);
            return DeserializeCreateChatCompletionResponse(document.RootElement, data, ModelSerializationExtensions.WireOptions);
        }

        public static explicit operator ChatCompletionResult(BinaryData data)
        {
            using JsonDocument document = JsonDocument.Parse(data);
            return DeserializeCreateChatCompletionResponse(document.RootElement, data, ModelSerializationExtensions.WireOptions);
        }
    }
}