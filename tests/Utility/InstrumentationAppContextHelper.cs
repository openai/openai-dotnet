using System;

namespace OpenAI.Tests.Utility;

internal class InstrumentationAppContextHelper : IDisposable
{
    private const string SwitchName = "OpenAI.Experimental.EnableInstrumentation";
    private InstrumentationAppContextHelper()
    {
        AppContext.SetSwitch(SwitchName, true);
    }

    public static IDisposable EnableInstrumentation()
    {
        return new InstrumentationAppContextHelper();
    }

    public void Dispose()
    {
        AppContext.SetSwitch(SwitchName, false);
    }
}
