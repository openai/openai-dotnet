using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("AnnotationUrlCitation")]
[CodeGenSuppress("UriCitationMessageAnnotation")]
public partial class UriCitationMessageAnnotation
{
    // CUSTOM: Renamed.
    [CodeGenMember("Url")]
    public Uri Uri { get; set; }
}
