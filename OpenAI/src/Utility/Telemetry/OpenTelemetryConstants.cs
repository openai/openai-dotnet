namespace OpenAI.Telemetry;

internal class OpenTelemetryConstants
{
    // OpenTelemetry GenAI semantic conventions.

    // Default (v1.27.0): https://github.com/open-telemetry/semantic-conventions/tree/v1.27.0/docs/gen-ai

    // Set OTEL_SEMCONV_STABILITY_OPT_IN=gen_ai_latest_experimental to use the latest conventions
    // (https://github.com/open-telemetry/semantic-conventions-genai/tree/main/docs/gen-ai).

    public const string ErrorTypeKey = "error.type";
    public const string ServerAddressKey = "server.address";
    public const string ServerPortKey = "server.port";

    public const string GenAiClientOperationDurationMetricName = "gen_ai.client.operation.duration";
    public const string GenAiClientTokenUsageMetricName = "gen_ai.client.token.usage";
    public const string GenAiClientInferenceUsageInputTokensMetricName = "gen_ai.client.inference.usage.input_tokens";
    public const string GenAiClientInferenceUsageOutputTokensMetricName = "gen_ai.client.inference.usage.output_tokens";
    public const string GenAiClientInferenceUsageCacheReadInputTokensMetricName = "gen_ai.client.inference.usage.cache_read.input_tokens";
    public const string GenAiClientInferenceUsageCacheWriteInputTokensMetricName = "gen_ai.client.inference.usage.cache_write.input_tokens";
    public const string GenAiClientInferenceUsageReasoningOutputTokensMetricName = "gen_ai.client.inference.usage.reasoning.output_tokens";
    public const string GenAiClientInferenceOperationInputTokensMetricName = "gen_ai.client.inference.operation.input_tokens";
    public const string GenAiClientInferenceOperationOutputTokensMetricName = "gen_ai.client.inference.operation.output_tokens";

    public const string GenAiOperationNameKey = "gen_ai.operation.name";
    public const string GenAiConversationCompactedKey = "gen_ai.conversation.compacted";
    public const string GenAiConversationIdKey = "gen_ai.conversation.id";
    public const string GenAiOutputTypeKey = "gen_ai.output.type";

    public const string GenAiRequestMaxTokensKey = "gen_ai.request.max_tokens";
    public const string GenAiRequestFrequencyPenaltyKey = "gen_ai.request.frequency_penalty";
    public const string GenAiRequestPresencePenaltyKey = "gen_ai.request.presence_penalty";
    public const string GenAiRequestSeedKey = "gen_ai.request.seed";
    public const string GenAiRequestModelKey = "gen_ai.request.model";
    public const string GenAiRequestPreviousResponseIdKey = "gen_ai.request.previous_response.id";
    public const string GenAiRequestReasoningLevelKey = "gen_ai.request.reasoning.level";
    public const string GenAiRequestTemperatureKey = "gen_ai.request.temperature";
    public const string GenAiRequestTopPKey = "gen_ai.request.top_p";

    public const string GenAiResponseIdKey = "gen_ai.response.id";
    public const string GenAiResponseFinishReasonKey = "gen_ai.response.finish_reasons";
    public const string GenAiResponseModelKey = "gen_ai.response.model";
    public const string GenAiResponseTimeToFirstChunkKey = "gen_ai.response.time_to_first_chunk";

    // v1.27.0: gen_ai.system
    public const string GenAiSystemKey = "gen_ai.system";
    public const string GenAiSystemValue = "openai";

    // Latest (replaces gen_ai.system): gen_ai.provider.name
    public const string GenAiProviderNameKey = "gen_ai.provider.name";

    public const string GenAiTokenTypeKey = "gen_ai.token.type";
    public const string GenAiTokenModalityKey = "gen_ai.token.modality";

    public const string GenAiUsageInputTokensKey = "gen_ai.usage.input_tokens";
    public const string GenAiUsageOutputTokensKey = "gen_ai.usage.output_tokens";
    public const string GenAiUsageCacheReadInputTokensKey = "gen_ai.usage.cache_read.input_tokens";
    public const string GenAiUsageCacheWriteInputTokensKey = "gen_ai.usage.cache_write.input_tokens";
    public const string GenAiUsageReasoningOutputTokensKey = "gen_ai.usage.reasoning.output_tokens";
    public const string GenAiUsageAudioInputTokensKey = "gen_ai.usage.audio.input_tokens";
    public const string GenAiUsageAudioOutputTokensKey = "gen_ai.usage.audio.output_tokens";

    public const string OpenAiApiTypeKey = "openai.api.type";
    public const string OpenAiApiTypeResponsesValue = "responses";
    public const string OpenAiApiTypeChatCompletionsValue = "chat_completions";
    public const string OpenAiRequestServiceTierKey = "openai.request.service_tier";
    public const string OpenAiResponseServiceTierKey = "openai.response.service_tier";
    public const string OpenAiResponseSystemFingerprintKey = "openai.response.system_fingerprint";
}
