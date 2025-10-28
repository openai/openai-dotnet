using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace OpenAI.Chat;

public class UpdateChatCompletionOptions : JsonModel<UpdateChatCompletionOptions>
{
    [Experimental("SCME0001")]
    private JsonPatch _patch;

    public UpdateChatCompletionOptions() : this(null, default) { }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    internal UpdateChatCompletionOptions(IDictionary<string, string> metadata, in JsonPatch patch)
    {
        // Plugin customization: ensure initialization of collections
        Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
        _patch = patch;
    }

    public string CompletionId { get; set; }

    public IDictionary<string, string> Metadata { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Experimental("SCME0001")]
    public ref JsonPatch Patch => ref _patch;

    protected override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<UpdateChatCompletionOptions>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(UpdateChatCompletionOptions)} does not support writing '{format}' format.");
        }
        writer.WriteStartObject();
        if (Optional.IsCollectionDefined(Metadata) && !Patch.Contains("$.metadata"u8))
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
        Patch.WriteTo(writer);
        writer.WriteEndObject();
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
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
        return DeserializeUpdateChatCompletionOptions(document.RootElement, null, options);
    }

    internal static UpdateChatCompletionOptions DeserializeUpdateChatCompletionOptions(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        IDictionary<string, string> metadata = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
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
            patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(prop.Name)], prop.Value.GetUtf8Bytes());
        }
        return new UpdateChatCompletionOptions(metadata, patch);
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