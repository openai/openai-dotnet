namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Made internal the constructor that does not take an ID, because contrary to other item types,
//   the ID of reference items is not read-only and is required.
[CodeGenType("DotNetItemReferenceItemResource")]
[CodeGenVisibility(nameof(ReferenceResponseItem), CodeGenVisibility.Internal)]
public partial class ReferenceResponseItem
{
    // CUSTOM: Added a constructor that takes an ID.
    public ReferenceResponseItem(string id) : base(InternalItemType.ItemReference)
    {
        Argument.AssertNotNull(id, nameof(id));

        Id = id;
    }
}
