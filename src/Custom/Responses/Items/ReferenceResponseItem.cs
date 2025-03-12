namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Suppressed constructor in favor of custom constructor with required `id` parameter.
[CodeGenType("ResponsesItemReferenceItem")]
[CodeGenSuppress(nameof(ReferenceResponseItem))]
public partial class ReferenceResponseItem
{
    public ReferenceResponseItem(string id) : base(InternalResponsesItemType.ItemReference, id)
    {
    }
}
