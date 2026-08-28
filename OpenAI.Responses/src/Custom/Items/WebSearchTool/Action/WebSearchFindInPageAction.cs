using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("WebSearchActionFind")]
[CodeGenSuppress("WebSearchFindInPageAction")]
public partial class WebSearchFindInPageAction
{
    public WebSearchFindInPageAction() : this(InternalWebSearchActionType.FindInPage, default, null, null)
    {
    }

    // CUSTOM: Renamed.
    [CodeGenMember("Url")]
    public Uri Uri { get; set; }
}