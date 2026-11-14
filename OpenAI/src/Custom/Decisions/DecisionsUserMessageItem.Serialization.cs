using Microsoft.TypeSpec.Generator.Customizations;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Decisions;

// CUSTOM: Accept string shorthand content and normalize it into the longhand input text content collection.
[CodeGenSerialization(nameof(Content), DeserializationValueHook = nameof(DeserializeContentValue))]
public partial class DecisionsUserMessageItem
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
