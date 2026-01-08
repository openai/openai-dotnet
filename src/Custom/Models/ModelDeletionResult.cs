using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Models;

// CUSTOM: Renamed.
/// <summary> The result of the model deletion. </summary>
[CodeGenType("DeleteModelResponse")]
public partial class ModelDeletionResult
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    [CodeGenMember("Object")]
    private string Object { get; } = "model";

    // CUSTOM: Renamed.
    /// <summary> The ID of the model. </summary>
    [CodeGenMember("Id")]
    public string ModelId { get; }
}