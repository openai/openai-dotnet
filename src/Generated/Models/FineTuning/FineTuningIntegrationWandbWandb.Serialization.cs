// <auto-generated/>

#nullable disable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.FineTuning
{
    internal partial class FineTuningIntegrationWandbWandb : IJsonModel<FineTuningIntegrationWandbWandb>
    {
        internal FineTuningIntegrationWandbWandb() : this(null, null, null, null, null)
        {
        }

        void IJsonModel<FineTuningIntegrationWandbWandb>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            JsonModelWriteCore(writer, options);
            writer.WriteEndObject();
        }

        protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<FineTuningIntegrationWandbWandb>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(FineTuningIntegrationWandbWandb)} does not support writing '{format}' format.");
            }
            if (_additionalBinaryDataProperties?.ContainsKey("project") != true)
            {
                writer.WritePropertyName("project"u8);
                writer.WriteStringValue(Project);
            }
            if (Optional.IsDefined(Name) && _additionalBinaryDataProperties?.ContainsKey("name") != true)
            {
                writer.WritePropertyName("name"u8);
                writer.WriteStringValue(Name);
            }
            if (Optional.IsDefined(Entity) && _additionalBinaryDataProperties?.ContainsKey("entity") != true)
            {
                writer.WritePropertyName("entity"u8);
                writer.WriteStringValue(Entity);
            }
            if (Optional.IsCollectionDefined(Tags) && _additionalBinaryDataProperties?.ContainsKey("tags") != true)
            {
                writer.WritePropertyName("tags"u8);
                writer.WriteStartArray();
                foreach (string item in Tags)
                {
                    if (item == null)
                    {
                        writer.WriteNullValue();
                        continue;
                    }
                    writer.WriteStringValue(item);
                }
                writer.WriteEndArray();
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

        FineTuningIntegrationWandbWandb IJsonModel<FineTuningIntegrationWandbWandb>.Create(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        protected virtual FineTuningIntegrationWandbWandb JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<FineTuningIntegrationWandbWandb>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(FineTuningIntegrationWandbWandb)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeFineTuningIntegrationWandbWandb(document.RootElement, options);
        }

        internal static FineTuningIntegrationWandbWandb DeserializeFineTuningIntegrationWandbWandb(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            string project = default;
            string name = default;
            string entity = default;
            IList<string> tags = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("project"u8))
                {
                    project = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("name"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        name = null;
                        continue;
                    }
                    name = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("entity"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        entity = null;
                        continue;
                    }
                    entity = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("tags"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<string> array = new List<string>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.Null)
                        {
                            array.Add(null);
                        }
                        else
                        {
                            array.Add(item.GetString());
                        }
                    }
                    tags = array;
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new FineTuningIntegrationWandbWandb(project, name, entity, tags ?? new ChangeTrackingList<string>(), additionalBinaryDataProperties);
        }

        BinaryData IPersistableModel<FineTuningIntegrationWandbWandb>.Write(ModelReaderWriterOptions options) => PersistableModelWriteCore(options);

        protected virtual BinaryData PersistableModelWriteCore(ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<FineTuningIntegrationWandbWandb>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    return ModelReaderWriter.Write(this, options, OpenAIContext.Default);
                default:
                    throw new FormatException($"The model {nameof(FineTuningIntegrationWandbWandb)} does not support writing '{options.Format}' format.");
            }
        }

        FineTuningIntegrationWandbWandb IPersistableModel<FineTuningIntegrationWandbWandb>.Create(BinaryData data, ModelReaderWriterOptions options) => PersistableModelCreateCore(data, options);

        protected virtual FineTuningIntegrationWandbWandb PersistableModelCreateCore(BinaryData data, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<FineTuningIntegrationWandbWandb>)this).GetFormatFromOptions(options) : options.Format;
            switch (format)
            {
                case "J":
                    using (JsonDocument document = JsonDocument.Parse(data))
                    {
                        return DeserializeFineTuningIntegrationWandbWandb(document.RootElement, options);
                    }
                default:
                    throw new FormatException($"The model {nameof(FineTuningIntegrationWandbWandb)} does not support reading '{options.Format}' format.");
            }
        }

        string IPersistableModel<FineTuningIntegrationWandbWandb>.GetFormatFromOptions(ModelReaderWriterOptions options) => "J";
    }
}
