using System;

namespace OpenAI;

/// <summary>
/// A vector database entry.
/// </summary>
internal readonly struct VectorDatabaseEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VectorDatabaseEntry"/> class.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="data"></param>
    /// <param name="id"></param>
    public VectorDatabaseEntry(ReadOnlyMemory<float> vector, BinaryData data, int? id = default)
    {
        Vector = vector;
        Data = data;
        Id = id;
    }

    /// <summary>
    /// Gets the data associated with the entry.
    /// </summary>
    public BinaryData Data { get; }

    /// <summary>
    /// Gets the vector associated with the entry.
    /// </summary>
    public ReadOnlyMemory<float> Vector { get; }

    /// <summary>
    /// Gets the id associated with the entry.
    /// </summary>
    public int? Id { get; }
}
