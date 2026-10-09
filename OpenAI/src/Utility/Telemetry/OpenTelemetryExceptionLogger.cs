using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace OpenAI.Telemetry;

internal static class OpenTelemetryExceptionLogger
{
    private static readonly EventId s_exceptionEvent = new(1, "gen_ai.client.operation.exception");

    public static ILogger Create(ILoggerFactory factory, string categoryName)
    {
        try
        {
            return factory?.CreateLogger(categoryName);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static bool IsEnabled(ILogger logger)
    {
        try
        {
            return logger?.IsEnabled(LogLevel.Warning) == true;
        }
        catch (Exception)
        {
            // Instrumentation must not make an operation fail because of a logging provider.
            return false;
        }
    }

    public static void Record(ILogger logger, Exception exception)
    {
        if ((exception is null) || (!IsEnabled(logger)))
        {
            return;
        }

        try
        {
            // Passing null prevents logging bridges from exporting the exception message, stack, or response content.
            logger.Log(LogLevel.Warning, s_exceptionEvent,
                new KeyValuePair<string, object>[] { new("exception.type", exception.GetType().FullName) },
                null, static (_, _) => "GenAI operation failed.");
        }
        catch (Exception)
        {
            // The operation exception must remain the caller-visible failure.
        }
    }
}
