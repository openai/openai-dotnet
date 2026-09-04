using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenAI.Models;

namespace OpenAI.Tests.Miscellaneous;

[Category("Smoke")]
public class RetryAfterTests
{
    public static IEnumerable<TestCaseData> ExcessiveDelays()
    {
        foreach (bool useAsync in new[] { false, true })
            foreach (int status in new[] { 429, 503 })
                foreach (string hint in new[] { "2147484", "2147483647", "2147483648", new string('9', 400), "Thu, 01 Jan 2099 00:00:00 GMT" })
                    yield return new TestCaseData(useAsync, status, hint);
    }

    [TestCaseSource(nameof(ExcessiveDelays))]
    public void ExcessiveDelayReturnsOriginalErrorWithoutWaiting(bool useAsync, int status, string hint)
    {
        using SyntheticHandler handler = new(status, hint);
        using HttpClient http = new(handler);
        RecordingRetryPolicy policy = new();
        OpenAIModelClient client = CreateClient(http, policy);

        ClientResultException error = Assert.ThrowsAsync<ClientResultException>(async () => { await Send(client, useAsync); });

        Assert.That(handler.Calls, Is.EqualTo(1));
        Assert.That(policy.Waits, Is.Empty);
        Assert.That(error.Status, Is.EqualTo(status));
        Assert.That(error.GetRawResponse().Content.ToString(), Is.EqualTo(SyntheticHandler.ErrorBody));
        Assert.That(error.GetRawResponse().Headers.TryGetValue("Retry-After", out string actualHint), Is.True);
        Assert.That(actualHint, Is.EqualTo(hint));
        Assert.That(error.GetRawResponse().Headers.TryGetValue("x-request-id", out string requestId), Is.True);
        Assert.That(requestId, Is.EqualTo("synthetic-request"));
    }

    [TestCase(false, 429, "fr-FR")]
    [TestCase(true, 503, "fr-FR")]
    [TestCase(false, 429, "ar-SA")]
    [TestCase(true, 503, "ar-SA")]
    [TestCase(false, 429, "th-TH")]
    [TestCase(true, 503, "th-TH")]
    public void ExcessiveHttpDateIsIndependentOfCurrentCulture(bool useAsync, int status, string culture)
    {
        CultureInfo previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            ExcessiveDelayReturnsOriginalErrorWithoutWaiting(useAsync, status, "Thu, 01 Jan 2099 00:00:00 GMT");
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [TestCase(false, 429, "ar-SA")]
    [TestCase(true, 503, "ar-SA")]
    [TestCase(false, 429, "th-TH")]
    [TestCase(true, 503, "th-TH")]
    public void UnrecognizedFutureHttpDateReturnsOriginalError(bool useAsync, int status, string culture)
    {
        CultureInfo previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            string hint = DateTimeOffset.UtcNow.AddMinutes(1).ToString("R", CultureInfo.InvariantCulture);
            ExcessiveDelayReturnsOriginalErrorWithoutWaiting(useAsync, status, hint);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    [SetCulture("en-US")]
    public void SupportedFutureHttpDateKeepsExistingRetryPolicy(bool useAsync)
    {
        string hint = DateTimeOffset.UtcNow.AddMinutes(1).ToString("R", CultureInfo.InvariantCulture);
        DateTimeOffset retryAt = DateTimeOffset.Parse(hint, CultureInfo.InvariantCulture);
        using SyntheticHandler handler = new(429, hint, succeedOnRetry: true);
        using HttpClient http = new(handler);
        RecordingRetryPolicy policy = new();
        OpenAIModelClient client = CreateClient(http, policy);
        DateTimeOffset startedAt = DateTimeOffset.UtcNow;

        ClientResult result = Send(client, useAsync).GetAwaiter().GetResult();

        Assert.That(result.GetRawResponse().Status, Is.EqualTo(200));
        Assert.That(handler.Calls, Is.EqualTo(2));
        Assert.That(policy.Waits, Has.Count.EqualTo(1));
        Assert.That(policy.Waits[0], Is.InRange(retryAt - DateTimeOffset.UtcNow, retryAt - startedAt));
    }

    [TestCase(false, "2147483647")]
    [TestCase(true, "2147483647")]
    [TestCase(false, "2147483648")]
    [TestCase(true, "2147483648")]
    public void DefaultPolicyReturnsStatusInsteadOfTimerOrOverflowFailure(bool useAsync, string hint)
    {
        using SyntheticHandler handler = new(503, hint);
        using HttpClient http = new(handler);
        OpenAIModelClient client = CreateClient(http);

        ClientResultException error = Assert.ThrowsAsync<ClientResultException>(async () => { await Send(client, useAsync); });

        Assert.That(error.Status, Is.EqualTo(503));
        Assert.That(handler.Calls, Is.EqualTo(1));
    }

    [TestCase(false, "90", 90)]
    [TestCase(true, "90", 90)]
    [TestCase(false, "2147483", 2147483)]
    [TestCase(true, "2147483", 2147483)]
    [TestCase(false, null, 0.8)]
    [TestCase(true, null, 0.8)]
    [TestCase(false, "invalid", 0.8)]
    [TestCase(true, "invalid", 0.8)]
    public void AllowedDelayKeepsExistingRetryPolicy(bool useAsync, string hint, double expectedSeconds)
    {
        using SyntheticHandler handler = new(429, hint, succeedOnRetry: true);
        using HttpClient http = new(handler);
        RecordingRetryPolicy policy = new();
        OpenAIModelClient client = CreateClient(http, policy);

        ClientResult result = Send(client, useAsync).GetAwaiter().GetResult();

        Assert.That(result.GetRawResponse().Status, Is.EqualTo(200));
        Assert.That(handler.Calls, Is.EqualTo(2));
        Assert.That(policy.Waits, Is.EqualTo(new[] { TimeSpan.FromSeconds(expectedSeconds) }));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void AllowedDelayStillHonorsCancellation(bool useAsync)
    {
        using CancellationTokenSource cancellation = new();
        using SyntheticHandler handler = new(429, "90");
        using HttpClient http = new(handler);
        RecordingRetryPolicy policy = new() { CancelDuringWait = cancellation };
        OpenAIModelClient client = CreateClient(http, policy);

        Assert.ThrowsAsync<OperationCanceledException>(async () => { await Send(client, useAsync, cancellation.Token); });

        Assert.That(handler.Calls, Is.EqualTo(1));
        Assert.That(policy.Waits, Is.EqualTo(new[] { TimeSpan.FromSeconds(90) }));
    }

    private static OpenAIModelClient CreateClient(HttpClient http, PipelinePolicy retryPolicy = null) => new(
        new ApiKeyCredential("synthetic-key"),
        new OpenAIClientOptions
        {
            Endpoint = new Uri("https://retry.test"),
            Transport = new HttpClientPipelineTransport(http),
            RetryPolicy = retryPolicy,
        });

    private static async Task<ClientResult> Send(OpenAIModelClient client, bool useAsync, CancellationToken cancellationToken = default)
    {
        RequestOptions options = new() { CancellationToken = cancellationToken };
        return useAsync ? await client.GetModelAsync("test", options) : client.GetModel("test", options);
    }

    private sealed class RecordingRetryPolicy : ClientRetryPolicy
    {
        public RecordingRetryPolicy() : base(maxRetries: 1) { }
        public List<TimeSpan> Waits { get; } = [];
        public CancellationTokenSource CancelDuringWait { get; init; }

        protected override void Wait(TimeSpan time, CancellationToken cancellationToken)
        {
            Waits.Add(time);
            CancelDuringWait?.Cancel();
            cancellationToken.ThrowIfCancellationRequested();
        }

        protected override Task WaitAsync(TimeSpan time, CancellationToken cancellationToken)
        {
            Wait(time, cancellationToken);
            return Task.CompletedTask;
        }
    }

    private sealed class SyntheticHandler(int status, string hint, bool succeedOnRetry = false) : HttpMessageHandler
    {
        public const string ErrorBody = "{\"error\":{\"message\":\"Synthetic error\",\"type\":\"synthetic_error\",\"code\":\"synthetic_code\"}}";
        public int Calls { get; private set; }
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken) => Respond(request);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(Respond(request));

        private HttpResponseMessage Respond(HttpRequestMessage request)
        {
            Assert.That(request.RequestUri.Host, Is.EqualTo("retry.test"));
            Calls++;
            bool success = succeedOnRetry && Calls > 1;
            HttpResponseMessage response = new((HttpStatusCode)(success ? 200 : status))
            {
                RequestMessage = request,
                Content = new StringContent(success ? "{}" : ErrorBody, Encoding.UTF8, "application/json"),
            };
            if (hint != null) response.Headers.TryAddWithoutValidation("Retry-After", hint);
            response.Headers.Add("x-request-id", "synthetic-request");
            return response;
        }
    }
}
