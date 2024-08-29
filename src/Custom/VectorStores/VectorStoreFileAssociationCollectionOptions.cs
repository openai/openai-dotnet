using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

/// <summary>
/// Represents addition options available when requesting a collection of <see cref="VectorStoreFileAssociation"/> instances.
/// </summary>
[Experimental("OPENAI001")]
public class VectorStoreFileAssociationCollectionOptions
{
    /// <summary>
    /// Creates a new instance of <see cref="VectorStoreFileAssociationCollectionOptions"/>.
    /// </summary>
    public VectorStoreFileAssociationCollectionOptions() { }

    /// <summary>
    /// The <c>order</c> that results should appear in the list according to
    /// their <c>created_at</c> timestamp.
    /// </summary>
    public ListOrder? Order { get; set; }

    /// <summary>
    /// The number of values to return in a page result.
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// The id of the item preceeding the first item in the collection.
    /// </summary>
    public string AfterId { get; set; }

    /// <summary>
    /// The id of the item following the last item in the collection.
    /// </summary>
    public string BeforeId { get; set; }

    /// <summary>
    /// A status filter that file associations must match to be included in the collection.
    /// </summary>
    public VectorStoreFileStatusFilter? Filter { get; set; }
}
