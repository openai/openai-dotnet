using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Decisions;

public partial class DecisionsJsonObject
{
    // CUSTOM: Preserve arbitrary object keys in a root patch instead of interpreting property names as JsonPath expressions.
    internal static DecisionsJsonObject DeserializeDecisionsJsonObject(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
    {
        if (element.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new JsonException($"Expected a decision JSON object but found {element.ValueKind}.");
        }

#pragma warning disable SCME0001
        JsonPatch patch = new();
        patch.Set("$"u8, (data ?? element.GetUtf8Bytes()).ToMemory().Span);
        return new DecisionsJsonObject(patch);
#pragma warning restore SCME0001
    }
}
