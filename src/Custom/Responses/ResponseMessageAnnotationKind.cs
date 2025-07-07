using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("AnnotationType")]
public enum ResponseMessageAnnotationKind
{
    FileCitation,

    [CodeGenMember("UrlCitation")]
    UriCitation,

    FilePath,

    ContainerFileCitation
}