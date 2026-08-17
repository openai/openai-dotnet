using NUnit.Framework;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.MutualTls;

public sealed partial class X509WorkloadIdentityTests
{
    [TestCase(false)]
    [TestCase(true)]
    public void ConstructorRejectsDestinationServerCredentials(bool useSocketsHandler)
    {
        NetworkCredential credentials = new("api-user", "api-secret");
        using HttpMessageHandler handler = useSocketsHandler
            ? new SocketsHttpHandler { AllowAutoRedirect = false, Credentials = credentials }
            : new HttpClientHandler { AllowAutoRedirect = false, Credentials = credentials };

        ArgumentException exception = Assert.Throws<ArgumentException>(() => CreateCredential(handler));

        Assert.That(exception.Message, Does.Contain("credentials").IgnoreCase);
        Assert.That(exception.Message, Does.Not.Contain("api-secret"));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ConstructorRejectsHttpsProxies(bool useSocketsHandler)
    {
        WebProxy proxy = new("https://proxy.example.test:8443");
        using HttpMessageHandler handler = useSocketsHandler
            ? new SocketsHttpHandler { AllowAutoRedirect = false, Proxy = proxy }
            : new HttpClientHandler { AllowAutoRedirect = false, Proxy = proxy };

        ArgumentException exception = Assert.Throws<ArgumentException>(() => CreateCredential(handler));

        Assert.That(exception.Message, Does.Contain("proxy").IgnoreCase);
    }

    [Test]
    public void ConstructorRejectsTheImplicitHttpsDefaultProxy()
    {
        IWebProxy previous = HttpClient.DefaultProxy;
        try
        {
            HttpClient.DefaultProxy = new WebProxy("https://proxy.example.test:8443");
            using SocketsHttpHandler handler = new() { AllowAutoRedirect = false, Proxy = null };

            Assert.Throws<ArgumentException>(() => CreateCredential(handler));
        }
        finally
        {
            HttpClient.DefaultProxy = previous;
        }
    }

    [Test]
    public void ClientRejectsAnHttpsProxySelectedOnlyForTheApiOrigin()
    {
        using SocketsHttpHandler handler = new()
        {
            AllowAutoRedirect = false,
            Proxy = new DestinationSelectiveProxy(),
        };
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClientOptions options = new();

        Assert.Throws<ArgumentException>(() => new OpenAIClient(credential, options));

        Assert.That(options.Transport, Is.Null);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void OrdinaryHttpConnectProxiesAndScopedProxyCredentialsRemainSupported(bool useSocketsHandler)
    {
        WebProxy proxy = new("http://proxy.example.test:8080")
        {
            Credentials = new NetworkCredential("proxy-user", "proxy-secret"),
        };
        using HttpMessageHandler handler = useSocketsHandler
            ? new SocketsHttpHandler { AllowAutoRedirect = false, Proxy = proxy }
            : new HttpClientHandler { AllowAutoRedirect = false, Proxy = proxy };

        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://mtls.api.openai.com/v1")));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ConstructorRejectsCookiesThatWouldBeSentToTheTokenEndpoint(bool useSocketsHandler)
    {
        CookieContainer cookies = new();
        cookies.Add(new Uri("https://mtls.auth.openai.com"), new Cookie("session", "cookie-secret"));
        using HttpMessageHandler handler = useSocketsHandler
            ? new SocketsHttpHandler { AllowAutoRedirect = false, CookieContainer = cookies }
            : new HttpClientHandler { AllowAutoRedirect = false, CookieContainer = cookies };

        ArgumentException exception = Assert.Throws<ArgumentException>(() => CreateCredential(handler));

        Assert.That(exception.Message, Does.Contain("cookies").IgnoreCase);
        Assert.That(exception.Message, Does.Not.Contain("cookie-secret"));
    }

    [Test]
    public async Task ApiCookiesCannotCrossIntoASubsequentTokenExchange()
    {
        int apiRequests = 0;
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (!exchange && Interlocked.Increment(ref apiRequests) == 1)
            {
                context.Response.Headers["Set-Cookie"] =
                    "session=cookie-secret; Domain=.openai.com; Path=/; Secure";
                context.Response.StatusCode = 401;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        ArgumentException exception = Assert.ThrowsAsync<ArgumentException>(
            async () => await SendAsync(client));

        Assert.That(exception.Message, Does.Not.Contain("cookie-secret"));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.EqualTo(1));
        Assert.That(handler.CookieContainer.GetCookies(new Uri("https://mtls.auth.openai.com")),
            Has.Count.EqualTo(1));
    }

    [TestCase("https://mtls.api.openai.com@attacker.example.test/v1")]
    [TestCase("https://user:password@mtls.api.openai.com/v1")]
    public void ClientRejectsApiEndpointsContainingUserInfo(string endpoint)
    {
        using HttpClientHandler handler = new() { AllowAutoRedirect = false };
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClientOptions options = new() { Endpoint = new Uri(endpoint) };

        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new OpenAIClient(credential, options));

        Assert.That(exception.Message, Does.Contain("userinfo").IgnoreCase);
        Assert.That(exception.Message, Does.Not.Contain("password"));
        Assert.That(options.Transport, Is.Null);
    }

    [TestCase("http://attacker.example.test/private")]
    [TestCase("https://attacker.example.test/private")]
    [TestCase("https://trusted@mtls.api.openai.com/private")]
    [TestCase("https://mtls.api.openai.com:8443/private")]
    public async Task PublicPipelineRejectsUntrustedDestinationsBeforeTokenAcquisition(string destination)
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        using PipelineMessage message = client.Pipeline.CreateMessage();
        message.Request.Method = "GET";
        message.Request.Uri = new Uri(destination);

        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.Pipeline.SendAsync(message));

        Assert.That(server.Requests, Is.Empty);
    }

    [TestCase("http://attacker.example.test/stolen")]
    [TestCase("https://attacker.example.test/stolen")]
    [TestCase("https://mtls.api.openai.com:8443/stolen")]
    public async Task ClientBeforeTransportPoliciesCannotChangeTheFinalOrigin(string destination)
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClientOptions options = new();
        options.AddPolicy(new SecurityMutationPolicy(message =>
            message.Request.Uri = new Uri(destination)), PipelinePosition.BeforeTransport);
        OpenAIClient client = new(credential, options);

        Assert.ThrowsAsync<InvalidOperationException>(async () => await SendAsync(client));

        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.Zero);
    }

    [TestCase("http://attacker.example.test/stolen")]
    [TestCase("https://attacker.example.test/stolen")]
    [TestCase("https://trusted@mtls.api.openai.com/stolen")]
    public async Task PerRequestBeforeTransportPoliciesCannotChangeTheFinalOrigin(string destination)
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        RequestOptions requestOptions = new();
        requestOptions.AddPolicy(new SecurityMutationPolicy(message =>
            message.Request.Uri = new Uri(destination)), PipelinePosition.BeforeTransport);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await client.GetOpenAIModelClient().GetModelsAsync(requestOptions));

        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.Zero);
    }

    [Test]
    public async Task PerRequestBeforeTransportPoliciesCannotReplaceTheIssuedBearer()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        RequestOptions requestOptions = new();
        requestOptions.AddPolicy(new SecurityMutationPolicy(message =>
            message.Request.Headers.Set("Authorization", "Bearer legacy-api-key-secret")),
            PipelinePosition.BeforeTransport);

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await client.GetOpenAIModelClient().GetModelsAsync(requestOptions));

        Assert.That(exception.Message, Does.Not.Contain("legacy-api-key-secret"));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.Zero);
    }

    [Test]
    public async Task FinalTransportRejectsAnHttpsProxyIntroducedByACallerPolicy()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        MutableApiProxy proxy = new();
        handler.UseProxy = true;
        handler.Proxy = proxy;
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClientOptions options = new();
        options.AddPolicy(new SecurityMutationPolicy(_ => proxy.UseHttps = true),
            PipelinePosition.BeforeTransport);
        OpenAIClient client = new(credential, options);

        Assert.ThrowsAsync<ArgumentException>(async () => await SendAsync(client));

        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.Zero);
    }

    [Test]
    public async Task HarmlessBeforeTransportPoliciesRemainSupported()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClientOptions options = new();
        options.AddPolicy(new SecurityMutationPolicy(message =>
            message.Request.Headers.Set("X-Customer-Request", "safe")), PipelinePosition.BeforeTransport);
        OpenAIClient client = new(credential, options);

        using PipelineMessage message = await SendAsync(client);

        Assert.That(message.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.EqualTo(1));
    }

    [Test]
    public async Task SharedCredentialPreservesEachClientsConfiguredHttpsOrigin()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient defaultClient = new(credential);
        OpenAIClient customClient = new(credential, new()
        {
            Endpoint = new Uri("https://customer.example.test/v1"),
        });

        using PipelineMessage first = await SendAsync(defaultClient);
        using PipelineMessage second = await SendAsync(customClient);

        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.EqualTo(2));
    }

    [Test]
    public async Task ARefreshWaiterHonorsItsOwnNetworkTimeout()
    {
        TaskCompletionSource exchangeStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        await using TestServer server = await TestServer.StartAsync(async (_, exchange) =>
        {
            if (exchange)
            {
                exchangeStarted.TrySetResult();
                await Task.Delay(700);
            }

            return false;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient owner = new(credential, new() { NetworkTimeout = TimeSpan.FromSeconds(5) });
        OpenAIClient waiter = new(credential, new() { NetworkTimeout = TimeSpan.FromMilliseconds(100) });
        Task<PipelineMessage> pending = SendAsync(owner);
        await exchangeStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Stopwatch elapsed = Stopwatch.StartNew();

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await SendAsync(waiter));

        Assert.That(exception.Message, Does.Contain("network timeout"));
        Assert.That(elapsed.Elapsed, Is.LessThan(TimeSpan.FromMilliseconds(500)));
        using PipelineMessage completed = await pending;
        Assert.That(completed.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
    }

    private sealed class DestinationSelectiveProxy : IWebProxy
    {
        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination)
        {
            return destination.Host == "mtls.auth.openai.com"
                ? new Uri("http://proxy.example.test:8080")
                : new Uri("https://proxy.example.test:8443");
        }

        public bool IsBypassed(Uri host) => false;
    }

    private sealed class SecurityMutationPolicy(Action<PipelineMessage> mutate) : PipelinePolicy
    {
        public override void Process(
            PipelineMessage message,
            IReadOnlyList<PipelinePolicy> pipeline,
            int currentIndex)
        {
            mutate(message);
            ProcessNext(message, pipeline, currentIndex);
        }

        public override ValueTask ProcessAsync(
            PipelineMessage message,
            IReadOnlyList<PipelinePolicy> pipeline,
            int currentIndex)
        {
            mutate(message);
            return ProcessNextAsync(message, pipeline, currentIndex);
        }
    }

    private sealed class MutableApiProxy : IWebProxy
    {
        internal bool UseHttps { get; set; }
        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination)
        {
            return new Uri(UseHttps
                ? "https://proxy.example.test:8443"
                : "http://proxy.example.test:8080");
        }

        public bool IsBypassed(Uri host) => host.Host == "mtls.auth.openai.com";
    }
}
