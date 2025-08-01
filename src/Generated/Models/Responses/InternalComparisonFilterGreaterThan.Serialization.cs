// <auto-generated/>

#nullable disable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Responses
{
    internal partial class InternalComparisonFilterGreaterThan : IJsonModel<InternalComparisonFilterGreaterThan>
    {
        internal InternalComparisonFilterGreaterThan() : this(InternalComparisonFilterType.Gt, null, null, null)
        {
        }

        void IJsonModel<InternalComparisonFilterGreaterThan>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalComparisonFilterGreaterThan>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalComparisonFilterGreaterThan)} does not support writing '{format}' format.");
            }
            base.JsonModelWriteCore(writer, options);
        }

        InternalComparisonFilterGreaterThan IJsonModel<InternalComparisonFilterGreaterThan>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => (InternalComparisonFilterGreaterThan)JsonModelCreateCore(ref reader, options);

        protected override InternalComparisonFilter JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalComparisonFilterGreaterThan>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalComparisonFilterGreaterThan)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeInternalComparisonFilterGreaterThan(document.RootElement, options);
        }

        internal static InternalComparisonFilterGreaterThan DeserializeInternalComparisonFilterGreaterThan(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            InternalComparisonFilterType kind = default;
            string key = default;
            BinaryData value = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("type"u8))
                {
                    kind = new InternalComparisonFilterType(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("key"u8))
                {
                    key = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("value"u8))
                {
                    value = BinaryData.FromString(prop.Value.GetRawText());
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new InternalComparisonFilterGreaterThan(kind, key, value, additionalBinaryDataProperties);
        }

        BinaryData IPersistableModel<InternalComparisonFilterGreaterThan>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalComparisonFilterGreaterThan>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(InternalComparisonFilterGreaterThan)} does not support writing '{options.Format}' format.");
            }
        }

        InternalComparisonFilterGreaterThan IPersistableModel<InternalComparisonFilterGreaterThan>.Create(BinaryData data, ModelReaderWriterOptions options) => (InternalComparisonFilterGreaterThan)PersistableModelCreateCore(data, options);

        protected override InternalComparisonFilter PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalComparisonFilterGreaterThan>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeInternalComparisonFilterGreaterThan(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(InternalComparisonFilterGreaterThan)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<InternalComparisonFilterGreaterThan>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
