// <auto-generated/>

#nullable disable

using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Responses
{
    internal partial class InternalResponsesResponseStreamEventResponseWebSearchCallCompleted : IJsonModel<InternalResponsesResponseStreamEventResponseWebSearchCallCompleted>
    {
        internal InternalResponsesResponseStreamEventResponseWebSearchCallCompleted()
        {
        }

        void IJsonModel<InternalResponsesResponseStreamEventResponseWebSearchCallCompleted>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalResponsesResponseStreamEventResponseWebSearchCallCompleted>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalResponsesResponseStreamEventResponseWebSearchCallCompleted)} does not support writing '{format}' format.");
            }
            base.JsonModelWriteCore(writer, options);
            if (_additionalBinaryDataProperties?.ContainsKey("item_id") != true)
            {
                writer.WritePropertyName("item_id"u8);
                writer.WriteStringValue(ItemId);
            }
            if (_additionalBinaryDataProperties?.ContainsKey("output_index") != true)
            {
                writer.WritePropertyName("output_index"u8);
                writer.WriteNumberValue(OutputIndex);
            }
        }

        InternalResponsesResponseStreamEventResponseWebSearchCallCompleted IJsonModel<InternalResponsesResponseStreamEventResponseWebSearchCallCompleted>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => (InternalResponsesResponseStreamEventResponseWebSearchCallCompleted)JsonModelCreateCore(ref reader, options);

        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalResponsesResponseStreamEventResponseWebSearchCallCompleted>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalResponsesResponseStreamEventResponseWebSearchCallCompleted)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeInternalResponsesResponseStreamEventResponseWebSearchCallCompleted(document.RootElement, options);
        }

        internal static InternalResponsesResponseStreamEventResponseWebSearchCallCompleted DeserializeInternalResponsesResponseStreamEventResponseWebSearchCallCompleted(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            StreamingResponseUpdateKind kind = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            string itemId = default;
            int outputIndex = default;
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("type"u8))
                {
                    kind = new StreamingResponseUpdateKind(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("item_id"u8))
                {
                    itemId = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("output_index"u8))
                {
                    outputIndex = prop.Value.GetInt32();
                    continue;
                }
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new InternalResponsesResponseStreamEventResponseWebSearchCallCompleted(kind, additionalBinaryDataProperties, itemId, outputIndex);
        }

        BinaryData IPersistableModel<InternalResponsesResponseStreamEventResponseWebSearchCallCompleted>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalResponsesResponseStreamEventResponseWebSearchCallCompleted>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options);
                default:
                    throw new FormatException($"The model {nameof(InternalResponsesResponseStreamEventResponseWebSearchCallCompleted)} does not support writing '{options.Format}' format.");
            }
        }

        InternalResponsesResponseStreamEventResponseWebSearchCallCompleted IPersistableModel<InternalResponsesResponseStreamEventResponseWebSearchCallCompleted>.Create(BinaryData data, ModelReaderWriterOptions options) => (InternalResponsesResponseStreamEventResponseWebSearchCallCompleted)PersistableModelCreateCore(data, options);

        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalResponsesResponseStreamEventResponseWebSearchCallCompleted>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeInternalResponsesResponseStreamEventResponseWebSearchCallCompleted(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(InternalResponsesResponseStreamEventResponseWebSearchCallCompleted)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<InternalResponsesResponseStreamEventResponseWebSearchCallCompleted>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

        public static implicit operator BinaryContent(InternalResponsesResponseStreamEventResponseWebSearchCallCompleted internalResponsesResponseStreamEventResponseWebSearchCallCompleted)
        {
            if (internalResponsesResponseStreamEventResponseWebSearchCallCompleted == null)
            {
                return null;
            }
            return BinaryContent.Create(internalResponsesResponseStreamEventResponseWebSearchCallCompleted, ModelSerializationExtensions.WireOptions);
        }

        public static explicit operator InternalResponsesResponseStreamEventResponseWebSearchCallCompleted(ClientResult result)
        {
            using PipelineResponse response = result.GetRawResponse();
            using JsonDocument document = JsonDocument.Parse(response.Content);
            return DeserializeInternalResponsesResponseStreamEventResponseWebSearchCallCompleted(document.RootElement, ModelSerializationExtensions.WireOptions);
        }
    }
}
