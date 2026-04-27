using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("CreateResponseReasoningSummary")]
public readonly partial struct ResponseReasoningSummaryVerbosity
{
}
