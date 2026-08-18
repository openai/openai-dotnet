using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.MutualTls;

public sealed partial class X509WorkloadIdentityTests
{
    [TestCase(false)]
    [TestCase(true)]
    public async Task SharedOptionsKeepApiKeyAndWorkloadIdentityAuthenticationIndependent(bool createApiKeyFirst)
    {
        string customHeader = null;
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (!exchange)
            {
                customHeader = context.Request.Headers["X-Customer-Request"];
            }

            return Task.FromResult(false);
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClientOptions options = new()
        {
            OrganizationId = "customer-organization",
            ProjectId = "customer-project",
            NetworkTimeout = TimeSpan.FromSeconds(20),
        };
        options.AddPolicy(new SecurityMutationPolicy(message =>
            message.Request.Headers.Set("X-Customer-Request", "preserved")),
            PipelinePosition.BeforeTransport);

        OpenAIClient apiKeyClient = createApiKeyFirst
            ? new OpenAIClient(new System.ClientModel.ApiKeyCredential("existing-api-key"), options)
            : null;
        OpenAIClient workloadClient = new(credential, options);
        apiKeyClient ??= new OpenAIClient(
            new System.ClientModel.ApiKeyCredential("existing-api-key"), options);

        using PipelineMessage message = await SendAsync(workloadClient);

        Assert.Multiple(() =>
        {
            Assert.That(options.Endpoint, Is.Null);
            Assert.That(options.Transport, Is.Null);
            Assert.That(apiKeyClient.Endpoint, Is.EqualTo(new Uri("https://api.openai.com/v1")));
            Assert.That(workloadClient.Endpoint, Is.EqualTo(new Uri("https://mtls.api.openai.com/v1")));
            Assert.That(customHeader, Is.EqualTo("preserved"));
            Assert.That(server.ExchangeCount, Is.EqualTo(1));
            Assert.That(server.ApiCount, Is.EqualTo(1));
        });
    }

    [TestCase(false)]
    [TestCase(true)]
    public void WorkloadIdentityConstructorsAreExplicitlyExperimental(bool hasOptions)
    {
        Type[] parameters = hasOptions
            ? [typeof(X509WorkloadIdentityCredential), typeof(OpenAIClientOptions)]
            : [typeof(X509WorkloadIdentityCredential)];
        ConstructorInfo constructor = typeof(OpenAIClient).GetConstructor(parameters);
        ExperimentalAttribute attribute = constructor.GetCustomAttribute<ExperimentalAttribute>();

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute.DiagnosticId, Is.EqualTo("OPENAI001"));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ConstructorRejectsAutomaticCookieHandling(bool useSocketsHandler)
    {
        using HttpMessageHandler handler = useSocketsHandler
            ? new SocketsHttpHandler { AllowAutoRedirect = false, UseCookies = true }
            : new HttpClientHandler { AllowAutoRedirect = false, UseCookies = true };

        ArgumentException exception = Assert.Throws<ArgumentException>(() => CreateCredential(handler));

        Assert.That(exception.Message, Does.Contain("cookies").IgnoreCase);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ConstructorRejectsDestinationServerCredentials(bool useSocketsHandler)
    {
        NetworkCredential credentials = new("api-user", "api-secret");
        using HttpMessageHandler handler = useSocketsHandler
            ? new SocketsHttpHandler { AllowAutoRedirect = false, UseCookies = false, Credentials = credentials }
            : new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false, Credentials = credentials };

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
            ? new SocketsHttpHandler { AllowAutoRedirect = false, UseCookies = false, Proxy = proxy }
            : new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false, Proxy = proxy };

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
            using SocketsHttpHandler handler = new()
            {
                AllowAutoRedirect = false,
                UseCookies = false,
                Proxy = null,
            };

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
            UseCookies = false,
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
            ? new SocketsHttpHandler { AllowAutoRedirect = false, UseCookies = false, Proxy = proxy }
            : new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false, Proxy = proxy };

        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        IWebProxy protectedProxy = useSocketsHandler
            ? ((SocketsHttpHandler)handler).Proxy
            : ((HttpClientHandler)handler).Proxy;

        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://mtls.api.openai.com/v1")));
        Assert.That(protectedProxy.Credentials, Is.SameAs(proxy.Credentials));

        NetworkCredential replacement = new("updated-proxy-user", "updated-proxy-secret");
        protectedProxy.Credentials = replacement;

        Assert.That(proxy.Credentials, Is.SameAs(replacement),
            "Explicit caller-owned proxies must retain their normal credential ownership.");
    }

    [TestCase(false)]
    [TestCase(true)]
    public void HandlerRejectsAProxyThatChangesAfterItsConfigurationWasValidated(bool useSocketsHandler)
    {
        MutableApiProxy proxy = new();
        using HttpMessageHandler handler = useSocketsHandler
            ? new SocketsHttpHandler { AllowAutoRedirect = false, UseCookies = false, Proxy = proxy }
            : new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false, Proxy = proxy };
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);

        proxy.UseHttps = true;
        IWebProxy actualProxy = useSocketsHandler
            ? ((SocketsHttpHandler)handler).Proxy
            : ((HttpClientHandler)handler).Proxy;

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            actualProxy.GetProxy(new Uri("https://mtls.api.openai.com/v1")));

        Assert.That(exception.Message, Does.Contain("proxy").IgnoreCase);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ImplicitProxyPreservesHandlerScopedDefaultProxyCredentials(bool useSocketsHandler)
    {
        IWebProxy previous = HttpClient.DefaultProxy;
        try
        {
            NetworkCredential sharedCredentials = new("shared-proxy-user", "shared-proxy-secret");
            HttpClient.DefaultProxy = new WebProxy("http://proxy.example.test:8080")
            {
                Credentials = sharedCredentials,
            };
            NetworkCredential credentials = new("proxy-user", "proxy-secret");
            using HttpMessageHandler handler = useSocketsHandler
                ? new SocketsHttpHandler
                {
                    AllowAutoRedirect = false,
                    UseCookies = false,
                    DefaultProxyCredentials = credentials,
                }
                : new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    UseCookies = false,
                    DefaultProxyCredentials = credentials,
                };
            using X509WorkloadIdentityCredential credential = CreateCredential(handler);
            IWebProxy protectedProxy = useSocketsHandler
                ? ((SocketsHttpHandler)handler).Proxy
                : ((HttpClientHandler)handler).Proxy;

            Assert.That(protectedProxy.Credentials, Is.SameAs(sharedCredentials),
                "Native handlers prefer the default proxy's credentials over handler defaults.");

            NetworkCredential replacement = new("updated-proxy-user", "updated-proxy-secret");
            protectedProxy.Credentials = replacement;

            Assert.That(protectedProxy.Credentials, Is.SameAs(replacement));
            Assert.That(HttpClient.DefaultProxy.Credentials, Is.SameAs(sharedCredentials),
                "Rotating handler-scoped credentials must not modify the shared system proxy.");

            using HttpClientHandler unrelatedHandler = new();
            Assert.That(unrelatedHandler.Proxy, Is.Null);
            Assert.That(HttpClient.DefaultProxy.Credentials, Is.SameAs(sharedCredentials),
                "An unrelated HTTP client must not observe workload proxy credentials.");
        }
        finally
        {
            HttpClient.DefaultProxy = previous;
        }
    }

    [Test]
    public async Task HandlerRejectsAProxyThatChangesBetweenPreflightAndActualResolution()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        LateSwitchingProxy proxy = new();
        handler.UseProxy = true;
        handler.Proxy = proxy;
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        Exception exception = Assert.CatchAsync(async () => await SendAsync(client));

        Assert.That(exception.ToString(), Does.Contain("proxy").IgnoreCase);
        Assert.That(proxy.UnsafeSelections, Is.GreaterThanOrEqualTo(1));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.Zero);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ClientRejectsHandlerProxyReplacementAfterCredentialConstruction(bool useSocketsHandler)
    {
        MutableApiProxy original = new();
        MutableApiProxy replacement = new();
        using HttpMessageHandler handler = useSocketsHandler
            ? new SocketsHttpHandler { AllowAutoRedirect = false, UseCookies = false, Proxy = original }
            : new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false, Proxy = original };
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        if (useSocketsHandler)
        {
            ((SocketsHttpHandler)handler).Proxy = replacement;
        }
        else
        {
            ((HttpClientHandler)handler).Proxy = replacement;
        }

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            new OpenAIClient(credential));

        Assert.That(exception.Message, Does.Contain("proxy").IgnoreCase);
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

        using PipelineMessage message = await SendAsync(client);

        Assert.That(message.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(2));
        Assert.That(server.ApiCount, Is.EqualTo(2));
        Assert.That(handler.CookieContainer.GetCookies(new Uri("https://mtls.auth.openai.com")),
            Is.Empty);
    }

    [Test]
    public async Task ExplicitCookiesScopedOnlyToTheApiRequestRemainSupported()
    {
        string apiCookie = null;
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (!exchange)
            {
                apiCookie = context.Request.Headers.Cookie.ToString();
            }

            return Task.FromResult(false);
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClientOptions options = new();
        options.AddPolicy(new SecurityMutationPolicy(message =>
            message.Request.Headers.Set("Cookie", "api-session=api-only-value")),
            PipelinePosition.BeforeTransport);
        OpenAIClient client = new(credential, options);

        using PipelineMessage message = await SendAsync(client);

        Assert.That(message.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.EqualTo(1));
        Assert.That(apiCookie, Is.EqualTo("api-session=api-only-value"));
    }

    [TestCase("https://mtls.api.openai.com@attacker.example.test/v1")]
    [TestCase("https://user:password@mtls.api.openai.com/v1")]
    public void ClientRejectsApiEndpointsContainingUserInfo(string endpoint)
    {
        using HttpClientHandler handler = new() { AllowAutoRedirect = false, UseCookies = false };
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

    [TestCase("attacker.example.test")]
    [TestCase("mtls.api.openai.com:8443")]
    public async Task PublicPipelineRejectsSpoofedHostAuthorityBeforeTokenAcquisition(string authority)
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        using PipelineMessage message = client.Pipeline.CreateMessage();
        message.Request.Method = "GET";
        message.Request.Uri = new Uri("https://mtls.api.openai.com/v1/test");
        message.Request.Headers.Set("Host", authority);

        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.Pipeline.SendAsync(message));

        Assert.That(server.Requests, Is.Empty);
    }

    [TestCase("api-key")]
    [TestCase("api_key")]
    [TestCase("x-api-key")]
    [TestCase("x_api_key")]
    [TestCase("proxy-authorization")]
    [TestCase("proxy_authorization")]
    public async Task PublicPipelineRejectsOtherCredentialHeaderAliasesBeforeTokenAcquisition(string header)
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        using PipelineMessage message = client.Pipeline.CreateMessage();
        message.Request.Method = "GET";
        message.Request.Uri = new Uri("https://mtls.api.openai.com/v1/test");
        message.Request.Headers.Set(header, "different-credential-secret");

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.Pipeline.SendAsync(message));

        Assert.That(exception.Message, Does.Not.Contain("different-credential-secret"));
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

    [TestCase("attacker.example.test")]
    [TestCase("mtls.api.openai.com:8443")]
    public async Task BeforeTransportPoliciesCannotSpoofTheOutgoingHostAuthority(string authority)
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        RequestOptions options = new();
        options.AddPolicy(new SecurityMutationPolicy(message =>
            message.Request.Headers.Set("Host", authority)), PipelinePosition.BeforeTransport);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await client.GetOpenAIModelClient().GetModelsAsync(options));

        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.Zero);
    }

    [TestCase("api-key")]
    [TestCase("api_key")]
    [TestCase("x-api-key")]
    [TestCase("x_api_key")]
    [TestCase("proxy-authorization")]
    [TestCase("proxy_authorization")]
    public async Task BeforeTransportPoliciesCannotAddOtherCredentialHeaderAliases(string header)
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        RequestOptions options = new();
        options.AddPolicy(new SecurityMutationPolicy(message =>
            message.Request.Headers.Set(header, "different-credential-secret")),
            PipelinePosition.BeforeTransport);

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await client.GetOpenAIModelClient().GetModelsAsync(options));

        Assert.That(exception.Message, Does.Not.Contain("different-credential-secret"));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.Zero);
    }

    [Test]
    public async Task AnExplicitMatchingHostAuthorityRemainsSupported()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClientOptions options = new();
        options.AddPolicy(new SecurityMutationPolicy(message =>
            message.Request.Headers.Set("Host", "mtls.api.openai.com")),
            PipelinePosition.BeforeTransport);
        OpenAIClient client = new(credential, options);

        using PipelineMessage message = await SendAsync(client);

        Assert.That(message.Response.Status, Is.EqualTo(200));
        Assert.That(server.ApiCount, Is.EqualTo(1));
    }

    [Test]
    public void FrameworkRejectsNonSeekableStreamsBeforeWorkloadIdentityCanSendThem()
    {
        using NonSeekableMemoryStream stream = new(Encoding.UTF8.GetBytes("single-use payload"));

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
        {
            using System.ClientModel.BinaryContent content = System.ClientModel.BinaryContent.Create(stream);
        });

        Assert.That(exception.Message, Does.Contain("seekable").IgnoreCase);
    }

    [TestCase("{\"access_token\":\"token\",\"expires_in\":3600,\"token_type\":\"MAC\"}")]
    [TestCase("{\"access_token\":\"token\",\"expires_in\":3600,\"token_type\":7}")]
    [TestCase("{\"access_token\":\"token\",\"expires_in\":3600,\"token_type\":null}")]
    [TestCase("{\"access_token\":\"invalid token\",\"expires_in\":3600,\"token_type\":\"Bearer\"}")]
    [TestCase("{\"access_token\":\"invalid=middle\",\"expires_in\":3600,\"token_type\":\"Bearer\"}")]
    public async Task ExchangeRejectsNonBearerTokenTypesAndInvalidBearerTokens(string response)
    {
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (!exchange)
            {
                return false;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(response);
            return true;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        Assert.ThrowsAsync<InvalidOperationException>(async () => await SendAsync(client));

        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.Zero);
    }

    [Test]
    public async Task ExchangeRejectsSuccessBodiesOverOneMebibyte()
    {
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (!exchange)
            {
                return false;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                "{\"access_token\":\"token\",\"expires_in\":3600,\"token_type\":\"Bearer\",\"padding\":\""
                + new string('a', 1024 * 1024)
                + "\"}");
            return true;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await SendAsync(client));

        Assert.That(exception.Message, Does.Contain("response size").IgnoreCase);
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
    public async Task SharedCredentialRejectsDifferentConfiguredHttpsOrigins()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient defaultClient = new(credential);
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new OpenAIClient(credential, new()
        {
            Endpoint = new Uri("https://customer.example.test/v1"),
        }));

        using PipelineMessage first = await SendAsync(defaultClient);

        Assert.That(exception.Message, Does.Contain("endpoint").IgnoreCase);
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.EqualTo(1));
    }

    [Test]
    public async Task FailedPipelineCreationDoesNotBindCredentialOrigin()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClientOptions invalidOptions = new()
        {
            Endpoint = new Uri("https://failed.example.test/v1"),
            UserAgentApplicationId = new string('a', 513),
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new OpenAIClient(credential, invalidOptions));

        Uri endpoint = new("https://recovered.example.test/v1");
        OpenAIClient client = new(credential, new() { Endpoint = endpoint });
        using PipelineMessage response = await SendAsync(client);

        Assert.That(client.Endpoint, Is.EqualTo(endpoint));
        Assert.That(response.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.EqualTo(1));
    }

    [Test]
    public void ConcurrentClientConstructionBindsOnlyOneCredentialOrigin()
    {
        using SocketsHttpHandler handler = new() { AllowAutoRedirect = false, UseCookies = false };
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        using Barrier start = new(participantCount: 2);

        Task<bool> first = Task.Run(() => TryCreateClient("https://first.example.test/v1"));
        Task<bool> second = Task.Run(() => TryCreateClient("https://second.example.test/v1"));
        Task.WaitAll(first, second);

        Assert.That(first.Result, Is.Not.EqualTo(second.Result));

        bool TryCreateClient(string endpoint)
        {
            start.SignalAndWait();
            try
            {
                _ = new OpenAIClient(credential, new() { Endpoint = new Uri(endpoint) });
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }

    [Test]
    public Task ARefreshWaiterHonorsItsOwnNetworkTimeout()
    {
        return AssertRefreshWaiterHonorsItsOwnNetworkTimeoutAsync(synchronous: false);
    }

    [Test]
    public Task ASynchronousRefreshWaiterHonorsItsOwnNetworkTimeout()
    {
        return AssertRefreshWaiterHonorsItsOwnNetworkTimeoutAsync(synchronous: true);
    }

    private static async Task AssertRefreshWaiterHonorsItsOwnNetworkTimeoutAsync(bool synchronous)
    {
        TaskCompletionSource exchangeStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource releaseExchange = new(TaskCreationOptions.RunContinuationsAsynchronously);
        await using TestServer server = await TestServer.StartAsync(async (_, exchange) =>
        {
            if (exchange)
            {
                exchangeStarted.TrySetResult();
                await releaseExchange.Task;
            }

            return false;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient owner = new(credential, new() { NetworkTimeout = TimeSpan.FromSeconds(10) });
        OpenAIClient waiter = new(credential, new() { NetworkTimeout = TimeSpan.FromMilliseconds(100) });
        Task<PipelineMessage> pending = SendAsync(owner);
        try
        {
            await exchangeStarted.Task.WaitAsync(TimeSpan.FromSeconds(10));

            InvalidOperationException exception;
            if (synchronous)
            {
                using PipelineMessage message = waiter.Pipeline.CreateMessage();
                message.Request.Method = "GET";
                message.Request.Uri = new Uri("https://mtls.api.openai.com/v1/test");
                exception = Assert.Throws<InvalidOperationException>(() => waiter.Pipeline.Send(message));
            }
            else
            {
                exception = Assert.ThrowsAsync<InvalidOperationException>(
                    async () => await SendAsync(waiter));
            }

            Assert.That(exception.Message, Does.Contain("network timeout"));
            Assert.That(pending.IsCompleted, Is.False,
                "The waiter must time out while the owner still holds the refresh lock.");
            Assert.That(server.ExchangeCount, Is.EqualTo(1));
        }
        finally
        {
            releaseExchange.TrySetResult();
        }

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

    private sealed class LateSwitchingProxy : IWebProxy
    {
        private int _apiBypassChecks;
        private int _unsafeSelections;

        internal int UnsafeSelections => Volatile.Read(ref _unsafeSelections);
        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination)
        {
            Interlocked.Increment(ref _unsafeSelections);
            return new Uri("https://proxy.example.test:8443");
        }

        public bool IsBypassed(Uri host)
        {
            return host.Host == "mtls.auth.openai.com"
                || Interlocked.Increment(ref _apiBypassChecks) < 2;
        }
    }

    private sealed class NonSeekableMemoryStream(byte[] content) : MemoryStream(content)
    {
        public override bool CanSeek => false;

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }
    }
}
