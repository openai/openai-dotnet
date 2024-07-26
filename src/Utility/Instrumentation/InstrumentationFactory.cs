using OpenAI.Chat;
using System;

namespace OpenAI.Instrumentation;

internal class InstrumentationFactory
{
    private const string ChatOperationName = "chat";
    private readonly bool IsInstrumentationEnabled = AppContextSwitchHelper
        .GetConfigValue("OpenAI.Experimental.EnableInstrumentation", "OPENAI_EXPERIMENTAL_ENABLE_INSTRUMENTATION");

    private readonly string _serverAddress;
    private readonly int _serverPort;
    private readonly string _model;

    public InstrumentationFactory(string model, Uri endpoint)
    {
        _serverAddress = endpoint.Host;
        _serverPort = endpoint.Port;
        _model = model;
    }

    public InstrumentationScope StartChatScope(ChatCompletionOptions completionsOptions)
    {
        return IsInstrumentationEnabled
            ? InstrumentationScope.StartChat(_model, ChatOperationName, _serverAddress, _serverPort, completionsOptions)
            : null;
    }

    internal static class AppContextSwitchHelper
    {
        /// <summary>
        /// Determines if either an AppContext switch or its corresponding Environment Variable is set
        /// </summary>
        /// <param name="appContexSwitchName">Name of the AppContext switch.</param>
        /// <param name="environmentVariableName">Name of the Environment variable.</param>
        /// <returns>If the AppContext switch has been set, returns the value of the switch.
        /// If the AppContext switch has not been set, returns the value of the environment variable.
        /// False if neither is set.
        /// </returns>
        public static bool GetConfigValue(string appContexSwitchName, string environmentVariableName)
        {
            // First check for the AppContext switch, giving it priority over the environment variable.
            if (AppContext.TryGetSwitch(appContexSwitchName, out bool value))
            {
                return value;
            }
            // AppContext switch wasn't used. Check the environment variable.
            string envVar = Environment.GetEnvironmentVariable(environmentVariableName);
            if (envVar != null && (envVar.Equals("true", StringComparison.OrdinalIgnoreCase) || envVar.Equals("1")))
            {
                return true;
            }

            // Default to false.
            return false;
        }
    }

}
