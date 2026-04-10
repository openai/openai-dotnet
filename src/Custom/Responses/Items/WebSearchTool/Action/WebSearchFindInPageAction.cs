using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("WebSearchActionFind")]
public partial class WebSearchFindInPageAction
{
    // CUSTOM: Renamed.
    [CodeGenMember("Url")]
    public Uri Uri { get; set; }
}