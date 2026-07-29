using Microsoft.ClientModel.TestFramework;
using Microsoft.ClientModel.TestFramework.TestProxy.Admin;
using NUnit.Framework;
using OpenAI.Realtime;
using OpenAI.Responses;

namespace OpenAI.Tests.Utility
{
    [LiveParallelizable(ParallelScope.Fixtures)]
    public class OpenAIRecordedTestBase : RecordedTestBase<OpenAITestEnvironment>
    {
        public OpenAIRecordedTestBase(bool isAsync, RecordedTestMode? mode = null) : base(isAsync, mode)
        {
            // Normalizes filepaths used as filenames in Content-Disposition headers, so record/playback works across different OSes
            CustomSanitizers.Add(new ContentDispositionFilePathSanitizer());

            // The SDK platform metadata headers describe the machine that issued the request, so their values vary by
            // recording machine and runtime. Removing them means recordings never capture them and playback requests
            // are stripped of them before matching, which keeps existing recordings valid without re-recording.
            // Coverage for these headers lives in mock-based unit tests instead.
            CustomSanitizers.Add(new RemoveHeaderSanitizer(
                new RemoveHeaderSanitizerBody(string.Join(",", PlatformTelemetryHeaderNames))));

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

        /// <summary>
        /// The SDK platform metadata headers that the client adds to every request.
        /// </summary>
        internal static readonly string[] PlatformTelemetryHeaderNames =
        [
            "X-Stainless-Lang",
            "X-Stainless-Package-Version",
            "X-Stainless-Runtime",
            "X-Stainless-Runtime-Version",
            "X-Stainless-OS",
            "X-Stainless-Arch",
        ];

        internal T GetProxiedOpenAIClient<T>(string overrideModel = null, OpenAIClientOptions options = default) where T : class
        {
            options ??= new OpenAIClientOptions();

            OpenAIClientOptions instrumentedOptions = InstrumentClientOptions(options);
            T client = TestEnvironment.GetTestClient<T>(overrideModel, instrumentedOptions);
            T proxiedClient = CreateProxyFromClient<T>(client, null);

            return proxiedClient;
        }

        internal ResponsesClient GetProxiedResponsesClient(ResponsesClientOptions options = default)
        {
            options ??= new ResponsesClientOptions();

            ResponsesClientOptions instrumentedOptions = InstrumentClientOptions(options);
            ResponsesClient client = TestEnvironment.GetTestResponsesClient(instrumentedOptions);
            ResponsesClient proxiedClient = CreateProxyFromClient<ResponsesClient>(client, null);

            return proxiedClient;
        }

        internal RealtimeClient GetProxiedRealtimeClient(RealtimeClientOptions options = default)
        {
            options ??= new RealtimeClientOptions();

            RealtimeClientOptions instrumentedOptions = InstrumentClientOptions(options);
            RealtimeClient client = TestEnvironment.GetTestRealtimeClient(instrumentedOptions);
            RealtimeClient proxiedClient = CreateProxyFromClient<RealtimeClient>(client, null);

            return proxiedClient;
        }
    }
}
