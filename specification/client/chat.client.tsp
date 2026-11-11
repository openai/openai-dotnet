import "../base/typespec/chat/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@access(ChatCompletionFunctionChoice, Access.public);
@@usage(ChatCompletionFunctionChoice, Usage.input);

@@access(ChatCompletionToolChoice, Access.public);
@@usage(ChatCompletionToolChoice, Usage.input);

@@access(CreateChatCompletionStreamResponse, Access.public);
@@usage(CreateChatCompletionStreamResponse, Usage.output);

@@visibility(ChatCompletionResponseMessage.tool_calls, Lifecycle.Read);
@@visibility(ChatCompletionResponseMessage.annotations, Lifecycle.Read);
@@visibility(ChatCompletionStreamResponseDelta.tool_calls, Lifecycle.Read);

@@clientName(Chat.createChatCompletion, "CompleteChat");

@@clientName(ChatCompletionCollectionOptions.after, "AfterId");
@@clientName(ChatCompletionCollectionOptions.limit, "PageSizeLimit");

@@clientName(ChatCompletionMessageCollectionOptions.after, "AfterId");
@@clientName(ChatCompletionMessageCollectionOptions.limit, "PageSizeLimit");
@@clientName(ChatReasoningEffort.xhigh, "ExtraHigh");

@@alternateType(CreateChatCompletionRequest.service_tier, DotNetChatServiceTier);
@@alternateType(CreateChatCompletionResponse.service_tier, DotNetChatServiceTier);
@@alternateType(CreateChatCompletionStreamResponse.service_tier, DotNetChatServiceTier);

// ------------ JsonPatch ------------

// models.tsp models
@@dynamicModel(CreateChatCompletionRequest, "csharp");
@@dynamicModel(CreateChatCompletionResponse, "csharp");
@@dynamicModel(ChatCompletionTool, "csharp");
@@dynamicModel(ChatCompletionNamedToolChoice, "csharp");
@@dynamicModel(ChatCompletionRequestMessageContentPartText, "csharp");
@@dynamicModel(ChatCompletionRequestMessageContentPartImage, "csharp");
@@dynamicModel(ChatCompletionRequestMessageContentPartRefusal, "csharp");
@@dynamicModel(ChatCompletionRequestMessageContentPartFile, "csharp");
@@dynamicModel(ChatCompletionMessageToolCall, "csharp");
@@dynamicModel(ChatCompletionRequestMessage, "csharp");
@@dynamicModel(ChatCompletionRequestMessageContentPartAudio, "csharp");
@@dynamicModel(ChatCompletionRequestSystemMessage, "csharp");
@@dynamicModel(ChatCompletionRequestDeveloperMessage, "csharp");
@@dynamicModel(ChatCompletionRequestUserMessage, "csharp");
@@dynamicModel(ChatCompletionRequestAssistantMessage, "csharp");
@@dynamicModel(ChatCompletionRequestToolMessage, "csharp");
@@dynamicModel(ChatCompletionRequestFunctionMessage, "csharp");
@@dynamicModel(ChatCompletionMessageToolCalls, "csharp");
@@dynamicModel(ChatCompletionModalities, "csharp");
@@dynamicModel(ChatCompletionResponseMessage, "csharp");
@@dynamicModel(ChatCompletionTokenLogprob, "csharp");
@@dynamicModel(ChatCompletionFunctionCallOption, "csharp");
@@dynamicModel(ChatCompletionFunctions, "csharp");
@@dynamicModel(ChatCompletionStreamResponseDelta, "csharp");
@@dynamicModel(ChatCompletionMessageToolCallChunk, "csharp");
@@dynamicModel(CreateChatCompletionStreamResponse, "csharp");
@@dynamicModel(ChatOutputPredictionContent, "csharp");
@@dynamicModel(ChatCompletionDeleted, "csharp");
@@dynamicModel(ChatCompletionList, "csharp");
@@dynamicModel(ChatCompletionMessageList, "csharp");
// custom.tsp models
@@dynamicModel(ChatOutputPrediction, "csharp");
@@dynamicModel(ChatCompletionFunctionChoice, "csharp");
@@dynamicModel(ChatCompletionToolChoice, "csharp");
@@dynamicModel(ChatCompletionRequestMessageContentPart, "csharp");
@@dynamicModel(ChatCompletionMessageAudioChunk, "csharp");
// client/models/chat.models.tsp models
@@dynamicModel(ChatCompletionCollectionOrder, "csharp");
@@dynamicModel(ChatCompletionCollectionOptions, "csharp");
@@dynamicModel(ChatCompletionMessageCollectionOrder, "csharp");
@@dynamicModel(ChatCompletionMessageCollectionOptions, "csharp");
// client/models/common.models.tsp models
@@dynamicModel(DotNetChatResponseFormat, "csharp");
@@dynamicModel(DotNetChatResponseFormatText, "csharp");
@@dynamicModel(DotNetChatResponseFormatJsonObject, "csharp");
@@dynamicModel(DotNetChatResponseFormatJsonSchema, "csharp");
@@dynamicModel(DotNetChatVoiceIds, "csharp");
@@dynamicModel(DotNetChatServiceTier, "csharp");
