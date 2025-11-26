namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Made internal the constructor that does not take the item ID because, contrary to other item types, the ID of
//   reference items is required in both input and output scenarios.
[CodeGenType("DotNetItemReferenceItemResource")]
[CodeGenVisibility(nameof(ReferenceResponseItem), CodeGenVisibility.Internal)]
public partial class ReferenceResponseItem
{
    // CUSTOM: Added a constructor that takes the item ID.
    public ReferenceResponseItem(string id) : base(InternalItemType.ItemReference)
    {
        Argument.AssertNotNull(id, nameof(id));

        Id = id;
    }
}
