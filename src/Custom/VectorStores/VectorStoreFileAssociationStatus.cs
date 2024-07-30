using System.ComponentModel;

namespace OpenAI.VectorStores;

/// <summary>
/// Represents the possible states for a vector store file association.
/// </summary>
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
