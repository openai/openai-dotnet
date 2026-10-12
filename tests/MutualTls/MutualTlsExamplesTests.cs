using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.MutualTls;

[NonParallelizable]
[Category("MutualTls")]
public class MutualTlsExamplesTests
{
    private const string ApiKey = "test-api-key";
    private const string PfxPassword = "test-password";

    private CertificateFixture _certificates;

    [SetUp]
    public void CreateCertificates()
    {
        _certificates = CertificateFixture.Create();
    }

    [TearDown]
    public void DisposeCertificates()
    {
        _certificates?.Dispose();
        _certificates = null;
    }

#if NET9_0_OR_GREATER
    [Test]
    public async Task FullChainCompletesSdkRequestEndToEnd()
    {
        await using MutualTlsServer server = MutualTlsServer.Start(
            _certificates.ServerCertificate,
            _certificates.RootCertificate);
        using DisposableCertificateCollection imported =
            DisposableCertificateCollection.Load(
                _certificates.ExportClientBundle(PfxPassword),
                PfxPassword);
        using HttpClient httpClient = CreateHttpClient(
            imported.Certificates,
            _certificates.ServerCertificate);

        ChatClient client = CreateChatClient(server.Endpoint, httpClient);
        ChatCompletion completion;
        try
        {
            completion = await client.CompleteChatAsync(
                "Reply with exactly: mTLS request succeeded.");
        }
        catch (Exception clientException)
        {
            await server.Completion.WaitAsync(TimeSpan.FromSeconds(10));
            Assert.Fail(
                $"Client request failed: {clientException}\n"
                + $"Server failure: {server.Failure}\n"
                + "Presented chain: "
                + string.Join(", ", server.PresentedClientChainThumbprints));
            throw;
        }
        await server.Completion.WaitAsync(TimeSpan.FromSeconds(10));
        using JsonDocument requestBody = JsonDocument.Parse(server.Request.Body);

        Assert.Multiple(() =>
        {
            Assert.That(imported.Certificates, Has.Count.EqualTo(2));
            Assert.That(
                imported.Certificates.Count(certificate => certificate.HasPrivateKey),
                Is.EqualTo(1));
            Assert.That(completion.Content[0].Text, Is.EqualTo("mTLS request succeeded."));
            Assert.That(server.Request.Method, Is.EqualTo("POST"));
            Assert.That(server.Request.Path, Is.EqualTo("/v1/chat/completions"));
            Assert.That(
                requestBody.RootElement.GetProperty("model").GetString(),
                Is.EqualTo("gpt-4o-mini"));
            Assert.That(
                requestBody.RootElement
                    .GetProperty("messages")[0]
                    .GetProperty("content")
                    .GetString(),
                Is.EqualTo("Reply with exactly: mTLS request succeeded."));
            Assert.That(
                server.Request.Headers["Authorization"],
                Is.EqualTo($"Bearer {ApiKey}"));
            Assert.That(server.ClientCertificateWasAccepted, Is.True);
            Assert.That(
                server.PresentedClientChainThumbprints,
                Does.Contain(_certificates.IntermediateCertificate.Thumbprint));
        });
    }
#endif

    [Test]
    public async Task MissingIntermediateFailsClosed()
    {
        await using MutualTlsServer server = MutualTlsServer.Start(
            _certificates.ServerCertificate,
            _certificates.RootCertificate);
        using HttpClient httpClient = CreateHttpClient(
            new X509Certificate2Collection(_certificates.ClientCertificate),
            _certificates.ServerCertificate);

        ChatClient client = CreateChatClient(server.Endpoint, httpClient);

        Assert.That(
            async () => await client.CompleteChatAsync("Hello"),
            Throws.InstanceOf<ClientResultException>()
                .Or.InstanceOf<OperationCanceledException>());
        await server.Completion.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Multiple(() =>
        {
            Assert.That(server.Request, Is.Null);
            Assert.That(server.ClientCertificateWasAccepted, Is.False);
            Assert.That(
                server.PresentedClientChainThumbprints,
                Does.Not.Contain(_certificates.IntermediateCertificate.Thumbprint));
            Assert.That(server.Failure, Is.Not.Null);
        });
    }

    [Test]
    public async Task MissingClientCertificateFailsClosed()
    {
        await using MutualTlsServer server = MutualTlsServer.Start(
            _certificates.ServerCertificate,
            _certificates.RootCertificate);
        using HttpClient httpClient = CreateHttpClient(
            new X509Certificate2Collection(),
            _certificates.ServerCertificate);

        ChatClient client = CreateChatClient(server.Endpoint, httpClient);

        Assert.That(
            async () => await client.CompleteChatAsync("Hello"),
            Throws.InstanceOf<ClientResultException>()
                .Or.InstanceOf<OperationCanceledException>());
        await server.Completion.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Multiple(() =>
        {
            Assert.That(server.Request, Is.Null);
            Assert.That(server.ClientCertificateWasAccepted, Is.False);
            Assert.That(server.Failure, Is.Not.Null);
        });
    }

    [Test]
    public async Task CertificateBearingHandlerDoesNotFollowRedirects()
    {
        await using MutualTlsServer redirectTarget = MutualTlsServer.Start(
            _certificates.ServerCertificate,
            _certificates.RootCertificate);
        await using MutualTlsServer redirectSource = MutualTlsServer.Start(
            _certificates.ServerCertificate,
            _certificates.RootCertificate,
            redirectLocation: new Uri(redirectTarget.Endpoint, "chat/completions"));
        using HttpClient httpClient = CreateHttpClient(
            new X509Certificate2Collection(
                new X509Certificate2[]
                {
                    _certificates.ClientCertificate,
                    _certificates.IntermediateCertificate,
                }),
            _certificates.ServerCertificate);

        ChatClient client = CreateChatClient(redirectSource.Endpoint, httpClient);

        Assert.That(
            async () => await client.CompleteChatAsync("Hello"),
            Throws.TypeOf<ClientResultException>()
                .With.Property(nameof(ClientResultException.Status))
                .EqualTo((int)HttpStatusCode.TemporaryRedirect));
        await redirectSource.Completion.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Multiple(() =>
        {
            Assert.That(redirectSource.Request, Is.Not.Null);
            Assert.That(redirectTarget.ConnectionCount, Is.Zero);
        });
    }

    [Test]
    public async Task UntrustedClientCertificateFailsClosed()
    {
        using CertificateFixture untrustedCertificates = CertificateFixture.Create();
        await using MutualTlsServer server = MutualTlsServer.Start(
            _certificates.ServerCertificate,
            _certificates.RootCertificate);
        using HttpClient httpClient = CreateHttpClient(
            new X509Certificate2Collection(
                new X509Certificate2[]
                {
                    untrustedCertificates.ClientCertificate,
                    untrustedCertificates.IntermediateCertificate,
                }),
            _certificates.ServerCertificate);

        ChatClient client = CreateChatClient(server.Endpoint, httpClient);

        Assert.That(
            async () => await client.CompleteChatAsync("Hello"),
            Throws.InstanceOf<ClientResultException>()
                .Or.InstanceOf<OperationCanceledException>());
        await server.Completion.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Multiple(() =>
        {
            Assert.That(server.Request, Is.Null);
            Assert.That(server.ClientCertificateWasAccepted, Is.False);
            Assert.That(server.Failure, Is.Not.Null);
        });
    }

    [Test]
    public async Task InvalidApiKeyIsRejectedAfterSuccessfulTlsAuthentication()
    {
        const string invalidApiKey = "invalid-api-key";
        await using MutualTlsServer server = MutualTlsServer.Start(
            _certificates.ServerCertificate,
            _certificates.RootCertificate,
            requiredApiKey: ApiKey);
        using HttpClient httpClient = CreateHttpClient(
            new X509Certificate2Collection(
                new X509Certificate2[]
                {
                    _certificates.ClientCertificate,
                    _certificates.IntermediateCertificate,
                }),
            _certificates.ServerCertificate);

        ChatClient client =
            CreateChatClient(server.Endpoint, httpClient, invalidApiKey);

        Assert.That(
            async () => await client.CompleteChatAsync("Hello"),
            Throws.TypeOf<ClientResultException>()
                .With.Property(nameof(ClientResultException.Status))
                .EqualTo((int)HttpStatusCode.Unauthorized));
        await server.Completion.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Multiple(() =>
        {
            Assert.That(server.ClientCertificateWasAccepted, Is.True);
            Assert.That(server.Request, Is.Not.Null);
            Assert.That(
                server.Request.Headers["Authorization"],
                Is.EqualTo($"Bearer {invalidApiKey}"));
        });
    }

    [Test]
    public async Task RequestBodyIsReadUsingContentLengthBytes()
    {
        const string requestBody = "Zażółć gęślą jaźń ☕";
        await using MutualTlsServer server = MutualTlsServer.Start(
            _certificates.ServerCertificate,
            _certificates.RootCertificate);
        using HttpClient httpClient = CreateHttpClient(
            new X509Certificate2Collection(
                new X509Certificate2[]
                {
                    _certificates.ClientCertificate,
                    _certificates.IntermediateCertificate,
                }),
            _certificates.ServerCertificate);

        using StringContent requestContent =
            new(requestBody, Encoding.UTF8);
        using HttpResponseMessage response = await httpClient.PostAsync(
            new Uri(server.Endpoint, "unicode"),
            requestContent);
        await server.Completion.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(server.Request.Body, Is.EqualTo(requestBody));
        });
    }

    private static HttpClient CreateHttpClient(
        X509Certificate2Collection clientCertificates,
        X509Certificate2 serverCertificate)
    {
        SocketsHttpHandler handler = new()
        {
            AllowAutoRedirect = false,
        };
        handler.SslOptions.RemoteCertificateValidationCallback =
            (_, certificate, _, _) =>
                certificate?.GetCertHashString(HashAlgorithmName.SHA256)
                    == serverCertificate.GetCertHashString(HashAlgorithmName.SHA256);
        if (clientCertificates.Count > 0)
        {
            X509Certificate2 clientCertificate =
                clientCertificates.Single(certificate => certificate.HasPrivateKey);
            X509Certificate2Collection intermediateCertificates = new(
                clientCertificates
                    .Where(certificate => !certificate.HasPrivateKey)
                    .ToArray());
            handler.SslOptions.ClientCertificateContext =
                SslStreamCertificateContext.Create(
                    clientCertificate,
                    intermediateCertificates,
                    offline: true);
        }

        return new HttpClient(handler)
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };
    }

    private static ChatClient CreateChatClient(
        Uri endpoint,
        HttpClient httpClient,
        string apiKey = ApiKey)
    {
        OpenAIClientOptions options = new()
        {
            Endpoint = endpoint,
            NetworkTimeout = TimeSpan.FromSeconds(5),
            RetryPolicy = new ClientRetryPolicy(maxRetries: 0),
            Transport = new HttpClientPipelineTransport(httpClient),
        };

        OpenAIClient client = new(new ApiKeyCredential(apiKey), options);
        return client.GetChatClient("gpt-4o-mini");
    }

#if NET9_0_OR_GREATER
    private sealed class DisposableCertificateCollection : IDisposable
    {
        public X509Certificate2Collection Certificates { get; }

        private DisposableCertificateCollection(X509Certificate2Collection certificates)
        {
            Certificates = certificates;
        }

        public static DisposableCertificateCollection Load(byte[] pfx, string password)
        {
            X509KeyStorageFlags keyStorageFlags = OperatingSystem.IsWindows()
                ? X509KeyStorageFlags.DefaultKeySet
                : X509KeyStorageFlags.EphemeralKeySet;
            X509Certificate2Collection certificates =
                X509CertificateLoader.LoadPkcs12Collection(
                    pfx,
                    password,
                    keyStorageFlags);
            return new DisposableCertificateCollection(certificates);
        }

        public void Dispose()
        {
            foreach (X509Certificate2 certificate in Certificates)
            {
                certificate.Dispose();
            }
        }
    }
#endif
}
