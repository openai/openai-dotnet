using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Realtime;

[CodeGenSuppress("StartRealtimeSessionAsync", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("StartRealtimeSession", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateStartRealtimeSessionRequest", typeof(BinaryContent), typeof(RequestOptions))]
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

    /// <summary> Start a new Realtime conversation session. </summary>
    public virtual RealtimeSessionClient StartConversationSession(string model, RealtimeSessionClientOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(model, nameof(model));
        return StartSession(
            model: model,
            intent: null,
            options: options,
            cancellationToken: cancellationToken);
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

    /// <summary> Start a new Realtime transcription session. </summary>
    public virtual RealtimeSessionClient StartTranscriptionSession(RealtimeSessionClientOptions options = null, CancellationToken cancellationToken = default)
    {
        return StartSession(
            model: null,
            intent: "transcription",
            options: options,
            cancellationToken: cancellationToken);
    }

    /// <summary> Starts a new realtime session. </summary>
    public virtual async Task<RealtimeSessionClient> StartSessionAsync(string model, string intent, RealtimeSessionClientOptions options = null, CancellationToken cancellationToken = default)
    {
        options ??= new();

        ApiKeyCredential credential = _keyCredential;

        // If we have an ephemeral client secret, we should use that to create the WebSocket session.
        // Otherwise, we should use the OpenAI API key that this client initialized with, if any.
        if (!string.IsNullOrEmpty(options.ClientSecret))
        {
            credential = new ApiKeyCredential(options.ClientSecret);
        }

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

    /// <summary> Starts a new realtime session. </summary>
    public virtual RealtimeSessionClient StartSession(string model, string intent, RealtimeSessionClientOptions options = null, CancellationToken cancellationToken = default)
    {
        return StartSessionAsync(model, intent, options, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
    }
}