using System.ClientModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

[CodeGenType("UpdateVectorStoreRequest")]
public partial class VectorStoreModificationOptions
{
    /// <summary> Gets or sets the policy that controls when the new vector store will be automatically deleted. </summary>
    [CodeGenMember("ExpiresAfter")]
    public VectorStoreExpirationPolicy ExpirationPolicy { get; set; }

    internal BinaryContent ToBinaryContent() => BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
}
