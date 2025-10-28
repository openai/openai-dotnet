using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Chat
{
    public partial class ChatCompletionMessageList : IJsonModel<ChatCompletionMessageList>
    {
        internal ChatCompletionMessageList() : this(null, null, null, null, default, default)
        {
        }

        void IJsonModel<ChatCompletionMessageList>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            if (Patch.Contains("$"u8))
            {
                writer.WriteRawValue(Patch.GetJson("$"u8));
                return;
            }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionMessageList>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatCompletionMessageList)} does not support writing '{format}' format.");
            }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            if (!Patch.Contains("$.object"u8))
            {
                writer.WritePropertyName("object"u8);
                writer.WriteStringValue(Object);
            }
            if (Patch.Contains("$.data"u8))
            {
                if (!Patch.IsRemoved("$.data"u8))
                {
                    writer.WritePropertyName("data"u8);
                    writer.WriteRawValue(Patch.GetJson("$.data"u8));
                }
            }
            else
            {
                writer.WritePropertyName("data"u8);
                writer.WriteStartArray();
                for (int i = 0; i < Data.Count; i++)
                {
                    if (Data[i].Patch.IsRemoved("$"u8))
                    {
                        continue;
                    }
                    writer.WriteObjectValue(Data[i], options);
                }
                Patch.WriteTo(writer, "$.data"u8);
                writer.WriteEndArray();
            }
            if (!Patch.Contains("$.first_id"u8))
            {
                writer.WritePropertyName("first_id"u8);
                writer.WriteStringValue(FirstId);
            }
            if (!Patch.Contains("$.last_id"u8))
            {
                writer.WritePropertyName("last_id"u8);
                writer.WriteStringValue(LastId);
            }
            if (!Patch.Contains("$.has_more"u8))
            {
                writer.WritePropertyName("has_more"u8);
                writer.WriteBooleanValue(HasMore);
            }

            Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        }

        ChatCompletionMessageList IJsonModel<ChatCompletionMessageList>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual ChatCompletionMessageList JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionMessageList>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatCompletionMessageList)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeChatCompletionMessageList(document.RootElement, null, options);
        }

        internal static ChatCompletionMessageList DeserializeChatCompletionMessageList(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            string @object = default;
            IList<ChatCompletionMessageListDatum> data0 = default;
            string firstId = default;
            string lastId = default;
            bool hasMore = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("object"u8))
                {
                    @object = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("data"u8))
                {
                    List<ChatCompletionMessageListDatum> array = new List<ChatCompletionMessageListDatum>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ChatCompletionMessageListDatum.DeserializeChatCompletionMessageListDatum(item, item.GetUtf8Bytes(), options));
                    }
                    data0 = array;
                    continue;
                }
                if (prop.NameEquals("first_id"u8))
                {
                    firstId = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("last_id"u8))
                {
                    lastId = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("has_more"u8))
                {
                    hasMore = prop.Value.GetBoolean();
                    continue;
                }
                patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(prop.Name)], prop.Value.GetUtf8Bytes());
            }
            return new ChatCompletionMessageList(
                @object,
                data0,
                firstId,
                lastId,
                hasMore,
                patch);
        }

        BinaryData IPersistableModel<ChatCompletionMessageList>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionMessageList>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(ChatCompletionMessageList)} does not support writing '{options.Format}' format.");
            }
        }

        ChatCompletionMessageList IPersistableModel<ChatCompletionMessageList>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        protected virtual ChatCompletionMessageList PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ChatCompletionMessageList>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeChatCompletionMessageList(document.RootElement, data, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(ChatCompletionMessageList)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<ChatCompletionMessageList>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

        public static explicit operator ChatCompletionMessageList(ClientResult result)
        {
            using PipelineResponse response = result.GetRawResponse();
            BinaryData data = response.Content;
            using JsonDocument document = JsonDocument.Parse(data);
            return DeserializeChatCompletionMessageList(document.RootElement, data, ModelSerializationExtensions.WireOptions);
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        private bool PropagateGet(ReadOnlySpan<byte> jsonPath, out JsonPatch.EncodedValue value)
        {
            ReadOnlySpan<byte> local = jsonPath.SliceToStartOfPropertyName();
            value = default;

            if (local.StartsWith("data"u8))
            {
                int propertyLength = "data"u8.Length;
                ReadOnlySpan<byte> currentSlice = local.Slice(propertyLength);
                if (!currentSlice.TryGetIndex(out int index, out int bytesConsumed))
                {
                    return false;
                }
                return Data[index].Patch.TryGetEncodedValue([.. "$"u8, .. currentSlice.Slice(bytesConsumed)], out value);
            }
            return false;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        private bool PropagateSet(ReadOnlySpan<byte> jsonPath, JsonPatch.EncodedValue value)
        {
            ReadOnlySpan<byte> local = jsonPath.SliceToStartOfPropertyName();

            if (local.StartsWith("data"u8))
            {
                int propertyLength = "data"u8.Length;
                ReadOnlySpan<byte> currentSlice = local.Slice(propertyLength);
                if (!currentSlice.TryGetIndex(out int index, out int bytesConsumed))
                {
                    return false;
                }
                Data[index].Patch.Set([.. "$"u8, .. currentSlice.Slice(bytesConsumed)], value);
                return true;
            }
            return false;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }
}
