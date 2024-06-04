using OpenAI.Files;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Assistants
{
    [CodeGenModel("CreateAssistantRequestToolResourcesFileSearchVectorStoreCreationHelpersVectorStore")]
    public partial class VectorStoreCreationHelper
    {
        public VectorStoreCreationHelper(IEnumerable<string> fileIds, IDictionary<string, string> metadata = null)
        {
            FileIds = fileIds.ToList();
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
        }

        public VectorStoreCreationHelper(IEnumerable<OpenAIFileInfo> files, IDictionary<string, string> metadata = null)
            : this(files?.Select(file => file.Id) ?? [], metadata)
        {}
    }
}
