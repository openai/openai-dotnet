using Microsoft.ClientModel.TestFramework.Mocks;
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
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests;

public class WorkloadIdentityFederationTests
{
    private const string AccessToken = "access-token-value";
    private const string SubjectToken = "subject-token-value";

    [TestCase(WorkloadIdentitySubjectTokenType.Jwt, "urn:ietf:params:oauth:token-type:jwt")]
    [TestCase(WorkloadIdentitySubjectTokenType.IdToken, "urn:ietf:params:oauth:token-type:id_token")]
    public async Task ExchangesSubjectTokenAndAppliesBearerToken(
        WorkloadIdentitySubjectTokenType subjectTokenType,
        string expectedSubjectTokenType)
    {
        string tokenRequestBody = null;
        string authorizationHeader = null;
        int subjectTokenCalls = 0;

        OpenAIClient client = CreateClient(
            cancellationToken =>
            {
                Interlocked.Increment(ref subjectTokenCalls);
                return new ValueTask<string>(SubjectToken);
            },
            message =>
            {
                if (IsTokenExchange(message))
                {
                    tokenRequestBody = ReadRequestContent(message);
                    return TokenResponse();
                }

                message.Request.Headers.TryGetValue("Authorization", out authorizationHeader);
                return ServiceResponse();
            },
            subjectTokenType,
            clientId: "client-id");

        await SendRequestAsync(client);

        Assert.That(subjectTokenCalls, Is.EqualTo(1));
        Assert.That(authorizationHeader, Is.EqualTo($"Bearer {AccessToken}"));

        using JsonDocument document = JsonDocument.Parse(tokenRequestBody);
        JsonElement root = document.RootElement;
        Assert.Multiple(() =>
        {
            Assert.That(root.GetProperty("grant_type").GetString(), Is.EqualTo("urn:ietf:params:oauth:grant-type:token-exchange"));
            Assert.That(root.GetProperty("subject_token").GetString(), Is.EqualTo(SubjectToken));
            Assert.That(root.GetProperty("subject_token_type").GetString(), Is.EqualTo(expectedSubjectTokenType));
            Assert.That(root.GetProperty("identity_provider_id").GetString(), Is.EqualTo("identity-provider-id"));
            Assert.That(root.GetProperty("service_account_id").GetString(), Is.EqualTo("service-account-id"));
            Assert.That(root.GetProperty("client_id").GetString(), Is.EqualTo("client-id"));
        });
    }

    [Test]
    public async Task CachesTokenAndCoalescesConcurrentRefresh()
    {
        int subjectTokenCalls = 0;
        int tokenExchangeCalls = 0;

        OpenAIClient client = CreateClient(
            async cancellationToken =>
            {
                Interlocked.Increment(ref subjectTokenCalls);
                await Task.Delay(50, cancellationToken);
                return SubjectToken;
            },
            message =>
            {
                if (IsTokenExchange(message))
                {
                    Interlocked.Increment(ref tokenExchangeCalls);
                    return TokenResponse();
                }

                return ServiceResponse();
            });

        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => SendRequestAsync(client)));
        await SendRequestAsync(client);

        Assert.Multiple(() =>
        {
            Assert.That(subjectTokenCalls, Is.EqualTo(1));
            Assert.That(tokenExchangeCalls, Is.EqualTo(1));
        });
    }

    [Test]
    public void SupportsSynchronousClientCalls()
    {
        string authorizationHeader = null;
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message =>
            {
                if (IsTokenExchange(message))
                {
                    return TokenResponse();
                }

                message.Request.Headers.TryGetValue("Authorization", out authorizationHeader);
                return ServiceResponse();
            },
            expectSyncPipeline: true);

        client.GetOpenAIModelClient().GetModels(new RequestOptions());

        Assert.That(authorizationHeader, Is.EqualTo($"Bearer {AccessToken}"));
    }

    [Test]
    public async Task OmitsOptionalClientIdWhenNotConfigured()
    {
        string tokenRequestBody = null;
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message =>
            {
                if (IsTokenExchange(message))
                {
                    tokenRequestBody = ReadRequestContent(message);
                    return TokenResponse();
                }

                return ServiceResponse();
            });

        await SendRequestAsync(client);

        using JsonDocument document = JsonDocument.Parse(tokenRequestBody);
        Assert.That(document.RootElement.TryGetProperty("client_id", out _), Is.False);
    }

    [TestCase(3600, 2399)]
    [TestCase(600, 299)]
    public async Task RefreshesExactlyAtSkewedExpiryBoundary(int expiresInSeconds, int secondsBeforeBoundary)
    {
        DateTimeOffset now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        int tokenExchangeCalls = 0;
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message =>
            {
                if (IsTokenExchange(message))
                {
                    Interlocked.Increment(ref tokenExchangeCalls);
                    return TokenResponse(expiresInSeconds);
                }

                return ServiceResponse();
            },
            configureWorkloadIdentityOptions: options => SetUtcNowProvider(options, () => now));

        await SendRequestAsync(client);
        now = now.AddSeconds(secondsBeforeBoundary);
        await SendRequestAsync(client);
        Assert.That(tokenExchangeCalls, Is.EqualTo(1));

        now = now.AddSeconds(1);
        await SendRequestAsync(client);

        Assert.That(tokenExchangeCalls, Is.EqualTo(2));
    }

    [Test]
    public void CancelsSubjectTokenAcquisition()
    {
        OpenAIClient client = CreateClient(
            async cancellationToken =>
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                return SubjectToken;
            },
            _ => throw new AssertionException("The transport should not run after subject-token cancellation."));

        using CancellationTokenSource cancellation = new(TimeSpan.FromMilliseconds(50));
        RequestOptions requestOptions = new() { CancellationToken = cancellation.Token };

        Assert.CatchAsync<OperationCanceledException>(async () =>
            await client.GetOpenAIModelClient().GetModelsAsync(requestOptions));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void RejectsEmptySubjectTokenWithoutCallingTransport(string subjectToken)
    {
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(subjectToken),
            _ => throw new AssertionException("The transport should not run for an empty subject token."));

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await SendRequestAsync(client));

        Assert.That(exception.Message, Does.Not.Contain(SubjectToken));
    }

    [Test]
    public void RejectsInvalidTokenResponseWithoutExposingSecrets()
    {
        const string responseSecret = "response-secret-value";
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message => IsTokenExchange(message)
                ? new MockPipelineResponse(400).WithContent($"{{\"error\":\"{responseSecret}\",\"subject_token\":\"{SubjectToken}\"}}")
                : throw new AssertionException("The service request should not run after a failed token exchange."));

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await SendRequestAsync(client));

        Assert.Multiple(() =>
        {
            Assert.That(exception.ToString(), Does.Not.Contain(SubjectToken));
            Assert.That(exception.ToString(), Does.Not.Contain(responseSecret));
        });
    }

    [Test]
    public void RedactsSecretsFromTransportExceptions()
    {
        const string transportSecret = "transport-secret-value";
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message => IsTokenExchange(message)
                ? throw new InvalidOperationException($"{SubjectToken}:{transportSecret}")
                : throw new AssertionException("The service request should not run after a failed token exchange."));

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await SendRequestAsync(client));

        Assert.Multiple(() =>
        {
            Assert.That(exception.ToString(), Does.Not.Contain(SubjectToken));
            Assert.That(exception.ToString(), Does.Not.Contain(transportSecret));
        });
    }

    [Test]
    public void RedactsAccessTokenFromInvalidResponseExceptions()
    {
        const string invalidAccessToken = "invalid-access-token-secret";
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message => IsTokenExchange(message)
                ? new MockPipelineResponse(200).WithContent(
                    $"{{\"access_token\":\"{invalidAccessToken}\",\"token_type\":\"mac\",\"expires_in\":3600}}")
                : throw new AssertionException("The service request should not run after an invalid token response."));

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await SendRequestAsync(client));

        Assert.Multiple(() =>
        {
            Assert.That(exception.ToString(), Does.Not.Contain(SubjectToken));
            Assert.That(exception.ToString(), Does.Not.Contain(invalidAccessToken));
        });
    }

    [TestCase("{\"expires_in\":3600}")]
    [TestCase("{\"access_token\":\"\",\"token_type\":\"bearer\",\"expires_in\":3600}")]
    [TestCase("{\"access_token\":\"token\",\"expires_in\":3600}")]
    [TestCase("{\"access_token\":\"token\",\"token_type\":\"mac\",\"expires_in\":3600}")]
    [TestCase("{\"access_token\":\"token\",\"token_type\":\"bearer\"}")]
    [TestCase("{\"access_token\":\"token\",\"token_type\":\"bearer\",\"expires_in\":0}")]
    [TestCase("{\"access_token\":\"token\",\"token_type\":\"bearer\",\"expires_in\":-1}")]
    [TestCase("{\"access_token\":\"token\",\"token_type\":\"bearer\",\"expires_in\":\"soon\"}")]
    [TestCase("not-json")]
    public void RejectsMalformedSuccessfulTokenResponse(string responseBody)
    {
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message => IsTokenExchange(message)
                ? new MockPipelineResponse(200).WithContent(responseBody)
                : throw new AssertionException("The service request should not run after an invalid token response."));

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await SendRequestAsync(client));

        Assert.That(exception.ToString(), Does.Not.Contain(SubjectToken));
    }

    [Test]
    public async Task RedactsTokensFromLogsAndDistributedTracing()
    {
        const string responseSecret = "token-response-secret-value";
        CapturingLoggerProvider loggerProvider = new();
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddProvider(loggerProvider);
        });
        ConcurrentQueue<string> diagnosticValues = new();
        using ActivityListener listener = new()
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => CaptureDiagnostics(activity, diagnosticValues),
        };
        ActivitySource.AddActivityListener(listener);

        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message => IsTokenExchange(message)
                ? new MockPipelineResponse(200).WithContent(
                    $"{{\"access_token\":\"{AccessToken}\",\"token_type\":\"bearer\",\"expires_in\":3600,\"diagnostic\":\"{responseSecret}\"}}")
                : ServiceResponse(),
            configureClientOptions: options =>
            {
                options.EnableDistributedTracing = true;
                options.ClientLoggingOptions = new ClientLoggingOptions
                {
                    EnableLogging = true,
                    EnableMessageLogging = true,
                    EnableMessageContentLogging = true,
                    LoggerFactory = loggerFactory,
                };
            });

        await SendRequestAsync(client);

        string captured = string.Join(Environment.NewLine, loggerProvider.Entries.Concat(diagnosticValues));
        Assert.Multiple(() =>
        {
            Assert.That(loggerProvider.Entries, Is.Not.Empty);
            Assert.That(captured, Does.Not.Contain(SubjectToken));
            Assert.That(captured, Does.Not.Contain(AccessToken));
            Assert.That(captured, Does.Not.Contain(responseSecret));
        });
    }

    [Test]
    public async Task PreservesPipelineOptionsAndRetriesTokenExchange()
    {
        int tokenExchangeCalls = 0;
        int serviceRequestCalls = 0;
        Uri serviceRequestUri = null;
        string organizationHeader = null;
        string projectHeader = null;
        string userAgentHeader = null;
        string telemetryLanguageHeader = null;
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message =>
            {
                if (IsTokenExchange(message))
                {
                    return Interlocked.Increment(ref tokenExchangeCalls) == 1
                        ? new MockPipelineResponse(500)
                        : TokenResponse();
                }

                serviceRequestUri = message.Request.Uri;
                int currentServiceRequest = Interlocked.Increment(ref serviceRequestCalls);
                message.Request.Headers.TryGetValue("OpenAI-Organization", out organizationHeader);
                message.Request.Headers.TryGetValue("OpenAI-Project", out projectHeader);
                message.Request.Headers.TryGetValue("User-Agent", out userAgentHeader);
                message.Request.Headers.TryGetValue("X-Stainless-Lang", out telemetryLanguageHeader);
                return currentServiceRequest == 1
                    ? new MockPipelineResponse(500)
                    : ServiceResponse();
            },
            configureClientOptions: options =>
            {
                options.Endpoint = new Uri("https://example.invalid/custom/v1");
                options.OrganizationId = "organization-id";
                options.ProjectId = "project-id";
                options.UserAgentApplicationId = "wif-test-app";
            });

        await SendRequestAsync(client);

        Assert.Multiple(() =>
        {
            Assert.That(tokenExchangeCalls, Is.EqualTo(2));
            Assert.That(serviceRequestCalls, Is.EqualTo(2));
            Assert.That(serviceRequestUri.AbsoluteUri, Does.StartWith("https://example.invalid/custom/v1/"));
            Assert.That(organizationHeader, Is.EqualTo("organization-id"));
            Assert.That(projectHeader, Is.EqualTo("project-id"));
            Assert.That(userAgentHeader, Does.Contain("wif-test-app"));
            Assert.That(telemetryLanguageHeader, Is.EqualTo("csharp"));
        });
    }

    [Test]
    public void ApiKeyAndWorkloadIdentityAuthenticationAreMutuallyExclusive()
    {
        bool hasConflictingConstructor = typeof(OpenAIClient).GetConstructors()
            .Any(constructor =>
            {
                Type[] parameterTypes = constructor.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
                return parameterTypes.Contains(typeof(ApiKeyCredential))
                    && parameterTypes.Contains(typeof(WorkloadIdentityFederationOptions));
            });

        Assert.That(hasConflictingConstructor, Is.False);
    }

    [Test]
    public void ValidatesRequiredConfiguration()
    {
        SubjectTokenProvider provider = _ => new ValueTask<string>(SubjectToken);

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentNullException>(() => new WorkloadIdentityFederationOptions(
                null, WorkloadIdentitySubjectTokenType.Jwt, "idp", "service-account"));
            Assert.Throws<ArgumentOutOfRangeException>(() => new WorkloadIdentityFederationOptions(
                provider, (WorkloadIdentitySubjectTokenType)99, "idp", "service-account"));
            Assert.Throws<ArgumentNullException>(() => new WorkloadIdentityFederationOptions(
                provider, WorkloadIdentitySubjectTokenType.Jwt, null, "service-account"));
            Assert.Throws<ArgumentException>(() => new WorkloadIdentityFederationOptions(
                provider, WorkloadIdentitySubjectTokenType.Jwt, " ", "service-account"));
            Assert.Throws<ArgumentNullException>(() => new WorkloadIdentityFederationOptions(
                provider, WorkloadIdentitySubjectTokenType.Jwt, "idp", null));
            Assert.Throws<ArgumentException>(() => new WorkloadIdentityFederationOptions(
                provider, WorkloadIdentitySubjectTokenType.Jwt, "idp", " "));
            Assert.Throws<ArgumentException>(() => new WorkloadIdentityFederationOptions(
                provider, WorkloadIdentitySubjectTokenType.Jwt, "idp", "service-account", " "));
        });
    }

    private static OpenAIClient CreateClient(
        SubjectTokenProvider subjectTokenProvider,
        Func<PipelineMessage, MockPipelineResponse> transport,
        WorkloadIdentitySubjectTokenType subjectTokenType = WorkloadIdentitySubjectTokenType.Jwt,
        string clientId = null,
        bool expectSyncPipeline = false,
        Action<WorkloadIdentityFederationOptions> configureWorkloadIdentityOptions = null,
        Action<OpenAIClientOptions> configureClientOptions = null)
    {
        WorkloadIdentityFederationOptions workloadIdentityOptions = new(
            subjectTokenProvider,
            subjectTokenType,
            identityProviderId: "identity-provider-id",
            serviceAccountId: "service-account-id",
            clientId);
        configureWorkloadIdentityOptions?.Invoke(workloadIdentityOptions);
        OpenAIClientOptions clientOptions = new()
        {
            Endpoint = new Uri("https://example.invalid/v1"),
            Transport = new MockPipelineTransport(transport)
            {
                ExpectSyncPipeline = expectSyncPipeline,
            },
        };
        configureClientOptions?.Invoke(clientOptions);
        return new OpenAIClient(workloadIdentityOptions, clientOptions);
    }

    private static async Task SendRequestAsync(OpenAIClient client)
        => await client.GetOpenAIModelClient().GetModelsAsync(new RequestOptions());

    private static bool IsTokenExchange(PipelineMessage message)
        => message.Request.Uri == new Uri("https://auth.openai.com/oauth/token");

    private static string ReadRequestContent(PipelineMessage message)
    {
        using MemoryStream stream = new();
        message.Request.Content.WriteTo(stream, CancellationToken.None);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static MockPipelineResponse TokenResponse(int expiresInSeconds = 3600)
        => new MockPipelineResponse(200).WithContent(
            $"{{\"access_token\":\"{AccessToken}\",\"token_type\":\"bearer\",\"expires_in\":{expiresInSeconds}}}");

    private static MockPipelineResponse ServiceResponse()
        => new MockPipelineResponse(200).WithContent("{}");

    private static void SetUtcNowProvider(
        WorkloadIdentityFederationOptions options,
        Func<DateTimeOffset> utcNowProvider)
        => typeof(WorkloadIdentityFederationOptions)
            .GetProperty("UtcNowProvider", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(options, utcNowProvider);

    private static void CaptureDiagnostics(Activity activity, ConcurrentQueue<string> values)
    {
        values.Enqueue(activity.DisplayName);
        foreach (KeyValuePair<string, object> tag in activity.TagObjects)
        {
            values.Enqueue($"{tag.Key}={tag.Value}");
        }
        foreach (ActivityEvent activityEvent in activity.Events)
        {
            values.Enqueue(activityEvent.Name);
            foreach (KeyValuePair<string, object> tag in activityEvent.Tags)
            {
                values.Enqueue($"{tag.Key}={tag.Value}");
            }
        }
    }

    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        public ConcurrentQueue<string> Entries { get; } = new();

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(Entries);

        public void Dispose()
        {
        }

        private sealed class CapturingLogger : ILogger
        {
            private readonly ConcurrentQueue<string> _entries;

            public CapturingLogger(ConcurrentQueue<string> entries)
            {
                _entries = entries;
            }

            public IDisposable BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception exception,
                Func<TState, Exception, string> formatter)
            {
                _entries.Enqueue(formatter(state, exception));
                if (exception is not null)
                {
                    _entries.Enqueue(exception.ToString());
                }
            }
        }
    }
}
