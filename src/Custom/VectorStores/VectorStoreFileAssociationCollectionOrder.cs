using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

// CUSTOM: Renamed.
[Experimental("OPENAI001")]
[CodeGenModel("ListVectorStoreFilesRequestOrder")]
public readonly partial struct VectorStoreFileAssociationCollectionOrder
{
    // CUSTOM: Renamed.
    [CodeGenMember("Asc")]
    public static VectorStoreFileAssociationCollectionOrder Ascending { get; } = new VectorStoreFileAssociationCollectionOrder(AscendingValue);

    // CUSTOM: Renamed.
    [CodeGenMember("Desc")]
    public static VectorStoreFileAssociationCollectionOrder Descending { get; } = new VectorStoreFileAssociationCollectionOrder(DescendingValue);
}
