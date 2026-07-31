using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.MutualTls;

internal sealed class MutualTlsServer : IAsyncDisposable
{
    private const string AuthorityKeyIdentifierOid = "2.5.29.35";
    private const string ClientAuthenticationOid = "1.3.6.1.5.5.7.3.2";
    private const string SubjectAlternativeNameOid = "2.5.29.17";
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
    public bool ClientCertificateWasAccepted { get; private set; }
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
        await _serveTask.ConfigureAwait(false);
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
                EnabledSslProtocols =
                    SslProtocols.Tls12 | SslProtocols.Tls13,
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
        catch (OperationCanceledException)
            when (_cancellationSource.IsCancellationRequested)
        {
            return;
        }
        catch (ObjectDisposedException)
            when (_cancellationSource.IsCancellationRequested)
        {
            return;
        }
        catch (SocketException)
            when (_cancellationSource.IsCancellationRequested)
        {
            return;
        }
        catch (OperationCanceledException exception)
        {
            Failure = exception;
        }
        catch (SocketException exception)
        {
            Failure = exception;
        }
        catch (IOException exception)
        {
            Failure = exception;
        }
        catch (AuthenticationException exception)
        {
            Failure = exception;
        }
        catch (CryptographicException exception)
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

        bool chainWasTrusted = chain.Build(clientCertificate);
        PresentedClientChainThumbprints = chain.ChainElements
            .Cast<X509ChainElement>()
            .Select(element => element.Certificate.Thumbprint)
            .ToArray();
        ClientCertificateWasAccepted =
            chainWasTrusted
            && MeetsCaCertificateRequirements(_trustedClientRoot)
            && MeetsClientCertificateRequirements(clientCertificate);
        return ClientCertificateWasAccepted;
    }

    private static bool MeetsCaCertificateRequirements(
        X509Certificate2 certificate)
    {
        const int MaximumCertificateSize = 16 * 1024;
        X509KeyUsageFlags requiredKeyUsage =
            X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign;

        return certificate.RawData.Length < MaximumCertificateSize
            && certificate.NotAfter.ToUniversalTime()
                > DateTime.UtcNow.AddDays(1)
            && certificate.Extensions
                .OfType<X509BasicConstraintsExtension>()
                .Any(extension => extension.CertificateAuthority)
            && HasKeyUsage(certificate, requiredKeyUsage)
            && HasSubjectKeyIdentifier(certificate)
            && HasKeyIdentifierAuthorityKeyIdentifier(certificate);
    }

    private static bool MeetsClientCertificateRequirements(
        X509Certificate2 certificate)
    {
        X509KeyUsageFlags requiredKeyUsage =
            X509KeyUsageFlags.DigitalSignature
            | X509KeyUsageFlags.KeyEncipherment;

        return HasKeyUsage(certificate, requiredKeyUsage)
            && certificate.Extensions
                .OfType<X509EnhancedKeyUsageExtension>()
                .Any(
                    extension => extension.EnhancedKeyUsages
                        .Cast<Oid>()
                        .Any(oid => oid.Value == ClientAuthenticationOid))
            && HasSubjectKeyIdentifier(certificate)
            && HasKeyIdentifierAuthorityKeyIdentifier(certificate)
            && certificate.Extensions
                .Cast<X509Extension>()
                .Any(
                    extension =>
                        extension.Oid?.Value == SubjectAlternativeNameOid
                        && extension.RawData.Length > 0);
    }

    private static bool HasKeyUsage(
        X509Certificate2 certificate,
        X509KeyUsageFlags requiredKeyUsage)
    {
        return certificate.Extensions
            .OfType<X509KeyUsageExtension>()
            .Any(
                extension =>
                    (extension.KeyUsages & requiredKeyUsage)
                    == requiredKeyUsage);
    }

    private static bool HasSubjectKeyIdentifier(
        X509Certificate2 certificate)
    {
        return certificate.Extensions
            .OfType<X509SubjectKeyIdentifierExtension>()
            .Any(
                extension =>
                    !string.IsNullOrEmpty(extension.SubjectKeyIdentifier));
    }

    private static bool HasKeyIdentifierAuthorityKeyIdentifier(
        X509Certificate2 certificate)
    {
        X509Extension extension = certificate.Extensions
            .Cast<X509Extension>()
            .SingleOrDefault(
                extension =>
                    extension.Oid?.Value == AuthorityKeyIdentifierOid);
        if (extension is null)
        {
            return false;
        }

        try
        {
            AsnReader reader =
                new(extension.RawData, AsnEncodingRules.DER);
            AsnReader sequence = reader.ReadSequence();
            byte[] keyIdentifier = sequence.ReadOctetString(
                new Asn1Tag(TagClass.ContextSpecific, 0));
            return keyIdentifier.Length > 0
                && !sequence.HasData
                && !reader.HasData;
        }
        catch (AsnContentException)
        {
            return false;
        }
    }

    private static async Task<ReceivedRequest> ReadRequestAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        const int MaximumHeaderLength = 64 * 1024;
        using MemoryStream headerBuffer = new();
        byte[] nextByte = new byte[1];
        bool headersComplete = false;
        while (headerBuffer.Length < MaximumHeaderLength)
        {
            int bytesRead = await stream.ReadAsync(
                    nextByte,
                    cancellationToken)
                .ConfigureAwait(false);
            if (bytesRead == 0)
            {
                throw new EndOfStreamException(
                    "HTTP request ended before its headers were complete.");
            }

            headerBuffer.WriteByte(nextByte[0]);
            if (headerBuffer.Length >= 4)
            {
                byte[] bytes = headerBuffer.GetBuffer();
                int length = checked((int)headerBuffer.Length);
                headersComplete =
                    bytes[length - 4] == '\r'
                    && bytes[length - 3] == '\n'
                    && bytes[length - 2] == '\r'
                    && bytes[length - 1] == '\n';
                if (headersComplete)
                {
                    break;
                }
            }
        }

        if (!headersComplete)
        {
            throw new InvalidDataException(
                $"HTTP request headers exceeded {MaximumHeaderLength} bytes.");
        }

        string headerText = Encoding.ASCII.GetString(
            headerBuffer.GetBuffer(),
            0,
            checked((int)headerBuffer.Length - 4));
        string[] headerLines =
            headerText.Split(
                new[] { "\r\n" },
                StringSplitOptions.None);
        if (headerLines.Length == 0
            || string.IsNullOrEmpty(headerLines[0]))
        {
            throw new InvalidDataException("Expected an HTTP request line.");
        }

        string[] requestParts = headerLines[0].Split(' ');
        if (requestParts.Length != 3)
        {
            throw new InvalidDataException(
                $"Unexpected HTTP request line: {headerLines[0]}");
        }

        Dictionary<string, string> headers =
            new(StringComparer.OrdinalIgnoreCase);
        foreach (string headerLine in headerLines.Skip(1))
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

        byte[] bodyBuffer = new byte[contentLength];
        await stream.ReadExactlyAsync(bodyBuffer, cancellationToken)
            .ConfigureAwait(false);

        return new ReceivedRequest(
            requestParts[0],
            requestParts[1],
            headers,
            Encoding.UTF8.GetString(bodyBuffer));
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

internal sealed record ReceivedRequest(
    string Method,
    string Path,
    IReadOnlyDictionary<string, string> Headers,
    string Body);
