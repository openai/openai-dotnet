using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Chat
{
    public partial class ResponseFormatJsonSchemaJsonSchema : IJsonModel<ResponseFormatJsonSchemaJsonSchema>
    {
        internal ResponseFormatJsonSchemaJsonSchema()
        {
        }

        void IJsonModel<ResponseFormatJsonSchemaJsonSchema>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ResponseFormatJsonSchemaJsonSchema>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ResponseFormatJsonSchemaJsonSchema)} does not support writing '{format}' format.");
            }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            if (Optional.IsDefined(Description) && !Patch.Contains("$.json_schema"u8))
            {
                writer.WritePropertyName("description"u8);
                writer.WriteStringValue(Description);
            }
            if (!Patch.Contains("$.name"u8))
            {
                writer.WritePropertyName("name"u8);
                writer.WriteStringValue(Name);
            }
            if (Optional.IsDefined(Schema) && !Patch.Contains("$.schema"u8))
            {
                writer.WritePropertyName("schema"u8);
#if NET6_0_OR_GREATER
                writer.WriteRawValue(Schema);
#else
                using (JsonDocument document = JsonDocument.Parse(Schema))
                {
                    JsonSerializer.Serialize(writer, document.RootElement);
                }
#endif
            }
            if (Optional.IsDefined(Strict) && !Patch.Contains("$.strict"u8))
            {
                writer.WritePropertyName("strict"u8);
                writer.WriteBooleanValue(Strict.Value);
            }
            Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        }

        ResponseFormatJsonSchemaJsonSchema IJsonModel<ResponseFormatJsonSchemaJsonSchema>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual ResponseFormatJsonSchemaJsonSchema JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ResponseFormatJsonSchemaJsonSchema>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ResponseFormatJsonSchemaJsonSchema)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeResponseFormatJsonSchemaJsonSchema(document.RootElement, null, options);
        }

        internal static ResponseFormatJsonSchemaJsonSchema DeserializeResponseFormatJsonSchemaJsonSchema(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            string description = default;
            string name = default;
            BinaryData schema = default;
            bool? strict = default;
            #pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("description"u8))
                {
                    description = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("name"u8))
                {
                    name = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("schema"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    schema = BinaryData.FromString(prop.Value.GetRawText());
                    continue;
                }
                if (prop.NameEquals("strict"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        strict = null;
                        continue;
                    }
                    strict = prop.Value.GetBoolean();
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(prop.Name)], prop.Value.GetUtf8Bytes());
            }
            return new ResponseFormatJsonSchemaJsonSchema(description, name, schema, strict, patch);
        }

        BinaryData IPersistableModel<ResponseFormatJsonSchemaJsonSchema>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ResponseFormatJsonSchemaJsonSchema>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(ResponseFormatJsonSchemaJsonSchema)} does not support writing '{options.Format}' format.");
            }
        }

        ResponseFormatJsonSchemaJsonSchema IPersistableModel<ResponseFormatJsonSchemaJsonSchema>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        protected virtual ResponseFormatJsonSchemaJsonSchema PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<ResponseFormatJsonSchemaJsonSchema>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeResponseFormatJsonSchemaJsonSchema(document.RootElement, data, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(ResponseFormatJsonSchemaJsonSchema)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<ResponseFormatJsonSchemaJsonSchema>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
