using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("AnnotationFileCitation")]
[CodeGenSuppress("FileCitationMessageAnnotation")]
public partial class FileCitationMessageAnnotation
{
    public FileCitationMessageAnnotation() : this(ResponseMessageAnnotationKind.FileCitation, default, null, default, null)
    {
    }
}
