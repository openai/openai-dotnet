using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

[Experimental("OPENAI001")]
[CodeGenModel("DeleteVectorStoreResponse")]
public partial class VectorStoreDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string VectorStoreId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `vector_store.deleted`. </summary>
    internal InternalDeleteVectorStoreResponseObject Object { get; } = InternalDeleteVectorStoreResponseObject.VectorStoreDeleted;
}
