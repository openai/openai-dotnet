using OpenAI.Chat;
using System;

namespace OpenAI.Telemetry;

internal class OpenTelemetrySource
{
    private const string ChatOperationName = "chat";
    private readonly string _serverAddress;
    private readonly int _serverPort;
    private readonly string _model;

    public OpenTelemetrySource(string model, Uri endpoint)
    {
        _serverAddress = endpoint.Host;
        _serverPort = endpoint.Port;
        _model = model;
    }

    public OpenTelemetryScope StartChatScope(ChatCompletionOptions completionsOptions)
    {
        return OpenTelemetryScope.StartChat(_model, ChatOperationName, _serverAddress, _serverPort, completionsOptions);
    }
}
