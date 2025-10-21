using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI.Chat;

namespace OpenAI.Chat
{
    public partial class ChatCompletionResponseMessage2 : IJsonModel<ChatCompletionResponseMessage2>
    {
        internal ChatCompletionResponseMessage2() : this(null, null, null, null, default, null, null, null)
        {
        }

        void IJsonModel<ChatCompletionResponseMessage2>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionResponseMessage2>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatCompletionResponseMessage2)} does not support writing '{format}' format.");
            }
            if (_additionalBinaryDataProperties?.ContainsKey("content") != true)
            {
                if (Optional.IsDefined(Content))
                {
                    writer.WritePropertyName("content"u8);
                    writer.WriteStringValue(Content);
                }
                else
                {
                    writer.WriteNull("content"u8);
                }
            }
            if (_additionalBinaryDataProperties?.ContainsKey("refusal") != true)
            {
                if (Optional.IsDefined(Refusal))
                {
                    writer.WritePropertyName("refusal"u8);
                    writer.WriteStringValue(Refusal);
                }
                else
                {
                    writer.WriteNull("refusal"u8);
                }
            }
            // Plugin customization: remove options.Format != "W" check
            if (Optional.IsCollectionDefined(ToolCalls) && _additionalBinaryDataProperties?.ContainsKey("tool_calls") != true)
            {
                writer.WritePropertyName("tool_calls"u8);
                writer.WriteStartArray();
                foreach (ChatToolCall item in ToolCalls)
                {
                    writer.WriteObjectValue(item, options);
                }
                writer.WriteEndArray();
            }
            // Plugin customization: remove options.Format != "W" check
            if (Optional.IsCollectionDefined(Annotations) && _additionalBinaryDataProperties?.ContainsKey("annotations") != true)
            {
                writer.WritePropertyName("annotations"u8);
                writer.WriteStartArray();
                foreach (ChatMessageAnnotation item in Annotations)
                {
                    writer.WriteObjectValue(item, options);
                }
                writer.WriteEndArray();
            }
            if (_additionalBinaryDataProperties?.ContainsKey("role") != true)
            {
                writer.WritePropertyName("role"u8);
                writer.WriteStringValue(Role.ToSerialString());
            }
            if (Optional.IsDefined(FunctionCall) && _additionalBinaryDataProperties?.ContainsKey("function_call") != true)
            {
                writer.WritePropertyName("function_call"u8);
                writer.WriteObjectValue(FunctionCall, options);
            }
            if (Optional.IsDefined(Audio) && _additionalBinaryDataProperties?.ContainsKey("audio") != true)
            {
                writer.WritePropertyName("audio"u8);
                writer.WriteObjectValue(Audio, options);
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

        ChatCompletionResponseMessage2 IJsonModel<ChatCompletionResponseMessage2>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual ChatCompletionResponseMessage2 JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionResponseMessage2>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatCompletionResponseMessage2)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeChatCompletionResponseMessage(document.RootElement, options);
        }

        internal static ChatCompletionResponseMessage2 DeserializeChatCompletionResponseMessage(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            string content = default;
            string refusal = default;
            IReadOnlyList<ChatToolCall> toolCalls = default;
            IReadOnlyList<ChatMessageAnnotation> annotations = default;
            ChatMessageRole role = default;
            ChatCompletionResponseMessageFunctionCall2 functionCall = default;
            ChatOutputAudio audio = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("content"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        content = null;
                        continue;
                    }
                    content = prop.Value.GetString();
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
                if (prop.NameEquals("tool_calls"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<ChatToolCall> array = new List<ChatToolCall>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ChatToolCall.DeserializeChatToolCall(item, item.GetUtf8Bytes(), options));
                    }
                    toolCalls = array;
                    continue;
                }
                if (prop.NameEquals("annotations"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<ChatMessageAnnotation> array = new List<ChatMessageAnnotation>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ChatMessageAnnotation.DeserializeChatMessageAnnotation(item, item.GetUtf8Bytes(), options));
                    }
                    annotations = array;
                    continue;
                }
                if (prop.NameEquals("role"u8))
                {
                    role = prop.Value.GetString().ToChatMessageRole();
                    continue;
                }
                if (prop.NameEquals("function_call"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    functionCall = ChatCompletionResponseMessageFunctionCall2.DeserializeChatCompletionResponseMessageFunctionCall(prop.Value, options);
                    continue;
                }
                if (prop.NameEquals("audio"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        audio = null;
                        continue;
                    }
                    audio = ChatOutputAudio.DeserializeChatOutputAudio(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new ChatCompletionResponseMessage2(
                content,
                refusal,
                toolCalls ?? new ChangeTrackingList<ChatToolCall>(),
                annotations ?? new ChangeTrackingList<ChatMessageAnnotation>(),
                role,
                functionCall,
                audio,
                additionalBinaryDataProperties);
        }

        BinaryData IPersistableModel<ChatCompletionResponseMessage2>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionResponseMessage2>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(ChatCompletionResponseMessage2)} does not support writing '{options.Format}' format.");
            }
        }

        ChatCompletionResponseMessage2 IPersistableModel<ChatCompletionResponseMessage2>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        protected virtual ChatCompletionResponseMessage2 PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionResponseMessage2>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeChatCompletionResponseMessage(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(ChatCompletionResponseMessage2)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<ChatCompletionResponseMessage2>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}