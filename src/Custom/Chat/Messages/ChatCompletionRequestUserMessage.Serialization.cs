using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using OpenAI.Chat;

namespace OpenAI.Chat
{
    public partial class ChatCompletionRequestUserMessage : ChatMessage, IJsonModel<ChatCompletionRequestUserMessage>
    {
        void IJsonModel<ChatCompletionRequestUserMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionRequestUserMessage>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatCompletionRequestUserMessage)} does not support writing '{format}' format.");
            }
            base.JsonModelWriteCore(writer, options);
            if (Optional.IsDefined(Name) && !Patch.Contains("$.name"u8))
            {
                writer.WritePropertyName("name"u8);
                writer.WriteStringValue(Name);
            }
            Patch.WriteTo(writer);
        }

        ChatCompletionRequestUserMessage IJsonModel<ChatCompletionRequestUserMessage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => (ChatCompletionRequestUserMessage)JsonModelCreateCore(ref reader, options);

        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionRequestUserMessage>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatCompletionRequestUserMessage)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeChatCompletionRequestUserMessage(document.RootElement, null, options);
        }

        internal static ChatCompletionRequestUserMessage DeserializeChatCompletionRequestUserMessage(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            ChatMessageRole role = default;
            ChatMessageContent content = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            string name = default;
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("role"u8))
                {
                    role = prop.Value.GetString().ToChatMessageRole();
                    continue;
                }
                if (prop.NameEquals("content"u8))
                {
                    DeserializeContentValue(prop, ref content);
                    continue;
                }
                if (prop.NameEquals("name"u8))
                {
                    name = prop.Value.GetString();
                    continue;
                }
                patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(prop.Name)], prop.Value.GetUtf8Bytes());
            }
            return new ChatCompletionRequestUserMessage(role, content, patch, name);
        }

        BinaryData IPersistableModel<ChatCompletionRequestUserMessage>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionRequestUserMessage>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(ChatCompletionRequestUserMessage)} does not support writing '{options.Format}' format.");
            }
        }

        ChatCompletionRequestUserMessage IPersistableModel<ChatCompletionRequestUserMessage>.Create(BinaryData data, ModelReaderWriterOptions options) => (ChatCompletionRequestUserMessage)PersistableModelCreateCore(data, options);

        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionRequestUserMessage>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeChatCompletionRequestUserMessage(document.RootElement, data, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(ChatCompletionRequestUserMessage)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<ChatCompletionRequestUserMessage>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}