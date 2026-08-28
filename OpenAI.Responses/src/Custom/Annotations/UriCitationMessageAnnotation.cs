using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("AnnotationUrlCitation")]
[CodeGenSuppress("UriCitationMessageAnnotation")]
public partial class UriCitationMessageAnnotation
{
    public UriCitationMessageAnnotation() : this(ResponseMessageAnnotationKind.UriCitation, default, null, default, default, null)
    {
    }

    // CUSTOM: Renamed.
    [CodeGenMember("Url")]
    public Uri Uri { get; set; }
}
