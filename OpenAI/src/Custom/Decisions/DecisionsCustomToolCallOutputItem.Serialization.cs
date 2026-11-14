using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Decisions;

// CUSTOM: Accept string shorthand custom tool output and normalize it into an input text content collection.
[CodeGenSerialization(nameof(Output), DeserializationValueHook = nameof(DeserializeOutputValue))]
public partial class DecisionsCustomToolCallOutputItem
{
    private static void DeserializeOutputValue(JsonProperty property, ref IList<DecisionsInputContentPart> output, ModelReaderWriterOptions options = null)
    {
        output = DecisionsSerializationHelpers.DeserializeTextOrParts<DecisionsInputContentPart>(
            property,
            text => new DecisionsInputTextContentPart { Text = text },
            DecisionsInputContentPart.DeserializeDecisionsInputContentPart,
            options);
    }
}
