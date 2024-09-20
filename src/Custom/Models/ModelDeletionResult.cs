namespace OpenAI.Models;

[CodeGenModel("DeleteModelResponse")]
public partial class ModelDeletionResult
{
    [CodeGenMember("Id")]
    public string ModelId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `model`. </summary>
    [CodeGenMember("Object")]
    internal InternalDeleteModelResponseObject Object { get; } = InternalDeleteModelResponseObject.Model;
}
