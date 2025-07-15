using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.VectorStores;

[CodeGenType("DeleteVectorStoreResponse")]
public partial class VectorStoreDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string VectorStoreId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `vector_store.deleted`. </summary>
    internal string Object { get; } = "vector_store.deleted";

    internal static VectorStoreDeletionResult FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeVectorStoreDeletionResult(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
