using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("Annotation")]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
[CodeGenSuppress("ResponseMessageAnnotation", typeof(ResponseMessageAnnotationKind))]
public partial class ResponseMessageAnnotation
{
    private protected ResponseMessageAnnotation(ResponseMessageAnnotationKind kind)
    {
        Kind = kind;
    }
}
