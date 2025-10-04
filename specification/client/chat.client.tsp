import "../base/typespec/chat/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

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

@@alternateType(CreateChatCompletionRequest.service_tier, DotNetChatServiceTier);
@@alternateType(CreateChatCompletionResponse.service_tier, DotNetChatServiceTier);
@@alternateType(CreateChatCompletionStreamResponse.service_tier, DotNetChatServiceTier);

// ------------ JsonPatch ------------

// models.tsp models
@@dynamicModel(CreateChatCompletionRequest);
@@dynamicModel(CreateChatCompletionResponse);
@@dynamicModel(ChatCompletionTool);
@@dynamicModel(ChatCompletionNamedToolChoice);
@@dynamicModel(ChatCompletionRequestMessageContentPartText);
@@dynamicModel(ChatCompletionRequestMessageContentPartImage);
@@dynamicModel(ChatCompletionRequestMessageContentPartRefusal);
@@dynamicModel(ChatCompletionRequestMessageContentPartFile);
@@dynamicModel(ChatCompletionMessageToolCall);
@@dynamicModel(ChatCompletionRequestMessage);
@@dynamicModel(ChatCompletionRequestMessageContentPartAudio);
@@dynamicModel(ChatCompletionRequestSystemMessage);
@@dynamicModel(ChatCompletionRequestDeveloperMessage);
@@dynamicModel(ChatCompletionRequestUserMessage);
@@dynamicModel(ChatCompletionRequestAssistantMessage);
@@dynamicModel(ChatCompletionRequestToolMessage);
@@dynamicModel(ChatCompletionRequestFunctionMessage);
@@dynamicModel(ChatCompletionMessageToolCalls);
@@dynamicModel(ChatCompletionModalities);
@@dynamicModel(ChatCompletionResponseMessage);
@@dynamicModel(ChatCompletionTokenLogprob);
@@dynamicModel(ChatCompletionFunctionCallOption);
@@dynamicModel(ChatCompletionFunctions);
@@dynamicModel(ChatCompletionStreamResponseDelta);
@@dynamicModel(ChatCompletionMessageToolCallChunk);
@@dynamicModel(CreateChatCompletionStreamResponse);
@@dynamicModel(ChatOutputPredictionContent);
@@dynamicModel(ChatCompletionDeleted);
@@dynamicModel(ChatCompletionList);
@@dynamicModel(ChatCompletionMessageList);
// custom.tsp models
@@dynamicModel(ChatOutputPrediction);
@@dynamicModel(ChatCompletionFunctionChoice);
@@dynamicModel(ChatCompletionToolChoice);
@@dynamicModel(ChatCompletionRequestMessageContentPart);
@@dynamicModel(ChatCompletionMessageAudioChunk);
// client/models/chat.models.tsp models
@@dynamicModel(ChatCompletionCollectionOrder);
@@dynamicModel(ChatCompletionCollectionOptions);
@@dynamicModel(ChatCompletionMessageCollectionOrder);
@@dynamicModel(ChatCompletionMessageCollectionOptions);
// client/models/common.models.tsp models
@@dynamicModel(DotNetChatResponseFormat);
@@dynamicModel(DotNetChatResponseFormatText);
@@dynamicModel(DotNetChatResponseFormatJsonObject);
@@dynamicModel(DotNetChatResponseFormatJsonSchema);
@@dynamicModel(DotNetChatVoiceIds);
@@dynamicModel(DotNetChatServiceTier);