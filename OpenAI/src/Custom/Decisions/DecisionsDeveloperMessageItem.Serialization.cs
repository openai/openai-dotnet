using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Decisions;

// CUSTOM: Accept string shorthand developer content and normalize it into an input text content collection.
[CodeGenSerialization(nameof(Content), DeserializationValueHook = nameof(DeserializeContentValue))]
public partial class DecisionsDeveloperMessageItem
{
    private static void DeserializeContentValue(JsonProperty property, ref IList<DecisionsInputContentPart> content, ModelReaderWriterOptions options = null)
    {
        content = DecisionsSerializationHelpers.DeserializeTextOrParts<DecisionsInputContentPart>(
            property,
            text => new DecisionsInputTextContentPart { Text = text },
            DecisionsInputContentPart.DeserializeDecisionsInputContentPart,
            options);
    }
}
