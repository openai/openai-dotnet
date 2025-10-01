using System.ClientModel;

namespace OpenAI.Chat;

[CodeGenType("ChatCompletionFunctionCallOption")]
public partial class InternalChatCompletionFunctionCallOption { }

[CodeGenType("ChatCompletionMessageToolCallChunkType")]
public readonly partial struct InternalChatCompletionMessageToolCallChunkType { }

[CodeGenType("ChatCompletionNamedToolChoice")]
public partial class InternalChatCompletionNamedToolChoice { }

[CodeGenType("ChatCompletionNamedToolChoiceFunction")]
public partial class InternalChatCompletionNamedToolChoiceFunction { }

[CodeGenType("ChatCompletionNamedToolChoiceType")]
public readonly partial struct InternalChatCompletionNamedToolChoiceType { }

[CodeGenType("ChatCompletionRequestMessageContentPartImage")]
public partial class InternalChatCompletionRequestMessageContentPartImage { }

[CodeGenType("ChatCompletionRequestMessageContentPartText")]
public partial class InternalChatCompletionRequestMessageContentPartText { }

[CodeGenType("ChatCompletionResponseMessageFunctionCall")]
public partial class InternalChatCompletionResponseMessageFunctionCall { }

[CodeGenType("ChatCompletionResponseMessageRole")]
public readonly partial struct InternalChatCompletionResponseMessageRole { }

[CodeGenType("ChatCompletionStreamOptions")]
public partial class InternalChatCompletionStreamOptions { }

[CodeGenType("CreateChatCompletionFunctionResponse")]
public partial class InternalCreateChatCompletionFunctionResponse { }

[CodeGenType("CreateChatCompletionFunctionResponseChoice")]
public partial class InternalCreateChatCompletionFunctionResponseChoice { }

[CodeGenType("CreateChatCompletionFunctionResponseChoiceFinishReason")]
public readonly partial struct InternalCreateChatCompletionFunctionResponseChoiceFinishReason { }

[CodeGenType("CreateChatCompletionFunctionResponseObject")]
public readonly partial struct InternalCreateChatCompletionFunctionResponseObject { }

[CodeGenType("ChatCompletionRequestMessageContentPartRefusal")]
public partial class InternalChatCompletionRequestMessageContentPartRefusal { }

[CodeGenType("CreateChatCompletionRequestModel")]
public readonly partial struct InternalCreateChatCompletionRequestModel { }

[CodeGenType("UpdateChatCompletionRequest")]
public partial class InternalUpdateChatCompletionRequest
{
    public static implicit operator BinaryContent(InternalUpdateChatCompletionRequest internalUpdateChatCompletionRequest)
    {
        if (internalUpdateChatCompletionRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(internalUpdateChatCompletionRequest, ModelSerializationExtensions.WireOptions);
    }
}

[CodeGenType("CreateChatCompletionRequestToolChoice")]
public readonly partial struct InternalCreateChatCompletionRequestToolChoice { }

[CodeGenType("CreateChatCompletionResponseChoice")]
public partial class InternalCreateChatCompletionResponseChoice { }

[CodeGenType("CreateChatCompletionResponseChoiceLogprobs1")]
public partial class InternalCreateChatCompletionResponseChoiceLogprobs { }

[CodeGenType("CreateChatCompletionResponseObject")]
public readonly partial struct InternalCreateChatCompletionResponseObject { }

[CodeGenType("CreateChatCompletionResponseServiceTier")]
public readonly partial struct InternalCreateChatCompletionResponseServiceTier { }

[CodeGenType("CreateChatCompletionStreamResponseChoiceFinishReason")]
public readonly partial struct InternalCreateChatCompletionStreamResponseChoiceFinishReason { }

[CodeGenType("CreateChatCompletionStreamResponseChoiceLogprobs1")]
public partial class InternalCreateChatCompletionStreamResponseChoiceLogprobs { }

[CodeGenType("CreateChatCompletionStreamResponseObject")]
public readonly partial struct InternalCreateChatCompletionStreamResponseObject { }

[CodeGenType("CreateChatCompletionStreamResponseServiceTier")]
public readonly partial struct InternalCreateChatCompletionStreamResponseServiceTier { }

[CodeGenType("CreateChatCompletionStreamResponseUsage")]
public partial class InternalCreateChatCompletionStreamResponseUsage { }

[CodeGenType("CreateChatCompletionRequestModality")]
public readonly partial struct InternalCreateChatCompletionRequestModality { }

[CodeGenType("ChatCompletionRequestMessageContentPartAudio")]
public partial class InternalChatCompletionRequestMessageContentPartAudio { }

[CodeGenType("ChatCompletionRequestMessageContentPartAudioInputAudio")]
public partial class InternalChatCompletionRequestMessageContentPartAudioInputAudio { }

[CodeGenType("ChatOutputPredictionType")]
public readonly partial struct InternalChatOutputPredictionKind { }

[CodeGenType("UnknownChatOutputPrediction")]
public partial class InternalUnknownChatOutputPrediction { }

[CodeGenType("CreateChatCompletionRequestWebSearchOptionsUserLocation1Type")]
public readonly partial struct InternalCreateChatCompletionRequestWebSearchOptionsUserLocation1Type { }

[CodeGenType("ChatCompletionResponseMessageAnnotationType")]
public readonly partial struct InternalChatCompletionResponseMessageAnnotationType { }

[CodeGenType("ChatCompletionRequestMessageContentPartFile")]
public partial class InternalChatCompletionRequestMessageContentPartFile { }

[CodeGenType("CreateChatCompletionRequestWebSearchOptionsUserLocation1")]
public partial class InternalCreateChatCompletionRequestWebSearchOptionsUserLocation1 { }

[CodeGenType("ChatCompletionResponseMessageAnnotationUrlCitation")]
public partial class InternalChatCompletionResponseMessageAnnotationUrlCitation { }

[CodeGenType("DotNetChatResponseFormatJsonSchemaJsonSchema")]
public partial class InternalDotNetChatResponseFormatJsonSchemaJsonSchema { }

[CodeGenType("UnknownChatCompletionRequestMessageContentPart")]
public partial class InternalUnknownChatCompletionRequestMessageContentPart { }

[CodeGenType("ChatCompletionListObject")] public readonly partial struct InternalChatCompletionListObject {}
[CodeGenType("ChatCompletionDeletedObject")] public readonly partial struct InternalChatCompletionDeletedObject {}
[CodeGenType("ChatCompletionMessageListObject")] public readonly partial struct InternalChatCompletionMessageListObject {}
[CodeGenType("ChatCompletionList")] public partial class InternalChatCompletionList {}
[CodeGenType("ChatCompletionMessageList")] public partial class InternalChatCompletionMessageList {}