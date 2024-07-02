using OpenAI.Chat;
using System;

namespace OpenAI.Custom.Common.Instrumentation;

internal class InstrumentationFactory
{
    private readonly string _serverAddress;
    private readonly int _serverPort;
    private readonly string _model;

    private const string ChatOperationName = "chat";

    public InstrumentationFactory(string model, Uri endpoint)
    {
        _serverAddress = endpoint.Host;
        _serverPort = endpoint.Port;
        _model = model;
    }

    public InstrumentationScope StartChatScope(ChatCompletionOptions completionsOptions)
    {
        return InstrumentationScope.StartChat(_model, ChatOperationName, _serverAddress, _serverPort, completionsOptions);
    }
}