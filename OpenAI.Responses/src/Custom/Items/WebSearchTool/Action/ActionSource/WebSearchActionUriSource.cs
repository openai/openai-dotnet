using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("WebSearchActionSearchSourceUrl")]
public partial class WebSearchActionUriSource
{
    // CUSTOM: Renamed.
    [CodeGenMember("Url")]
    public Uri Uri { get; set; }
}
