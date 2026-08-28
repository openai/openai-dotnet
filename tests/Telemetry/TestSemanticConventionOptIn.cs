using System;

namespace OpenAI.Tests.Telemetry;

/// <summary>
/// Temporarily sets the OTEL_SEMCONV_STABILITY_OPT_IN environment variable.
/// Restores the original value on dispose.
/// Must be used before constructing <see cref="OpenAI.Telemetry.OpenTelemetrySource"/>.
/// </summary>
internal class TestSemanticConventionOptIn : IDisposable
{
    private const string EnvVarName = "OTEL_SEMCONV_STABILITY_OPT_IN";

    private readonly string _originalEnvValue;

    private TestSemanticConventionOptIn(string envValue)
    {
        _originalEnvValue = Environment.GetEnvironmentVariable(EnvVarName);
        Environment.SetEnvironmentVariable(EnvVarName, envValue);
    }

    public static IDisposable SetLatestGenAiSemanticConvention(bool enabled)
    {
        return new TestSemanticConventionOptIn(enabled ? "gen_ai_latest_experimental" : null);
    }

    public static IDisposable SetSemanticConventionOptIn(string value)
    {
        return new TestSemanticConventionOptIn(value);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(EnvVarName, _originalEnvValue);
    }
}
