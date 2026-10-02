using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Realtime;

// CUSTOM: This type is not its own object. Instead, it represents a union, and as such, it must directly forward
// its serialization and deserialization logic to the components of said union.
public partial class RealtimeTracing
{
    // CUSTOM: Edited to remove calls to WriteStartObject() and WriteEndObject(). 
    void IJsonModel<RealtimeTracing>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
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
        string format = options.Format == "W" ? ((IPersistableModel<RealtimeTracing>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(RealtimeTracing)} does not support writing '{format}' format.");
        }
        if (Optional.IsDefined(DefaultTracing))
        {
            writer.WriteStringValue(DefaultTracing.Value.ToString());
        }
        else if (Optional.IsDefined(CustomTracing))
        {
            writer.WriteObjectValue(CustomTracing, options);
        }
    }

    // CUSTOM: Edited to deserialize the different components of the union.
    internal static RealtimeTracing DeserializeRealtimeTracing(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        RealtimeDefaultTracing? defaultTracing = default;
        RealtimeCustomTracing customTracing = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        if (element.ValueKind == JsonValueKind.String)
        {
            defaultTracing = new RealtimeDefaultTracing(element.GetString());
        }
        else if (element.ValueKind == JsonValueKind.Object)
        {
            customTracing = RealtimeCustomTracing.DeserializeRealtimeCustomTracing(element, element.GetUtf8Bytes(), options);
        }
        else
        {
            throw new JsonException($"Expected realtime tracing to be null, an object, or a string but found {element.ValueKind}.");
        }

        return new RealtimeTracing(defaultTracing, customTracing, patch);
    }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    private bool PropagateGet(ReadOnlySpan<byte> jsonPath, out JsonPatch.EncodedValue value)
    {
        value = default;
        return CustomTracing is not null
            && !jsonPath.SequenceEqual("$"u8)
            && CustomTracing.Patch.TryGetEncodedValue(jsonPath, out value);
    }

    private bool PropagateSet(ReadOnlySpan<byte> jsonPath, JsonPatch.EncodedValue value)
    {
        if (CustomTracing is null || jsonPath.SequenceEqual("$"u8))
        {
            return false;
        }

        CustomTracing.Patch.Set(jsonPath, value);
        return true;
    }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
}
