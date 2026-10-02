using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Realtime;

public partial class RealtimeClient
{
    /// <summary> Start a new Realtime conversation session. </summary>
    public virtual async Task<RealtimeSessionClient> StartConversationSessionAsync(string model, RealtimeSessionClientOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(model, nameof(model));
        return await StartSessionAsync(
            model: model,
            intent: null,
            options: options,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary> Start a new Realtime transcription session. </summary>
    public virtual async Task<RealtimeSessionClient> StartTranscriptionSessionAsync(RealtimeSessionClientOptions options = null, CancellationToken cancellationToken = default)
    {
        return await StartSessionAsync(
            model: null,
            intent: "transcription",
            options: options,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary> Starts a new realtime session. </summary>
    public virtual async Task<RealtimeSessionClient> StartSessionAsync(string model, string intent, RealtimeSessionClientOptions options = null, CancellationToken cancellationToken = default)
    {
        options ??= new();

        ApiKeyCredential credential = await GetSessionCredentialAsync(options, cancellationToken).ConfigureAwait(false);

        // Resolve an ephemeral client secret, a workload-identity access token, or the API key
        // that initialized this client, in that order.
        RealtimeSessionClient sessionClient = new(
            credential: credential,
            endpoint: _webSocketEndpoint,
            model: model,
            intent: intent,
            parentClient: this);

        try
        {
            await sessionClient.ConnectAsync(options.QueryString, options.Headers, cancellationToken).ConfigureAwait(false);
            RealtimeSessionClient result = sessionClient;
            sessionClient = null;
            return result;
        }
        finally
        {
            sessionClient?.Dispose();
        }
    }

    internal async ValueTask<ApiKeyCredential> GetSessionCredentialAsync(
        RealtimeSessionClientOptions options,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(options?.ClientSecret))
        {
            return new ApiKeyCredential(options.ClientSecret);
        }

        if (_workloadIdentityTokenProvider is not null)
        {
            string accessToken = await _workloadIdentityTokenProvider
                .GetAccessTokenAsync(cancellationToken)
                .ConfigureAwait(false);
            return new ApiKeyCredential(accessToken);
        }

        return _keyCredential;
    }
}
