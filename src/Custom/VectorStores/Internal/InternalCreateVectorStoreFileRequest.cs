using System;
using System.Collections.Generic;

namespace OpenAI.VectorStores;

internal partial class InternalCreateVectorStoreFileRequest
{
    [CodeGenMember("ChunkingStrategy")]
    public FileChunkingStrategy ChunkingStrategy { get; set; }
}
