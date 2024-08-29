using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

/// <summary>
/// Represents information about a bulk ingestion job of files into a vector store.
/// </summary>
[Experimental("OPENAI001")]
[CodeGenModel("VectorStoreFileBatchObject")]
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
}