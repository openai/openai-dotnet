#nullable enable

using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

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

    internal static void ValidateBeforeAuthorization(PipelineMessage message, Uri endpoint)
    {
        ValidateDestination(message.Request.Uri, endpoint);

        if (message.Request.Headers.TryGetValues("Host", out IEnumerable<string>? hostValues))
        {
            ValidateHostAuthority(hostValues, endpoint);
        }

        foreach (KeyValuePair<string, string> header in message.Request.Headers)
        {
            ValidateCredentialHeaderName(header.Key);
            if (string.Equals(header.Key, "Authorization", StringComparison.OrdinalIgnoreCase)
                && (!message.TryGetProperty(typeof(WorkloadIdentityPipelineTransport), out object? value)
                    || value is not RequestAuthorization authorization
                    || !string.Equals(header.Value, "Bearer " + authorization.Token, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException(
                    "X.509 workload identity requests cannot contain another authentication credential.");
            }
        }
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
        if (request.Headers.TryGetValues("Host", out IEnumerable<string>? hostValues))
        {
            ValidateHostAuthority(hostValues, authorization.Endpoint);
        }

        ValidateCredentialHeaders(request.Headers);
        if (request.Content is not null)
        {
            ValidateCredentialHeaders(request.Content.Headers);
        }

        _credential.ValidateEndpoint(request.RequestUri!);

        if (!request.Headers.TryGetValues("Authorization", out IEnumerable<string>? values)
            || !ContainsOnlyExpectedAuthorization(values, authorization.Token))
        {
            throw new InvalidOperationException(
                "X.509 workload identity request authorization changed after token acquisition.");
        }
    }

    private static void ValidateHostAuthority(IEnumerable<string>? values, Uri expectedEndpoint)
    {
        if (values is null)
        {
            throw new InvalidOperationException(
                "X.509 workload identity requests must use their configured HTTPS host authority.");
        }

        using IEnumerator<string> enumerator = values.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            throw new InvalidOperationException(
                "X.509 workload identity requests must use their configured HTTPS host authority.");
        }

        string authority = enumerator.Current;
        if (enumerator.MoveNext()
            || authority.IndexOf(',') >= 0
            || !Uri.TryCreate(expectedEndpoint.Scheme + "://" + authority, UriKind.Absolute, out Uri? actual)
            || actual.AbsolutePath != "/"
            || actual.Query.Length != 0
            || actual.Fragment.Length != 0)
        {
            throw new InvalidOperationException(
                "X.509 workload identity requests must use their configured HTTPS host authority.");
        }

        ValidateDestination(actual, expectedEndpoint);
    }

    private static void ValidateCredentialHeaders(HttpHeaders headers)
    {
        foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
        {
            ValidateCredentialHeaderName(header.Key);
        }
    }

    private static void ValidateCredentialHeaderName(string name)
    {
        string normalized = name.Replace('_', '-');
        if (string.Equals(normalized, "Api-Key", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "X-Api-Key", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "Proxy-Authorization", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "X.509 workload identity requests cannot contain another authentication credential.");
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
