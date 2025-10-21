using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Chat;

public class UpdateChatCompletionOptions : JsonModel<UpdateChatCompletionOptions>
{
    private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

    public UpdateChatCompletionOptions() { }

    internal UpdateChatCompletionOptions(IDictionary<string, string> metadata, IDictionary<string, BinaryData> additionalBinaryDataProperties)
    {
        // Plugin customization: ensure initialization of collections
        Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
        _additionalBinaryDataProperties = additionalBinaryDataProperties;
    }

    public string CompletionId { get; set; }

    public IDictionary<string, string> Metadata { get; }

    internal IDictionary<string, BinaryData> SerializedAdditionalRawData
    {
        get => _additionalBinaryDataProperties;
        set => _additionalBinaryDataProperties = value;
    }

    protected override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<UpdateChatCompletionOptions>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(UpdateChatCompletionOptions)} does not support writing '{format}' format.");
        }
        if (_additionalBinaryDataProperties?.ContainsKey("metadata") != true)
        {
            writer.WritePropertyName("metadata"u8);
            writer.WriteStartObject();
            foreach (var item in Metadata)
            {
                writer.WritePropertyName(item.Key);
                if (item.Value == null)
                {
                    writer.WriteNullValue();
                    continue;
                }
                writer.WriteStringValue(item.Value);
            }
            writer.WriteEndObject();
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

    protected override UpdateChatCompletionOptions CreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        => JsonModelCreateCore(ref reader, options);

    protected virtual UpdateChatCompletionOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<UpdateChatCompletionOptions>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(UpdateChatCompletionOptions)} does not support reading '{format}' format.");
        }
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        return DeserializeUpdateChatCompletionOptions(document.RootElement, options);
    }

    internal static UpdateChatCompletionOptions DeserializeUpdateChatCompletionOptions(JsonElement element, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        IDictionary<string, string> metadata = default;
        IDictionary<string, BinaryData> additionalBinaryDataProperties = new ChangeTrackingDictionary<string, BinaryData>();
        foreach (var prop in element.EnumerateObject())
        {
            if (prop.NameEquals("metadata"u8))
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                foreach (var prop0 in prop.Value.EnumerateObject())
                {
                    if (prop0.Value.ValueKind == JsonValueKind.Null)
                    {
                        dictionary.Add(prop0.Name, null);
                    }
                    else
                    {
                        dictionary.Add(prop0.Name, prop0.Value.GetString());
                    }
                }
                metadata = dictionary;
                continue;
            }
            // Plugin customization: remove options.Format != "W" check
            additionalBinaryDataProperties.Add(prop.Name, BinaryData.FromString(prop.Value.GetRawText()));
        }
        return new UpdateChatCompletionOptions(metadata, additionalBinaryDataProperties);
    }

    public static implicit operator BinaryContent(UpdateChatCompletionOptions options)
    {
        if (options == null)
        {
            return null;
        }
        return BinaryContent.Create(options, ModelSerializationExtensions.WireOptions);
    }
}