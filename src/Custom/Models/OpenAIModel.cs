using System;

namespace OpenAI.Models;

/// <summary>
/// Represents information about a single available model entry.
/// </summary>
[CodeGenModel("Model")]
public partial class OpenAIModel
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    /// <summary> The object type, which is always "model". </summary>
    private InternalModelObject Object { get; } = InternalModelObject.Model;

    // CUSTOM: Renamed.
    /// <summary> The Unix timestamp (in seconds) when the model was created. </summary>
    [CodeGenMember("Created")]
    public DateTimeOffset CreatedAt { get; }
}
