using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

[CodeGenType("CreateVectorStoreRequest")]
public partial class VectorStoreCreationOptions
{
    /// <summary> Gets or sets the policy that controls when the new vector store will be automatically deleted. </summary>
    [CodeGenMember("ExpiresAfter")]
    public VectorStoreExpirationPolicy ExpirationPolicy { get; set; }

    [CodeGenMember("ChunkingStrategy")]
    public FileChunkingStrategy ChunkingStrategy { get; set; }

    internal BinaryContent ToBinaryContent() => BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
}
