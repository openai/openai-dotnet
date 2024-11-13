using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

/// <summary>
/// Represents the possible states for a vector store file association.
/// </summary>
[Experimental("OPENAI001")]
[CodeGenModel("VectorStoreFileObjectStatus")]
public enum VectorStoreFileAssociationStatus
{
    /// <summary>
    /// An unknown vector store file association status.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    Unknown,

    [CodeGenMember("InProgress")]
    InProgress,

    [CodeGenMember("Completed")]
    Completed,

    [CodeGenMember("Cancelled")]
    Cancelled,

    [CodeGenMember("Failed")]
    Failed,
}
