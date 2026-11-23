using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ResponseError")]
[CodeGenSerialization(nameof(Code), DeserializationValueHook = nameof(DeserializeCodeValue))]
public partial class ResponseError
{
    // Protocol errors can have a null code, but ResponseError.Code is non-nullable.
    private static void DeserializeCodeValue(JsonProperty property, ref ResponseErrorCode code, ModelReaderWriterOptions options = null)
    {
        code = property.Value.ValueKind == JsonValueKind.Null
            ? default
            : new ResponseErrorCode(property.Value.GetString());
    }
}