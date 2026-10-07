using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using OpenAI.Responses;
using System;
using System.ClientModel.Primitives;
using System.Diagnostics;

namespace OpenAI.Telemetry;

internal class OpenTelemetrySource
{
    private const string ChatOperationName = "chat";
    private readonly bool _isOTelEnabled = AppContextSwitchHelper
        .GetConfigValue("OpenAI.Experimental.EnableOpenTelemetry", "OPENAI_EXPERIMENTAL_ENABLE_OPEN_TELEMETRY");

    private readonly string _providerAttributeKey;
    private readonly string _serverAddress;
    private readonly int _serverPort;
    private readonly string _model;
    private readonly bool _useLatestSemanticConventions;
    private readonly ILogger _exceptionLogger;

    public OpenTelemetrySource(string model, Uri endpoint, ClientLoggingOptions loggingOptions = null)
        : this(model, endpoint, loggingOptions, "OpenAI.ChatClient.Operations")
    {
    }

    private OpenTelemetrySource(string model, Uri endpoint, ClientLoggingOptions loggingOptions, string loggerCategory)
    {
        _useLatestSemanticConventions = OpenTelemetrySemanticConventionStabilityOptIn.IsLatestGenAiSemanticConventionEnabled;
        _providerAttributeKey = _useLatestSemanticConventions
            ? OpenTelemetryConstants.GenAiProviderNameKey
            : OpenTelemetryConstants.GenAiSystemKey;
        _serverAddress = endpoint.Host;
        _serverPort = endpoint.Port;
        _model = model;

        if ((_isOTelEnabled) && (_useLatestSemanticConventions) && (loggingOptions?.EnableLogging != false))
        {
            _exceptionLogger = loggingOptions?.LoggerFactory?.CreateLogger(loggerCategory);
        }
    }

    public OpenTelemetrySource(Uri endpoint, ClientLoggingOptions loggingOptions = null)
        : this(null, endpoint, loggingOptions, "OpenAI.ResponsesClient.Operations")
    {
    }

    public OpenTelemetryScope StartChatScope(ChatCompletionOptions completionsOptions)
    {
        return _isOTelEnabled
            ? OpenTelemetryScope.StartChat(_model, ChatOperationName, _serverAddress, _serverPort, completionsOptions, _providerAttributeKey, _useLatestSemanticConventions, exceptionLogger: _exceptionLogger)
            : null;
    }

    public OpenTelemetryScope StartResponsesScope(CreateResponseOptions options)
    {
        return _isOTelEnabled
            ? OpenTelemetryScope.StartResponses(options?.Model, ChatOperationName, _serverAddress, _serverPort, options, _providerAttributeKey, _useLatestSemanticConventions, exceptionLogger: _exceptionLogger)
            : null;
    }

    public SseLifecycle<StreamingResponseUpdate> StartResponsesStreamingScope(CreateResponseOptions options)
    {
        if (!_isOTelEnabled)
        {
            return null;
        }

        var previous = Activity.Current;

        try
        {
            return OpenTelemetryScope.StartResponses(options?.Model, ChatOperationName, _serverAddress, _serverPort,
                options, _providerAttributeKey, _useLatestSemanticConventions, streaming: true, exceptionLogger: _exceptionLogger)?.CreateStreamingLifecycle();
        }
        finally
        {
            Activity.Current = previous;
        }
    }
}
