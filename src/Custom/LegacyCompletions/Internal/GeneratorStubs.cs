using OpenAI.Files;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;

namespace OpenAI.LegacyCompletions;

// CUSTOM: Made internal.

[CodeGenType("CreateCompletionRequest")]
public partial class InternalCreateCompletionRequest
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
public readonly partial struct InternalCreateCompletionRequestModel { }

[CodeGenType("CreateCompletionResponse")]
public partial class InternalCreateCompletionResponse
{
}

[CodeGenType("CreateCompletionResponseChoice")]
public partial class InternalCreateCompletionResponseChoice { }

[CodeGenType("CreateCompletionResponseChoiceFinishReason")]
public readonly partial struct InternalCreateCompletionResponseChoiceFinishReason { }

[CodeGenType("CreateCompletionResponseChoiceLogprobs1")]
public partial class InternalCreateCompletionResponseChoiceLogprobs { }

[CodeGenType("CreateCompletionResponseObject")]
public readonly partial struct InternalCreateCompletionResponseObject { }