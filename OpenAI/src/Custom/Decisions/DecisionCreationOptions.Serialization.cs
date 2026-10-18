using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Decisions;

// CUSTOM: Accept string shorthand input and normalize it into a user message containing an input text part.
[CodeGenSerialization(nameof(InputItems), DeserializationValueHook = nameof(DeserializeInputValue))]
public partial class DecisionCreationOptions
{
    private static void DeserializeInputValue(JsonProperty property, ref IList<DecisionsItem> input, ModelReaderWriterOptions options = null)
    {
        if (property.Value.ValueKind == JsonValueKind.String)
        {
            input = [new DecisionsMessageItem
            {
                Content = { new DecisionsInputTextContentPart { Text = property.Value.GetString() } }
            }];
            return;
        }

        if (property.Value.ValueKind == JsonValueKind.Array)
        {
            List<DecisionsItem> items = [];
            foreach (JsonElement item in property.Value.EnumerateArray())
            {
                items.Add(DecisionsItem.DeserializeDecisionsItem(item, item.GetUtf8Bytes(), options));
            }
            input = items;
            return;
        }

        throw new JsonException($"Expected decision input to be a string or an array but found {property.Value.ValueKind}.");
    }
}
