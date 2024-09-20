using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

[Experimental("OPENAI001")]
[CodeGenModel("DeleteVectorStoreFileResponse")]
public partial class FileFromStoreRemovalResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string FileId { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Deleted")]
    public bool Removed { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `vector_store.file.deleted`. </summary>
    [CodeGenMember("Object")]
    internal InternalDeleteVectorStoreFileResponseObject Object { get; } = InternalDeleteVectorStoreFileResponseObject.VectorStoreFileDeleted;
}
