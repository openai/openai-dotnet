using System.ClientModel;

namespace OpenAI.VectorStores;

public partial class InternalCreateVectorStoreFileRequest
{
    [CodeGenMember("ChunkingStrategy")]
    public FileChunkingStrategy ChunkingStrategy { get; set; }

    internal BinaryContent ToBinaryContent() => BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
}
