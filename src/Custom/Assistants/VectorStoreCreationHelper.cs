using OpenAI.Files;
using OpenAI.VectorStores;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Assistants;

[CodeGenType("ToolResourcesFileSearchVectorStore")]
public partial class VectorStoreCreationHelper
{
    [CodeGenMember("ChunkingStrategy")]
    public FileChunkingStrategy ChunkingStrategy { get; set; }

    public VectorStoreCreationHelper(IEnumerable<string> fileIds)
        : this(fileIds?.ToList(), null, null, null)
    { }

    public VectorStoreCreationHelper(IEnumerable<OpenAIFile> files)
        : this(files?.Select(file => file.Id).ToList() ?? [], null, null, null)
    { }
}
