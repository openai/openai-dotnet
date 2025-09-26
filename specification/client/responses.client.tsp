import "../base/typespec/responses/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@alternateType(CreateResponse.service_tier, DotNetResponseServiceTier);
@@alternateType(Response.service_tier, DotNetResponseServiceTier);

// ------------ ItemResources ------------
@@usage(ItemResource, Usage.input | Usage.output);
@@visibility(ItemResource.id, Lifecycle.Read);

@@visibility(CodeInterpreterToolCallItemResource.status, Lifecycle.Read);
@@clientName(CodeInterpreterToolImageOutput.url, "ImageUri");

@@visibility(ComputerToolCallItemResource.status, Lifecycle.Read);

@@visibility(ComputerToolCallOutputItemResource.status, Lifecycle.Read);

@@visibility(FileSearchToolCallItemResource.status, Lifecycle.Read);

@@visibility(FunctionToolCallItemResource.status, Lifecycle.Read);
@@alternateType(FunctionToolCallItemResource.arguments, unknown);
@@clientName(FunctionToolCallItemResource.name, "FunctionName");
@@clientName(FunctionToolCallItemResource.arguments, "FunctionArguments");

@@visibility(FunctionToolCallOutputItemResource.status, Lifecycle.Read);
@@clientName(FunctionToolCallOutputItemResource.output, "FunctionOutput");

@@visibility(ReasoningItemResource.status, Lifecycle.Read);
@@clientName(ReasoningItemResource.summary, "SummaryParts");

@@visibility(WebSearchToolCallItemResource.status, Lifecycle.Read);

@@visibility(ResponsesMessageItemResource.status, Lifecycle.Read);

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

@@visibility(ImageGenToolCallItemResource.status, Lifecycle.Read);
@@clientName(ImageGenToolCallItemResource.result, "GeneratedImageBytes");

// ------------ Tools ------------

@@clientName(FileSearchTool.max_num_results, "MaxResultCount");

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

// ------------ Streaming ------------

@@alternateType(ResponseFunctionCallArgumentsDoneEvent.arguments, unknown);
// @@clientName(ResponseFunctionCallArgumentsDoneEvent.arguments, "FunctionArguments");

@@alternateType(ResponseFunctionCallArgumentsDeltaEvent.delta, unknown);

@@alternateType(ResponseMCPCallArgumentsDoneEvent.arguments, unknown);
// @@clientName(ResponseMCPCallArgumentsDoneEvent.arguments, "ToolArguments");

@@alternateType(ResponseMCPCallArgumentsDeltaEvent.delta, unknown);

@@clientName(ResponseImageGenCallPartialImageEvent.partial_image_b64, "PartialImageBytes");