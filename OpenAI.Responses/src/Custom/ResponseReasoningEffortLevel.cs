using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ResponsesReasoningEffort")]
public readonly partial struct ResponseReasoningEffortLevel
{
}
