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
@@visibility(ChatCompletionStreamResponseDelta.tool_calls, Lifecycle.Read);

@@clientName(Chat.createChatCompletion, "CompleteChat");
