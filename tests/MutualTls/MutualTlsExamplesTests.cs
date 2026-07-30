using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
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
    private const string ClientAuthenticationOid = "1.3.6.1.5.5.7.3.2";
    private const string ServerAuthenticationOid = "1.3.6.1.5.5.7.3.1";
    private const string PfxPassword = "test-password";

    // Loading a PFX private key affects the temporary key store on macOS, so run it last.
    [Test]
    [Order(6)]
    public async Task FullChainCompletesSdkRequestEndToEnd()
    {
        using CertificateFixture certificates = CertificateFixture.Create();
        await using MutualTlsServer server = MutualTlsServer.Start(
            certificates.ServerCertificate,
            certificates.RootCertificate);
        using DisposableCertificateCollection imported =
            DisposableCertificateCollection.Load(
                certificates.ExportClientBundle(),
                PfxPassword);
        using HttpClient httpClient = CreateHttpClient(
            imported.Certificates,
            certificates.ServerCertificate);

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
            Assert.That(server.ClientCertificateWasTrusted, Is.True);
            Assert.That(
                server.PresentedClientChainThumbprints,
                Does.Contain(certificates.IntermediateCertificate.Thumbprint));
        });
    }

    [Test]
    [Order(2)]
    public async Task MissingIntermediateFailsClosed()
    {
        using CertificateFixture certificates = CertificateFixture.Create();
        await using MutualTlsServer server = MutualTlsServer.Start(
            certificates.ServerCertificate,
            certificates.RootCertificate);
        using HttpClient httpClient = CreateHttpClient(
            new X509Certificate2Collection(certificates.ClientCertificate),
            certificates.ServerCertificate);

        ChatClient client = CreateChatClient(server.Endpoint, httpClient);

        Exception exception =
            Assert.CatchAsync(
                async () => await client.CompleteChatAsync("Hello"));
        await server.Completion.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Multiple(() =>
        {
            Assert.That(
                exception,
                Is.InstanceOf<ClientResultException>()
                    .Or.InstanceOf<OperationCanceledException>());
            Assert.That(server.Request, Is.Null);
            Assert.That(server.ClientCertificateWasTrusted, Is.False);
            Assert.That(
                server.PresentedClientChainThumbprints,
                Does.Not.Contain(certificates.IntermediateCertificate.Thumbprint));
            Assert.That(server.Failure, Is.Not.Null);
        });
    }

    [Test]
    [Order(1)]
    public async Task MissingClientCertificateFailsClosed()
    {
        using CertificateFixture certificates = CertificateFixture.Create();
        await using MutualTlsServer server = MutualTlsServer.Start(
            certificates.ServerCertificate,
            certificates.RootCertificate);
        using HttpClient httpClient = CreateHttpClient(
            new X509Certificate2Collection(),
            certificates.ServerCertificate);

        ChatClient client = CreateChatClient(server.Endpoint, httpClient);

        Exception exception =
            Assert.CatchAsync(
                async () => await client.CompleteChatAsync("Hello"));
        await server.Completion.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Multiple(() =>
        {
            Assert.That(
                exception,
                Is.InstanceOf<ClientResultException>()
                    .Or.InstanceOf<OperationCanceledException>());
            Assert.That(server.Request, Is.Null);
            Assert.That(server.ClientCertificateWasTrusted, Is.False);
            Assert.That(server.Failure, Is.Not.Null);
        });
    }

    [Test]
    [Order(5)]
    public async Task CertificateBearingHandlerDoesNotFollowRedirects()
    {
        using CertificateFixture certificates = CertificateFixture.Create();
        await using MutualTlsServer redirectTarget = MutualTlsServer.Start(
            certificates.ServerCertificate,
            certificates.RootCertificate);
        await using MutualTlsServer redirectSource = MutualTlsServer.Start(
            certificates.ServerCertificate,
            certificates.RootCertificate,
            redirectLocation: new Uri(redirectTarget.Endpoint, "chat/completions"));
        using HttpClient httpClient = CreateHttpClient(
            new X509Certificate2Collection(
                new X509Certificate2[]
                {
                    certificates.ClientCertificate,
                    certificates.IntermediateCertificate,
                }),
            certificates.ServerCertificate);

        ChatClient client = CreateChatClient(redirectSource.Endpoint, httpClient);

        ClientResultException exception =
            Assert.ThrowsAsync<ClientResultException>(
                async () => await client.CompleteChatAsync("Hello"));
        await redirectSource.Completion.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Multiple(() =>
        {
            Assert.That(exception.Status, Is.EqualTo((int)HttpStatusCode.TemporaryRedirect));
            Assert.That(redirectSource.Request, Is.Not.Null);
            Assert.That(redirectTarget.ConnectionCount, Is.Zero);
        });
    }

    [Test]
    [Order(3)]
    public async Task UntrustedClientCertificateFailsClosed()
    {
        using CertificateFixture trustedCertificates = CertificateFixture.Create();
        using CertificateFixture untrustedCertificates = CertificateFixture.Create();
        await using MutualTlsServer server = MutualTlsServer.Start(
            trustedCertificates.ServerCertificate,
            trustedCertificates.RootCertificate);
        using HttpClient httpClient = CreateHttpClient(
            new X509Certificate2Collection(
                new X509Certificate2[]
                {
                    untrustedCertificates.ClientCertificate,
                    untrustedCertificates.IntermediateCertificate,
                }),
            trustedCertificates.ServerCertificate);

        ChatClient client = CreateChatClient(server.Endpoint, httpClient);

        Exception exception =
            Assert.CatchAsync(
                async () => await client.CompleteChatAsync("Hello"));
        await server.Completion.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Multiple(() =>
        {
            Assert.That(
                exception,
                Is.InstanceOf<ClientResultException>()
                    .Or.InstanceOf<OperationCanceledException>());
            Assert.That(server.Request, Is.Null);
            Assert.That(server.ClientCertificateWasTrusted, Is.False);
            Assert.That(server.Failure, Is.Not.Null);
        });
    }

    [Test]
    [Order(4)]
    public async Task InvalidApiKeyIsRejectedAfterSuccessfulTlsAuthentication()
    {
        const string invalidApiKey = "invalid-api-key";
        using CertificateFixture certificates = CertificateFixture.Create();
        await using MutualTlsServer server = MutualTlsServer.Start(
            certificates.ServerCertificate,
            certificates.RootCertificate,
            requiredApiKey: ApiKey);
        using HttpClient httpClient = CreateHttpClient(
            new X509Certificate2Collection(
                new X509Certificate2[]
                {
                    certificates.ClientCertificate,
                    certificates.IntermediateCertificate,
                }),
            certificates.ServerCertificate);

        ChatClient client =
            CreateChatClient(server.Endpoint, httpClient, invalidApiKey);

        ClientResultException exception =
            Assert.ThrowsAsync<ClientResultException>(
                async () => await client.CompleteChatAsync("Hello"));
        await server.Completion.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Multiple(() =>
        {
            Assert.That(exception.Status, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            Assert.That(server.ClientCertificateWasTrusted, Is.True);
            Assert.That(server.Request, Is.Not.Null);
            Assert.That(
                server.Request.Headers["Authorization"],
                Is.EqualTo($"Bearer {invalidApiKey}"));
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

        return new HttpClient(handler);
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

    private sealed class DisposableCertificateCollection : IDisposable
    {
        public X509Certificate2Collection Certificates { get; }

        private DisposableCertificateCollection(X509Certificate2Collection certificates)
        {
            Certificates = certificates;
        }

        public static DisposableCertificateCollection Load(byte[] pfx, string password)
        {
#if NET9_0_OR_GREATER
            X509Certificate2Collection certificates =
                X509CertificateLoader.LoadPkcs12Collection(
                    pfx,
                    password,
                    X509KeyStorageFlags.DefaultKeySet);
#else
            X509Certificate2Collection certificates = new();
#pragma warning disable SYSLIB0057
            certificates.Import(
                pfx,
                password,
                X509KeyStorageFlags.DefaultKeySet);
#pragma warning restore SYSLIB0057
#endif
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

    private sealed class CertificateFixture : IDisposable
    {
        public X509Certificate2 RootCertificate { get; }
        public X509Certificate2 IntermediateCertificate { get; }
        public X509Certificate2 ClientCertificate { get; }
        public X509Certificate2 ServerCertificate { get; }

        private CertificateFixture(
            X509Certificate2 rootCertificate,
            X509Certificate2 intermediateCertificate,
            X509Certificate2 clientCertificate,
            X509Certificate2 serverCertificate)
        {
            RootCertificate = rootCertificate;
            IntermediateCertificate = intermediateCertificate;
            ClientCertificate = clientCertificate;
            ServerCertificate = serverCertificate;
        }

        public static CertificateFixture Create()
        {
            DateTimeOffset notBefore = DateTimeOffset.UtcNow.AddMinutes(-5);
            DateTimeOffset notAfter = DateTimeOffset.UtcNow.AddHours(1);
            string certificateSetId = Guid.NewGuid().ToString("N");

            using RSA rootKey = RSA.Create(2048);
            CertificateRequest rootRequest = CreateCertificateRequest(
                $"CN=OpenAI mTLS Test Root {certificateSetId}",
                rootKey);
            rootRequest.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(
                    certificateAuthority: true,
                    hasPathLengthConstraint: true,
                    pathLengthConstraint: 1,
                    critical: true));
            rootRequest.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
                    critical: true));
            AddSubjectKeyIdentifier(rootRequest);
            X509Certificate2 rootCertificate =
                rootRequest.CreateSelfSigned(notBefore, notAfter);

            using RSA intermediateKey = RSA.Create(2048);
            CertificateRequest intermediateRequest = CreateCertificateRequest(
                $"CN=OpenAI mTLS Test Intermediate {certificateSetId}",
                intermediateKey);
            intermediateRequest.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(
                    certificateAuthority: true,
                    hasPathLengthConstraint: true,
                    pathLengthConstraint: 0,
                    critical: true));
            intermediateRequest.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
                    critical: true));
            AddSubjectKeyIdentifier(intermediateRequest);
            using X509Certificate2 intermediatePublicCertificate =
                intermediateRequest.Create(
                    rootCertificate,
                    notBefore,
                    notAfter,
                    CreateSerialNumber());
            X509Certificate2 intermediateCertificateWithPrivateKey =
                intermediatePublicCertificate.CopyWithPrivateKey(intermediateKey);

            using RSA clientKey = RSA.Create(2048);
            CertificateRequest clientRequest = CreateCertificateRequest(
                $"CN=OpenAI mTLS Test Client {certificateSetId}",
                clientKey);
            AddEndEntityExtensions(clientRequest, ClientAuthenticationOid);
            using X509Certificate2 clientPublicCertificate =
                clientRequest.Create(
                    intermediateCertificateWithPrivateKey,
                    notBefore,
                    notAfter,
                    CreateSerialNumber());
            X509Certificate2 clientCertificate =
                clientPublicCertificate.CopyWithPrivateKey(clientKey);
            X509Certificate2 intermediateCertificate =
                LoadPublicCertificate(
                    intermediateCertificateWithPrivateKey.RawData);
            intermediateCertificateWithPrivateKey.Dispose();

            using RSA serverKey = RSA.Create(2048);
            CertificateRequest serverRequest = CreateCertificateRequest(
                "CN=127.0.0.1",
                serverKey);
            AddEndEntityExtensions(serverRequest, ServerAuthenticationOid);
            SubjectAlternativeNameBuilder subjectAlternativeNames = new();
            subjectAlternativeNames.AddIpAddress(IPAddress.Loopback);
            subjectAlternativeNames.AddDnsName("localhost");
            serverRequest.CertificateExtensions.Add(subjectAlternativeNames.Build());
            using X509Certificate2 serverPublicCertificate =
                serverRequest.Create(
                    rootCertificate,
                    notBefore,
                    notAfter,
                    CreateSerialNumber());
            X509Certificate2 serverCertificate =
                serverPublicCertificate.CopyWithPrivateKey(serverKey);

            return new CertificateFixture(
                rootCertificate,
                intermediateCertificate,
                clientCertificate,
                serverCertificate);
        }

        public byte[] ExportClientBundle()
        {
            X509Certificate2Collection certificates =
                new(
                    new X509Certificate2[]
                    {
                        ClientCertificate,
                        IntermediateCertificate,
                    });
            return certificates.Export(X509ContentType.Pkcs12, PfxPassword);
        }

        public void Dispose()
        {
            ServerCertificate.Dispose();
            ClientCertificate.Dispose();
            IntermediateCertificate.Dispose();
            RootCertificate.Dispose();
        }

        private static CertificateRequest CreateCertificateRequest(
            string subjectName,
            RSA key)
        {
            return new CertificateRequest(
                new X500DistinguishedName(subjectName),
                key,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }

        private static void AddSubjectKeyIdentifier(CertificateRequest request)
        {
            request.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(request.PublicKey, critical: false));
        }

        private static void AddEndEntityExtensions(
            CertificateRequest request,
            string enhancedKeyUsageOid)
        {
            request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(
                    certificateAuthority: false,
                    hasPathLengthConstraint: false,
                    pathLengthConstraint: 0,
                    critical: true));
            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DigitalSignature,
                    critical: true));
            OidCollection enhancedKeyUsages = new();
            enhancedKeyUsages.Add(new Oid(enhancedKeyUsageOid));
            request.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(enhancedKeyUsages, critical: true));
            AddSubjectKeyIdentifier(request);
        }

        private static byte[] CreateSerialNumber()
        {
            byte[] serialNumber = RandomNumberGenerator.GetBytes(16);
            serialNumber[0] &= 0x7F;
            serialNumber[^1] |= 0x01;
            return serialNumber;
        }

        private static X509Certificate2 LoadPublicCertificate(byte[] rawData)
        {
#if NET9_0_OR_GREATER
            return X509CertificateLoader.LoadCertificate(rawData);
#else
#pragma warning disable SYSLIB0057
            return new X509Certificate2(rawData);
#pragma warning restore SYSLIB0057
#endif
        }
    }

    private sealed class MutualTlsServer : IAsyncDisposable
    {
        private const string SuccessBody = """
            {
              "id": "chatcmpl-test",
              "object": "chat.completion",
              "created": 1704096000,
              "model": "gpt-4o-mini",
              "choices": [
                {
                  "index": 0,
                  "message": {
                    "role": "assistant",
                    "content": "mTLS request succeeded.",
                    "refusal": null,
                    "annotations": []
                  },
                  "logprobs": null,
                  "finish_reason": "stop"
                }
              ],
              "usage": {
                "prompt_tokens": 9,
                "completion_tokens": 5,
                "total_tokens": 14
              }
            }
            """;

        private readonly CancellationTokenSource _cancellationSource = new();
        private readonly Uri _redirectLocation;
        private readonly string _requiredApiKey;
        private readonly X509Certificate2 _serverCertificate;
        private readonly TcpListener _listener;
        private readonly X509Certificate2 _trustedClientRoot;
        private readonly Task _serveTask;
        private int _connectionCount;

        public Task Completion => _serveTask;
        public int ConnectionCount => Volatile.Read(ref _connectionCount);
        public bool ClientCertificateWasTrusted { get; private set; }
        public Uri Endpoint { get; }
        public Exception Failure { get; private set; }
        public IReadOnlyList<string> PresentedClientChainThumbprints { get; private set; } =
            Array.Empty<string>();
        public ReceivedRequest Request { get; private set; }

        private MutualTlsServer(
            X509Certificate2 serverCertificate,
            X509Certificate2 trustedClientRoot,
            Uri redirectLocation,
            string requiredApiKey)
        {
            _serverCertificate = serverCertificate;
            _trustedClientRoot = trustedClientRoot;
            _redirectLocation = redirectLocation;
            _requiredApiKey = requiredApiKey;
            _listener = new TcpListener(IPAddress.Loopback, port: 0);
            _listener.Start();
            int port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            Endpoint = new Uri($"https://127.0.0.1:{port}/v1/");
            _serveTask = ServeOnceAsync();
        }

        public static MutualTlsServer Start(
            X509Certificate2 serverCertificate,
            X509Certificate2 trustedClientRoot,
            Uri redirectLocation = null,
            string requiredApiKey = null)
        {
            return new MutualTlsServer(
                serverCertificate,
                trustedClientRoot,
                redirectLocation,
                requiredApiKey);
        }

        public async ValueTask DisposeAsync()
        {
            _cancellationSource.Cancel();
            _listener.Stop();

            try
            {
                await _serveTask.ConfigureAwait(false);
            }
            catch (Exception) when (_cancellationSource.IsCancellationRequested)
            {
            }

            _cancellationSource.Dispose();
        }

        private async Task ServeOnceAsync()
        {
            try
            {
                using TcpClient client =
                    await _listener.AcceptTcpClientAsync(_cancellationSource.Token)
                        .ConfigureAwait(false);
                Interlocked.Increment(ref _connectionCount);

                using SslStream sslStream = new(
                    client.GetStream(),
                    leaveInnerStreamOpen: false,
                    ValidateClientCertificate);
                SslServerAuthenticationOptions authenticationOptions = new()
                {
                    CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
                    ClientCertificateRequired = true,
                    EnabledSslProtocols = SslProtocols.Tls12,
                    ServerCertificate = _serverCertificate,
                };
                await sslStream.AuthenticateAsServerAsync(
                        authenticationOptions,
                        _cancellationSource.Token)
                    .ConfigureAwait(false);

                Request = await ReadRequestAsync(
                        sslStream,
                        _cancellationSource.Token)
                    .ConfigureAwait(false);
                bool isAuthorized = _requiredApiKey is null
                    || Request.Headers.TryGetValue(
                        "Authorization",
                        out string authorization)
                    && authorization == $"Bearer {_requiredApiKey}";
                await WriteResponseAsync(
                        sslStream,
                        _redirectLocation,
                        isAuthorized,
                        _cancellationSource.Token)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
                when (!_cancellationSource.IsCancellationRequested)
            {
                Failure = exception;
            }
        }

        private bool ValidateClientCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (certificate is not X509Certificate2 clientCertificate || chain is null)
            {
                return false;
            }

            chain.ChainPolicy.ApplicationPolicy.Add(new Oid(ClientAuthenticationOid));
            chain.ChainPolicy.CustomTrustStore.Add(_trustedClientRoot);
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;

            ClientCertificateWasTrusted = chain.Build(clientCertificate);
            PresentedClientChainThumbprints = chain.ChainElements
                .Cast<X509ChainElement>()
                .Select(element => element.Certificate.Thumbprint)
                .ToArray();
            return ClientCertificateWasTrusted;
        }

        private static async Task<ReceivedRequest> ReadRequestAsync(
            Stream stream,
            CancellationToken cancellationToken)
        {
            using StreamReader reader = new(
                stream,
                Encoding.ASCII,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);
            string requestLine = await reader.ReadLineAsync(cancellationToken)
                .ConfigureAwait(false);
            if (string.IsNullOrEmpty(requestLine))
            {
                throw new InvalidDataException("Expected an HTTP request line.");
            }

            string[] requestParts = requestLine.Split(' ');
            if (requestParts.Length != 3)
            {
                throw new InvalidDataException(
                    $"Unexpected HTTP request line: {requestLine}");
            }

            Dictionary<string, string> headers =
                new(StringComparer.OrdinalIgnoreCase);
            while (await reader.ReadLineAsync(cancellationToken)
                .ConfigureAwait(false) is { Length: > 0 } headerLine)
            {
                int separatorIndex = headerLine.IndexOf(':');
                if (separatorIndex <= 0)
                {
                    throw new InvalidDataException(
                        $"Unexpected HTTP header: {headerLine}");
                }

                headers[headerLine[..separatorIndex]] =
                    headerLine[(separatorIndex + 1)..].Trim();
            }

            if (!headers.TryGetValue("Content-Length", out string contentLengthValue)
                || !int.TryParse(contentLengthValue, out int contentLength)
                || contentLength < 0)
            {
                throw new InvalidDataException(
                    "Expected a valid Content-Length header.");
            }

            char[] bodyBuffer = new char[contentLength];
            int bodyLength = 0;
            while (bodyLength < contentLength)
            {
                int read = await reader.ReadAsync(
                        bodyBuffer.AsMemory(bodyLength, contentLength - bodyLength),
                        cancellationToken)
                    .ConfigureAwait(false);
                if (read == 0)
                {
                    throw new EndOfStreamException(
                        "HTTP request body ended before Content-Length bytes were read.");
                }

                bodyLength += read;
            }

            return new ReceivedRequest(
                requestParts[0],
                requestParts[1],
                headers,
                new string(bodyBuffer));
        }

        private static async Task WriteResponseAsync(
            Stream stream,
            Uri redirectLocation,
            bool isAuthorized,
            CancellationToken cancellationToken)
        {
            string statusLine;
            string body;
            string locationHeader;
            if (!isAuthorized)
            {
                statusLine = "HTTP/1.1 401 Unauthorized";
                body = """
                    {
                      "error": {
                        "message": "Invalid API key",
                        "type": "invalid_request_error"
                      }
                    }
                    """;
                locationHeader = string.Empty;
            }
            else if (redirectLocation is null)
            {
                statusLine = "HTTP/1.1 200 OK";
                body = SuccessBody;
                locationHeader = string.Empty;
            }
            else
            {
                statusLine = "HTTP/1.1 307 Temporary Redirect";
                body = "{}";
                locationHeader = $"Location: {redirectLocation}\r\n";
            }

            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            string responseHeaders =
                $"{statusLine}\r\n"
                + "Content-Type: application/json\r\n"
                + $"Content-Length: {bodyBytes.Length}\r\n"
                + locationHeader
                + "Connection: close\r\n"
                + "\r\n";
            byte[] headerBytes = Encoding.ASCII.GetBytes(responseHeaders);

            await stream.WriteAsync(headerBytes, cancellationToken)
                .ConfigureAwait(false);
            await stream.WriteAsync(bodyBytes, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private sealed record ReceivedRequest(
        string Method,
        string Path,
        IReadOnlyDictionary<string, string> Headers,
        string Body);
}
