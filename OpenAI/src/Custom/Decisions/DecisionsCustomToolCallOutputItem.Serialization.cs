using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Decisions;

[CodeGenSerialization(nameof(Output), DeserializationValueHook = nameof(DeserializeOutputValue))]
public partial class DecisionsCustomToolCallOutputItem
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
