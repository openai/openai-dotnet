using Microsoft.TypeSpec.Generator.Customizations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.VectorStores;

/// <summary>
/// Represents the possible states for a vector store file.
/// </summary>
[Experimental("OPENAI001")]
[CodeGenType("VectorStoreFileObjectStatus")]
public enum VectorStoreFileStatus
{
    /// <summary>
    /// An unknown vector store file status.
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
