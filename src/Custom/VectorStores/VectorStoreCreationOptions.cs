using System.Collections.Generic;

namespace OpenAI.VectorStores;

[CodeGenModel("CreateVectorStoreRequest")]
public partial class VectorStoreCreationOptions
{
    /// <summary> A list of [File](/docs/api-reference/files) IDs that the vector store should use. Useful for tools like `file_search` that can access files. </summary>
    public IList<string> FileIds { get; init; }

    /// <summary> Gets or sets the policy that controls when the new vector store will be automatically deleted. </summary>
    [CodeGenMember("ExpiresAfter")]
    public VectorStoreExpirationPolicy ExpirationPolicy { get; init; }

    [CodeGenMember("ChunkingStrategy")]
    public FileChunkingStrategy ChunkingStrategy { get; init; }
}
