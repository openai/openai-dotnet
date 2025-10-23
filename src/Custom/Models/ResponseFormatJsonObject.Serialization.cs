using System;
using System.ClientModel.Primitives;
using System.Text;
using System.Text.Json;

namespace OpenAI.Chat
{
    public partial class ResponseFormatJsonObject : ResponseFormat, IJsonModel<ResponseFormatJsonObject>
    {
        void IJsonModel<ResponseFormatJsonObject>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
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

        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ResponseFormatJsonObject>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ResponseFormatJsonObject)} does not support writing '{format}' format.");
            }
            base.JsonModelWriteCore(writer, options);
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

            Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        }

        ResponseFormatJsonObject IJsonModel<ResponseFormatJsonObject>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => (ResponseFormatJsonObject)JsonModelCreateCore(ref reader, options);

        protected override ResponseFormat JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ResponseFormatJsonObject>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ResponseFormatJsonObject)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeResponseFormatJsonObject(document.RootElement, null, options);
        }

        internal static ResponseFormatJsonObject DeserializeResponseFormatJsonObject(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            ResponseFormatType kind = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("type"u8))
                {
                    kind = (ResponseFormatType)Enum.Parse(typeof(ResponseFormatType), prop.Value.GetString());
                    continue;
                }
                patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(prop.Name)], prop.Value.GetUtf8Bytes());
            }
            return new ResponseFormatJsonObject(kind, patch);
        }

        BinaryData IPersistableModel<ResponseFormatJsonObject>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected override BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ResponseFormatJsonObject>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(ResponseFormatJsonObject)} does not support writing '{options.Format}' format.");
            }
        }

        ResponseFormatJsonObject IPersistableModel<ResponseFormatJsonObject>.Create(BinaryData data, ModelReaderWriterOptions options) => (ResponseFormatJsonObject)PersistableModelCreateCore(data, options);

        protected override ResponseFormat PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ResponseFormatJsonObject>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeResponseFormatJsonObject(document.RootElement, data, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(ResponseFormatJsonObject)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<ResponseFormatJsonObject>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
