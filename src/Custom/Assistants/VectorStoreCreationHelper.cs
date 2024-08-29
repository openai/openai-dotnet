using OpenAI.Files;
using OpenAI.VectorStores;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Assistants;

[Experimental("OPENAI001")]
[CodeGenModel("ToolResourcesFileSearchVectorStore")]
public partial class VectorStoreCreationHelper
{
    [CodeGenMember("ChunkingStrategy")]
    public FileChunkingStrategy ChunkingStrategy { get; set; }

    public VectorStoreCreationHelper(IEnumerable<string> fileIds, IDictionary<string, string> metadata = null)
    {
        FileIds = fileIds.ToList();
        Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
    }

    public VectorStoreCreationHelper(IEnumerable<OpenAIFileInfo> files, IDictionary<string, string> metadata = null)
        : this(files?.Select(file => file.Id) ?? [], metadata)
    { }
}
