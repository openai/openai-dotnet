using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Decisions;

// CUSTOM: Accept string shorthand function output and normalize it into a tool output text content collection.
[CodeGenSerialization(nameof(Output), DeserializationValueHook = nameof(DeserializeOutputValue))]
public partial class DecisionsFunctionToolCallOutputItem
{
    private static void DeserializeOutputValue(JsonProperty property, ref IList<DecisionsToolOutputContentPart> output, ModelReaderWriterOptions options = null)
    {
        output = DecisionsSerializationHelpers.DeserializeTextOrParts<DecisionsToolOutputContentPart>(
            property,
            text => new DecisionsToolOutputTextContentPart { Text = text },
            DecisionsToolOutputContentPart.DeserializeDecisionsToolOutputContentPart,
            options);
    }
}
