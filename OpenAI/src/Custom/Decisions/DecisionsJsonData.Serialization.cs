using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Decisions;

public partial class DecisionsJsonData
{
    // The composition model represents the JSON value itself, without synthetic component properties.
    void IJsonModel<DecisionsJsonData>.Write(Utf8JsonWriter writer, ModelReaderWriterOptions options)
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
        string format = options.Format == "W" ? ((IPersistableModel<DecisionsJsonData>)this).GetFormatFromOptions(options) : options.Format;
        if (format != "J")
        {
            throw new FormatException($"The model {nameof(DecisionsJsonData)} does not support writing '{format}' format.");
        }

        if (ObjectValue is not null)
        {
            writer.WriteObjectValue(ObjectValue, options);
        }
        else if (ArrayValue is not null)
        {
            writer.WriteStartArray();
            foreach (DecisionsJsonObject item in ArrayValue)
            {
#pragma warning disable SCME0001
                if (!item.Patch.IsRemoved("$"u8))
                {
                    writer.WriteObjectValue(item, options);
                }
#pragma warning restore SCME0001
            }
#pragma warning disable SCME0001
            Patch.WriteTo(writer);
#pragma warning restore SCME0001
            writer.WriteEndArray();
        }
        else
        {
            throw new InvalidOperationException("Decision JSON data must contain an object or an array of objects.");
        }
    }

    internal static DecisionsJsonData DeserializeDecisionsJsonData(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        DecisionsJsonObject objectValue = null;
        IReadOnlyList<DecisionsJsonObject> arrayValue = null;
        if (element.ValueKind == JsonValueKind.Object)
        {
            objectValue = DecisionsJsonObject.DeserializeDecisionsJsonObject(element, element.GetUtf8Bytes(), options);
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            List<DecisionsJsonObject> objects = [];
            foreach (JsonElement item in element.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                {
                    throw new JsonException($"Expected decision JSON data array elements to be objects but found {item.ValueKind}.");
                }
                objects.Add(DecisionsJsonObject.DeserializeDecisionsJsonObject(item, item.GetUtf8Bytes(), options));
            }
            arrayValue = objects.AsReadOnly();
        }
        else
        {
            throw new JsonException($"Expected decision JSON data to be an object or an array of objects but found {element.ValueKind}.");
        }

#pragma warning disable SCME0001
        JsonPatch patch = new(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
        return new DecisionsJsonData(objectValue, arrayValue, patch);
#pragma warning restore SCME0001
    }

#pragma warning disable SCME0001
    private bool PropagateGet(ReadOnlySpan<byte> jsonPath, out JsonPatch.EncodedValue value)
    {
        value = default;
        if (jsonPath.SequenceEqual("$"u8))
        {
            return false;
        }
        if (ObjectValue is not null)
        {
            return ObjectValue.Patch.TryGetEncodedValue(jsonPath, out value);
        }

        ReadOnlySpan<byte> local = jsonPath.Slice(1);
        return ArrayValue is not null
            && local.TryGetIndex(out int index, out int bytesConsumed)
            && index >= 0 && index < ArrayValue.Count
            && ArrayValue[index].Patch.TryGetEncodedValue([.. "$"u8, .. local.Slice(bytesConsumed)], out value);
    }

    private bool PropagateSet(ReadOnlySpan<byte> jsonPath, JsonPatch.EncodedValue value)
    {
        if (jsonPath.SequenceEqual("$"u8))
        {
            return false;
        }
        if (ObjectValue is not null)
        {
            ObjectValue.Patch.Set(jsonPath, value);
            return true;
        }

        ReadOnlySpan<byte> local = jsonPath.Slice(1);
        if (ArrayValue is null || !local.TryGetIndex(out int index, out int bytesConsumed) || index < 0 || index >= ArrayValue.Count)
        {
            return false;
        }
        ArrayValue[index].Patch.Set([.. "$"u8, .. local.Slice(bytesConsumed)], value);
        return true;
    }
#pragma warning restore SCME0001
}
