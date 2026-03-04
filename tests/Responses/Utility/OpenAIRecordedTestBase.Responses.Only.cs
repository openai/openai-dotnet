using OpenAI.Responses;

namespace OpenAI.Tests.Utility
{
    public partial class OpenAIRecordedTestBase
    {
        internal T GetProxiedOpenAIClient<T>(string overrideModel = null, ResponsesClientOptions options = default) where T : class
        {
            options ??= new ResponsesClientOptions();

            ResponsesClientOptions instrumentedOptions = InstrumentClientOptions(options);
            T client = TestEnvironment.GetTestClient<T>(overrideModel, instrumentedOptions);
            T proxiedClient = CreateProxyFromClient<T>(client, null);

            return proxiedClient;
        }
    }
}
