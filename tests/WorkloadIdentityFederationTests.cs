using Microsoft.ClientModel.TestFramework.Mocks;
using NUnit.Framework;
using System;
using System.ClientModel.Primitives;
using System.IO;
using System.Linq;
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

    [Test]
    public async Task RefreshesAtConfiguredTokenRefreshTime()
    {
        int tokenExchangeCalls = 0;
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message =>
            {
                if (IsTokenExchange(message))
                {
                    Interlocked.Increment(ref tokenExchangeCalls);
                    return TokenResponse(expiresInSeconds: 2);
                }

                return ServiceResponse();
            });

        await SendRequestAsync(client);
        await Task.Delay(TimeSpan.FromMilliseconds(1200));
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

    [Test]
    public void RejectsEmptySubjectTokenWithoutCallingTransport()
    {
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(" "),
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
            Assert.That(exception.Message, Does.Not.Contain(SubjectToken));
            Assert.That(exception.Message, Does.Not.Contain(responseSecret));
        });
    }

    [Test]
    public void RejectsMalformedSuccessfulTokenResponse()
    {
        OpenAIClient client = CreateClient(
            _ => new ValueTask<string>(SubjectToken),
            message => IsTokenExchange(message)
                ? new MockPipelineResponse(200).WithContent("{\"expires_in\":3600}")
                : throw new AssertionException("The service request should not run after an invalid token response."));

        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await SendRequestAsync(client));

        Assert.That(exception.Message, Does.Not.Contain(SubjectToken));
    }

    [Test]
    public void ValidatesRequiredConfiguration()
    {
        SubjectTokenProvider provider = _ => new ValueTask<string>(SubjectToken);

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentNullException>(() => new WorkloadIdentityFederationOptions(
                null, WorkloadIdentitySubjectTokenType.Jwt, "idp", "service-account"));
            Assert.Throws<ArgumentException>(() => new WorkloadIdentityFederationOptions(
                provider, WorkloadIdentitySubjectTokenType.Jwt, " ", "service-account"));
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
        bool expectSyncPipeline = false)
    {
        WorkloadIdentityFederationOptions workloadIdentityOptions = new(
            subjectTokenProvider,
            subjectTokenType,
            identityProviderId: "identity-provider-id",
            serviceAccountId: "service-account-id",
            clientId);
        OpenAIClientOptions clientOptions = new()
        {
            Endpoint = new Uri("https://example.invalid/v1"),
            Transport = new MockPipelineTransport(transport)
            {
                ExpectSyncPipeline = expectSyncPipeline,
            },
        };
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
}
