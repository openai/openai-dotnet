using System;

namespace OpenAI;

internal static class AppContextSwitchHelper
{
    /// <summary>
    /// Determines if either an AppContext switch or its corresponding Environment Variable is set
    /// </summary>
    /// <param name="appContextSwitchName">Name of the AppContext switch.</param>
    /// <param name="environmentVariableName">Name of the Environment variable.</param>
    /// <returns>If the AppContext switch has been set, returns the value of the switch.
    /// If the AppContext switch has not been set, returns the value of the environment variable.
    /// False if neither is set.
    /// </returns>
    public static bool GetConfigValue(string appContextSwitchName, string environmentVariableName)
    {
        // First check for the AppContext switch, giving it priority over the environment variable.
        bool isSwitchSet = AppContext.TryGetSwitch(appContextSwitchName, out bool switchValue);
        string environmentValue = Environment.GetEnvironmentVariable(environmentVariableName);
        return GetConfigValue(isSwitchSet, switchValue, environmentValue);
    }

    internal static bool GetConfigValue(bool isSwitchSet, bool switchValue, string environmentValue)
    {
        if (isSwitchSet)
        {
            return switchValue;
        }

        if ((environmentValue != null)
            && ((environmentValue.Equals("true", StringComparison.OrdinalIgnoreCase)) || (environmentValue.Equals("1"))))
        {
            return true;
        }

        return false;
    }
}
