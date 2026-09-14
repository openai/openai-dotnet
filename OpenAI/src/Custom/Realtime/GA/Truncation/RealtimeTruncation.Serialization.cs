using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Realtime;

// CUSTOM: This type is not its own object. Instead, it represents a union, and as such, it must directly forward
// its serialization and deserialization logic to the components of said union.
public partial class RealtimeTruncation
{
    // CUSTOM: Edited to remove calls to WriteStartObject() and WriteEndObject(). 
    void IJsonModel<RealtimeTruncation>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
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
        string format = options.Format == "W" ? ((IPersistableModel<RealtimeTruncation>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(RealtimeTruncation)} does not support writing '{format}' format.");
        }
        if (Optional.IsDefined(DefaultTruncation))
        {
            writer.WriteStringValue(DefaultTruncation.Value.ToString());
        }
        else if (Optional.IsDefined(CustomTruncation))
        {
            writer.WriteObjectValue(CustomTruncation, options);
        }
    }

    // CUSTOM: Edited to deserialize the different components of the union.
    internal static RealtimeTruncation DeserializeRealtimeTruncation(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        RealtimeDefaultTruncation? defaultTruncation = default;
        RealtimeCustomTruncation customTruncation = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        if (element.ValueKind == JsonValueKind.String)
        {
            defaultTruncation = new RealtimeDefaultTruncation(element.GetString());
        }
        else if (element.ValueKind == JsonValueKind.Object)
        {
            customTruncation = RealtimeCustomTruncation.DeserializeRealtimeCustomTruncation(element, element.GetUtf8Bytes(), options);
        }
        else
        {
            throw new JsonException($"Expected realtime truncation to be null, an object, or a string but found {element.ValueKind}.");
        }

        return new RealtimeTruncation(defaultTruncation, customTruncation, patch);
    }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    private bool PropagateGet(ReadOnlySpan<byte> jsonPath, out JsonPatch.EncodedValue value)
    {
        value = default;
        return CustomTruncation is not null
            && !jsonPath.SequenceEqual("$"u8)
            && CustomTruncation.Patch.TryGetEncodedValue(jsonPath, out value);
    }

    private bool PropagateSet(ReadOnlySpan<byte> jsonPath, JsonPatch.EncodedValue value)
    {
        if (CustomTruncation is null || jsonPath.SequenceEqual("$"u8))
        {
            return false;
        }

        CustomTruncation.Patch.Set(jsonPath, value);
        return true;
    }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
}
