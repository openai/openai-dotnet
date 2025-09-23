using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Responses
{
    public partial class CodeInterpreterTool : IJsonModel<CodeInterpreterTool>
    {
        protected override void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<CodeInterpreterTool>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(CodeInterpreterTool)} does not support writing '{format}' format.");
            }
            base.JsonModelWriteCore(writer, options);
            if (_additionalBinaryDataProperties?.ContainsKey("container") != true)
            {
                writer.WritePropertyName("container"u8);
#if NET6_0_OR_GREATER
                writer.WriteRawValue(Container.AsBinaryData());
#else
                using (JsonDocument document = JsonDocument.Parse(Container.AsBinaryData()))
                {
                    JsonSerializer.Serialize(writer, document.RootElement);
                }
#endif
            }
        }

        internal static CodeInterpreterTool DeserializeCodeInterpreterTool(JsonElement element, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            InternalToolType kind = default;
            IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
            CodeInterpreterContainer container = default;
            
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("type"u8))
                {
                    kind = new InternalToolType(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("container"u8))
                {
                    container = CodeInterpreterContainer.DeserializeCodeInterpreterContainer(prop.Value, options);
                    continue;
                }
                // Plugin customization: remove options.Format != "W" check
                additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
            }
            return new CodeInterpreterTool(kind, additionalBinaryDataProperties, container);
        }
    }
}
