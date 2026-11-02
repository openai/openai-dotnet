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
        input = DecisionsSerializationHelpers.DeserializeTextOrParts<DecisionsItem>(
            property,
            text => new DecisionsUserMessageItem([new DecisionsContentInputTextPart { Text = text }]),
            DecisionsItem.DeserializeDecisionsItem,
            options);
    }
}
