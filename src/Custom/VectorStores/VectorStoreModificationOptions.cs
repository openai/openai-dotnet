using System.Collections.Generic;

namespace OpenAI.VectorStores;

[CodeGenModel("UpdateVectorStoreRequest")]
public partial class VectorStoreModificationOptions
{
    /// <summary> Gets or sets the policy that controls when the new vector store will be automatically deleted. </summary>
    [CodeGenMember("ExpiresAfter")]
    public VectorStoreExpirationPolicy ExpirationPolicy { get; set; }
}
