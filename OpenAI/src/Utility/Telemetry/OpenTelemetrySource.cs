using OpenAI.Chat;
using OpenAI.Responses;
using System;

namespace OpenAI.Telemetry;

internal class OpenTelemetrySource
{
    private const string ChatOperationName = "chat";
    private readonly bool IsOTelEnabled = AppContextSwitchHelper
        .GetConfigValue("OpenAI.Experimental.EnableOpenTelemetry", "OPENAI_EXPERIMENTAL_ENABLE_OPEN_TELEMETRY");

    private readonly string _providerAttributeKey;
    private readonly string _serverAddress;
    private readonly int _serverPort;
    private readonly string _model;
    private readonly bool _useLatestSemanticConventions;

    public OpenTelemetrySource(string model, Uri endpoint)
    {
        _useLatestSemanticConventions = OpenTelemetrySemanticConventionStabilityOptIn.IsLatestGenAiSemanticConventionEnabled;
        _providerAttributeKey = _useLatestSemanticConventions
            ? OpenTelemetryConstants.GenAiProviderNameKey
            : OpenTelemetryConstants.GenAiSystemKey;
        _serverAddress = endpoint.Host;
        _serverPort = endpoint.Port;
        _model = model;
    }

    public OpenTelemetrySource(Uri endpoint)
        : this(null, endpoint)
    {
    }

    public OpenTelemetryScope StartChatScope(ChatCompletionOptions completionsOptions)
    {
        return IsOTelEnabled
            ? OpenTelemetryScope.StartChat(_model, ChatOperationName, _serverAddress, _serverPort, completionsOptions, _providerAttributeKey)
            : null;
    }

    public OpenTelemetryScope StartResponsesScope(CreateResponseOptions options)
    {
        return IsOTelEnabled
            ? OpenTelemetryScope.StartResponses(options?.Model, ChatOperationName, _serverAddress, _serverPort, options, _providerAttributeKey, _useLatestSemanticConventions)
            : null;
    }
}
