using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Decisions;

public partial class DecisionsChoiceValue
{
    void IJsonModel<DecisionsChoiceValue>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
#pragma warning disable SCME0001
        if (Patch.Contains("$"u8))
        {
            writer.WriteRawValue(Patch.GetJson("$"u8));
            return;
        }
#pragma warning restore SCME0001
        JsonModelWriteCore(writer, options);
    }

    protected virtual void JsonModelWriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
    {
        string format = options.Format == "W" ? ((IPersistableModel<DecisionsChoiceValue>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(DecisionsChoiceValue)} does not support writing '{format}' format.");
        }

        if (StringValue is not null)
        {
            writer.WriteStringValue(StringValue);
        }
        else if (BooleanValue.HasValue)
        {
            writer.WriteBooleanValue(BooleanValue.Value);
        }
        else
        {
            throw new InvalidOperationException("A decision choice value must contain a string or a boolean.");
        }
    }

    internal static DecisionsChoiceValue DeserializeDecisionsChoiceValue(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        string stringValue = null;
        bool? booleanValue = null;
        if (element.ValueKind == JsonValueKind.String)
        {
            stringValue = element.GetString();
        }
        else if (element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False)
        {
            booleanValue = element.GetBoolean();
        }
        else
        {
            throw new JsonException($"Expected a decision choice value to be a string or a boolean but found {element.ValueKind}.");
        }

#pragma warning disable SCME0001
        JsonPatch patch = new(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
        return new DecisionsChoiceValue(stringValue, booleanValue, patch);
#pragma warning restore SCME0001
    }

#pragma warning disable SCME0001
    private bool PropagateGet(ReadOnlySpan<byte> jsonPath, out JsonPatch.EncodedValue value)
    {
        value = default;
        return false;
    }

    private bool PropagateSet(ReadOnlySpan<byte> jsonPath, JsonPatch.EncodedValue value) => false;
#pragma warning restore SCME0001
}
