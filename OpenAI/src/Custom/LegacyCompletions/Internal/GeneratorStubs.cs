using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

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
}

[CodeGenType("CreateCompletionResponseChoice")]
internal partial class InternalCreateCompletionResponseChoice { }

[CodeGenType("CreateCompletionResponseChoiceFinishReason")]
internal readonly partial struct InternalCreateCompletionResponseChoiceFinishReason { }

[CodeGenType("CreateCompletionResponseChoiceLogprobs1")]
internal partial class InternalCreateCompletionResponseChoiceLogprobs { }

[CodeGenType("CreateCompletionResponseObject")]
internal readonly partial struct InternalCreateCompletionResponseObject { }

[CodeGenType("CompletionsCompletionUsage")]
internal partial class InternalCompletionsCompletionUsage {}

[CodeGenType("CompletionsCompletionUsageCompletionTokensDetails")]
internal partial class InternalCompletionsCompletionUsageCompletionTokensDetails {}

 [CodeGenType("CompletionsCompletionUsagePromptTokensDetails")]
 internal partial class InternalCompletionsCompletionUsagePromptTokensDetails {}

[CodeGenType("LegacyChatCompletionStreamOptions")]
internal partial class InternalLegacyChatCompletionStreamOptions {}
[CodeGenType("CompletionsError")] internal partial class InternalCompletionsError {}
[CodeGenType("CompletionsErrorResponse")] internal partial class InternalCompletionsErrorResponse {}
