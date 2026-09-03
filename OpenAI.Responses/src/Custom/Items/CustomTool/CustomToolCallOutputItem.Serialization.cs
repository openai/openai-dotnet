using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Responses;

[CodeGenSerialization(nameof(Output), DeserializationValueHook = nameof(DeserializeOutputValue))]
public partial class CustomToolCallOutputItem : IJsonModel<CustomToolCallOutputItem>
{
    // CUSTOM: Accepts the string shorthand output form and normalizes it into an input text content part.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeOutputValue(JsonProperty property, ref IList<ResponseContentPart> output, ModelReaderWriterOptions options = null)
    {
        if (property.Value.ValueKind == JsonValueKind.String)
        {
            output = [ResponseContentPart.CreateInputTextPart(property.Value.GetString())];
            return;
        }

        if (property.Value.ValueKind == JsonValueKind.Array)
        {
            List<ResponseContentPart> contentParts = [];
            foreach (JsonElement item in property.Value.EnumerateArray())
            {
                contentParts.Add(ResponseContentPart.DeserializeResponseContentPart(item, item.GetUtf8Bytes(), options));
            }
            output = contentParts;
            return;
        }

        throw new JsonException($"Expected output to be a string or an array but found {property.Value.ValueKind}.");
    }
}
