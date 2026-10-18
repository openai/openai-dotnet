using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Decisions;

// CUSTOM: Accept string shorthand content and normalize it into the longhand input text content collection.
[CodeGenSerialization(nameof(Content), DeserializationValueHook = nameof(DeserializeContentValue))]
public partial class DecisionsMessageItem
{
    private static void DeserializeContentValue(JsonProperty property, ref IList<DecisionsContentPart> content, ModelReaderWriterOptions options = null)
    {
        if (property.Value.ValueKind == JsonValueKind.String)
        {
            content = [new DecisionsInputTextContentPart { Text = property.Value.GetString() }];
            return;
        }

        if (property.Value.ValueKind == JsonValueKind.Array)
        {
            List<DecisionsContentPart> parts = [];
            foreach (JsonElement item in property.Value.EnumerateArray())
            {
                parts.Add(DecisionsContentPart.DeserializeDecisionsContentPart(item, item.GetUtf8Bytes(), options));
            }
            content = parts;
            return;
        }

        throw new JsonException($"Expected decision message content to be a string or an array but found {property.Value.ValueKind}.");
    }
}
