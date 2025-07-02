using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[Experimental("OPENAI001")]
[CodeGenType("WebSearchToolCallItemResource")]
public partial class WebSearchCallResponseItem
{
    // CUSTOM: Retain optionality of OpenAPI read-only property value
    [CodeGenMember("Status")]
    public WebSearchCallStatus? Status { get; }
}
