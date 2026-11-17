import "../base/typespec/realtime/main.tsp";
import "../base/typespec/realtime/models_server_events_GA.tsp";
import "../base/typespec/realtime/models_client_events_GA.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@alternateType(RealtimeCreateClientSecretRequestExpiresAfter_GA.seconds, int32);
@@alternateType(RealtimeSessionCreateRequestGAAudioOutput_GA.speed, float32);
@@alternateType(RealtimeTruncationRetentionRatio_GA.retention_ratio, float32);
@@alternateType(TokenLimits_GA.post_instructions, int32);
@@alternateType(RealtimeServerVADTurnDetection_GA.threshold, float32);
@@alternateType(RealtimeMCPProtocolError_GA.code, int32);
@@alternateType(RealtimeMCPHTTPError_GA.code, int32);
@@alternateType(DotNetRealtimeMaxOutputTokenCount_GA.custom_max_output_token_count, int32);
@@alternateType(RealtimeClientEventConversationItemTruncate_GA.content_index, int32);
@@alternateType(RealtimeServerEventRateLimitsUpdatedRateLimits_GA.limit, int32);
@@alternateType(RealtimeServerEventRateLimitsUpdatedRateLimits_GA.remaining, int32);
@@alternateType(LogProbProperties_GA.logprob, float32);
@@alternateType(LogProbProperties_GA.bytes, int32[]);
@@alternateType(TranscriptTextUsageTokens_GA.input_tokens, int32);
@@alternateType(TranscriptTextUsageTokens_GA.output_tokens, int32);
@@alternateType(TranscriptTextUsageTokens_GA.total_tokens, int32);
@@alternateType(TranscriptTextUsageTokensInputTokenDetails_GA.text_tokens, int32);
@@alternateType(TranscriptTextUsageTokensInputTokenDetails_GA.audio_tokens, int32);
@@alternateType(RealtimeResponseUsage_GA.total_tokens, int32);
@@alternateType(RealtimeResponseUsage_GA.input_tokens, int32);
@@alternateType(RealtimeResponseUsage_GA.output_tokens, int32);
@@alternateType(RealtimeResponseUsageInputTokenDetails_GA.cached_tokens, int32);
@@alternateType(RealtimeResponseUsageInputTokenDetails_GA.text_tokens, int32);
@@alternateType(RealtimeResponseUsageInputTokenDetails_GA.image_tokens, int32);
@@alternateType(RealtimeResponseUsageInputTokenDetails_GA.audio_tokens, int32);
@@alternateType(RealtimeResponseUsageInputTokenDetailsCachedTokensDetails_GA.text_tokens, int32);
@@alternateType(RealtimeResponseUsageInputTokenDetailsCachedTokensDetails_GA.image_tokens, int32);
@@alternateType(RealtimeResponseUsageInputTokenDetailsCachedTokensDetails_GA.audio_tokens, int32);
@@alternateType(RealtimeResponseUsageOutputTokenDetails_GA.text_tokens, int32);
@@alternateType(RealtimeResponseUsageOutputTokenDetails_GA.audio_tokens, int32);
@@alternateType(RealtimeServerEventConversationItemInputAudioTranscriptionCompleted_GA.content_index, int32);
@@alternateType(RealtimeServerEventConversationItemInputAudioTranscriptionDelta_GA.content_index, int32);
@@alternateType(RealtimeServerEventConversationItemInputAudioTranscriptionSegment_GA.content_index, int32);
@@alternateType(RealtimeServerEventConversationItemInputAudioTranscriptionFailed_GA.content_index, int32);
@@alternateType(RealtimeServerEventConversationItemTruncated_GA.content_index, int32);
@@alternateType(RealtimeServerEventResponseOutputItemAdded_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseOutputItemDone_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseContentPartAdded_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseContentPartAdded_GA.content_index, int32);
@@alternateType(RealtimeServerEventResponseContentPartDone_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseContentPartDone_GA.content_index, int32);
@@alternateType(RealtimeServerEventResponseOutputTextDelta_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseOutputTextDelta_GA.content_index, int32);
@@alternateType(RealtimeServerEventResponseOutputTextDone_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseOutputTextDone_GA.content_index, int32);
@@alternateType(RealtimeServerEventResponseOutputAudioTranscriptDelta_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseOutputAudioTranscriptDelta_GA.content_index, int32);
@@alternateType(RealtimeServerEventResponseOutputAudioTranscriptDone_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseOutputAudioTranscriptDone_GA.content_index, int32);
@@alternateType(RealtimeServerEventResponseOutputAudioDelta_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseOutputAudioDelta_GA.content_index, int32);
@@alternateType(RealtimeServerEventResponseOutputAudioDone_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseOutputAudioDone_GA.content_index, int32);
@@alternateType(RealtimeServerEventResponseFunctionCallArgumentsDelta_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseFunctionCallArgumentsDone_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseMCPCallArgumentsDelta_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseMCPCallArgumentsDone_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseMCPCallInProgress_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseMCPCallCompleted_GA.output_index, int32);
@@alternateType(RealtimeServerEventResponseMCPCallFailed_GA.output_index, int32);

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

@access(Access.public)
@usage(Usage.output)
model DotNetRealtimeLogProbsProperties {
  /** The token that was used to generate the log probability. */
  token: string;

  /** The log probability of the token. */
  logprob: float32;

  /** The bytes that were used to generate the log probability. */
  bytes: int32[];
}

// Different definitions have been converged into a single definition for simplicity.
@@alternateType(RealtimeServerEventError_GA.error, DotNetRealtimeError_GA);
@@alternateType(RealtimeServerEventConversationItemInputAudioTranscriptionFailed_GA.error, DotNetRealtimeError_GA);
@@alternateType(RealtimeMCPToolCall_GA.error, DotNetRealtimeError_GA);
@@alternateType(RealtimeResponseStatusDetails_GA.error, DotNetRealtimeError_GA);

// Common definition has been forked into a feature-specific variations to keep them separate.
@@alternateType(RealtimeServerEventConversationItemInputAudioTranscriptionCompleted_GA.logprobs, DotNetRealtimeLogProbsProperties[] | null);
@@alternateType(RealtimeServerEventConversationItemInputAudioTranscriptionDelta_GA.logprobs, DotNetRealtimeLogProbsProperties[] | null);

// ------------ JsonPatch ------------

@@dynamicModel(RealtimeCreateClientSecretRequest_GA, "csharp");
@@dynamicModel(RealtimeCreateClientSecretResponse_GA, "csharp");

@@dynamicModel(RealtimeFunctionTool_GA, "csharp");
@@dynamicModel(MCPTool_GA, "csharp");

@@dynamicModel(RealtimeSessionCreateRequestGA_GA, "csharp");
@@dynamicModel(RealtimeTranscriptionSessionCreateRequestGA_GA, "csharp");

@@dynamicModel(RealtimeSessionCreateResponseGA_GA, "csharp");
@@dynamicModel(RealtimeTranscriptionSessionCreateResponseGA_GA, "csharp");

@@dynamicModel(RealtimeAudioFormatsAudioPcm_GA, "csharp");
@@dynamicModel(RealtimeAudioFormatsAudioPcmu_GA, "csharp");
@@dynamicModel(RealtimeAudioFormatsAudioPcma_GA, "csharp");

@@dynamicModel(RealtimeServerEvent_GA, "csharp");
@@dynamicModel(RealtimeClientEvent_GA, "csharp");

@@dynamicModel(DotNetRealtimeSessionCreateRequestGADefaultTracing_GA, "csharp");
@@dynamicModel(DotNetRealtimeTracing_GA, "csharp");

@@dynamicModel(DotNetRealtimeDefaultTruncation_GA, "csharp");
@@dynamicModel(DotNetRealtimeTruncation_GA, "csharp");
@@dynamicModel(RealtimeTruncationRetentionRatio_GA, "csharp");

@@dynamicModel(DotNetRealtimeDefaultToolChoice_GA, "csharp");
@@dynamicModel(DotNetRealtimeToolChoice_GA, "csharp");
@@dynamicModel(ToolChoiceFunction_GA, "csharp");
@@dynamicModel(ToolChoiceMCP_GA, "csharp");

@@dynamicModel(DotNetRealtimeDefaultToolCallApprovalPolicy_GA, "csharp");
@@dynamicModel(DotNetRealtimeToolCallApprovalPolicy_GA, "csharp");
@@dynamicModel(MCPToolRequireApproval_GA, "csharp");

@@dynamicModel(DotNetRealtimeDefaultMaxOutputTokenCount_GA, "csharp");
@@dynamicModel(DotNetRealtimeMaxOutputTokenCount_GA, "csharp");