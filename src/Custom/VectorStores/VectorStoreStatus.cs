using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

/// <summary>
/// Represents the possible states for a vector store.
/// </summary>
[Experimental("OPENAI001")]
[CodeGenModel("VectorStoreObjectStatus")]
public enum VectorStoreStatus
{
    /// <summary>
    /// An unknown vector store status.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    Unknown,

    [CodeGenMember("InProgress")]
    InProgress,

    [CodeGenMember("Completed")]
    Completed,

    [CodeGenMember("Expired")]
    Expired,
}
