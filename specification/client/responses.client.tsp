import "../base/typespec/responses/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

namespace OpenAI.Responses;

@@clientName(OpenAI.Responses, "ResponsesClient", "csharp");
@@clientNamespace(OpenAI.Responses, "OpenAI.Responses", "csharp");
@@dynamicModel(OpenAI.Responses);

@@alternateType(TokenCountsResource.input_tokens, int32);
@@alternateType(ResponseCustomToolCallInputDeltaEvent.output_index, int32);
@@alternateType(ResponseCustomToolCallInputDoneEvent.output_index, int32);

@@alternateType(CreateResponse.conversation, `ConversationParam-2`);
@@alternateType(CreateResponse.`model`, string);
@@alternateType(CreateResponse.service_tier, DotNetResponseServiceTier);
@@alternateType(Response.`model`, string);
@@alternateType(Response.service_tier, DotNetResponseServiceTier);

// Do not generate convenience methods for these operations
@@convenientAPI(getInputTokenCounts, false);
@@convenientAPI(compactConversation, false);

@@clientName(getInputTokenCounts, "GetInputTokenCount");
@@clientName(compactConversation, "CompactResponse");

@@clientName(DotNetResponseItemCollectionOptions.after, "AfterId");
@@clientName(DotNetResponseItemCollectionOptions.before, "BeforeId");
@@clientName(DotNetResponseItemCollectionOptions.limit, "PageSizeLimit");

@@clientName(Includable.`computer_call_output.output.image_url`, "ComputerCallOutputImageUri");
@@clientName(Includable.`message.input_image.image_url`, "MessageInputImageUri");
@@clientName(Includable.`message.output_text.logprobs`, "MessageOutputTextLogProbabilities");

// ------------ ItemResources ------------
@@usage(ItemResource, Usage.input | Usage.output);
@@visibility(ItemResource.id, Lifecycle.Read);

@@clientName(CodeInterpreterToolImageOutput.url, "ImageUri");

@@alternateType(FunctionToolCallItemResource.arguments, unknown);
@@clientName(FunctionToolCallItemResource.name, "FunctionName");
@@clientName(FunctionToolCallItemResource.arguments, "FunctionArguments");

@@clientName(FunctionToolCallOutputItemResource.output, "FunctionOutput");

@@clientName(ReasoningItemResource.summary, "SummaryParts");

@@alternateType(MCPApprovalRequestItemResource.arguments, unknown);
@@clientName(MCPApprovalRequestItemResource.name, "ToolName");
@@clientName(MCPApprovalRequestItemResource.arguments, "ToolArguments");

@@clientName(MCPApprovalResponseItemResource.approve, "Approved");

@@alternateType(MCPCallItemResource.arguments, unknown);
@@alternateType(MCPCallItemResource.error, unknown | null);
@@clientName(MCPCallItemResource.name, "ToolName");
@@clientName(MCPCallItemResource.arguments, "ToolArguments");
@@clientName(MCPCallItemResource.output, "ToolOutput");

@@alternateType(MCPListToolsItemResource.error, unknown | null);
@@clientName(MCPListToolsItemResource.tools, "ToolDefinitions");

@@clientName(ImageGenToolCallItemResource.result, "ImageResultBytes");

// This is a data URL. Treat it as a string internally to avoid Uri limits.
@@alternateType(ItemContentInputImage.image_url, string | null); 
@@clientName(ItemContentInputImage.image_url, "ImageUri");

// ------------ Tools ------------

@@alternateType(CodeInterpreterTool.container, DotNetCodeInterpreterToolContainer);

@@clientName(FileSearchTool.max_num_results, "MaxResultCount");

@@clientName(CustomTool.name, "ToolName");
@@clientName(CustomTool.description, "ToolDescription");
@@clientName(CustomTool.format, "ToolFormat");

@@clientName(CustomToolCallItem.name, "ToolName");
@@alternateType(CustomToolCallOutputItem.output, ItemContent[]);

@@clientName(ResponseCustomToolCallInputDeltaEvent.delta, "InputDelta");

@@clientName(FunctionTool.name, "FunctionName");
@@clientName(FunctionTool.description, "FunctionDescription");
@@clientName(FunctionTool.parameters, "FunctionParameters");
@@clientName(FunctionTool.strict, "StrictModeEnabled");

@@alternateType(MCPTool.server_url, url);
@@alternateType(MCPTool.allowed_tools, MCPToolFilter | null);
@@clientName(MCPTool.server_url, "ServerUri");
@@clientName(MCPTool.authorization, "AuthorizationToken");

@@clientName(MCPToolFilter.read_only, "IsReadOnly");

@@clientName(Responses.listInputItems, "GetResponseInputItems");

// This is a data URL. Treat it as a string internally to avoid Uri limits.
@@alternateType(ImageGenToolInputImageMask.image_url, string);
@@clientName(ImageGenToolInputImageMask.image_url, "ImageUri");

// ------------ Streaming ------------

@@usage(ResponseStreamEvent, Usage.json);

@@alternateType(ResponseFunctionCallArgumentsDoneEvent.arguments, unknown);
@@clientName(ResponseFunctionCallArgumentsDoneEvent.name, "FunctionName");
@@clientName(ResponseFunctionCallArgumentsDoneEvent.arguments, "FunctionArguments");

@@alternateType(ResponseFunctionCallArgumentsDeltaEvent.delta, unknown);

@@alternateType(ResponseMcpCallArgumentsDoneEvent.arguments, unknown);
@@clientName(ResponseMcpCallArgumentsDoneEvent.arguments, "ToolArguments");

@@alternateType(ResponseMcpCallArgumentsDeltaEvent.delta, unknown);

@@clientName(ResponseImageGenerationCallPartialImageEvent.partial_image_b64, "PartialImageBytes");

@@clientName(ResponseOutputTextDeltaEvent.logprobs, "TokenLogProbabilities");
@@clientName(ResponseOutputTextDoneEvent.logprobs, "TokenLogProbabilities");


// ------------ Others ------------

@@clientName(CreateResponse.prompt_cache_retention, "PromptCacheRetentionPolicy");
@@clientName(Response.prompt_cache_retention, "PromptCacheRetentionPolicy");
@@clientName(PromptCacheRetention.`24h`, "Max24Hours");
@@clientName(ResponsesReasoningEffort.xhigh, "ExtraHigh");
@@clientName(ResponseUsage.input_tokens_details::type.cache_write_tokens, "CacheWriteTokenCount");
