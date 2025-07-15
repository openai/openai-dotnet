using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.VectorStores;

/// <summary>
/// A representation of a file association between an uploaded file and a vector store.
/// </summary>
[CodeGenType("VectorStoreFileObject")]
public partial class VectorStoreFileAssociation
{
    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `vector_store.file`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "vector_store.file";

    /// <summary>
    /// The ID of the file that is associated with the vector store.
    /// </summary>
    [CodeGenMember("Id")]
    public string FileId { get; }

    /// <summary>
    /// The total count of bytes used for vector storage of the file. Note that this may differ from the size of the
    /// file.
    /// </summary>
    [CodeGenMember("UsageBytes")]
    public int Size { get; }

    // CUSTOM: Changed type.
    [CodeGenMember("Attributes")]
    public IDictionary<string, BinaryData> Attributes { get; }

    [CodeGenMember("ChunkingStrategy")]
    public FileChunkingStrategy ChunkingStrategy { get; }

    internal static VectorStoreFileAssociation FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeVectorStoreFileAssociation(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}