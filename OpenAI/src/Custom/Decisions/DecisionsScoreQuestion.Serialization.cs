using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Decisions;

// CUSTOM: Accept string shorthand score instructions and normalize them into an instruction text content collection.
[CodeGenSerialization(nameof(Instructions), DeserializationValueHook = nameof(DeserializeInstructionsValue))]
public partial class DecisionsScoreQuestion
{
    private static void DeserializeInstructionsValue(JsonProperty property, ref IList<DecisionsInstructionsPart> instructions, ModelReaderWriterOptions options = null)
    {
        instructions = DecisionsSerializationHelpers.DeserializeTextOrParts<DecisionsInstructionsPart>(
            property,
            text => new DecisionsInstructionsInputTextPart { Text = text },
            DecisionsInstructionsPart.DeserializeDecisionsInstructionsPart,
            options);
    }
}
