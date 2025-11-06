using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("AnnotationUrlCitation")]
public partial class UriCitationMessageAnnotation
{
    // CUSTOM: Renamed.
    [CodeGenMember("Url")]
    public Uri Uri { get; set; }
}
