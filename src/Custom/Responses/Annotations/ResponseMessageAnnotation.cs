using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("Annotation")]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class ResponseMessageAnnotation
{
}
