using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Realtime;

// CUSTOM: This type is not its own object. Instead, it represents a union, and as such, it must directly forward
// its serialization and deserialization logic to the components of said union.
public partial class GARealtimeTruncation
{
    // CUSTOM: Edited to remove calls to WriteStartObject() and WriteEndObject(). 
    void IJsonModel<GARealtimeTruncation>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
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

    // CUSTOM:
    // - Edited to serialize the DefaultTracing component as a string value.
    // - Edited to serialize the CustomTracing component as an object value.
    // - Removed serialization of additional properties.
    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<GARealtimeTruncation>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(GARealtimeTruncation)} does not support writing '{format}' format.");
        }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        if (Optional.IsDefined(DefaultTruncation) && !Patch.Contains("$.default_truncation"u8))
        {
            writer.WriteStringValue(DefaultTruncation.Value.ToString());
        }
        if (Optional.IsDefined(CustomTruncation) && !Patch.Contains("$.custom_truncation"u8))
        {
            writer.WriteObjectValue(CustomTruncation, options);
        }

        Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
    }

    // CUSTOM:
    // - Edited to deserialize a string value into a GlobalPolicy component.
    // - Edited to deserialize an object value into a CustomPolicy component.
    internal static GARealtimeTruncation DeserializeGARealtimeTruncation(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        GARealtimeDefaultTruncation? defaultTruncation = default;
        GARealtimeCustomTruncation customTruncation = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        if (element.ValueKind == JsonValueKind.String)
        {
            defaultTruncation = new GARealtimeDefaultTruncation(element.GetString());
        }
        else
        {
            customTruncation = GARealtimeCustomTruncation.DeserializeGARealtimeCustomTruncation(element, element.GetUtf8Bytes(), options);
        }

        return new GARealtimeTruncation(defaultTruncation, customTruncation, patch);
    }
}
