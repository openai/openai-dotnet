using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ResponseIncompleteDetailsReason")]
public readonly partial struct ResponseIncompleteStatusReason
{
}