namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("WebSearchToolCallItemResource")]
public partial class WebSearchCallResponseItem
{
    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public WebSearchCallStatus? Status { get; set; }
}
