namespace OpenAI.Custom.Common.Instrumentation;

internal class Constants
{
    public const string ErrorTypeKey = "error.type";
    public const string ServerAddressKey = "server.address";
    public const string ServerPortKey = "server.port";

    public const string GenAiClientOperationDurationMetricName = "gen_ai.client.operation.duration";
    public const string GenAiClientTokenUsageMetricName = "gen_ai.client.token.usage";

    public const string GenAiOperationNameKey = "gen_ai.operation.name";

    public const string GenAiRequestMaxTokensKey = "gen_ai.request.max_tokens";
    public const string GenAiRequestModelKey = "gen_ai.request.model";
    public const string GenAiRequestTemperatureKey = "gen_ai.request.temperature";
    public const string GenAiRequestTopPKey = "gen_ai.request.top_p";

    public const string GenAiResponseIdKey = "gen_ai.response.id";
    public const string GenAiResponseFinishReasonKey = "gen_ai.response.finish_reason";
    public const string GenAiResponseModelKey = "gen_ai.response.model";

    public const string GenAiSystemKey = "gen_ai.system";
    public const string GenAiSystemValue = "openai";

    public const string GenAiTokenTypeKey = "gen_ai.token_type";

    public const string GenAiUsageInputTokensKey = "gen_ai.usage.input_tokens";
    public const string GenAiUsageOutputTokensKey = "gen_ai.usage.output_tokens";
}
