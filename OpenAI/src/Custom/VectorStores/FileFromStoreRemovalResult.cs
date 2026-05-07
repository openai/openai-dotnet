using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.VectorStores;

[CodeGenType("DeleteVectorStoreFileResponse")]
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
    internal string Object { get; } = "vector_store.file.deleted";
}
