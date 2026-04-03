using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("WebSearchActionSearchSourceUrl")]
public partial class WebSearchUriActionSource
{
    // CUSTOM: Renamed.
    [CodeGenMember("Url")]
    public Uri Uri { get; set; }
}
