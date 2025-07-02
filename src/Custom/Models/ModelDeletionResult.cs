using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Models;

[CodeGenType("DeleteModelResponse")]
public partial class ModelDeletionResult
{
    // CUSTOM: Renamed.
    [CodeGenMember("Id")]
    public string ModelId { get; }

    // CUSTOM: Made internal.
    /// <summary> The object type, which is always `model`. </summary>
    [CodeGenMember("Object")]
    internal string Object { get; } = "model";

    internal static ModelDeletionResult FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeModelDeletionResult(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
