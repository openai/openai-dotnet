using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Models;

// CUSTOM: Renamed.
/// <summary> Represents information about a single available model entry. </summary>
[CodeGenType("Model")]
public partial class OpenAIModel
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    [CodeGenMember("Object")]
    private string Object { get; } = "model";

    // CUSTOM: Renamed.
    /// <summary> The Unix timestamp (in seconds) when the model was created. </summary>
    [CodeGenMember("Created")]
    public DateTimeOffset CreatedAt { get; }
}
