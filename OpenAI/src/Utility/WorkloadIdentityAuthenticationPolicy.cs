#nullable enable

using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI;

internal sealed class WorkloadIdentityAuthenticationPolicy : AuthenticationPolicy
{
    private static readonly object s_replayMarker = new();
    private readonly X509WorkloadIdentityCredential _credential;
    private readonly Uri _endpoint;
    private readonly TimeSpan _networkTimeout;

    internal WorkloadIdentityAuthenticationPolicy(
        X509WorkloadIdentityCredential credential,
        Uri endpoint,
        TimeSpan? networkTimeout)
    {
        Argument.AssertNotNull(credential, nameof(credential));
        _credential = credential;
        _endpoint = endpoint;
        _networkTimeout = networkTimeout ?? TimeSpan.FromSeconds(100);
    }

    public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        WorkloadIdentityPipelineTransport.ValidateBeforeAuthorization(message, _endpoint);
        string token = _credential.GetToken(message.NetworkTimeout ?? _networkTimeout, message.CancellationToken);
        _credential.Transport.Authorize(message, _endpoint, token);
        ProcessNext(message, pipeline, currentIndex);

        if (message.Response?.Status != 401)
        {
            return;
        }

        _credential.Invalidate(token);
        if (!ShouldReplay(message))
        {
            return;
        }

        message.SetProperty(typeof(WorkloadIdentityAuthenticationPolicy), s_replayMarker);
        message.ExtractResponse()?.Dispose();
        WorkloadIdentityPipelineTransport.ValidateBeforeAuthorization(message, _endpoint);
        token = _credential.GetToken(message.NetworkTimeout ?? _networkTimeout, message.CancellationToken);
        _credential.Transport.Authorize(message, _endpoint, token);
        ProcessNext(message, pipeline, currentIndex);

        if (message.Response?.Status == 401)
        {
            _credential.Invalidate(token);
        }
    }

    public override async ValueTask ProcessAsync(
        PipelineMessage message,
        IReadOnlyList<PipelinePolicy> pipeline,
        int currentIndex)
    {
        WorkloadIdentityPipelineTransport.ValidateBeforeAuthorization(message, _endpoint);
        string token = await _credential.GetTokenAsync(message.NetworkTimeout ?? _networkTimeout, message.CancellationToken).ConfigureAwait(false);
        _credential.Transport.Authorize(message, _endpoint, token);
        await ProcessNextAsync(message, pipeline, currentIndex).ConfigureAwait(false);

        if (message.Response?.Status != 401)
        {
            return;
        }

        _credential.Invalidate(token);
        if (!ShouldReplay(message))
        {
            return;
        }

        message.SetProperty(typeof(WorkloadIdentityAuthenticationPolicy), s_replayMarker);
        message.ExtractResponse()?.Dispose();
        WorkloadIdentityPipelineTransport.ValidateBeforeAuthorization(message, _endpoint);
        token = await _credential.GetTokenAsync(message.NetworkTimeout ?? _networkTimeout, message.CancellationToken).ConfigureAwait(false);
        _credential.Transport.Authorize(message, _endpoint, token);
        await ProcessNextAsync(message, pipeline, currentIndex).ConfigureAwait(false);

        if (message.Response?.Status == 401)
        {
            _credential.Invalidate(token);
        }
    }

    private static bool ShouldReplay(PipelineMessage message)
    {
        return !message.TryGetProperty(typeof(WorkloadIdentityAuthenticationPolicy), out _)
            && IsReplayable(message.Request.Content);
    }

    private static bool IsReplayable(BinaryContent? content)
    {
        if (content is null || content is Utf8JsonBinaryContent)
        {
            return true;
        }

        // ClientModel's own nested BinaryContent implementations are backed by immutable bytes,
        // repeatable model serialization, or streams whose seekability was verified at construction.
        // Arbitrary subclasses and multipart/file wrappers cannot make the same guarantee.
        return content.GetType().DeclaringType == typeof(BinaryContent);
    }
}
