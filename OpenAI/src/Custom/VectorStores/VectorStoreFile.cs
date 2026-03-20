using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.Collections.Generic;

namespace OpenAI.VectorStores;

/// <summary>
/// A representation of a file association between an uploaded file and a vector store.
/// </summary>
[CodeGenType("VectorStoreFileObject")]
public partial class VectorStoreFile
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
}