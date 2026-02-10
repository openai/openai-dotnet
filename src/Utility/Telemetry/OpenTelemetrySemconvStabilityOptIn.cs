using System;

namespace OpenAI.Telemetry;

/// <summary>
/// Reads the OTEL_SEMCONV_STABILITY_OPT_IN environment variable to determine
/// which version of the OpenTelemetry GenAI semantic conventions to emit.
///
/// See https://github.com/open-telemetry/semantic-conventions for details on
/// the transition policy for existing instrumentations.
/// </summary>
internal static class OpenTelemetrySemconvStabilityOptIn
{
    private const string EnvVarName = "OTEL_SEMCONV_STABILITY_OPT_IN";
    private const string GenAiLatestExperimentalValue = "gen_ai_latest_experimental";

    /// <summary>
    /// When true, the instrumentation emits the latest experimental GenAI
    /// semantic conventions.
    /// When false (default), the instrumentation continues to emit v1.27.0 conventions.
    /// </summary>
    public static bool IsLatestGenAiSemconvEnabled => ParseOptIn();

    private static bool ParseOptIn()
    {
        string value = Environment.GetEnvironmentVariable(EnvVarName);
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        // The env var is a comma-separated list of opt-in values.
        foreach (string part in value.Split(','))
        {
            if (part.Trim().Equals(GenAiLatestExperimentalValue, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
