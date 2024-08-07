using System;

namespace OpenAI.Tests.Telemetry;

internal class TestAppContextSwitchHelper : IDisposable
{
    private const string OpenTelemetrySwitchName = "OpenAI.Experimental.EnableOpenTelemetry";

    private string _switchName;
    private TestAppContextSwitchHelper(string switchName)
    {
        _switchName = switchName;
        AppContext.SetSwitch(_switchName, true);
    }

    public static IDisposable EnableOpenTelemetry()
    {
        return new TestAppContextSwitchHelper(OpenTelemetrySwitchName);
    }

    public void Dispose()
    {
        AppContext.SetSwitch(_switchName, false);
    }
}
