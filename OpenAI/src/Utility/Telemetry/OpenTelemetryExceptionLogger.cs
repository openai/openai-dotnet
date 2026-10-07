using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace OpenAI.Telemetry;

internal static class OpenTelemetryExceptionLogger
{
    private static readonly EventId s_exceptionEvent = new(1, "gen_ai.client.operation.exception");

    public static void Record(ILogger logger, Exception exception)
    {
        if ((logger?.IsEnabled(LogLevel.Warning) != true) || (exception is null))
        {
            return;
        }

        // Passing null prevents logging bridges from exporting the exception message, stack, or response content.
        logger.Log(LogLevel.Warning, s_exceptionEvent,
            new KeyValuePair<string, object>[] { new("exception.type", exception.GetType().FullName) },
            null, static (_, _) => "GenAI operation failed.");
    }
}
