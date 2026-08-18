using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.MutualTls;

[Category("MutualTls")]
[NonParallelizable]
public sealed partial class X509WorkloadIdentityTests
{
    [Test]
    public void ConstructorRejectsAutomaticRedirects()
    {
        using HttpClientHandler handler = new() { AllowAutoRedirect = true };

        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => CreateCredential(handler));

        Assert.That(exception.Message, Does.Contain("redirect"));
    }

    [Test]
    public void ConstructorRejectsSocketsHandlerAutomaticRedirects()
    {
        using SocketsHttpHandler handler = new() { AllowAutoRedirect = true };

        Assert.Throws<ArgumentException>(() => CreateCredential(handler));
    }

    [Test]
    public async Task RedirectSettingChangedAfterConstructionFailsClosed()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        handler.AllowAutoRedirect = true;

        Assert.ThrowsAsync<ArgumentException>(async () => await SendAsync(client));
        Assert.That(server.Requests, Is.Empty);
    }

    [Test]
    public async Task UsedHandlerCannotEnableRedirectsWhileTokenIsCached()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        using PipelineMessage first = await SendAsync(client);

        Assert.Throws<InvalidOperationException>(() => handler.AllowAutoRedirect = true);

        using PipelineMessage second = await SendAsync(client);
        using PipelineMessage third = client.Pipeline.CreateMessage();
        third.Request.Method = "GET";
        third.Request.Uri = new Uri("https://mtls.api.openai.com/v1/test");
        client.Pipeline.Send(third);

        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.EqualTo(3));
        Assert.That(handler.AllowAutoRedirect, Is.False);
    }

    [Test]
    public void ConstructorRejectsUnsupportedHandler()
    {
        using UnsupportedHandler handler = new();

        Assert.Throws<ArgumentException>(() => CreateCredential(handler));
    }

    [Test]
    public void ConstructorRejectsInvalidConfiguration()
    {
        using HttpClientHandler handler = new() { AllowAutoRedirect = false, UseCookies = false };

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => new X509WorkloadIdentityCredential(
                " ", "service", new() { Handler = handler }));
            Assert.Throws<ArgumentException>(() => new X509WorkloadIdentityCredential(
                "provider", " ", new() { Handler = handler }));
            ArgumentOutOfRangeException invalidBuffer = Assert.Throws<ArgumentOutOfRangeException>(() => new X509WorkloadIdentityCredential(
                "provider", "service", new() { Handler = handler, RefreshBuffer = TimeSpan.FromSeconds(-1) }));
            Assert.That(invalidBuffer.ParamName, Is.EqualTo(nameof(X509WorkloadIdentityCredentialOptions.RefreshBuffer)));
        });
    }

    [Test]
    public void ClientRejectsIndependentTransport()
    {
        using HttpClientHandler handler = new() { AllowAutoRedirect = false, UseCookies = false };
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        using HttpClientPipelineTransport transport = new();

        Assert.Throws<ArgumentException>(() => new OpenAIClient(credential, new()
        {
            Transport = transport,
        }));

        Assert.DoesNotThrow(() => new OpenAIClient(credential));
    }

    [Test]
    public async Task ClientOptionsCanBeReusedWithTheSameCredential()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClientOptions options = new();
        OpenAIClient first = new(credential, options);
        OpenAIClient second = new(credential, options);

        using PipelineMessage firstMessage = await SendAsync(first);
        using PipelineMessage secondMessage = await SendAsync(second);

        Assert.That(options.IsReadOnly, Is.False);
        Assert.That(options.Endpoint, Is.Null);
        Assert.That(options.Transport, Is.Null);
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.EqualTo(2));
    }

    [Test]
    public void ExistingApiKeyAuthenticationKeepsOrdinaryEndpoint()
    {
        OpenAIClient client = new(new ApiKeyCredential("api-key"));

        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://api.openai.com/v1")));
    }

    [Test]
    public async Task ExistingApiKeyAuthenticationKeepsCallerConfiguredTransport()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using HttpClient httpClient = new(handler, disposeHandler: false);
        OpenAIClient client = new(new ApiKeyCredential("existing-api-key"), new()
        {
            Endpoint = new Uri("https://mtls.api.openai.com/v1"),
            Transport = new HttpClientPipelineTransport(httpClient),
        });

        using PipelineMessage message = await SendAsync(client);

        Assert.That(server.ExchangeCount, Is.Zero);
        Assert.That(server.Requests.Single().Authorization, Is.EqualTo("Bearer existing-api-key"));
    }

    [Test]
    public void RealtimeIsExplicitlyUnsupported()
    {
        using HttpClientHandler handler = new() { AllowAutoRedirect = false, UseCookies = false };
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        Assert.Throws<NotSupportedException>(() => client.GetRealtimeClient());
    }

    [Test]
    public async Task ExchangeIsLazyUsesExactEndpointAndSharesTransport()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        Assert.That(server.Requests, Is.Empty);
        Assert.That(client.Endpoint, Is.EqualTo(new Uri("https://mtls.api.openai.com/v1")));

        using PipelineMessage message = await SendAsync(client);
        RequestRecord exchange = server.Requests.Single(request => request.Host == "mtls.auth.openai.com");
        RequestRecord api = server.Requests.Single(request => request.Host == "mtls.api.openai.com");
        using JsonDocument body = JsonDocument.Parse(exchange.Body);

        Assert.Multiple(() =>
        {
            Assert.That(exchange.Method, Is.EqualTo("POST"));
            Assert.That(exchange.Path, Is.EqualTo("/oauth/token"));
            Assert.That(body.RootElement.GetProperty("identity_provider_id").GetString(), Is.EqualTo("idp_test"));
            Assert.That(body.RootElement.GetProperty("service_account_id").GetString(), Is.EqualTo("svc_test"));
            Assert.That(body.RootElement.GetProperty("subject_token_type").GetString(),
                Is.EqualTo("urn:openai:params:oauth:token-type:x509"));
            Assert.That(body.RootElement.TryGetProperty("subject_token", out _), Is.False);
            Assert.That(api.Authorization, Is.EqualTo("Bearer token-1"));
            Assert.That(message.Response.Status, Is.EqualTo(200));
            Assert.That(client.GetOpenAIModelClient().Pipeline, Is.SameAs(client.Pipeline));
        });
    }

    [Test]
    public async Task SynchronousPipelineAcquiresToken()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        using PipelineMessage message = client.Pipeline.CreateMessage();
        message.Request.Method = "GET";
        message.Request.Uri = new Uri("https://mtls.api.openai.com/v1/test");

        client.Pipeline.Send(message);

        Assert.That(message.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
    }

    [Test]
    public async Task ExplicitEndpointIsPreserved()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        Uri endpoint = new("https://custom.example.test/v1");
        OpenAIClient client = new(credential, new() { Endpoint = endpoint });

        using PipelineMessage message = await SendAsync(client);

        Assert.That(client.Endpoint, Is.EqualTo(endpoint));
        Assert.That(server.Requests.Any(request => request.Host == "custom.example.test"), Is.True);
    }

    [Test]
    public async Task ConcurrentRequestsShareOneExchangeAndCachedToken()
    {
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (exchange)
            {
                await Task.Delay(100);
            }

            return false;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        PipelineMessage[] messages = await Task.WhenAll(
            Enumerable.Range(0, 16).Select(_ => SendAsync(client)));
        foreach (PipelineMessage message in messages)
        {
            message.Dispose();
        }

        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.EqualTo(16));
    }

    [Test]
    public async Task CanceledWaiterDoesNotCancelSharedExchange()
    {
        TaskCompletionSource exchangeStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource releaseExchange = new(TaskCreationOptions.RunContinuationsAsynchronously);
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
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
        OpenAIClient client = new(credential);
        Task<PipelineMessage> owner = SendAsync(client);
        await exchangeStarted.Task.WaitAsync(TimeSpan.FromSeconds(10));

        using CancellationTokenSource cancellation = new();
        Task<PipelineMessage> waiter = SendAsync(client, cancellationToken: cancellation.Token);
        cancellation.Cancel();

        Assert.ThrowsAsync<OperationCanceledException>(async () => await waiter);
        releaseExchange.TrySetResult();
        using PipelineMessage completed = await owner.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.That(completed.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
    }

    [TestCase("0")]
    [TestCase("-1")]
    [TestCase("\"3600\"")]
    [TestCase("null")]
    [TestCase("0.000000000001")]
    public async Task RejectsInvalidTokenLifetime(string lifetime)
    {
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (!exchange)
            {
                return false;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"access_token\":\"secret-token\",\"expires_in\":" + lifetime + "}");
            return true;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await SendAsync(client));

        Assert.That(exception.Message, Does.Contain("lifetime"));
        Assert.That(exception.Message, Does.Not.Contain("secret-token"));
        Assert.That(server.ApiCount, Is.Zero);
    }

    [Test]
    public async Task RefreshBufferIsCappedAtHalfTokenLifetime()
    {
        int issuedTokenCount = 0;
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (!exchange)
            {
                return false;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                "{\"access_token\":\"token-" + Interlocked.Increment(ref issuedTokenCount) + "\",\"expires_in\":1}");
            return true;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        using PipelineMessage first = await SendAsync(client);
        using PipelineMessage second = await SendAsync(client);
        Assert.That(server.ExchangeCount, Is.EqualTo(1));

        await Task.Delay(650);
        using PipelineMessage third = await SendAsync(client);

        Assert.That(server.ExchangeCount, Is.EqualTo(2));
    }

    [Test]
    public async Task UnauthorizedReplayableRequestRefreshesExactlyOnce()
    {
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (!exchange && context.Request.Headers.Authorization == "Bearer token-1")
            {
                context.Response.StatusCode = 401;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        using BinaryContent content = BinaryContent.Create(BinaryData.FromString("buffered"));

        using PipelineMessage message = await SendAsync(client, content);

        Assert.That(message.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(2));
        Assert.That(server.ApiCount, Is.EqualTo(2));
    }

    [Test]
    public async Task UnauthorizedSeekableStreamIsSafelyReplayed()
    {
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (!exchange && context.Request.Headers.Authorization == "Bearer token-1")
            {
                context.Response.StatusCode = 401;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        using MemoryStream stream = new(Encoding.UTF8.GetBytes("seekable payload"));
        using BinaryContent content = BinaryContent.Create(stream);

        using PipelineMessage message = await SendAsync(client, content);

        Assert.That(message.Response.Status, Is.EqualTo(200));
        Assert.That(server.ApiCount, Is.EqualTo(2));
        Assert.That(server.Requests.Where(request => request.Host == "mtls.api.openai.com")
            .All(request => request.Body == "seekable payload"), Is.True);
    }

    [Test]
    public async Task UnauthorizedNonReplayableRequestIsNeverRetried()
    {
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (!exchange)
            {
                context.Response.StatusCode = 401;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        using NonReplayableContent content = new();

        using PipelineMessage message = await SendAsync(client, content);

        Assert.That(message.Response.Status, Is.EqualTo(401));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.ApiCount, Is.EqualTo(1));
        Assert.That(content.WriteCount, Is.EqualTo(1));
    }

    [Test]
    public async Task UnauthorizedNonReplayableRequestInvalidatesTokenForNextRequest()
    {
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (!exchange && context.Request.Headers.Authorization == "Bearer token-1")
            {
                context.Response.StatusCode = 401;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        using NonReplayableContent firstContent = new();
        using NonReplayableContent secondContent = new();

        using PipelineMessage first = await SendAsync(client, firstContent);
        using PipelineMessage second = await SendAsync(client, secondContent);

        Assert.That(first.Response.Status, Is.EqualTo(401));
        Assert.That(second.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(2));
        Assert.That(server.ApiCount, Is.EqualTo(2));
        Assert.That(firstContent.WriteCount, Is.EqualTo(1));
        Assert.That(secondContent.WriteCount, Is.EqualTo(1));
    }

    [Test]
    public async Task PersistentUnauthorizedResponseIsRetriedOnlyOnce()
    {
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (!exchange)
            {
                context.Response.StatusCode = 401;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        using PipelineMessage message = await SendAsync(client);

        Assert.That(message.Response.Status, Is.EqualTo(401));
        Assert.That(server.ExchangeCount, Is.EqualTo(2));
        Assert.That(server.ApiCount, Is.EqualTo(2));
    }

    [Test]
    public async Task UnauthorizedReplayInvalidatesReplacementTokenForNextRequest()
    {
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (!exchange && context.Request.Headers.Authorization != "Bearer token-3")
            {
                context.Response.StatusCode = 401;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        using PipelineMessage rejected = await SendAsync(client);
        using NonReplayableContent nextContent = new();
        using PipelineMessage next = await SendAsync(client, nextContent);

        Assert.That(rejected.Response.Status, Is.EqualTo(401));
        Assert.That(next.Response.Status, Is.EqualTo(200));
        Assert.That(server.ExchangeCount, Is.EqualTo(3));
        Assert.That(server.ApiCount, Is.EqualTo(3));
        Assert.That(nextContent.WriteCount, Is.EqualTo(1));
    }

    [Test]
    public async Task TransientExchangeErrorsAreRetried()
    {
        int exchangeAttempts = 0;
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (exchange && Interlocked.Increment(ref exchangeAttempts) < 3)
            {
                context.Response.StatusCode = exchangeAttempts == 1 ? 429 : 503;
                context.Response.Headers.RetryAfter = "0";
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        using PipelineMessage message = await SendAsync(client);

        Assert.That(server.ExchangeCount, Is.EqualTo(3));
        Assert.That(message.Response.Status, Is.EqualTo(200));
    }

    [Test]
    public async Task TransientExchangeRetriesAreBounded()
    {
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (exchange)
            {
                context.Response.StatusCode = 503;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        Assert.ThrowsAsync<InvalidOperationException>(async () => await SendAsync(client));

        Assert.That(server.ExchangeCount, Is.EqualTo(3));
        Assert.That(server.ApiCount, Is.Zero);
    }

    [Test]
    public void NetworkExchangeRetriesAreNotMultipliedByPipelineRetries()
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
                throw new HttpRequestException("simulated token endpoint connection failure");
            },
        };
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential, new() { RetryPolicy = new ClientRetryPolicy(2) });

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await SendAsync(client));

        Assert.That(exception.Message, Does.Contain("exhausted"));
        Assert.That(connectionAttempts, Is.EqualTo(3));
    }

    [TestCase(408)]
    [TestCase(409)]
    [TestCase(429)]
    [TestCase(500)]
    [TestCase(503)]
    public async Task SupportedTransientExchangeStatusesAreRetried(int status)
    {
        int exchangeAttempts = 0;
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (exchange && Interlocked.Increment(ref exchangeAttempts) == 1)
            {
                context.Response.StatusCode = status;
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
    }

    [Test]
    public async Task ExchangeRetriesHonorRetryAfter()
    {
        int exchangeAttempts = 0;
        await using TestServer server = await TestServer.StartAsync((context, exchange) =>
        {
            if (exchange && Interlocked.Increment(ref exchangeAttempts) == 1)
            {
                context.Response.StatusCode = 429;
                context.Response.Headers.RetryAfter = "1";
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        Stopwatch elapsed = Stopwatch.StartNew();

        using PipelineMessage message = await SendAsync(client);

        Assert.That(server.ExchangeCount, Is.EqualTo(2));
        Assert.That(elapsed.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(900)));
    }

    [TestCase(400)]
    [TestCase(401)]
    [TestCase(403)]
    [TestCase(307)]
    public async Task PermanentExchangeErrorsAndRedirectsAreNotRetried(int status)
    {
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (!exchange)
            {
                return false;
            }

            context.Response.StatusCode = status;
            context.Response.Headers.Location = "https://attacker.example.test/oauth/token";
            await context.Response.WriteAsync("sensitive-error-body");
            return true;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await SendAsync(client));

        Assert.That(exception.Message, Does.Contain(status.ToString()));
        Assert.That(exception.Message, Does.Not.Contain("sensitive-error-body"));
        Assert.That(server.ExchangeCount, Is.EqualTo(1));
        Assert.That(server.Requests.Any(request => request.Host == "attacker.example.test"), Is.False);
    }

    [Test]
    public async Task ExchangeErrorsDoNotExposeTokensCertificateMaterialOrProviderDetails()
    {
        const string secretToken = "secret-access-token";
        const string certificate = "-----BEGIN PRIVATE KEY-----";
        const string providerDetails = "sensitive-provider-attributes";
        await using TestServer server = await TestServer.StartAsync(async (context, exchange) =>
        {
            if (!exchange)
            {
                return false;
            }

            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(secretToken + certificate + providerDetails);
            return true;
        });
        using SocketsHttpHandler handler = server.CreateHandler();
        using X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await SendAsync(client));

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Not.Contain(secretToken));
            Assert.That(exception.Message, Does.Not.Contain(certificate));
            Assert.That(exception.Message, Does.Not.Contain(providerDetails));
        });
    }

    [Test]
    public async Task CredentialDisposalDoesNotDisposeCallerOwnedHandler()
    {
        await using TestServer server = await TestServer.StartAsync();
        using SocketsHttpHandler handler = server.CreateHandler();
        X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);
        using PipelineMessage initial = await SendAsync(client);

        credential.Dispose();

        using HttpClient anotherClient = new(handler, disposeHandler: false);
        using HttpResponseMessage response = await anotherClient.GetAsync("https://mtls.api.openai.com/v1/check");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task BothHttpsLegsPresentTheSameCallerOwnedClientCertificate()
    {
        using CertificateFixture certificates = CertificateFixture.Create();
        await using TestServer server = await TestServer.StartAsync(
            requiredClientCertificate: certificates.ClientCertificate);
        using SocketsHttpHandler handler = server.CreateHandler();
        handler.SslOptions.ClientCertificateContext = SslStreamCertificateContext.Create(
            certificates.ClientCertificate,
            new X509Certificate2Collection(certificates.IntermediateCertificate),
            offline: true);
        X509WorkloadIdentityCredential credential = CreateCredential(handler);
        OpenAIClient client = new(credential);

        using PipelineMessage message = await SendAsync(client);
        credential.Dispose();

        Assert.Multiple(() =>
        {
            Assert.That(server.Requests, Has.Count.EqualTo(2));
            Assert.That(server.Requests.All(request =>
                request.ClientCertificateThumbprint == certificates.ClientCertificate.Thumbprint), Is.True);
            Assert.That(certificates.ClientCertificate.HasPrivateKey, Is.True);
        });

        using RSA key = certificates.ClientCertificate.GetRSAPrivateKey();
        byte[] signature = key.SignData([1, 2, 3], HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        Assert.That(signature, Is.Not.Empty);
    }

    private static X509WorkloadIdentityCredential CreateCredential(HttpMessageHandler handler)
    {
        return new("idp_test", "svc_test", new() { Handler = handler });
    }

    private static async Task<PipelineMessage> SendAsync(
        OpenAIClient client,
        BinaryContent content = null,
        CancellationToken cancellationToken = default)
    {
        PipelineMessage message = client.Pipeline.CreateMessage();
        message.Request.Method = content is null ? "GET" : "POST";
        message.Request.Uri = new Uri(client.Endpoint.AbsoluteUri.TrimEnd('/') + "/test");
        message.Request.Content = content;
        message.Apply(new RequestOptions { CancellationToken = cancellationToken });

        try
        {
            await client.Pipeline.SendAsync(message);
            return message;
        }
        catch
        {
            message.Dispose();
            throw;
        }
    }

    private sealed class UnsupportedHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class NonReplayableContent : BinaryContent
    {
        internal int WriteCount { get; private set; }

        public override bool TryComputeLength(out long length)
        {
            length = 7;
            return true;
        }

        public override void WriteTo(Stream stream, CancellationToken cancellationToken = default)
        {
            WriteCount++;
            byte[] value = Encoding.UTF8.GetBytes("payload");
            stream.Write(value, 0, value.Length);
        }

        public override Task WriteToAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            WriteTo(stream, cancellationToken);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
        }
    }

    private sealed record RequestRecord(
        string Host,
        string Method,
        string Path,
        string Authorization,
        string Body,
        string ClientCertificateThumbprint);

    private sealed class TestServer : IAsyncDisposable
    {
        private readonly WebApplication _application;
        private readonly X509Certificate2 _certificate;
        private readonly Func<HttpContext, bool, Task<bool>> _intercept;
        private readonly ConcurrentQueue<RequestRecord> _requests = new();
        private int _exchangeCount;
        private int _apiCount;

        private TestServer(WebApplication application, X509Certificate2 certificate, Func<HttpContext, bool, Task<bool>> intercept)
        {
            _application = application;
            _certificate = certificate;
            _intercept = intercept;
        }

        internal IReadOnlyCollection<RequestRecord> Requests => _requests.ToArray();
        internal int ExchangeCount => Volatile.Read(ref _exchangeCount);
        internal int ApiCount => Volatile.Read(ref _apiCount);

        internal static async Task<TestServer> StartAsync(
            Func<HttpContext, bool, Task<bool>> intercept = null,
            X509Certificate2 requiredClientCertificate = null)
        {
            using ECDsa key = ECDsa.Create();
            CertificateRequest request = new("CN=localhost", key, HashAlgorithmName.SHA256);
            X509Certificate2 certificate = request.CreateSelfSigned(
                DateTimeOffset.UtcNow.AddMinutes(-1),
                DateTimeOffset.UtcNow.AddHours(1));
            WebApplicationBuilder builder = WebApplication.CreateSlimBuilder();
            builder.Logging.ClearProviders();
            builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Loopback, 0, listen =>
                listen.UseHttps(https =>
                {
                    https.ServerCertificate = certificate;
                    if (requiredClientCertificate is not null)
                    {
                        https.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                        https.ClientCertificateValidation = (presented, _, _) =>
                            presented.Thumbprint == requiredClientCertificate.Thumbprint;
                    }
                })));
            WebApplication application = builder.Build();
            TestServer server = new(application, certificate, intercept);
            application.Run(server.HandleAsync);
            await application.StartAsync();
            return server;
        }

        internal SocketsHttpHandler CreateHandler()
        {
            int port = new Uri(_application.Urls.Single()).Port;
            return new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                UseCookies = false,
                UseProxy = false,
                SslOptions = new SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (_, certificate, _, _) =>
                        certificate is not null
                        && certificate.GetCertHashString() == _certificate.GetCertHashString(),
                },
                ConnectCallback = async (_, cancellationToken) =>
                {
                    TcpClient client = new();
                    try
                    {
                        await client.ConnectAsync(IPAddress.Loopback, port, cancellationToken);
                        return client.GetStream();
                    }
                    catch
                    {
                        client.Dispose();
                        throw;
                    }
                },
            };
        }

        public async ValueTask DisposeAsync()
        {
            await _application.StopAsync();
            await _application.DisposeAsync();
            _certificate.Dispose();
        }

        private async Task HandleAsync(HttpContext context)
        {
            using StreamReader reader = new(context.Request.Body);
            string body = await reader.ReadToEndAsync();
            string host = context.Request.Host.Host;
            bool exchange = host == "mtls.auth.openai.com";
            _requests.Enqueue(new RequestRecord(
                host,
                context.Request.Method,
                context.Request.Path,
                context.Request.Headers.Authorization.ToString(),
                body,
                context.Connection.ClientCertificate?.Thumbprint));

            if (exchange)
            {
                Interlocked.Increment(ref _exchangeCount);
            }
            else
            {
                Interlocked.Increment(ref _apiCount);
            }

            if (_intercept is not null && await _intercept(context, exchange))
            {
                return;
            }

            if (exchange)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    "{\"access_token\":\"token-" + ExchangeCount + "\",\"expires_in\":3600}");
            }
            else
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("{}");
            }
        }
    }
}
