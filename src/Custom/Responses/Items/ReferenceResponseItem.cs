namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
[CodeGenType("ResponsesItemReferenceItem")]
public partial class ReferenceResponseItem
{
    public ReferenceResponseItem(string id)
        : base(
            InternalResponsesItemType.ItemReference,
            id,
            additionalBinaryDataProperties: null)
    {
    }

    // CUSTOM: Supply an internal default constructor for serialization and mocking.
    internal ReferenceResponseItem()
    { }
}
