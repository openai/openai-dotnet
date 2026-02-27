using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Realtime;

// CUSTOM: This type is not its own object. Instead, it represents a union, and as such, it must directly forward
// its serialization and deserialization logic to the components of said union.
public partial class RealtimeMaxOutputTokenCount
{
    // CUSTOM: Edited to remove calls to WriteStartObject() and WriteEndObject(). 
    void IJsonModel<RealtimeMaxOutputTokenCount>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (Patch.Contains("$"u8))
        {
            writer.WriteRawValue(Patch.GetJson("$"u8));
            return;
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        JsonModelWriteCore(writer, options);
    }

    // CUSTOM: Edited to serialize the different components of the union.
    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<RealtimeMaxOutputTokenCount>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(RealtimeMaxOutputTokenCount)} does not support writing '{format}' format.");
        }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (Optional.IsDefined(DefaultMaxOutputTokenCount) && !Patch.Contains("$.default_max_output_token_count"u8))
        {
            writer.WriteStringValue(DefaultMaxOutputTokenCount.Value.ToString());
        }
        if (Optional.IsDefined(CustomMaxOutputTokenCount) && !Patch.Contains("$.custom_max_output_token_count"u8))
        {
            writer.WriteNumberValue(CustomMaxOutputTokenCount.Value);
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    // CUSTOM: Edited to deserialize the different components of the union.
    internal static RealtimeMaxOutputTokenCount DeserializeRealtimeMaxOutputTokenCount(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        RealtimeDefaultMaxOutputTokenCount? defaultMaxOutputTokenCount = default;
        int? customMaxOutputTokenCount = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        if (element.ValueKind == JsonValueKind.String)
        {
            defaultMaxOutputTokenCount = new RealtimeDefaultMaxOutputTokenCount(element.GetString());
        }
        else
        {
            customMaxOutputTokenCount = element.GetInt32();
        }

        return new RealtimeMaxOutputTokenCount(defaultMaxOutputTokenCount, customMaxOutputTokenCount, patch);
    }
}
