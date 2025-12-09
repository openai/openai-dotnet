using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.TestProxy.Admin;
using NUnit.Framework;
using System.ClientModel;
using System.Collections.Generic;
using static OpenAI.Tests.TestHelpers;

namespace OpenAI.Tests.Utility
{
    [LiveParallelizable(ParallelScope.Fixtures)]
    public class OpenAIRecordedTestBase : RecordedTestBase<OpenAITestEnvironment>
    {
        private static List<string> _unneededSanitizers = ["AZSDK1001", "AZSDK1002", "AZSDK1003", "AZSDK1004", "AZSDK1005", "AZSDK1006",
            "AZSDK1007", "AZSDK1008", "AZSDK2002", "AZSDK2006", "AZSDK2007", "AZSDK2008", "AZSDK2009", "AZSDK2010", "AZSDK2011", "AZSDK2012",
            "AZSDK2013", "AZSDK2014", "AZSDK2018", "AZSDK2019", "AZSDK2020", "AZSDK2021", "AZSDK2022", "AZSDK2023", "AZSDK2024", "AZSDK2025",
            "AZSDK202", "AZSDK2027", "AZSDK2028", "AZSDK2029", "AZSDK2031", "AZSDK3000", "AZSDK3001", "AZSDK3002", "AZSDK3004", "AZSDK3005",
            "AZSDK3006", "AZSDK3007", "AZSDK3008", "AZSDK3009", "AZSDK3010", "AZSDK3011", "AZSDK3012", "AZSDK3400", "AZSDK3401", "AZSDK3402",
            "AZSDK3403", "AZSDK3405", "AZSDK3406", "AZSDK3407", "AZSDK3408", "AZSDK3409", "AZSDK3410", "AZSDK3411", "AZSDK3412", "AZSDK3413",
            "AZSDK3414", "AZSDK3415", "AZSDK3416", "AZSDK3417", "AZSDK3418", "AZSDK3419", "AZSDK3420", "AZSDK3421", "AZSDK3422", "AZSDK3423",
            "AZSDK3424", "AZSDK3425", "AZSDK3426", "AZSDK3427", "AZSDK3428", "AZSDK3429", "AZSDK3430", "AZSDK3431", "AZSDK3432", "AZSDK3433",
            "AZSDK3435", "AZSDK3436", "AZSDK3437", "AZSDK3438", "AZSDK3439", "AZSDK3440", "AZSDK3441", "AZSDK3442", "AZSDK3443", "AZSDK3444",
            "AZSDK3445", "AZSDK3446", "AZSDK3447", "AZSDK3448", "AZSDK3449", "AZSDK3450", "AZSDK3451", "AZSDK3452", "AZSDK3453", "AZSDK3454",
            "AZSDK3455", "AZSDK3456", "AZSDK3457", "AZSDK3458", "AZSDK3459", "AZSDK3460", "AZSDK3461", "AZSDK3462", "AZSDK3463", "AZSDK3464",
            "AZSDK3465", "AZSDK3466", "AZSDK3467", "AZSDK3468", "AZSDK3469", "AZSDK3470", "AZSDK3471", "AZSDK3472", "AZSDK3473", "AZSDK3474",
            "AZSDK3475", "AZSDK3477", "AZSDK3478", "AZSDK3479", "AZSDK3481", "AZSDK3482", "AZSDK3483", "AZSDK3484", "AZSDK3485", "AZSDK3486",
            "AZSDK3487", "AZSDK3488", "AZSDK3489", "AZSDK3490", "AZSDK3491", "AZSDK3492", "AZSDK3493", "AZSDK3494", "AZSDK3495", "AZSDK3496",
            "AZSDK3497", "AZSDK3498"];

        public OpenAIRecordedTestBase(bool isAsync, RecordedTestMode? mode = null) : base(isAsync, mode)
        {
            SanitizedHeaders.Add("openai-organization");
            SanitizedHeaders.Add("openai-project");
            SanitizedHeaders.Add("X-Request-ID");
            SanitizedHeaders.Add("openai-processing-ms");
            SanitizedHeaders.Add("Date");
            JsonPathSanitizers.Add("$.system_fingerprint");

            SanitizersToRemove.AddRange(_unneededSanitizers);

            NormalizeMultipartContentDispositionHeaders = true;
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
