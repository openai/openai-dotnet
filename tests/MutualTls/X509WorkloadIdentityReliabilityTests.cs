using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System;
using System.ClientModel.Primitives;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.MutualTls;

public sealed partial class X509WorkloadIdentityTests
{
    [Test]
    public void ExistingNamedNullApiKeyCredentialOverloadsRemainUnambiguous()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentNullException>(
                () => new OpenAIClient(credential: null));
            Assert.Throws<ArgumentNullException>(
                () => new OpenAIClient(credential: null, options: new OpenAIClientOptions()));
        });
    }

    [Test]
    public void WorkloadIdentityConstructorSupportsUnambiguousNamedCredential()
    {
        using HttpClientHandler handler = new() { AllowAutoRedirect = false, UseCookies = false };
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);

        OpenAIClient first = new(workloadIdentityCredential: credential);
        OpenAIClient second = new(workloadIdentityCredential: credential, options: new());

        Assert.That(first.Endpoint, Is.EqualTo(second.Endpoint));
    }

    [Test]
    public void GeneratedClientSettingsDoNotStoreWorkloadIdentityCredentials()
    {
        Type settingsType = typeof(OpenAIClient).Assembly.GetType("OpenAI.InternalOpenAIClientSettings");

        Assert.That(settingsType, Is.Not.Null);
        Assert.That(settingsType.GetProperty("WorkloadIdentityCredential"), Is.Null);
    }

    [TestCase("http://insecure.example.test/v1")]
    [TestCase("ftp://insecure.example.test/v1")]
    public void WorkloadIdentityRejectsNonHttpsApiEndpoints(string endpoint)
    {
        using HttpClientHandler handler = new() { AllowAutoRedirect = false, UseCookies = false };
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClientOptions options = new() { Endpoint = new Uri(endpoint) };

        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new OpenAIClient(credential, options));

        Assert.That(exception.Message, Does.Contain("HTTPS"));
        Assert.That(options.Transport, Is.Null);
    }

    [Test]
    public void ApiKeyClientsContinueToAllowCustomHttpEndpoints()
    {
        OpenAIClient client = new(new System.ClientModel.ApiKeyCredential("existing-api-key"), new()
        {
            Endpoint = new Uri("http://existing-loopback.example.test/v1"),
        });

        Assert.That(client.Endpoint.Scheme, Is.EqualTo(Uri.UriSchemeHttp));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task DelayedExchangeBodiesCannotCacheOrSendExpiredTokens(bool async)
    {
        int exchangeNumber = 0;
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (!exchange || Interlocked.Increment(ref exchangeNumber) != 1)
            {
                return false;
            }

            context.Response.ContentType = "application/json";
            await context.Response.StartAsync();
            await context.Response.WriteAsync("{\"access_token\":\"expired-secret-token\",");
            await context.Response.Body.FlushAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(250), context.RequestAborted);
            await context.Response.WriteAsync("\"expires_in\":0.1}");
            return true;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = new(
            "idp_test",
            "svc_test",
            new() { Handler = handler, RefreshBuffer = TimeSpan.Zero });
        OpenAIClient client = new(credential, new() { NetworkTimeout = TimeSpan.FromSeconds(5) });

        InvalidOperationException exception;
        if (async)
        {
            exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await SendAsync(client));
        }
        else
        {
            using PipelineMessage message = client.Pipeline.CreateMessage();
            message.Request.Method = "GET";
            message.Request.Uri = new Uri("https://mtls.api.openai.com/v1/test");
            exception = Assert.Throws<InvalidOperationException>(() => client.Pipeline.Send(message));
        }

        Assert.That(exception.Message, Does.Contain("expired").IgnoreCase);
        Assert.That(exception.Message, Does.Not.Contain("expired-secret-token"));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.Zero);

        using PipelineMessage recovered = await SendAsync(client);

        Assert.That(recovered.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(2));
        Assert.That(server.ApiCount, Is.EqualTo(1));
    }

    [Test]
    public async Task ExchangeBodyDelayConsumesTheSameMonotonicRefreshWindowAsCachedReuse()
    {
        int exchangeNumber = 0;
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (!exchange || Interlocked.Increment(ref exchangeNumber) != 1)
            {
                return false;
            }

            context.Response.ContentType = "application/json";
            await context.Response.StartAsync();
            await context.Response.WriteAsync("{\"access_token\":\"short-lived-token\",");
            await context.Response.Body.FlushAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(350), context.RequestAborted);
            await context.Response.WriteAsync("\"expires_in\":0.8}");
            return true;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = new(
            "idp_test",
            "svc_test",
            new() { Handler = handler, RefreshBuffer = TimeSpan.FromMilliseconds(200) });
        OpenAIClient client = new(credential);

        using PipelineMessage first = await SendAsync(client);
        using PipelineMessage immediatelyCached = await SendAsync(client);
        Assert.That(server.ExchangeCount, Is.EqualTo(1));

        await Task.Delay(TimeSpan.FromMilliseconds(350));
        using PipelineMessage refreshed = await SendAsync(client);

        Assert.That(server.ExchangeCount, Is.EqualTo(2));
        Assert.That(server.ApiCount, Is.EqualTo(3));
    }

    [TestCase(StatusCodes.Status200OK)]
    [TestCase(StatusCodes.Status503ServiceUnavailable)]
    public async Task ConfiguredNetworkTimeoutCoversStalledExchangeResponseBodies(int responseStatus)
    {
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (!exchange)
            {
                return false;
            }

            context.Response.StatusCode = responseStatus;
            context.Response.ContentType = "application/json";
            await context.Response.StartAsync();
            await context.Response.WriteAsync("{\"access_token\":\"partial");
            await context.Response.Body.FlushAsync();
            await Task.Delay(Timeout.InfiniteTimeSpan, context.RequestAborted);
            return true;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential, new()
        {
            NetworkTimeout = TimeSpan.FromMilliseconds(200),
            RetryPolicy = new ClientRetryPolicy(4),
        });
        Stopwatch elapsed = Stopwatch.StartNew();

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await SendAsync(client));

        Assert.That(exception.Message, Does.Contain("network timeout"));
        Assert.That(elapsed.Elapsed, Is.LessThan(TimeSpan.FromSeconds(5)));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.Zero);
    }

    [Test]
    public async Task PerMessageNetworkTimeoutOverridesClientNetworkTimeout()
    {
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (exchange)
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, context.RequestAborted);
            }

            return false;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential, new() { NetworkTimeout = TimeSpan.FromSeconds(30) });
        using PipelineMessage message = client.Pipeline.CreateMessage();
        message.Request.Method = "GET";
        message.Request.Uri = new Uri("https://mtls.api.openai.com/v1/test");
        message.NetworkTimeout = TimeSpan.FromMilliseconds(200);
        Stopwatch elapsed = Stopwatch.StartNew();

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.Pipeline.SendAsync(message));

        Assert.That(exception.Message, Does.Contain("network timeout"));
        Assert.That(elapsed.Elapsed, Is.LessThan(TimeSpan.FromSeconds(5)));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
    }

    [TestCase(StatusCodes.Status200OK)]
    [TestCase(StatusCodes.Status503ServiceUnavailable)]
    public async Task SynchronousExchangeBodyReadsHonorCallerCancellation(int responseStatus)
    {
        TaskCompletionSource bodyStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int exchangeNumber = 0;
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (!exchange || Interlocked.Increment(ref exchangeNumber) > 1)
            {
                return false;
            }

            context.Response.StatusCode = responseStatus;
            context.Response.ContentType = "application/json";
            await context.Response.StartAsync();
            await context.Response.WriteAsync("{\"access_token\":\"partial");
            await context.Response.Body.FlushAsync();
            bodyStarted.TrySetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, context.RequestAborted);
            return true;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        using CancellationTokenSource cancellation = new();
        using PipelineMessage message = client.Pipeline.CreateMessage();
        message.Request.Method = "GET";
        message.Request.Uri = new Uri("https://mtls.api.openai.com/v1/test");
        message.Apply(new RequestOptions { CancellationToken = cancellation.Token });
        Task pending = Task.Run(() => client.Pipeline.Send(message));
        await bodyStarted.Task.WaitAsync(TimeSpan.FromSeconds(10));

        cancellation.Cancel();

        Assert.CatchAsync<OperationCanceledException>(async () =>
            await pending.WaitAsync(TimeSpan.FromSeconds(5)));
        using PipelineMessage recovered = await SendAsync(client);
        Assert.That(recovered.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(2));
        Assert.That(server.ApiCount, Is.EqualTo(1));
    }

    [Test]
    public async Task ConnectionTimeoutRetriesRemainBoundedByTokenExchangePolicy()
    {
        int connectionAttempts = 0;
        using SocketsHttpHandler handler = new()
        {
            AllowAutoRedirect = false,
            UseCookies = false,
            UseProxy = false,
            ConnectCallback = (_, _) =>
            {
                Interlocked.Increment(ref connectionAttempts);
                return ValueTask.FromException<Stream>(
                    new OperationCanceledException("hermetic connection timeout"));
            },
        };
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential, new()
        {
            RetryPolicy = new ClientRetryPolicy(4),
        });

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await SendAsync(client));

        Assert.That(exception.Message, Does.Contain("exhausted"));
        Assert.That(connectionAttempts, Is.EqualTo(3));
    }

    [TestCase(StatusCodes.Status200OK)]
    [TestCase(StatusCodes.Status503ServiceUnavailable)]
    public async Task TruncatedExchangeResponseRetriesRemainBounded(int responseStatus)
    {
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (!exchange)
            {
                return false;
            }

            context.Response.StatusCode = responseStatus;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength = 2048;
            await context.Response.StartAsync();
            await context.Response.WriteAsync("{\"access_token\":\"truncated");
            await context.Response.Body.FlushAsync();
            context.Abort();
            return true;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential, new()
        {
            RetryPolicy = new ClientRetryPolicy(4),
        });

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await SendAsync(client));

        Assert.That(exception.Message, Does.Contain("exhausted"));
        Assert.That(server.ExchangeCount, Is.EqualTo(3));
        Assert.That(server.ApiCount, Is.Zero);
    }
}
