using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Decisions;

public partial class DecisionsJsonObject
{
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

        // Preserve arbitrary JSON keys without interpreting property names as JsonPath expressions.
#pragma warning disable SCME0001
        JsonPatch patch = new();
        patch.Set("$"u8, (data ?? element.GetUtf8Bytes()).ToMemory().Span);
        return new DecisionsJsonObject(patch);
#pragma warning restore SCME0001
    }
}
