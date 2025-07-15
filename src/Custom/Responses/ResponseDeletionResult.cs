using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("DeleteResponseResponse")]
public partial class ResponseDeletionResult
{
    // CUSTOM: Made internal.
    [CodeGenMember("Object")]
    internal string Object { get; } = "response.deleted";

    internal static ResponseDeletionResult FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeResponseDeletionResult(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}