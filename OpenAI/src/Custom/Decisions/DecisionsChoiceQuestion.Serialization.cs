using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Decisions;

// CUSTOM: Accept string shorthand choice instructions and normalize them into an instruction text content collection.
[CodeGenSerialization(nameof(Instructions), DeserializationValueHook = nameof(DeserializeInstructionsValue))]
public partial class DecisionsChoiceQuestion
{
    private static void DeserializeInstructionsValue(JsonProperty property, ref IList<DecisionsInstructionContentPart> instructions, ModelReaderWriterOptions options = null)
    {
        instructions = DecisionsSerializationHelpers.DeserializeTextOrParts<DecisionsInstructionContentPart>(
            property,
            text => new DecisionsInstructionTextContentPart { Text = text },
            DecisionsInstructionContentPart.DeserializeDecisionsInstructionContentPart,
            options);
    }
}
