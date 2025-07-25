// <auto-generated/>

#nullable disable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Chat
{
    internal partial class InternalChatCompletionRequestMessageContentPartRefusal : IJsonModel<InternalChatCompletionRequestMessageContentPartRefusal>
    {
        void IJsonModel<InternalChatCompletionRequestMessageContentPartRefusal>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalChatCompletionRequestMessageContentPartRefusal>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalChatCompletionRequestMessageContentPartRefusal)} does not support writing '{format}' format.");
            }
            base.JsonModelWriteCore(writer, options);
        }

        InternalChatCompletionRequestMessageContentPartRefusal IJsonModel<InternalChatCompletionRequestMessageContentPartRefusal>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => (InternalChatCompletionRequestMessageContentPartRefusal)JsonModelCreateCore(ref reader, options);

        protected override ChatMessageContentPart JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalChatCompletionRequestMessageContentPartRefusal>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalChatCompletionRequestMessageContentPartRefusal)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeInternalChatCompletionRequestMessageContentPartRefusal(document.RootElement, options);
        }

        internal static InternalChatCompletionRequestMessageContentPartRefusal DeserializeInternalChatCompletionRequestMessageContentPartRefusal(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (var prop in element.EnumerateObject())
            {
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new InternalChatCompletionRequestMessageContentPartRefusal(additionalBinaryDataProperties);
        }

        BinaryData IPersistableModel<InternalChatCompletionRequestMessageContentPartRefusal>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalChatCompletionRequestMessageContentPartRefusal>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(InternalChatCompletionRequestMessageContentPartRefusal)} does not support writing '{options.Format}' format.");
            }
        }

        InternalChatCompletionRequestMessageContentPartRefusal IPersistableModel<InternalChatCompletionRequestMessageContentPartRefusal>.Create(BinaryData data, ModelReaderWriterOptions options) => (InternalChatCompletionRequestMessageContentPartRefusal)PersistableModelCreateCore(data, options);

        protected override ChatMessageContentPart PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalChatCompletionRequestMessageContentPartRefusal>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeInternalChatCompletionRequestMessageContentPartRefusal(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(InternalChatCompletionRequestMessageContentPartRefusal)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<InternalChatCompletionRequestMessageContentPartRefusal>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
