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
        if (Optional.IsDefined(DefaultMaxOutputTokenCount))
        {
            writer.WriteStringValue(DefaultMaxOutputTokenCount.Value.ToString());
        }
        else if (Optional.IsDefined(CustomMaxOutputTokenCount))
        {
            writer.WriteNumberValue(CustomMaxOutputTokenCount.Value);
        }
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
        else if (element.ValueKind == JsonValueKind.Number)
        {
            customMaxOutputTokenCount = element.GetInt32();
        }
        else
        {
            throw new JsonException($"Expected realtime max output token count to be null, a number, or a string but found {element.ValueKind}.");
        }

        return new RealtimeMaxOutputTokenCount(defaultMaxOutputTokenCount, customMaxOutputTokenCount, patch);
    }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    private bool PropagateGet(ReadOnlySpan<byte> jsonPath, out JsonPatch.EncodedValue value)
    {
        value = default;
        return false;
    }

    private bool PropagateSet(ReadOnlySpan<byte> jsonPath, JsonPatch.EncodedValue value) => false;
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
}
