namespace OpenAI.Tests.Utility
{
    public partial class OpenAIRecordedTestBase
    {
        internal T GetProxiedOpenAIClient<T>(string overrideModel = null, OpenAIClientOptions options = default) where T : class
        {
            options ??= new OpenAIClientOptions();

            OpenAIClientOptions instrumentedOptions = InstrumentClientOptions(options);
            T client = TestEnvironment.GetTestClient<T>(overrideModel, instrumentedOptions);
            T proxiedClient = CreateProxyFromClient<T>(client, null);

            return proxiedClient;
        }
    }
}
