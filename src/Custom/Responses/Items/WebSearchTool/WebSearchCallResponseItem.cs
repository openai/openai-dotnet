namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("WebSearchToolCallItemResource")]
public partial class WebSearchCallResponseItem
{
    // CUSTOM: Made nullable since this is a read-only property.
    [CodeGenMember("Status")]
    public WebSearchCallStatus? Status { get; }
}
