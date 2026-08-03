using Microsoft.TypeSpec.Generator.Customizations;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Converted to extensible enum.
[CodeGenType("AnnotationType")]
public readonly partial struct ResponseMessageAnnotationKind
{
    [CodeGenMember("UrlCitation")]
    public static ResponseMessageAnnotationKind UriCitation { get; } = new ResponseMessageAnnotationKind(UrlCitationValue);
}
