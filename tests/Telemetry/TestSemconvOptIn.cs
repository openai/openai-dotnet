using System;

namespace OpenAI.Tests.Telemetry;

/// <summary>
/// Test helper that enables the latest GenAI semantic conventions by setting
/// the OTEL_SEMCONV_STABILITY_OPT_IN environment variable.
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

    /// <summary>
    /// Enables the latest GenAI semantic conventions for the duration of the test.
    /// </summary>
    public static IDisposable EnableLatestGenAiSemconv()
    {
        return new TestSemconvOptIn("gen_ai_latest_experimental");
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(EnvVarName, _originalEnvValue);
    }
}
