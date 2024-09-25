using OpenAI.Chat;
using System;

namespace OpenAI.Telemetry;

internal class OpenTelemetrySource(string model, Uri endpoint)
{
    private const string ChatOperationName = "chat";
    private readonly bool IsOTelEnabled = AppContextSwitchHelper
        .GetConfigValue("OpenAI.Experimental.EnableOpenTelemetry", "OPENAI_EXPERIMENTAL_ENABLE_OPEN_TELEMETRY");

    public OpenTelemetryScope StartChatScope(ChatCompletionOptions completionsOptions)
    {
        return IsOTelEnabled
            ? OpenTelemetryScope.StartChat(model, ChatOperationName, endpoint.Host, endpoint.Port, completionsOptions)
            : null;
    }

}
