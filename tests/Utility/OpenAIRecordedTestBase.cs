using Microsoft.ClientModel.TestFramework;
using NUnit.Framework;
using System.ClientModel;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Utility
{
    [LiveParallelizable(ParallelScope.Fixtures)]
    public class OpenAIRecordedTestBase : RecordedTestBase<OpenAITestEnvironment>
    {
        public OpenAIRecordedTestBase(bool isAsync, RecordedTestMode? mode = null) : base(isAsync, mode)
        {
            SanitizedHeaders.Add("openai-organization");
            SanitizedHeaders.Add("openai-project");
            SanitizedHeaders.Add("X-Request-ID");
            JsonPathSanitizers.Add("$.system_fingerprint");
        }

        internal T GetProxiedOpenAIClient<T>(TestScenario scenario, string overrideModel = null, bool excludeDumpPolicy = false, OpenAIClientOptions options = default) where T : class
        {
            options ??= new OpenAIClientOptions();
            OpenAIClientOptions instrumentedOptions = InstrumentClientOptions(options);
            T client = GetTestClient<T>(scenario, overrideModel, excludeDumpPolicy, options: instrumentedOptions, credential: GetTestApiKeyCredential());
            T proxiedClient = CreateProxyFromClient<T>(client, null);
            return proxiedClient;
        }

        public ApiKeyCredential GetTestApiKeyCredential() => new(TestEnvironment.OpenApiKey);

        public OpenAIClient GetProxiedTestTopLevelClient() => GetProxiedOpenAIClient<OpenAIClient>(TestScenario.TopLevel);
    }
}
