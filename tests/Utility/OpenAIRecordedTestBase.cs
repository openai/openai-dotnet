using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.TestProxy.Admin;
using NUnit.Framework;

namespace OpenAI.Tests.Utility
{
    [LiveParallelizable(ParallelScope.Fixtures)]
    public partial class OpenAIRecordedTestBase : RecordedTestBase<OpenAITestEnvironment>
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
            JsonPathSanitizers.Add("$..encrypted_content");
        }
    }
}
