using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.VectorStores;

[CodeGenType("DeleteVectorStoreFileResponse")]
public partial class FileFromStoreRemovalResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string FileId { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Deleted")]
    public bool Removed { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `vector_store.file.deleted`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "vector_store.file.deleted";

    internal static FileFromStoreRemovalResult FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeFileFromStoreRemovalResult(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
