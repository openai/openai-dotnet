using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

/// <summary> The options to configure how <see cref="VectorStore"/> objects are retrieved and paginated. </summary>
[Experimental("OPENAI001")]
public class VectorStoreCollectionOptions
{
    /// <summary> Initializes a new instance of <see cref="VectorStoreCollectionOptions"/>. </summary>
    public VectorStoreCollectionOptions() { }

    /// <summary> 
    ///     A limit on the number of <see cref="VectorStore"/> objects to be returned per page.
    /// </summary>
    public int? PageSizeLimit { get; set; }

    /// <summary>
    ///     The order in which to retrieve <see cref="VectorStore"/> objects when sorted by their
    ///     <see cref="VectorStore.CreatedAt"/> timestamp.
    /// </summary>
    public VectorStoreCollectionOrder? Order { get; set; }

    /// <summary>
    ///     The <see cref="VectorStore.Id"/> used to retrieve the page of <see cref="VectorStore"/> objects that come
    ///     after this one.
    /// </summary>
    public string AfterId { get; set; }

    /// <summary>
    ///     The <see cref="VectorStore.Id"/> used to retrieve the page of <see cref="VectorStore"/> objects that come
    ///     before this one.
    /// </summary>
    public string BeforeId { get; set; }
}
