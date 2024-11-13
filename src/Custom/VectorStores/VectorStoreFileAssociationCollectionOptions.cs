using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

/// <summary> The options to configure how <see cref="VectorStoreFileAssociation"/> objects are retrieved and paginated. </summary>
[Experimental("OPENAI001")]
public class VectorStoreFileAssociationCollectionOptions
{
    /// <summary> Initializes a new instance of <see cref="VectorStoreFileAssociationCollectionOptions"/>. </summary>
    public VectorStoreFileAssociationCollectionOptions() { }

    /// <summary> 
    ///     A limit on the number of <see cref="VectorStoreFileAssociation"/> objects to be returned per page.
    /// </summary>
    public int? PageSizeLimit { get; set; }

    /// <summary>
    ///     The order in which to retrieve <see cref="VectorStoreFileAssociation"/> objects when sorted by their
    ///     <see cref="VectorStoreFileAssociation.CreatedAt"/> timestamp.
    /// </summary>
    public VectorStoreFileAssociationCollectionOrder? Order { get; set; }

    /// <summary>
    ///     The <see cref="VectorStoreFileAssociation.Id"/> used to retrieve the page of <see cref="VectorStoreFileAssociation"/> objects that come
    ///     after this one.
    /// </summary>
    public string AfterId { get; set; }

    /// <summary>
    ///     The <see cref="VectorStoreFileAssociation.Id"/> used to retrieve the page of <see cref="VectorStoreFileAssociation"/> objects that come
    ///     before this one.
    /// </summary>
    public string BeforeId { get; set; }

    /// <summary>
    ///     A filter to only retrieve the <see cref="VectorStoreFileAssociation"/> objects with a matching
    ///     <see cref="VectorStoreFileAssociation.Status"/>.
    /// </summary>
    public VectorStoreFileStatusFilter? Filter { get; set; }
}
