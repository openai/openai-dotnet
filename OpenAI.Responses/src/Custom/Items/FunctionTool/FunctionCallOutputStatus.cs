using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("FunctionToolCallOutputStatus")]
public readonly partial struct FunctionCallOutputStatus
{
}