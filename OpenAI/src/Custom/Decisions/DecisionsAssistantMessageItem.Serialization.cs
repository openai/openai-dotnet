using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenAI.Decisions;

[CodeGenSerialization(nameof(Content), DeserializationValueHook = nameof(DeserializeContentValue))]
public partial class DecisionsAssistantMessageItem
{
    private static void DeserializeContentValue(JsonProperty property, ref IList<DecisionsAssistantContentPart> content, ModelReaderWriterOptions options = null)
    {
        content = DecisionsSerializationHelpers.DeserializeTextOrParts<DecisionsAssistantContentPart>(
            property,
            text => new DecisionsOutputTextContentPart { Text = text },
            DecisionsAssistantContentPart.DeserializeDecisionsAssistantContentPart,
            options);
    }
}
