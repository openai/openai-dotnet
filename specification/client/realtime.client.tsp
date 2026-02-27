import "../base/typespec/realtime/main.tsp";
import "../base/typespec/realtime/models_server_events_GA.tsp";
import "../base/typespec/realtime/models_client_events_GA.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

// ------------ Events ------------

@@access(RealtimeServerEvent_GA, Access.public);
@@usage(RealtimeServerEvent_GA, Usage.output);

@@access(RealtimeClientEvent_GA, Access.public);
@@usage(RealtimeClientEvent_GA, Usage.input);

@@alternateType(RealtimeServerEventResponseFunctionCallArgumentsDelta_GA.delta, unknown);

@@alternateType(RealtimeServerEventResponseMCPCallArgumentsDelta_GA.delta, unknown);

// ------------ Items ------------

@@usage(RealtimeConversationItem_GA, Usage.input | Usage.output);

// ------------ Tools ------------

@@alternateType(MCPTool_GA.server_url, url);
@@alternateType(MCPTool_GA.allowed_tools, MCPToolFilter_GA | null);

// ------------ Response ------------

@@alternateType(RealtimeResponseCreateParams_GA.audio, RealtimeResponseAudio_GA);

// ------------ Custom Types ------------

// Different definitions have been converged into a single definition for simplicity.
@@alternateType(RealtimeServerEventError_GA.error, DotNetRealtimeError_GA);
@@alternateType(RealtimeServerEventConversationItemInputAudioTranscriptionFailed_GA.error, DotNetRealtimeError_GA);
@@alternateType(RealtimeMCPToolCall_GA.error, DotNetRealtimeError_GA);
@@alternateType(RealtimeResponseStatusDetails_GA.error, DotNetRealtimeError_GA);

// Common definition has been forked into a feature-specific variations to keep them separate.
@@alternateType(RealtimeServerEventConversationItemInputAudioTranscriptionCompleted_GA.logprobs, DotNetRealtimeLogProbsProperties[] | null);
@@alternateType(RealtimeServerEventConversationItemInputAudioTranscriptionDelta_GA.logprobs, DotNetRealtimeLogProbsProperties[] | null);

// ------------ JsonPatch ------------

@@dynamicModel(RealtimeCreateClientSecretRequest_GA);
@@dynamicModel(RealtimeCreateClientSecretResponse_GA);

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
@@dynamicModel(MCPToolRequireApproval_GA);

@@dynamicModel(DotNetRealtimeDefaultMaxOutputTokenCount_GA);
@@dynamicModel(DotNetRealtimeMaxOutputTokenCount_GA);