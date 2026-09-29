using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI;

/// <summary>
/// Represents an asynchronous callback that supplies a subject token for workload identity federation.
/// </summary>
/// <param name="cancellationToken">A token that can cancel subject-token acquisition.</param>
/// <returns>The subject token to exchange for an OpenAI access token.</returns>
public delegate ValueTask<string> SubjectTokenProvider(CancellationToken cancellationToken);

/// <summary>Identifies the kind of subject token supplied for workload identity federation.</summary>
public enum WorkloadIdentitySubjectTokenType
{
    /// <summary>A JSON Web Token.</summary>
    Jwt,

    /// <summary>An OpenID Connect ID token.</summary>
    IdToken,
}

/// <summary>Configures workload identity federation authentication for an <see cref="OpenAIClient"/>.</summary>
public sealed class WorkloadIdentityFederationOptions
{
    /// <summary>Initializes a new instance of <see cref="WorkloadIdentityFederationOptions"/>.</summary>
    /// <param name="subjectTokenProvider">The asynchronous callback that supplies subject tokens.</param>
    /// <param name="subjectTokenType">The kind of subject token supplied by <paramref name="subjectTokenProvider"/>.</param>
    /// <param name="identityProviderId">The OpenAI identity-provider identifier.</param>
    /// <param name="serviceAccountId">The OpenAI service-account identifier.</param>
    /// <param name="clientId">An optional OAuth client identifier.</param>
    public WorkloadIdentityFederationOptions(
        SubjectTokenProvider subjectTokenProvider,
        WorkloadIdentitySubjectTokenType subjectTokenType,
        string identityProviderId,
        string serviceAccountId,
        string clientId = null)
    {
        SubjectTokenProvider = subjectTokenProvider ?? throw new ArgumentNullException(nameof(subjectTokenProvider));
        IdentityProviderId = AssertNotNullOrWhiteSpace(identityProviderId, nameof(identityProviderId));
        ServiceAccountId = AssertNotNullOrWhiteSpace(serviceAccountId, nameof(serviceAccountId));

        if (clientId is not null && string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("The client ID cannot be empty or whitespace.", nameof(clientId));
        }

        SubjectTokenType = subjectTokenType;
        ClientId = clientId;
    }

    /// <summary>Gets the asynchronous callback that supplies subject tokens.</summary>
    public SubjectTokenProvider SubjectTokenProvider { get; }

    /// <summary>Gets the kind of subject token supplied by <see cref="SubjectTokenProvider"/>.</summary>
    public WorkloadIdentitySubjectTokenType SubjectTokenType { get; }

    /// <summary>Gets the OpenAI identity-provider identifier.</summary>
    public string IdentityProviderId { get; }

    /// <summary>Gets the OpenAI service-account identifier.</summary>
    public string ServiceAccountId { get; }

    /// <summary>Gets the optional OAuth client identifier.</summary>
    public string ClientId { get; }

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
