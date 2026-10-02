using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.VectorStores;

/// <summary>
/// A representation of a file storage and indexing container used by the <c>file_search</c> tool for assistants.
/// </summary>
[CodeGenType("VectorStoreObject")]
public partial class VectorStore
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `vector_store`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "vector_store";

    // CUSTOM: Changed type.
    /// <summary> The total number of bytes used by the files in the vector store. </summary>
    public long UsageBytes { get; }

    /// <summary>
    /// Gets the policy that controls when this vector store will be automatically deleted.
    /// </summary>
    [CodeGenMember("ExpiresAfter")]
    public VectorStoreExpirationPolicy ExpirationPolicy { get; }
}