using Microsoft.ClientModel.TestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    public class OpenAIRecordedTestBase : RecordedTestBase<OpenAITestEnvironment>
    {
        public OpenAIRecordedTestBase(bool isAsync, RecordedTestMode? mode = null) : base(isAsync, mode)
        {
            SanitizedHeaders.Add("openai-organization");
            SanitizedHeaders.Add("openai-project");
            SanitizedHeaders.Add("X-Request-ID");
        }

        internal T GetProxiedOpenAIClient<T>(TestScenario scenario, string overrideModel = null, OpenAIClientOptions options = default) where T : class
        {
            options ??= new OpenAIClientOptions();
            OpenAIClientOptions instrumentedOptions = InstrumentClientOptions(options);
            T client = TestHelpers.GetTestClient<T>(scenario, overrideModel, instrumentedOptions);
            T proxiedClient = CreateProxyFromClient<T>(client, null);
            return proxiedClient;
        }
    }
}
