#nullable enable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Net.Http;

namespace OpenAI;

internal sealed class WorkloadIdentityPipelineTransport : HttpClientPipelineTransport
{
    private readonly X509WorkloadIdentityCredential _credential;

    internal WorkloadIdentityPipelineTransport(HttpClient client, X509WorkloadIdentityCredential credential)
        : base(client)
    {
        _credential = credential;
    }

    internal static void ValidateDestination(Uri? destination, Uri expectedEndpoint)
    {
        if (destination is null
            || !destination.IsAbsoluteUri
            || !string.Equals(destination.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            || destination.UserInfo.Length != 0
            || !string.Equals(destination.IdnHost, expectedEndpoint.IdnHost, StringComparison.OrdinalIgnoreCase)
            || destination.Port != expectedEndpoint.Port)
        {
            throw new InvalidOperationException(
                "X.509 workload identity requests must remain on the configured HTTPS API origin.");
        }
    }

    internal void Authorize(PipelineMessage message, Uri endpoint, string token)
    {
        ValidateDestination(message.Request.Uri, endpoint);
        message.Request.Headers.Set("Authorization", "Bearer " + token);
        message.SetProperty(typeof(WorkloadIdentityPipelineTransport), new RequestAuthorization(endpoint, token));
    }

    protected override void OnSendingRequest(PipelineMessage message, HttpRequestMessage request)
    {
        base.OnSendingRequest(message, request);

        if (!message.TryGetProperty(typeof(WorkloadIdentityPipelineTransport), out object? value)
            || value is not RequestAuthorization authorization)
        {
            throw new InvalidOperationException(
                "X.509 workload identity requests must be authenticated by their configured credential.");
        }

        ValidateDestination(request.RequestUri, authorization.Endpoint);
        _credential.ValidateEndpoint(request.RequestUri!);

        if (!request.Headers.TryGetValues("Authorization", out IEnumerable<string>? values)
            || !ContainsOnlyExpectedAuthorization(values, authorization.Token))
        {
            throw new InvalidOperationException(
                "X.509 workload identity request authorization changed after token acquisition.");
        }
    }

    private static bool ContainsOnlyExpectedAuthorization(IEnumerable<string> values, string token)
    {
        using IEnumerator<string> enumerator = values.GetEnumerator();
        return enumerator.MoveNext()
            && string.Equals(enumerator.Current, "Bearer " + token, StringComparison.Ordinal)
            && !enumerator.MoveNext();
    }

    private sealed class RequestAuthorization(Uri endpoint, string token)
    {
        internal Uri Endpoint { get; } = endpoint;
        internal string Token { get; } = token;
    }
}
