using System;

namespace OpenAI.Tests.Telemetry;

/// <summary>
/// Temporarily sets the OTEL_SEMCONV_STABILITY_OPT_IN environment variable.
/// Restores the original value on dispose.
/// Must be used before constructing <see cref="OpenAI.Telemetry.OpenTelemetrySource"/>.
/// </summary>
internal class TestSemconvOptIn : IDisposable
{
    private const string EnvVarName = "OTEL_SEMCONV_STABILITY_OPT_IN";

    private readonly string _originalEnvValue;

    private TestSemconvOptIn(string envValue)
    {
        _originalEnvValue = Environment.GetEnvironmentVariable(EnvVarName);
        Environment.SetEnvironmentVariable(EnvVarName, envValue);
    }

    public static IDisposable SetLatestGenAiSemconv(bool enabled)
    {
        return new TestSemconvOptIn(enabled ? "gen_ai_latest_experimental" : null);
    }

    public static IDisposable SetSemconvOptIn(string value)
    {
        return new TestSemconvOptIn(value);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(EnvVarName, _originalEnvValue);
    }
}
