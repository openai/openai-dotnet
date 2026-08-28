using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("AnnotationFilePath")]
[CodeGenSuppress("FilePathMessageAnnotation")]
public partial class FilePathMessageAnnotation
{
    public FilePathMessageAnnotation() : this(ResponseMessageAnnotationKind.FilePath, default, null, default)
    {
    }
}
