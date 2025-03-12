namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Suppressed constructor in favor of custom constructor with required `id` parameter.
[CodeGenType("ResponsesWebSearchCallItem")]
[CodeGenSuppress(nameof(WebSearchCallResponseItem))]
public partial class WebSearchCallResponseItem
{
    public WebSearchCallResponseItem(string id) : base(InternalResponsesItemType.WebSearchCall, id)
    {
        Argument.AssertNotNull(id, nameof(id));
    }
}
