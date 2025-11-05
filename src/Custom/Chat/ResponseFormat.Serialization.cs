using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Chat
{
    [PersistableModelProxy(typeof(ResponseFormat))]
    public partial class ResponseFormat : IJsonModel<ResponseFormat>
    {
        internal ResponseFormat()
        {
        }

        [Experimental("OPENAI001")]
        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ResponseFormat>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ResponseFormat)} does not support writing '{format}' format.");
            }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            if (!Patch.Contains("$.type"u8))
            {
                writer.WritePropertyName("type"u8);
                writer.WriteStringValue(Kind.ToString());
            }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        }

        ResponseFormat IJsonModel<ResponseFormat>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        [Experimental("OPENAI001")]
        protected virtual ResponseFormat JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ResponseFormat>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ResponseFormat)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeResponseFormat(document.RootElement, null, options);
        }

        internal static ResponseFormat DeserializeResponseFormat(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            if (element.TryGetProperty("type"u8, out JsonElement discriminator))
            {
                var kind = discriminator.GetString();
                switch (kind)
                {
                    case "json_schema":
                        return ResponseFormatJsonSchema.DeserializeResponseFormatJsonSchema(element, data, options);
                    default:
                        return new ResponseFormat((ResponseFormatType)Enum.Parse(typeof(ResponseFormatType), kind));
                }
            }
            return new ResponseFormat((ResponseFormatType)Enum.Parse(typeof(ResponseFormatType), "unknown"));
        }

        BinaryData IPersistableModel<ResponseFormat>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        [Experimental("OPENAI001")]
        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ResponseFormat>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(ResponseFormat)} does not support writing '{options.Format}' format.");
            }
        }

        ResponseFormat IPersistableModel<ResponseFormat>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        [Experimental("OPENAI001")]
        protected virtual ResponseFormat PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ResponseFormat>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeResponseFormat(document.RootElement, data, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(ResponseFormat)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<ResponseFormat>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";

        void IJsonModel<ResponseFormat>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
            => CustomSerializationHelpers.SerializeInstance(this, WriteCore, writer, options);

        internal static void WriteCore(ResponseFormat instance, Utf8JsonWriter writer, ModelReaderWriterOptions options)
            => instance.WriteCore(writer, options);

        internal virtual void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
            => throw new InvalidOperationException($"The {nameof(WriteCore)} method should be invoked on an overriding type derived from {nameof(ResponseFormat)}.");
    }
}
