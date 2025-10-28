using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OpenAI.Chat
{
    public partial class ChatCompletionRequestAssistantMessage : ChatMessage, IJsonModel<ChatCompletionRequestAssistantMessage>
    {
        void IJsonModel<ChatCompletionRequestAssistantMessage>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionRequestAssistantMessage>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatCompletionRequestAssistantMessage)} does not support writing '{format}' format.");
            }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            base.JsonModelWriteCore(writer, options);
            if (Optional.IsDefined(Refusal) && !Patch.Contains("$.refusal"u8))
            {
                writer.WritePropertyName("refusal"u8);
                writer.WriteStringValue(Refusal);
            }
            if (Optional.IsDefined(Name) && !Patch.Contains("$.name"u8))
            {
                writer.WritePropertyName("name"u8);
                writer.WriteStringValue(Name);
            }
            if (Optional.IsDefined(Audio) && !Patch.Contains("$.audio"u8))
            {
                writer.WritePropertyName("audio"u8);
                writer.WriteObjectValue(Audio, options);
            }
            if (Optional.IsCollectionDefined(ToolCalls) && !Patch.Contains("$.tool_calls"u8))
            {
                writer.WritePropertyName("tool_calls"u8);
                writer.WriteStartArray();
                foreach (ChatToolCall item in ToolCalls)
                {
                    writer.WriteObjectValue(item, options);
                }
                writer.WriteEndArray();
            }
            if (Optional.IsDefined(FunctionCall) && !Patch.Contains("$.function_call"u8))
            {
                writer.WritePropertyName("function_call"u8);
                writer.WriteObjectValue(FunctionCall, options);
            }
            Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        }

        ChatCompletionRequestAssistantMessage IJsonModel<ChatCompletionRequestAssistantMessage>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => (ChatCompletionRequestAssistantMessage)JsonModelCreateCore(ref reader, options);

        protected override ChatMessage JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionRequestAssistantMessage>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatCompletionRequestAssistantMessage)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeChatCompletionRequestAssistantMessage(document.RootElement, null, options);
        }

        internal static ChatCompletionRequestAssistantMessage DeserializeChatCompletionRequestAssistantMessage(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
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
            string refusal = default;
            string name = default;
            ChatOutputAudioReference audio = default;
            IList<ChatToolCall> toolCalls = default;
            ChatFunctionCall functionCall = default;
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
                if (prop.NameEquals("refusal"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        refusal = null;
                        continue;
                    }
                    refusal = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("name"u8))
                {
                    name = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("audio"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        audio = null;
                        continue;
                    }
                    audio = ChatOutputAudioReference.DeserializeChatOutputAudioReference(prop.Value, data, options);
                    continue;
                }
                if (prop.NameEquals("tool_calls"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<ChatToolCall> array = new List<ChatToolCall>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ChatToolCall.DeserializeChatToolCall(item, data, options));
                    }
                    toolCalls = array;
                    continue;
                }
                if (prop.NameEquals("function_call"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        functionCall = null;
                        continue;
                    }
                    functionCall = ChatFunctionCall.DeserializeChatFunctionCall(prop.Value, data, options);
                    continue;
                }
                patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(prop.Name)], prop.Value.GetUtf8Bytes());
            }
            return new ChatCompletionRequestAssistantMessage(
                role,
                content,
                patch,
                refusal,
                name,
                audio,
                toolCalls ?? new ChangeTrackingList<ChatToolCall>(),
                functionCall);
        }

        BinaryData IPersistableModel<ChatCompletionRequestAssistantMessage>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionRequestAssistantMessage>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(ChatCompletionRequestAssistantMessage)} does not support writing '{options.Format}' format.");
            }
        }

        ChatCompletionRequestAssistantMessage IPersistableModel<ChatCompletionRequestAssistantMessage>.Create(BinaryData data, ModelReaderWriterOptions options) => (ChatCompletionRequestAssistantMessage)PersistableModelCreateCore(data, options);

        protected override ChatMessage PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionRequestAssistantMessage>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeChatCompletionRequestAssistantMessage(document.RootElement, data, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(ChatCompletionRequestAssistantMessage)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<ChatCompletionRequestAssistantMessage>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}