using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Converted to extensible enum.
[CodeGenType("FunctionCallStatus")]
public readonly partial struct FunctionCallStatus { }