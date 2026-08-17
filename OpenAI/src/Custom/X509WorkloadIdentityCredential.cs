#nullable enable

using System;
using System.ClientModel.Primitives;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI;

/// <summary>Authenticates an OpenAI workload by exchanging its transport's X.509 identity.</summary>
/// <remarks>
/// This credential owns its internally created <see cref="HttpClient"/> wrapper, but never owns or
/// disposes the caller-provided HTTP handler. Keep the credential and handler alive for every client
/// that uses them, and rebuild the handler, credential, and clients together when certificates rotate.
/// </remarks>
[Experimental("OPENAI001")]
public sealed class X509WorkloadIdentityCredential : IDisposable
{
    private const int MaximumExchangeAttempts = 3;
    private const int MaximumErrorBodyBytes = 4096;
    private const int MaximumTokenResponseBytes = 1024 * 1024;
    private static readonly Uri s_exchangeEndpoint = new("https://mtls.auth.openai.com/oauth/token");

    internal static Uri DefaultApiEndpoint { get; } = new("https://mtls.api.openai.com/v1");

    private readonly string _identityProviderId;
    private readonly string _serviceAccountId;
    private readonly HttpMessageHandler _handler;
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private readonly TimeSpan _refreshBuffer;
    private CachedToken? _cachedToken;
    private int _disposed;

    /// <summary>Creates an X.509 workload identity credential.</summary>
    /// <param name="identityProviderId">The configured OpenAI identity-provider identifier.</param>
    /// <param name="serviceAccountId">The mapped OpenAI service-account identifier.</param>
    /// <param name="options">The options containing the caller-owned mTLS handler.</param>
    public X509WorkloadIdentityCredential(
        string identityProviderId,
        string serviceAccountId,
        X509WorkloadIdentityCredentialOptions options)
    {
        Argument.AssertNotNullOrWhiteSpace(identityProviderId, nameof(identityProviderId));
        Argument.AssertNotNullOrWhiteSpace(serviceAccountId, nameof(serviceAccountId));
        Argument.AssertNotNull(options, nameof(options));
        HttpMessageHandler handler = options.Handler
            ?? throw new ArgumentNullException(nameof(options.Handler));

        if (options.RefreshBuffer < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(options.RefreshBuffer), "The refresh buffer cannot be negative.");
        }

        ValidateHandler(handler, s_exchangeEndpoint);
        ProtectHandlerProxy(handler);

        _identityProviderId = identityProviderId;
        _serviceAccountId = serviceAccountId;
        _handler = handler;
        _refreshBuffer = options.RefreshBuffer;
        _httpClient = new HttpClient(_handler, disposeHandler: false)
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };
        Transport = new WorkloadIdentityPipelineTransport(_httpClient, this);
    }

    internal WorkloadIdentityPipelineTransport Transport { get; }

    internal string GetToken(TimeSpan networkTimeout, CancellationToken cancellationToken)
    {
        return GetTokenCoreAsync(async: false, networkTimeout, cancellationToken).GetAwaiter().GetResult();
    }

    internal ValueTask<string> GetTokenAsync(TimeSpan networkTimeout, CancellationToken cancellationToken)
    {
        return GetTokenCoreAsync(async: true, networkTimeout, cancellationToken);
    }

    internal void ValidateEndpoint(Uri endpoint)
    {
        ThrowIfDisposed();
        ValidateHandler(_handler, endpoint);

        if (_handler is HttpClientHandler { UseProxy: true, Proxy: not ValidatingProxy }
#if NET8_0_OR_GREATER
            || _handler is SocketsHttpHandler { UseProxy: true, Proxy: not ValidatingProxy }
#endif
            )
        {
            throw new InvalidOperationException(
                "The workload identity HTTP handler proxy configuration cannot change after credential creation.");
        }
    }

    private async ValueTask<string> GetTokenCoreAsync(
        bool async,
        TimeSpan networkTimeout,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ValidateEndpoint(s_exchangeEndpoint);
        CachedToken? token = Volatile.Read(ref _cachedToken);
        if (token is not null && IsFresh(token))
        {
            return token.Value;
        }

        using CancellationTokenSource deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(networkTimeout);
        bool acquired = false;
        try
        {
            if (async)
            {
                await _refreshLock.WaitAsync(deadline.Token).ConfigureAwait(false);
            }
            else
            {
                _refreshLock.Wait(deadline.Token);
            }

            acquired = true;
            ValidateEndpoint(s_exchangeEndpoint);
            token = Volatile.Read(ref _cachedToken);
            if (token is not null && IsFresh(token))
            {
                return token.Value;
            }

            token = await ExchangeTokenAsync(async, deadline.Token).ConfigureAwait(false);
            if (!IsFresh(token))
            {
                throw new InvalidOperationException(
                    "X.509 workload identity token exchange returned a token that expired before it could be used.");
            }

            Volatile.Write(ref _cachedToken, token);
            return token.Value;
        }
        catch (OperationCanceledException exception) when (
            deadline.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new InvalidOperationException(
                "X.509 workload identity token exchange exceeded the configured network timeout.",
                exception);
        }
        finally
        {
            if (acquired)
            {
                _refreshLock.Release();
            }
        }
    }

    internal void Invalidate(string rejectedToken)
    {
        CachedToken? current = Volatile.Read(ref _cachedToken);
        if (current is not null && string.Equals(current.Value, rejectedToken, StringComparison.Ordinal))
        {
            Interlocked.CompareExchange(ref _cachedToken, null, current);
        }
    }

    /// <summary>Disposes the SDK-created HTTP client without disposing the caller-owned handler.</summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0)
        {
            Interlocked.Exchange(ref _cachedToken, null);
            _httpClient.Dispose();
        }
    }

    private async ValueTask<CachedToken> ExchangeTokenAsync(bool async, CancellationToken cancellationToken)
    {
        ValidateEndpoint(s_exchangeEndpoint);

        for (int attempt = 0; attempt < MaximumExchangeAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using HttpRequestMessage request = CreateExchangeRequest();
            try
            {
#if NET8_0_OR_GREATER
                using HttpResponseMessage response = async
                    ? await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false)
                    : _httpClient.Send(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
#else
                using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
#endif
                long issuedAt = GetTimestamp();
                if (IsTransient(response.StatusCode) && attempt + 1 < MaximumExchangeAttempts)
                {
                    TimeSpan delay = GetRetryDelay(attempt, response.Headers.RetryAfter);
                    await DrainErrorBodyAsync(response, async, cancellationToken).ConfigureAwait(false);
                    await DelayAsync(delay, async, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                if (!response.IsSuccessStatusCode)
                {
                    await ThrowForFailedExchangeAsync(response, async, cancellationToken).ConfigureAwait(false);
                }

                return await ReadTokenAsync(response, issuedAt, async, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (
                cancellationToken.IsCancellationRequested
                && exception is IOException or ObjectDisposedException or JsonException or NotSupportedException)
            {
                throw new OperationCanceledException(
                    "X.509 workload identity token exchange was canceled.",
                    exception,
                    cancellationToken);
            }
            catch (Exception exception) when (
                exception is HttpRequestException or IOException
                || exception is OperationCanceledException && !cancellationToken.IsCancellationRequested)
            {
                if (attempt + 1 >= MaximumExchangeAttempts)
                {
                    throw new InvalidOperationException(
                        "X.509 workload identity token exchange exhausted its retry attempts.",
                        exception);
                }

                await DelayAsync(GetRetryDelay(attempt, retryAfter: null), async, cancellationToken).ConfigureAwait(false);
            }
        }

        throw new InvalidOperationException("X.509 workload identity token exchange exhausted its retry attempts.");
    }

    private HttpRequestMessage CreateExchangeRequest()
    {
        using MemoryStream buffer = new();
        using (Utf8JsonWriter writer = new(buffer))
        {
            writer.WriteStartObject();
            writer.WriteString("grant_type", "urn:ietf:params:oauth:grant-type:token-exchange");
            writer.WriteString("subject_token_type", "urn:openai:params:oauth:token-type:x509");
            writer.WriteString("identity_provider_id", _identityProviderId);
            writer.WriteString("service_account_id", _serviceAccountId);
            writer.WriteEndObject();
        }

        ByteArrayContent content = new(buffer.ToArray());
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return new HttpRequestMessage(HttpMethod.Post, s_exchangeEndpoint) { Content = content };
    }

    private async ValueTask<CachedToken> ReadTokenAsync(
        HttpResponseMessage response,
        long issuedAt,
        bool async,
        CancellationToken cancellationToken)
    {
#if NET8_0_OR_GREATER
        using Stream stream = async
            ? await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false)
            : response.Content.ReadAsStream(cancellationToken);
#else
        using Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif
        using CancellationTokenRegistration registration = RegisterSynchronousStreamCancellation(
            stream,
            async,
            cancellationToken);
        using JsonDocument document = await ParseBoundedResponseAsync(stream, async, cancellationToken)
            .ConfigureAwait(false);

        JsonElement body = document.RootElement;
        if (body.TryGetProperty("token_type", out JsonElement tokenType)
            && (tokenType.ValueKind != JsonValueKind.String
                || !string.Equals(tokenType.GetString(), "Bearer", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("X.509 workload identity token exchange returned an invalid token type.");
        }

        if (!body.TryGetProperty("access_token", out JsonElement tokenElement)
            || tokenElement.ValueKind != JsonValueKind.String)
        {
            throw new InvalidOperationException("X.509 workload identity token exchange returned an invalid access token.");
        }

        string? accessToken = tokenElement.GetString();
        if (accessToken is null || !IsValidBearerToken(accessToken))
        {
            throw new InvalidOperationException("X.509 workload identity token exchange returned an invalid access token.");
        }

        if (!body.TryGetProperty("expires_in", out JsonElement expiryElement)
            || expiryElement.ValueKind != JsonValueKind.Number
            || !expiryElement.TryGetDouble(out double lifetimeSeconds)
            || double.IsNaN(lifetimeSeconds)
            || double.IsInfinity(lifetimeSeconds)
            || lifetimeSeconds <= 0
            || lifetimeSeconds > TimeSpan.MaxValue.TotalSeconds)
        {
            throw new InvalidOperationException("X.509 workload identity token exchange returned an invalid token lifetime.");
        }

        TimeSpan lifetime = TimeSpan.FromSeconds(lifetimeSeconds);
        if (lifetime <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("X.509 workload identity token exchange returned an invalid token lifetime.");
        }

        TimeSpan maximumBuffer = TimeSpan.FromTicks(lifetime.Ticks / 2);
        TimeSpan effectiveBuffer = _refreshBuffer < maximumBuffer ? _refreshBuffer : maximumBuffer;
        return new CachedToken(accessToken, issuedAt, lifetime - effectiveBuffer);
    }

    private static async ValueTask<JsonDocument> ParseBoundedResponseAsync(
        Stream stream,
        bool async,
        CancellationToken cancellationToken)
    {
        using MemoryStream response = new();
        byte[] buffer = new byte[8192];
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int count = async
                ? await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)
                : stream.Read(buffer, 0, buffer.Length);
            if (count == 0)
            {
                break;
            }

            if (response.Length > MaximumTokenResponseBytes - count)
            {
                throw new InvalidOperationException(
                    "X.509 workload identity token exchange exceeded the maximum response size.");
            }

            response.Write(buffer, 0, count);
        }

        return JsonDocument.Parse(new ReadOnlyMemory<byte>(response.GetBuffer(), 0, (int)response.Length));
    }

    private static bool IsValidBearerToken(string token)
    {
        if (token.Length == 0)
        {
            return false;
        }

        bool paddingStarted = false;
        bool containsTokenCharacter = false;
        foreach (char character in token)
        {
            if (character == '=')
            {
                paddingStarted = true;
                continue;
            }

            if (paddingStarted
                || !(character is >= 'A' and <= 'Z'
                    or >= 'a' and <= 'z'
                    or >= '0' and <= '9'
                    or '-' or '.' or '_' or '~' or '+' or '/'))
            {
                return false;
            }

            containsTokenCharacter = true;
        }

        return containsTokenCharacter;
    }

    private static async ValueTask DrainErrorBodyAsync(
        HttpResponseMessage response,
        bool async,
        CancellationToken cancellationToken)
    {
        if (response.Content is null)
        {
            return;
        }

#if NET8_0_OR_GREATER
        using Stream stream = async
            ? await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false)
            : response.Content.ReadAsStream(cancellationToken);
#else
        using Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif
        using CancellationTokenRegistration registration = RegisterSynchronousStreamCancellation(
            stream,
            async,
            cancellationToken);
        byte[] buffer = new byte[1024];
        int remaining = MaximumErrorBodyBytes;
        while (remaining > 0)
        {
            int count = async
                ? await stream.ReadAsync(buffer, 0, Math.Min(buffer.Length, remaining), cancellationToken).ConfigureAwait(false)
                : stream.Read(buffer, 0, Math.Min(buffer.Length, remaining));
            if (count == 0)
            {
                break;
            }

            remaining -= count;
        }
    }

    private static async ValueTask ThrowForFailedExchangeAsync(
        HttpResponseMessage response,
        bool async,
        CancellationToken cancellationToken)
    {
        try
        {
            await DrainErrorBodyAsync(response, async, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (
            exception is IOException or HttpRequestException
            && !IsTransient(response.StatusCode)
            && !cancellationToken.IsCancellationRequested)
        {
            // The received HTTP status remains authoritative when its error body cannot be drained.
        }

        throw new InvalidOperationException(
            $"X.509 workload identity token exchange failed with HTTP status {(int)response.StatusCode}.");
    }

    private static CancellationTokenRegistration RegisterSynchronousStreamCancellation(
        Stream stream,
        bool async,
        CancellationToken cancellationToken)
    {
        return async
            ? default
            : cancellationToken.Register(static value => ((Stream)value!).Dispose(), stream);
    }

    private static TimeSpan GetRetryDelay(int attempt, RetryConditionHeaderValue? retryAfter)
    {
        if (retryAfter?.Delta is TimeSpan delta && delta > TimeSpan.Zero)
        {
            return delta < TimeSpan.FromSeconds(30) ? delta : TimeSpan.FromSeconds(30);
        }

        if (retryAfter?.Date is DateTimeOffset retryAt)
        {
            TimeSpan delay = retryAt - DateTimeOffset.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                return delay < TimeSpan.FromSeconds(30) ? delay : TimeSpan.FromSeconds(30);
            }
        }

        return TimeSpan.FromMilliseconds(100 * (1 << attempt));
    }

    private static async ValueTask DelayAsync(TimeSpan delay, bool async, CancellationToken cancellationToken)
    {
        if (async)
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
        else if (cancellationToken.WaitHandle.WaitOne(delay))
        {
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    private static bool IsTransient(HttpStatusCode statusCode)
    {
        int value = (int)statusCode;
        return value is 408 or 409 or 429 || value >= 500 && value <= 599;
    }

    private static bool IsFresh(CachedToken token)
    {
        return GetElapsedTime(token.IssuedAt) < token.RefreshAfter;
    }

    private static long GetTimestamp()
    {
#if NET8_0_OR_GREATER
        return TimeProvider.System.GetTimestamp();
#else
        return Stopwatch.GetTimestamp();
#endif
    }

    private static TimeSpan GetElapsedTime(long timestamp)
    {
#if NET8_0_OR_GREATER
        return TimeProvider.System.GetElapsedTime(timestamp);
#else
        return TimeSpan.FromSeconds((double)(Stopwatch.GetTimestamp() - timestamp) / Stopwatch.Frequency);
#endif
    }

    private static void ValidateHandler(HttpMessageHandler handler, Uri destination)
    {
        if (handler.GetType() == typeof(HttpClientHandler))
        {
            HttpClientHandler native = (HttpClientHandler)handler;
            ValidateHandlerConfiguration(
                handler,
                destination,
                native.AllowAutoRedirect,
                native.Credentials,
                native.UseCookies,
                native.UseProxy,
                native.Proxy);
            return;
        }

#if NET8_0_OR_GREATER
        if (handler.GetType() == typeof(SocketsHttpHandler))
        {
            SocketsHttpHandler native = (SocketsHttpHandler)handler;
            ValidateHandlerConfiguration(
                handler,
                destination,
                native.AllowAutoRedirect,
                native.Credentials,
                native.UseCookies,
                native.UseProxy,
                native.Proxy);
            return;
        }
#endif

        throw new ArgumentException(
            "The workload identity HTTP handler must be a directly configured HttpClientHandler or supported SocketsHttpHandler.",
            nameof(handler));
    }

    private static void ValidateHandlerConfiguration(
        HttpMessageHandler handler,
        Uri destination,
        bool allowsRedirects,
        ICredentials? credentials,
        bool usesCookies,
        bool usesProxy,
        IWebProxy? configuredProxy)
    {
        if (allowsRedirects)
        {
            throw new ArgumentException("The workload identity HTTP handler must disable automatic redirects.", nameof(handler));
        }

        if (credentials is not null)
        {
            throw new ArgumentException(
                "The workload identity HTTP handler must not contain destination-server credentials.",
                nameof(handler));
        }

        if (usesCookies)
        {
            throw new ArgumentException(
                "The workload identity HTTP handler must disable automatic cookies.",
                nameof(handler));
        }

        if (!usesProxy)
        {
            return;
        }

        IWebProxy? proxy = GetEffectiveProxy(configuredProxy);
        if (proxy is not null
            && !proxy.IsBypassed(destination)
            && proxy.GetProxy(destination) is Uri proxyEndpoint
            && !string.Equals(proxyEndpoint.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "The workload identity HTTP handler must use an HTTP CONNECT proxy rather than a TLS proxy.",
                nameof(handler));
        }
    }

    private static void ProtectHandlerProxy(HttpMessageHandler handler)
    {
        if (handler is HttpClientHandler { UseProxy: true } clientHandler)
        {
            IWebProxy? configuredProxy = clientHandler.Proxy;
            IWebProxy proxy = GetEffectiveProxy(configuredProxy)
                ?? throw new ArgumentException("The workload identity HTTP handler proxy is unavailable.", nameof(handler));
#if NET8_0_OR_GREATER
            ICredentials? defaultCredentials = configuredProxy is null
                ? clientHandler.DefaultProxyCredentials
                : null;
#else
            ICredentials? defaultCredentials = null;
#endif
            clientHandler.Proxy = new ValidatingProxy(
                proxy,
                configuredProxy,
                defaultCredentials ?? proxy.Credentials);
            return;
        }

#if NET8_0_OR_GREATER
        if (handler is SocketsHttpHandler { UseProxy: true } socketsHandler)
        {
            IWebProxy? configuredProxy = socketsHandler.Proxy;
            IWebProxy proxy = GetEffectiveProxy(configuredProxy)
                ?? throw new ArgumentException("The workload identity HTTP handler proxy is unavailable.", nameof(handler));
            ICredentials? defaultCredentials = configuredProxy is null
                ? socketsHandler.DefaultProxyCredentials
                : null;
            socketsHandler.Proxy = new ValidatingProxy(
                proxy,
                configuredProxy,
                defaultCredentials ?? proxy.Credentials);
        }
#endif
    }

    private static IWebProxy? GetEffectiveProxy(IWebProxy? configuredProxy)
    {
#if NET8_0_OR_GREATER
        return configuredProxy ?? HttpClient.DefaultProxy;
#else
        return configuredProxy
            ?? typeof(HttpClient).GetProperty("DefaultProxy")?.GetValue(null) as IWebProxy
            ?? WebRequest.DefaultWebProxy;
#endif
    }

    private void ThrowIfDisposed()
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(X509WorkloadIdentityCredential));
        }
    }

    private sealed class CachedToken
    {
        internal CachedToken(string value, long issuedAt, TimeSpan refreshAfter)
        {
            Value = value;
            IssuedAt = issuedAt;
            RefreshAfter = refreshAfter;
        }

        internal string Value { get; }
        internal long IssuedAt { get; }
        internal TimeSpan RefreshAfter { get; }
    }

    private sealed class ValidatingProxy(
        IWebProxy proxy,
        IWebProxy? credentialOwner,
        ICredentials? credentials) : IWebProxy
    {
        public ICredentials? Credentials
        {
            get => credentialOwner is null ? credentials : credentialOwner.Credentials;
            set
            {
                if (credentialOwner is null)
                {
                    credentials = value;
                }
                else
                {
                    credentialOwner.Credentials = value;
                }
            }
        }

        public Uri GetProxy(Uri destination)
        {
            Uri? endpoint = proxy.GetProxy(destination);
            if (endpoint is null
                || !string.Equals(endpoint.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "The workload identity HTTP handler must use an HTTP CONNECT proxy rather than a TLS proxy.",
                    nameof(proxy));
            }

            return endpoint;
        }

        public bool IsBypassed(Uri host) => proxy.IsBypassed(host);
    }
}
