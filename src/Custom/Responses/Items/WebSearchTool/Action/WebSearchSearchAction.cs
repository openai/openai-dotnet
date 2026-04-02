using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("WebSearchActionSearch")]
public partial class WebSearchSearchAction
{
    // CUSTOM: Made obsolete.
    [Obsolete("This property is obsolete. Use the Queries property instead.")]
    [CodeGenMember("Query")]
    public string Query { get; set; }
}