using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.VectorStores;

/// <summary>
/// A representation of a file storage and indexing container used by the <c>file_search</c> tool for assistants.
/// </summary>
[CodeGenType("VectorStoreObject")]
public partial class VectorStore
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `vector_store`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "vector_store";

    /// <summary>
    /// Gets the policy that controls when this vector store will be automatically deleted.
    /// </summary>
    [CodeGenMember("ExpiresAfter")]
    public VectorStoreExpirationPolicy ExpirationPolicy { get; }

    internal static VectorStore FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeVectorStore(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}