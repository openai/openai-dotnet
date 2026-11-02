using System;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Decisions;

public partial class DecisionsJsonObject
{
    public DecisionsJsonObject(BinaryData json)
    {
        Argument.AssertNotNull(json, nameof(json));
        using JsonDocument document = JsonDocument.Parse(json, ModelSerializationExtensions.JsonDocumentOptions);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            throw new JsonException($"Expected a decision JSON object but found {document.RootElement.ValueKind}.");
        }
#pragma warning disable SCME0001
        _patch = DeserializeDecisionsJsonObject(document.RootElement, json, ModelReaderWriterOptions.Json)._patch;
#pragma warning restore SCME0001
    }
}
