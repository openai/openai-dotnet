import "../base/typespec/realtime/main.tsp";
import "../base/typespec/realtime/models_server_events_GA.tsp";
import "../base/typespec/realtime/models_client_events_GA.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

// ------------ Temporary ------------

@@access(RealtimeCreateClientSecretResponse, Access.public);
@@usage(RealtimeCreateClientSecretResponse, Usage.output);

@@access(RealtimeServerEvent_GA, Access.public);
@@usage(RealtimeServerEvent_GA, Usage.output);

@@access(RealtimeClientEvent_GA, Access.public);
@@usage(RealtimeClientEvent_GA, Usage.input);

// ------------ Converged Types ------------

@@alternateType(RealtimeServerEventError_GA.error, DotNetRealtimeError_GA);
@@alternateType(RealtimeServerEventConversationItemInputAudioTranscriptionFailed_GA.error, DotNetRealtimeError_GA);
@@alternateType(RealtimeMCPToolCall_GA.error, DotNetRealtimeError_GA);
@@alternateType(RealtimeResponseStatusDetails_GA.error, DotNetRealtimeError_GA);

// ------------ Items ------------

@@usage(RealtimeConversationItem_GA, Usage.input | Usage.output);

@@alternateType(RealtimeConversationItemFunctionCall_GA.arguments, unknown);
@@clientName(RealtimeConversationItemFunctionCall_GA.name, "FunctionName");
@@clientName(RealtimeConversationItemFunctionCall_GA.arguments, "FunctionArguments");

@@clientName(RealtimeConversationItemFunctionCallOutput_GA.output, "FunctionOutput");

@@alternateType(RealtimeMCPApprovalRequest_GA.arguments, unknown);
@@clientName(RealtimeMCPApprovalRequest_GA.name, "ToolName");
@@clientName(RealtimeMCPApprovalRequest_GA.arguments, "ToolArguments");

@@clientName(RealtimeMCPApprovalResponse_GA.approve, "Approved");

@@alternateType(RealtimeMCPToolCall_GA.arguments, unknown);
// @@alternateType(RealtimeMCPToolCall_GA.error, unknown | null);
@@clientName(RealtimeMCPToolCall_GA.name, "ToolName");
@@clientName(RealtimeMCPToolCall_GA.arguments, "ToolArguments");
@@clientName(RealtimeMCPToolCall_GA.output, "ToolOutput");

// @@alternateType(MCPListToolsItemResource.error, unknown | null);
@@clientName(MCPListToolsItemResource.tools, "ToolDefinitions");

// ------------ Tools ------------

@@alternateType(MCPTool_GA.server_url, url);
@@alternateType(MCPTool_GA.allowed_tools, MCPToolFilter_GA | null);
@@clientName(MCPTool_GA.server_url, "ServerUri");
@@clientName(MCPTool_GA.authorization, "AuthorizationToken");
@@clientName(MCPToolFilter_GA.read_only, "IsReadOnly");

// ------------ Response ------------

@@alternateType(RealtimeResponseCreateParams_GA.audio, RealtimeResponseAudio_GA);


// ------------ Server Events ------------

@@alternateType(RealtimeServerEventResponseFunctionCallArgumentsDelta_GA.delta, unknown);

@@alternateType(RealtimeServerEventResponseFunctionCallArgumentsDone_GA.arguments, unknown);
@@clientName(RealtimeServerEventResponseFunctionCallArgumentsDone_GA.arguments, "FunctionArguments");

@@alternateType(RealtimeServerEventResponseMCPCallArgumentsDelta_GA.delta, unknown);

@@alternateType(RealtimeServerEventResponseMCPCallArgumentsDone_GA.arguments, unknown);
@@clientName(RealtimeServerEventResponseMCPCallArgumentsDone_GA.arguments, "ToolArguments");

// ------------ JsonPatch ------------

// models.tsp
@@dynamicModel(RealtimeCreateClientSecretRequest_GA);
@@dynamicModel(RealtimeCreateClientSecretResponse_GA);

@@dynamicModel(DotNetRealtimeSessionCreateRequestGADefaultTracing_GA);
@@dynamicModel(DotNetRealtimeTracing_GA);

@@dynamicModel(DotNetRealtimeDefaultTruncation_GA);
@@dynamicModel(DotNetRealtimeTruncation_GA);
@@dynamicModel(RealtimeTruncationRetentionRatio_GA);

@@dynamicModel(DotNetRealtimeDefaultToolChoice_GA);
@@dynamicModel(DotNetRealtimeToolChoice_GA);
@@dynamicModel(ToolChoiceFunction_GA);
@@dynamicModel(ToolChoiceMCP_GA);

@@dynamicModel(DotNetRealtimeDefaultToolCallApprovalPolicy_GA);
@@dynamicModel(DotNetRealtimeToolCallApprovalPolicy_GA);

@@dynamicModel(RealtimeFunctionTool_GA);
@@dynamicModel(MCPTool_GA);

@@dynamicModel(RealtimeSessionCreateRequestGA_GA);
@@dynamicModel(RealtimeTranscriptionSessionCreateRequestGA_GA);

@@dynamicModel(RealtimeSessionCreateResponseGA_GA);
@@dynamicModel(RealtimeTranscriptionSessionCreateResponseGA_GA);

@@dynamicModel(RealtimeAudioFormatsAudioPcm_GA);
@@dynamicModel(RealtimeAudioFormatsAudioPcmu_GA);
@@dynamicModel(RealtimeAudioFormatsAudioPcma_GA);

@@dynamicModel(RealtimeServerEvent_GA);
@@dynamicModel(RealtimeClientEvent_GA);