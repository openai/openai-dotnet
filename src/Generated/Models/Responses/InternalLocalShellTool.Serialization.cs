// <auto-generated/>

#nullable disable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Responses
{
    internal partial class InternalLocalShellTool : IJsonModel<InternalLocalShellTool>
    {
        void IJsonModel<InternalLocalShellTool>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalLocalShellTool>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalLocalShellTool)} does not support writing '{format}' format.");
            }
            base.JsonModelWriteCore(writer, options);
        }

        InternalLocalShellTool IJsonModel<InternalLocalShellTool>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => (InternalLocalShellTool)JsonModelCreateCore(ref reader, options);

        protected override ResponseTool JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalLocalShellTool>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(InternalLocalShellTool)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeInternalLocalShellTool(document.RootElement, options);
        }

        internal static InternalLocalShellTool DeserializeInternalLocalShellTool(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            InternalToolType kind = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("type"u8))
                {
                    kind = new InternalToolType(prop.Value.GetString());
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new InternalLocalShellTool(kind, additionalBinaryDataProperties);
        }

        BinaryData IPersistableModel<InternalLocalShellTool>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalLocalShellTool>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(InternalLocalShellTool)} does not support writing '{options.Format}' format.");
            }
        }

        InternalLocalShellTool IPersistableModel<InternalLocalShellTool>.Create(BinaryData data, ModelReaderWriterOptions options) => (InternalLocalShellTool)PersistableModelCreateCore(data, options);

        protected override ResponseTool PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<InternalLocalShellTool>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeInternalLocalShellTool(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(InternalLocalShellTool)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<InternalLocalShellTool>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
