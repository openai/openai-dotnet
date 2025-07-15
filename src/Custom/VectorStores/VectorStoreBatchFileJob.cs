using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.VectorStores;

/// <summary>
/// Represents information about a bulk ingestion job of files into a vector store.
/// </summary>
[CodeGenType("VectorStoreFileBatchObject")]
public partial class VectorStoreBatchFileJob
{
    private readonly object Object;

    /// <summary>
    /// The ID of the batch file ingestion job into the vector store corresponding to <see cref="VectorStoreId"/>.
    /// </summary>
    [CodeGenMember("Id")]
    public string BatchId { get; }

    /// <summary> Gets the file counts. </summary>
    [CodeGenMember("Counts")]
    public VectorStoreFileCounts FileCounts { get; }

    internal static VectorStoreBatchFileJob FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeVectorStoreBatchFileJob(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}