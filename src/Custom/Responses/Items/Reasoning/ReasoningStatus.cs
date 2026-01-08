using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("ReasoningItemResourceStatus")]
public enum ReasoningStatus
{
    InProgress,
    Completed,
    Incomplete
}