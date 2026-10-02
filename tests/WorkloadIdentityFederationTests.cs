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
    public async Task SendsExchangeRequestFieldsAndAppliesBearerToken(
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
    public void SynchronousClientCallsDoNotCaptureTheCallingSynchronizationContext()
    {
        Exception capturedException = null;
        using ManualResetEventSlim completed = new();
        Thread thread = new(() =>
        {
            SynchronizationContext.SetSynchronizationContext(new NonPumpingSynchronizationContext());
            try
            {
                OpenAIClient client = CreateClient(
                    async _ =>
                    {
                        await Task.Yield();
                        return SubjectToken;
                    },
                    message => IsTokenExchange(message) ? TokenResponse() : ServiceResponse(),
                    expectSyncPipeline: true);

                client.GetOpenAIModelClient().GetModels(new RequestOptions());
            }
            catch (Exception exception)
            {
                capturedException = exception;
            }
            finally
            {
                completed.Set();
            }
        })
        {
            IsBackground = true,
        };

        thread.Start();

        Assert.Multiple(() =>
        {
            Assert.That(completed.Wait(TimeSpan.FromSeconds(10)), Is.True, "The synchronous call deadlocked.");
            Assert.That(capturedException, Is.Null);
        });
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
    public async Task UsesUnexpiredCachedTokenWhenProactiveRefreshFails()
    {
        DateTimeOffset initialTime = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset now = initialTime;
        int tokenExchangeCalls = 0;
        int serviceRequestCalls = 0;
        string authorizationHeader = null;
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message =>
            {
                if (IsTokenExchange(message))
                {
                    return Interlocked.Increment(ref tokenExchangeCalls) == 1
                        ? TokenResponse()
                        : new MockPipelineResponse(400).WithContent("{\"error\":\"invalid_grant\"}");
                }

                Interlocked.Increment(ref serviceRequestCalls);
                message.Request.Headers.TryGetValue("Authorization", out authorizationHeader);
                return ServiceResponse();
            },
            configureWorkloadIdentityOptions: options => SetUtcNowProvider(options, () => now));

        await SendRequestAsync(client);
        now = initialTime.AddMinutes(40);
        await SendRequestAsync(client);

        now = initialTime.AddHours(1);
        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await SendRequestAsync(client));

        Assert.Multiple(() =>
        {
            Assert.That(tokenExchangeCalls, Is.EqualTo(3));
            Assert.That(serviceRequestCalls, Is.EqualTo(2));
            Assert.That(authorizationHeader, Is.EqualTo($"Bearer {AccessToken}"));
            Assert.That(exception.Message, Does.Contain("HTTP status 400"));
        });
    }

    [Test]
    public async Task ConcurrentRequestsUseCachedTokenDuringProactiveRefresh()
    {
        DateTimeOffset now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        int subjectTokenCalls = 0;
        int tokenExchangeCalls = 0;
        int serviceRequestCalls = 0;
        TaskCompletionSource<bool> refreshStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<bool> releaseRefresh = new(TaskCreationOptions.RunContinuationsAsynchronously);
        OpenAIClient client = CreateClient(
            async _ =>
            {
                if (Interlocked.Increment(ref subjectTokenCalls) == 2)
                {
                    refreshStarted.TrySetResult(true);
                    await releaseRefresh.Task.ConfigureAwait(false);
                }
                return SubjectToken;
            },
            message =>
            {
                if (IsTokenExchange(message))
                {
                    Interlocked.Increment(ref tokenExchangeCalls);
                    return TokenResponse();
                }

                Interlocked.Increment(ref serviceRequestCalls);
                return ServiceResponse();
            },
            configureWorkloadIdentityOptions: options => SetUtcNowProvider(options, () => now));

        await SendRequestAsync(client);
        now = now.AddMinutes(40);
        Task refreshingRequest = SendRequestAsync(client);
        await refreshStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

        try
        {
            Task cachedRequest = SendRequestAsync(client);
            await cachedRequest.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.That(serviceRequestCalls, Is.EqualTo(2));
        }
        finally
        {
            releaseRefresh.TrySetResult(true);
        }

        await refreshingRequest;

        Assert.Multiple(() =>
        {
            Assert.That(subjectTokenCalls, Is.EqualTo(2));
            Assert.That(tokenExchangeCalls, Is.EqualTo(2));
            Assert.That(serviceRequestCalls, Is.EqualTo(3));
        });
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
                ? new MockPipelineResponse(400).WithContent(
                    $"{{\"error\":\"invalid_grant\",\"error_description\":\"{responseSecret}\",\"subject_token\":\"{SubjectToken}\"}}")
                : throw new AssertionException("The service request should not run after a failed token exchange."));

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await SendRequestAsync(client));

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("HTTP status 400"));
            Assert.That(exception.Message, Does.Contain("invalid_grant"));
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
    public async Task CallerAddedPoliciesDoNotObserveTokenExchangeRequests()
    {
        int servicePolicyCalls = 0;
        int tokenExchangePolicyCalls = 0;
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message => IsTokenExchange(message) ? TokenResponse() : ServiceResponse(),
            configureClientOptions: options => options.AddPolicy(
                new TestPipelinePolicy(message =>
                {
                    if (IsTokenExchange(message))
                    {
                        Interlocked.Increment(ref tokenExchangePolicyCalls);
                    }
                    else
                    {
                        Interlocked.Increment(ref servicePolicyCalls);
                    }
                }),
                PipelinePosition.BeforeTransport));

        await SendRequestAsync(client);

        Assert.Multiple(() =>
        {
            Assert.That(tokenExchangePolicyCalls, Is.Zero);
            Assert.That(servicePolicyCalls, Is.GreaterThan(0));
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
    public void ExposesSubjectTokenProviderInterfaceAndWifFeatureClientConstructors()
    {
        Assert.Multiple(() =>
        {
            Assert.That(typeof(ISubjectTokenProvider).IsPublic, Is.True);
            Assert.That(typeof(ISubjectTokenProvider).IsInterface, Is.True);
            Assert.That(typeof(ISubjectTokenProvider).GetProperty(nameof(ISubjectTokenProvider.TokenType)), Is.Not.Null);
            Assert.That(typeof(ISubjectTokenProvider).GetMethod(nameof(ISubjectTokenProvider.GetTokenAsync)), Is.Not.Null);
        });

        Type[] featureClientTypes =
        [
            typeof(global::OpenAI.Assistants.AssistantClient),
            typeof(global::OpenAI.Audio.AudioClient),
            typeof(global::OpenAI.Batch.BatchClient),
            typeof(global::OpenAI.Chat.ChatClient),
            typeof(global::OpenAI.Containers.ContainerClient),
            typeof(global::OpenAI.Conversations.ConversationClient),
            typeof(global::OpenAI.Embeddings.EmbeddingClient),
            typeof(global::OpenAI.Evals.EvaluationClient),
            typeof(global::OpenAI.Files.OpenAIFileClient),
            typeof(global::OpenAI.FineTuning.FineTuningClient),
            typeof(global::OpenAI.Graders.GraderClient),
            typeof(global::OpenAI.Images.ImageClient),
            typeof(global::OpenAI.Models.OpenAIModelClient),
            typeof(global::OpenAI.Moderations.ModerationClient),
            typeof(global::OpenAI.Realtime.RealtimeClient),
            typeof(global::OpenAI.Responses.ResponsesClient),
            typeof(global::OpenAI.Skills.SkillClient),
            typeof(global::OpenAI.VectorStores.VectorStoreClient),
            typeof(global::OpenAI.Videos.VideoClient),
        ];

        Assert.Multiple(() =>
        {
            foreach (Type featureClientType in featureClientTypes)
            {
                Assert.That(
                    featureClientType.GetConstructors().Any(constructor => constructor.GetParameters()
                        .Any(parameter => parameter.ParameterType == typeof(WorkloadIdentityFederationOptions))),
                    Is.True,
                    $"{featureClientType.FullName} should accept workload identity federation options.");
            }
        });
    }

    [Test]
    public async Task StandaloneFeatureClientAuthenticatesWithWorkloadIdentity()
    {
        string authorizationHeader = null;
        WorkloadIdentityFederationOptions workloadIdentityOptions = CreateWorkloadIdentityOptions(
            _ => new ValueTask<string>(SubjectToken));
        OpenAIClientOptions clientOptions = new()
        {
            Endpoint = new Uri("https://example.invalid/v1"),
            Transport = new MockPipelineTransport(message =>
            {
                if (IsTokenExchange(message))
                {
                    return TokenResponse();
                }

                message.Request.Headers.TryGetValue("Authorization", out authorizationHeader);
                return ServiceResponse();
            })
            {
                ExpectSyncPipeline = false,
            },
        };
        global::OpenAI.Models.OpenAIModelClient client = new(workloadIdentityOptions, clientOptions);

        await client.GetModelsAsync(new RequestOptions());

        Assert.That(authorizationHeader, Is.EqualTo($"Bearer {AccessToken}"));
    }

    [Test]
    public async Task RealtimeClientsUseWifAccessTokenForWebSocketSessions()
    {
        int tokenExchangeCalls = 0;
        Func<PipelineMessage, MockPipelineResponse> transport = message =>
        {
            if (IsTokenExchange(message))
            {
                Interlocked.Increment(ref tokenExchangeCalls);
                return TokenResponse();
            }

            throw new AssertionException("Only the token exchange should use the HTTP transport.");
        };
        WorkloadIdentityFederationOptions workloadIdentityOptions = CreateWorkloadIdentityOptions(
            _ => new ValueTask<string>(SubjectToken));
        global::OpenAI.Realtime.RealtimeClient standaloneClient = new(
            workloadIdentityOptions,
            new global::OpenAI.Realtime.RealtimeClientOptions
            {
                Transport = new MockPipelineTransport(transport) { ExpectSyncPipeline = false },
            });
        OpenAIClient topLevelClient = new(
            workloadIdentityOptions,
            new OpenAIClientOptions
            {
                Transport = new MockPipelineTransport(transport) { ExpectSyncPipeline = false },
            });

        ApiKeyCredential standaloneCredential = await GetRealtimeSessionCredentialAsync(standaloneClient);
        ApiKeyCredential sharedPipelineCredential = await GetRealtimeSessionCredentialAsync(topLevelClient.GetRealtimeClient());
        standaloneCredential.Deconstruct(out string standaloneToken);
        sharedPipelineCredential.Deconstruct(out string sharedPipelineToken);

        Assert.Multiple(() =>
        {
            Assert.That(standaloneToken, Is.EqualTo(AccessToken));
            Assert.That(sharedPipelineToken, Is.EqualTo(AccessToken));
            Assert.That(tokenExchangeCalls, Is.EqualTo(2));
        });
    }

    [Test]
    public void ValidatesRequiredConfiguration()
    {
        ISubjectTokenProvider provider = new TestSubjectTokenProvider(
            _ => new ValueTask<string>(SubjectToken),
            WorkloadIdentitySubjectTokenType.Jwt);
        ISubjectTokenProvider unsupportedProvider = new TestSubjectTokenProvider(
            _ => new ValueTask<string>(SubjectToken),
            (WorkloadIdentitySubjectTokenType)99);

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentNullException>(() => new WorkloadIdentityFederationOptions(
                null, "idp", "service-account"));
            Assert.Throws<ArgumentOutOfRangeException>(() => new WorkloadIdentityFederationOptions(
                unsupportedProvider, "idp", "service-account"));
            Assert.Throws<ArgumentNullException>(() => new WorkloadIdentityFederationOptions(
                provider, null, "service-account"));
            Assert.Throws<ArgumentException>(() => new WorkloadIdentityFederationOptions(
                provider, " ", "service-account"));
            Assert.Throws<ArgumentNullException>(() => new WorkloadIdentityFederationOptions(
                provider, "idp", null));
            Assert.Throws<ArgumentException>(() => new WorkloadIdentityFederationOptions(
                provider, "idp", " "));
            Assert.Throws<ArgumentException>(() => new WorkloadIdentityFederationOptions(
                provider, "idp", "service-account", " "));
        });
    }

    private static OpenAIClient CreateClient(
        Func<CancellationToken, ValueTask<string>> subjectTokenProvider,
        Func<PipelineMessage, MockPipelineResponse> transport,
        WorkloadIdentitySubjectTokenType subjectTokenType = WorkloadIdentitySubjectTokenType.Jwt,
        string clientId = null,
        bool expectSyncPipeline = false,
        Action<WorkloadIdentityFederationOptions> configureWorkloadIdentityOptions = null,
        Action<OpenAIClientOptions> configureClientOptions = null)
    {
        WorkloadIdentityFederationOptions workloadIdentityOptions = CreateWorkloadIdentityOptions(
            subjectTokenProvider,
            subjectTokenType,
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

    private static WorkloadIdentityFederationOptions CreateWorkloadIdentityOptions(
        Func<CancellationToken, ValueTask<string>> subjectTokenProvider,
        WorkloadIdentitySubjectTokenType subjectTokenType = WorkloadIdentitySubjectTokenType.Jwt,
        string clientId = null)
        => new(
            new TestSubjectTokenProvider(subjectTokenProvider, subjectTokenType),
            identityProviderId: "identity-provider-id",
            serviceAccountId: "service-account-id",
            clientId);

    private static async ValueTask<ApiKeyCredential> GetRealtimeSessionCredentialAsync(
        global::OpenAI.Realtime.RealtimeClient client)
    {
        MethodInfo method = typeof(global::OpenAI.Realtime.RealtimeClient).GetMethod(
            "GetSessionCredentialAsync",
            BindingFlags.Instance | BindingFlags.NonPublic);
        ValueTask<ApiKeyCredential> credentialTask = (ValueTask<ApiKeyCredential>)method.Invoke(
            client,
            [new global::OpenAI.Realtime.RealtimeSessionClientOptions(), CancellationToken.None]);
        return await credentialTask;
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

    private sealed class NonPumpingSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object state)
        {
        }
    }

    private sealed class TestSubjectTokenProvider : ISubjectTokenProvider
    {
        private readonly Func<CancellationToken, ValueTask<string>> _getToken;

        public TestSubjectTokenProvider(
            Func<CancellationToken, ValueTask<string>> getToken,
            WorkloadIdentitySubjectTokenType tokenType)
        {
            _getToken = getToken;
            TokenType = tokenType;
        }

        public WorkloadIdentitySubjectTokenType TokenType { get; }

        public ValueTask<string> GetTokenAsync(CancellationToken cancellationToken)
            => _getToken(cancellationToken);
    }
}
