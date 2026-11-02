using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Decisions;

[CodeGenSerialization(nameof(Content), DeserializationValueHook = nameof(DeserializeContentValue))]
public partial class DecisionsSystemMessageItem
{
    private static void DeserializeContentValue(JsonProperty property, ref IList<DecisionsInstructionMessageContentPart> content, ModelReaderWriterOptions options = null)
    {
        content = DecisionsSerializationHelpers.DeserializeTextOrParts<DecisionsInstructionMessageContentPart>(
            property,
            text => new DecisionsInstructionMessageTextContentPart { Text = text },
            DecisionsInstructionMessageContentPart.DeserializeDecisionsInstructionMessageContentPart,
            options);
    }
}
