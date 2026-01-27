using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.TestProxy.Admin;
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
            // Normalizes filepaths used as filenames in Content-Disposition headers, so record/playback works across different OSes
            CustomSanitizers.Add(new ContentDispositionFilePathSanitizer());

            // Turn off default sanitizers to improve performance in large recordings (audio/image specifically)
            // This turns off all sanitizers exception Authorization headers, so we have to be very careful to explicitly sanitize
            // all other sensitive information
            UseDefaultSanitizers = false;

            SanitizedHeaders.Add("openai-organization");
            SanitizedHeaders.Add("openai-project");
            SanitizedHeaders.Add("X-Request-ID");
            SanitizedHeaders.Add("openai-processing-ms");
            SanitizedHeaders.Add("Date");
            SanitizedHeaders.Add("Set-Cookie");
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
