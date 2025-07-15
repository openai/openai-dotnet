using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

// CUSTOM: Renamed.
[CodeGenType("PageOrderOptions")]
public readonly partial struct VectorStoreCollectionOrder
{
    // CUSTOM: Renamed.
    [CodeGenMember("Asc")]
    public static VectorStoreCollectionOrder Ascending { get; } = new VectorStoreCollectionOrder(AscValue);

    // CUSTOM: Renamed.
    [CodeGenMember("Desc")]
    public static VectorStoreCollectionOrder Descending { get; } = new VectorStoreCollectionOrder(DescValue);
}
