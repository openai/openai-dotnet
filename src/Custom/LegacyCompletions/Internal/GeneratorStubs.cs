using OpenAI.Files;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.LegacyCompletions;

// CUSTOM: Made internal.

[CodeGenType("CreateCompletionRequest")]
internal partial class InternalCreateCompletionRequest
{
    public static implicit operator BinaryContent(InternalCreateCompletionRequest internalCreateCompletionRequest)
    {
        if (internalCreateCompletionRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(internalCreateCompletionRequest, ModelSerializationExtensions.WireOptions);
    }
}

[CodeGenType("CreateCompletionRequestModel")]
internal readonly partial struct InternalCreateCompletionRequestModel { }

[CodeGenType("CreateCompletionResponse")]
internal partial class InternalCreateCompletionResponse
{
    public static explicit operator InternalCreateCompletionResponse(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalCreateCompletionResponse(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}

[CodeGenType("CreateCompletionResponseChoice")]
internal partial class InternalCreateCompletionResponseChoice { }

[CodeGenType("CreateCompletionResponseChoiceFinishReason")]
internal readonly partial struct InternalCreateCompletionResponseChoiceFinishReason { }

[CodeGenType("CreateCompletionResponseChoiceLogprobs1")]
internal partial class InternalCreateCompletionResponseChoiceLogprobs { }

[CodeGenType("CreateCompletionResponseObject")]
internal readonly partial struct InternalCreateCompletionResponseObject { }