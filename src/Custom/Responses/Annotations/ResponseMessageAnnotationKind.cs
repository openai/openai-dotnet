using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("AnnotationType")]
public enum ResponseMessageAnnotationKind
{
    [CodeGenMember("FileCitation")]
    FileCitation,

    [CodeGenMember("UrlCitation")]
    UriCitation,

    [CodeGenMember("FilePath")]
    FilePath,

    [CodeGenMember("ContainerFileCitation")]
    ContainerFileCitation
}
