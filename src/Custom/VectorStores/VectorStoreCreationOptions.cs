using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

[Experimental("OPENAI001")]
[CodeGenModel("CreateVectorStoreRequest")]
public partial class VectorStoreCreationOptions
{
    /// <summary> Gets or sets the policy that controls when the new vector store will be automatically deleted. </summary>
    [CodeGenMember("ExpiresAfter")]
    public VectorStoreExpirationPolicy ExpirationPolicy { get; set; }

    [CodeGenMember("ChunkingStrategy")]
    public FileChunkingStrategy ChunkingStrategy { get; set; }
}
