using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Decisions;

// CUSTOM: Accept string shorthand assistant content and normalize it into an output text content collection.
[CodeGenSerialization(nameof(Content), DeserializationValueHook = nameof(DeserializeContentValue))]
public partial class DecisionsAssistantMessageItem
{
    private static void DeserializeContentValue(JsonProperty property, ref IList<DecisionsOutputContentPart> content, ModelReaderWriterOptions options = null)
    {
        content = DecisionsSerializationHelpers.DeserializeTextOrParts<DecisionsOutputContentPart>(
            property,
            text => new DecisionsOutputTextContentPart { Text = text },
            DecisionsOutputContentPart.DeserializeDecisionsOutputContentPart,
            options);
    }
}
