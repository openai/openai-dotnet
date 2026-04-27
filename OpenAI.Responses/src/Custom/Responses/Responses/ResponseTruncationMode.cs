using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("CreateResponseTruncation")]
public readonly partial struct ResponseTruncationMode
{
}