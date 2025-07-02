using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.Models;

/// <summary>
/// Represents information about a single available model entry.
/// </summary>
[CodeGenType("Model")]
public partial class OpenAIModel
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    /// <summary> The object type, which is always "model". </summary>
    private string Object { get; } = "model";

    // CUSTOM: Renamed.
    /// <summary> The Unix timestamp (in seconds) when the model was created. </summary>
    [CodeGenMember("Created")]
    public DateTimeOffset CreatedAt { get; }

    internal static OpenAIModel FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeOpenAIModel(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
