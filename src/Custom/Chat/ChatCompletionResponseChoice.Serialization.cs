using System;
using System.ClientModel.Primitives;
using System.Text;
using System.Text.Json;

namespace OpenAI.Chat
{
    public partial class ChatCompletionResponseChoice : IJsonModel<ChatCompletionResponseChoice>
    {
        internal ChatCompletionResponseChoice()
        {
        }

        void IJsonModel<ChatCompletionResponseChoice>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalCreateChatCompletionResponseChoice>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalCreateChatCompletionResponseChoice)} does not support writing '{format}' format.");
            }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            if (!Patch.Contains("$.finish_reason"u8))
            {
                writer.WritePropertyName("finish_reason"u8);
                writer.WriteStringValue(FinishReason.ToSerialString());
            }
            if (!Patch.Contains("$.index"u8))
            {
                writer.WritePropertyName("index"u8);
                writer.WriteNumberValue(Index);
            }
            if (!Patch.Contains("$.message"u8))
            {
                writer.WritePropertyName("message"u8);
                writer.WriteObjectValue(Message, options);
            }
            if (Optional.IsDefined(Logprobs) && !Patch.Contains("$.logprobs"u8))
            {
                writer.WritePropertyName("logprobs"u8);
                writer.WriteObjectValue(Logprobs, options);
            }
            else
            {
                writer.WriteNull("logprobs"u8);
            }

            Patch.WriteTo(writer);
        }

        ChatCompletionResponseChoice IJsonModel<ChatCompletionResponseChoice>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual ChatCompletionResponseChoice JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionResponseChoice>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatCompletionResponseChoice)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeCreateChatCompletionResponseChoice(document.RootElement, null, options);
        }

        internal static ChatCompletionResponseChoice DeserializeCreateChatCompletionResponseChoice(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            ChatFinishReason finishReason = default;
            int index = default;
            ChatCompletionResponseMessage message = default;
            ChatCompletionResponseChoiceLogprobs logprobs = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.ƒ

            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("finish_reason"u8))
                {
                    finishReason = prop.Value.GetString().ToChatFinishReason();
                    continue;
                }
                if (prop.NameEquals("index"u8))
                {
                    index = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("message"u8))
                {
                    message = ChatCompletionResponseMessage.DeserializeChatCompletionResponseMessage(prop.Value, options);
                    continue;
                }
                if (prop.NameEquals("logprobs"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        logprobs = null;
                        continue;
                    }
                    logprobs = ChatCompletionResponseChoiceLogprobs.DeserializeCreateChatCompletionResponseChoiceLogprobs(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(prop.Name)], prop.Value.GetUtf8Bytes());
            }
            return new ChatCompletionResponseChoice(finishReason, index, message, logprobs, patch);
        }

        BinaryData IPersistableModel<ChatCompletionResponseChoice>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionResponseChoice>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(ChatCompletionResponseChoice)} does not support writing '{options.Format}' format.");
            }
        }

        ChatCompletionResponseChoice IPersistableModel<ChatCompletionResponseChoice>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        protected virtual ChatCompletionResponseChoice PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionResponseChoice>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeCreateChatCompletionResponseChoice(document.RootElement, data, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(ChatCompletionResponseChoice)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<ChatCompletionResponseChoice>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}