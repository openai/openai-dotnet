namespace OpenAI.VectorStores;

[CodeGenModel("DeleteVectorStoreFileResponse")]
public partial class FileFromStoreRemovalResult
{
    [CodeGenMember("Id")]
    public string FileId { get; }

    [CodeGenMember("Deleted")]
    public bool Removed { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `vector_store.file.deleted`. </summary>
    [CodeGenMember("Object")]
    internal InternalDeleteVectorStoreFileResponseObject Object { get; } = InternalDeleteVectorStoreFileResponseObject.VectorStoreFileDeleted;
}
