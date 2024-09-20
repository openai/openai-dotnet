using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

// CUSTOM: Renamed.
[Experimental("OPENAI001")]
[CodeGenModel("ListVectorStoresRequestOrder")]
public readonly partial struct VectorStoreCollectionOrder
{
    // CUSTOM: Renamed.
    [CodeGenMember("Asc")]
    public static VectorStoreCollectionOrder Ascending { get; } = new VectorStoreCollectionOrder(AscendingValue);

    // CUSTOM: Renamed.
    [CodeGenMember("Desc")]
    public static VectorStoreCollectionOrder Descending { get; } = new VectorStoreCollectionOrder(DescendingValue);
}
