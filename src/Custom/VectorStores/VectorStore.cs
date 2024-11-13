using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

/// <summary>
/// A representation of a file storage and indexing container used by the <c>file_search</c> tool for assistants.
/// </summary>
[Experimental("OPENAI001")]
[CodeGenModel("VectorStoreObject")]
public partial class VectorStore
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `vector_store`. </summary>
    [CodeGenMember("Object")]
    internal InternalVectorStoreObjectObject Object { get; } = InternalVectorStoreObjectObject.VectorStore;

    /// <summary>
    /// Gets the policy that controls when this vector store will be automatically deleted.
    /// </summary>
    [CodeGenMember("ExpiresAfter")]
    public VectorStoreExpirationPolicy ExpirationPolicy { get; }
}