using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ContainerFileCitationBody")]
[CodeGenSuppress("ContainerFileCitationMessageAnnotation")]
public partial class ContainerFileCitationMessageAnnotation
{
    public ContainerFileCitationMessageAnnotation() : this(ResponseMessageAnnotationKind.ContainerFileCitation, default, null, null, default, default, null)
    {
    }
}