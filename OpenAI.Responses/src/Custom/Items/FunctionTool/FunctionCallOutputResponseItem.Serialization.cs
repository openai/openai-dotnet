using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace OpenAI.Responses;

[CodeGenSerialization(nameof(FunctionOutput), DeserializationValueHook = nameof(DeserializeFunctionOutputValue))]
public partial class FunctionCallOutputResponseItem : IJsonModel<FunctionCallOutputResponseItem>
{
    // CUSTOM: Accepts the string shorthand output form and normalizes it into an input text content part.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeFunctionOutputValue(JsonProperty property, ref IList<ResponseContentPart> functionOutput, ModelReaderWriterOptions options = null)
    {
        if (property.Value.ValueKind == JsonValueKind.String)
        {
            functionOutput = [ResponseContentPart.CreateInputTextPart(property.Value.GetString())];
            return;
        }

        if (property.Value.ValueKind == JsonValueKind.Array)
        {
            List<ResponseContentPart> contentParts = [];
            foreach (JsonElement item in property.Value.EnumerateArray())
            {
                contentParts.Add(ResponseContentPart.DeserializeResponseContentPart(item, item.GetUtf8Bytes(), options));
            }
            functionOutput = contentParts;
            return;
        }

        throw new InvalidOperationException($"Expected output to be a string or an array but found {property.Value.ValueKind}.");
    }
}