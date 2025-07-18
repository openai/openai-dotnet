// <auto-generated/>

#nullable disable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Responses
{
    public partial class StreamingResponseIncompleteUpdate : IJsonModel<StreamingResponseIncompleteUpdate>
    {
        internal StreamingResponseIncompleteUpdate() : this(InternalResponseStreamEventType.ResponseIncomplete, default, null, null)
        {
        }

        void IJsonModel<StreamingResponseIncompleteUpdate>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<StreamingResponseIncompleteUpdate>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(StreamingResponseIncompleteUpdate)} does not support writing '{format}' format.");
            }
            base.JsonModelWriteCore(writer, options);
            if (_additionalBinaryDataProperties?.ContainsKey("response") != true)
            {
                writer.WritePropertyName("response"u8);
                writer.WriteObjectValue(Response, options);
            }
        }

        StreamingResponseIncompleteUpdate IJsonModel<StreamingResponseIncompleteUpdate>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => (StreamingResponseIncompleteUpdate)JsonModelCreateCore(ref reader, options);

        protected override StreamingResponseUpdate JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<StreamingResponseIncompleteUpdate>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(StreamingResponseIncompleteUpdate)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeStreamingResponseIncompleteUpdate(document.RootElement, options);
        }

        internal static StreamingResponseIncompleteUpdate DeserializeStreamingResponseIncompleteUpdate(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            InternalResponseStreamEventType kind = default;
            int sequenceNumber = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            OpenAIResponse response = default;
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("type"u8))
                {
                    kind = new InternalResponseStreamEventType(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("sequence_number"u8))
                {
                    sequenceNumber = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("response"u8))
                {
                    response = OpenAIResponse.DeserializeOpenAIResponse(prop.Value, options);
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new StreamingResponseIncompleteUpdate(kind, sequenceNumber, additionalBinaryDataProperties, response);
        }

        BinaryData IPersistableModel<StreamingResponseIncompleteUpdate>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<StreamingResponseIncompleteUpdate>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(StreamingResponseIncompleteUpdate)} does not support writing '{options.Format}' format.");
            }
        }

        StreamingResponseIncompleteUpdate IPersistableModel<StreamingResponseIncompleteUpdate>.Create(BinaryData data, ModelReaderWriterOptions options) => (StreamingResponseIncompleteUpdate)PersistableModelCreateCore(data, options);

        protected override StreamingResponseUpdate PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<StreamingResponseIncompleteUpdate>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeStreamingResponseIncompleteUpdate(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(StreamingResponseIncompleteUpdate)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<StreamingResponseIncompleteUpdate>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
