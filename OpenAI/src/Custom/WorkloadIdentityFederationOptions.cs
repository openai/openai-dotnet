using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI;

/// <summary>Identifies the kind of subject token supplied for workload identity federation.</summary>
public enum WorkloadIdentitySubjectTokenType
{
    /// <summary>A JSON Web Token.</summary>
    Jwt,

    /// <summary>An OpenID Connect ID token.</summary>
    IdToken,
}

/// <summary>Supplies short-lived subject tokens for workload identity federation.</summary>
public interface ISubjectTokenProvider
{
    /// <summary>Gets the kind of subject token supplied by this provider.</summary>
    WorkloadIdentitySubjectTokenType TokenType { get; }

    /// <summary>Gets a fresh subject token to exchange for an OpenAI access token.</summary>
    /// <param name="cancellationToken">A token that can cancel subject-token acquisition.</param>
    /// <returns>The subject token to exchange for an OpenAI access token.</returns>
    ValueTask<string> GetTokenAsync(CancellationToken cancellationToken);
}

/// <summary>Configures workload identity federation authentication for an <see cref="OpenAIClient"/>.</summary>
public sealed class WorkloadIdentityFederationOptions
{
    /// <summary>Initializes a new instance of <see cref="WorkloadIdentityFederationOptions"/>.</summary>
    /// <param name="subjectTokenProvider">The provider that supplies subject tokens.</param>
    /// <param name="identityProviderId">The OpenAI identity-provider identifier.</param>
    /// <param name="serviceAccountId">The OpenAI service-account identifier.</param>
    /// <param name="clientId">An optional OAuth client identifier.</param>
    public WorkloadIdentityFederationOptions(
        ISubjectTokenProvider subjectTokenProvider,
        string identityProviderId,
        string serviceAccountId,
        string clientId = null)
    {
        SubjectTokenProvider = subjectTokenProvider ?? throw new ArgumentNullException(nameof(subjectTokenProvider));
        IdentityProviderId = AssertNotNullOrWhiteSpace(identityProviderId, nameof(identityProviderId));
        ServiceAccountId = AssertNotNullOrWhiteSpace(serviceAccountId, nameof(serviceAccountId));

        if (!Enum.IsDefined(typeof(WorkloadIdentitySubjectTokenType), subjectTokenProvider.TokenType))
        {
            throw new ArgumentOutOfRangeException(nameof(subjectTokenProvider), "The subject token type is not supported.");
        }

        if (clientId is not null && string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("The client ID cannot be empty or whitespace.", nameof(clientId));
        }

        ClientId = clientId;
    }

    /// <summary>Gets the provider that supplies subject tokens.</summary>
    public ISubjectTokenProvider SubjectTokenProvider { get; }

    /// <summary>Gets the OpenAI identity-provider identifier.</summary>
    public string IdentityProviderId { get; }

    /// <summary>Gets the OpenAI service-account identifier.</summary>
    public string ServiceAccountId { get; }

    /// <summary>Gets the optional OAuth client identifier.</summary>
    public string ClientId { get; }

    internal Func<DateTimeOffset> UtcNowProvider { get; set; } = () => DateTimeOffset.UtcNow;

    private static string AssertNotNullOrWhiteSpace(string value, string parameterName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be empty or whitespace.", parameterName);
        }

        return value;
    }
}
