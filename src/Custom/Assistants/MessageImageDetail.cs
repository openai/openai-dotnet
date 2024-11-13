using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary>
/// The available detail settings to use when processing an image.
/// These settings balance token consumption and the resolution of evaluation performed.
/// </summary>
[Experimental("OPENAI001")]
public enum MessageImageDetail
{
    /// <summary> Default. Allows the model to automatically select detail. </summary>
    Auto,

    /// <summary> Reduced detail that uses fewer tokens than <see cref="High"/>. </summary>
    Low,

    /// <summary> Increased detail that uses more tokens than <see cref="Low"/>. </summary>
    High,
}
