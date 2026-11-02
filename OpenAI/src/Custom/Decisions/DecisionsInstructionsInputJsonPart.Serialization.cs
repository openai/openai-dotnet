using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Decisions;

// CUSTOM: Accept a single free-form JSON object as shorthand for the instruction JSON data collection.
[CodeGenSerialization(nameof(Data), DeserializationValueHook = nameof(DeserializeDataValue))]
public partial class DecisionsInstructionsInputJsonPart
{
    private static void DeserializeDataValue(JsonProperty property, ref IList<BinaryData> data, ModelReaderWriterOptions options = null)
    {
        data = DecisionsSerializationHelpers.DeserializeJsonObjects(property);
    }
}
