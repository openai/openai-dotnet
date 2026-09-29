using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI;

internal sealed class WorkloadIdentityAuthenticationTokenProvider : AuthenticationTokenProvider
{
    private const string GrantType = "urn:ietf:params:oauth:grant-type:token-exchange";
    private const string JwtSubjectTokenType = "urn:ietf:params:oauth:token-type:jwt";
    private const string IdTokenSubjectTokenType = "urn:ietf:params:oauth:token-type:id_token";
    private const string TokenEndpoint = "https://auth.openai.com/oauth/token";
    private static readonly TimeSpan s_defaultRefreshBuffer = TimeSpan.FromMinutes(20);

    private readonly WorkloadIdentityFederationOptions _options;
    private readonly Lazy<ClientPipeline> _pipeline;
    private readonly object _cacheLock = new();
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private AuthenticationToken _cachedToken;

    public WorkloadIdentityAuthenticationTokenProvider(
        WorkloadIdentityFederationOptions options,
        ClientPipelineOptions clientOptions)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        clientOptions ??= new OpenAIClientOptions();

        ClientPipelineOptions tokenExchangePipelineOptions = clientOptions.Clone();
        ClientLoggingOptions loggingOptions = tokenExchangePipelineOptions.ClientLoggingOptions?.Clone()
            ?? new ClientLoggingOptions();
        loggingOptions.EnableMessageContentLogging = false;
        tokenExchangePipelineOptions.ClientLoggingOptions = loggingOptions;
        tokenExchangePipelineOptions.MessageLoggingPolicy = new MessageLoggingPolicy(loggingOptions);

        _pipeline = new Lazy<ClientPipeline>(
            () => ClientPipeline.Create(
                options: tokenExchangePipelineOptions,
                perCallPolicies: [],
                perTryPolicies: [],
                beforeTransportPolicies: []),
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public override GetTokenOptions CreateTokenOptions(IReadOnlyDictionary<string, object> properties)
        => new(properties);

    public override AuthenticationToken GetToken(GetTokenOptions options, CancellationToken cancellationToken)
        => GetTokenCoreAsync(cancellationToken, async: false).AsTask().GetAwaiter().GetResult();

    public override ValueTask<AuthenticationToken> GetTokenAsync(GetTokenOptions options, CancellationToken cancellationToken)
        => GetTokenCoreAsync(cancellationToken, async: true);

    internal async ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        => (await GetTokenCoreAsync(cancellationToken, async: true).ConfigureAwait(false)).TokenValue;

    private async ValueTask<AuthenticationToken> GetTokenCoreAsync(CancellationToken cancellationToken, bool async)
    {
        AuthenticationToken cachedToken = GetFreshCachedToken();
        if (cachedToken is not null)
        {
            return cachedToken;
        }

        if (async)
        {
            await _refreshLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            _refreshLock.Wait(cancellationToken);
        }

        try
        {
            cachedToken = GetFreshCachedToken();
            if (cachedToken is not null)
            {
                return cachedToken;
            }

            AuthenticationToken refreshedToken = await ExchangeTokenAsync(cancellationToken, async).ConfigureAwait(false);
            lock (_cacheLock)
            {
                _cachedToken = refreshedToken;
            }
            return refreshedToken;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private AuthenticationToken GetFreshCachedToken()
    {
        lock (_cacheLock)
        {
            if (_cachedToken is null)
            {
                return null;
            }

            DateTimeOffset refreshOn = _cachedToken.RefreshOn ?? _cachedToken.ExpiresOn ?? DateTimeOffset.MinValue;
            return _options.UtcNowProvider() < refreshOn ? _cachedToken : null;
        }
    }

    private async ValueTask<AuthenticationToken> ExchangeTokenAsync(CancellationToken cancellationToken, bool async)
    {
        string subjectToken;
        try
        {
            subjectToken = await _options.SubjectTokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            throw new InvalidOperationException("The subject token provider failed.");
        }
        if (string.IsNullOrWhiteSpace(subjectToken))
        {
            throw new InvalidOperationException("The subject token provider returned an empty token.");
        }

        string subjectTokenType = _options.SubjectTokenProvider.TokenType switch
        {
            WorkloadIdentitySubjectTokenType.Jwt => JwtSubjectTokenType,
            WorkloadIdentitySubjectTokenType.IdToken => IdTokenSubjectTokenType,
            _ => throw new InvalidOperationException("The workload identity subject token type is not supported."),
        };

        Dictionary<string, string> requestBody = new()
        {
            ["grant_type"] = GrantType,
            ["subject_token"] = subjectToken,
            ["subject_token_type"] = subjectTokenType,
            ["identity_provider_id"] = _options.IdentityProviderId,
            ["service_account_id"] = _options.ServiceAccountId,
        };
        if (_options.ClientId is not null)
        {
            requestBody["client_id"] = _options.ClientId;
        }

        using PipelineMessage message = _pipeline.Value.CreateMessage();
        message.Apply(new RequestOptions { CancellationToken = cancellationToken });
        message.Request.Method = "POST";
        message.Request.Uri = new Uri(TokenEndpoint);
        message.Request.Headers.Set("Content-Type", "application/json");
        message.Request.Content = CreateRequestContent(requestBody);

        try
        {
            if (async)
            {
                await _pipeline.Value.SendAsync(message).ConfigureAwait(false);
            }
            else
            {
                _pipeline.Value.Send(message);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            throw new InvalidOperationException("The workload identity token exchange failed.");
        }

        PipelineResponse response = message.Response;
        if (response is null || response.Status < 200 || response.Status >= 300)
        {
            throw new InvalidOperationException("The workload identity token exchange failed.");
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(response.Content.ToString());
            JsonElement root = document.RootElement;
            if (!root.TryGetProperty("access_token", out JsonElement accessTokenElement)
                || accessTokenElement.ValueKind != JsonValueKind.String
                || string.IsNullOrWhiteSpace(accessTokenElement.GetString())
                || !root.TryGetProperty("token_type", out JsonElement tokenTypeElement)
                || tokenTypeElement.ValueKind != JsonValueKind.String
                || !string.Equals(tokenTypeElement.GetString(), "Bearer", StringComparison.OrdinalIgnoreCase)
                || !root.TryGetProperty("expires_in", out JsonElement expiresInElement)
                || !expiresInElement.TryGetInt32(out int expiresInSeconds)
                || expiresInSeconds <= 0)
            {
                throw new InvalidOperationException("The workload identity token exchange returned an invalid response.");
            }

            DateTimeOffset now = _options.UtcNowProvider();
            TimeSpan lifetime = TimeSpan.FromSeconds(expiresInSeconds);
            TimeSpan halfLifetime = TimeSpan.FromTicks(lifetime.Ticks / 2);
            TimeSpan refreshBuffer = s_defaultRefreshBuffer <= halfLifetime
                ? s_defaultRefreshBuffer
                : halfLifetime;
            return new AuthenticationToken(
                accessTokenElement.GetString(),
                "Bearer",
                now + lifetime,
                now + lifetime - refreshBuffer);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException("The workload identity token exchange returned an invalid response.", exception);
        }
    }

    private static BinaryContent CreateRequestContent(IReadOnlyDictionary<string, string> values)
    {
        using MemoryStream stream = new();
        using (Utf8JsonWriter writer = new(stream))
        {
            writer.WriteStartObject();
            foreach (KeyValuePair<string, string> pair in values)
            {
                writer.WriteString(pair.Key, pair.Value);
            }
            writer.WriteEndObject();
        }

        return BinaryContent.Create(BinaryData.FromBytes(stream.ToArray()));
    }
}
