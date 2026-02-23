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
    /// <summary> Starts a new <see cref="RealtimeSessionClient"/> for multimodal conversation. </summary>
    /// <remarks>
    ///     The <see cref="RealtimeSessionClient"/> abstracts bidirectional communication between the caller and service,
    ///     simultaneously sending and receiving WebSocket messages.
    /// </remarks>
    public virtual async Task<RealtimeSessionClient> StartConversationSessionAsync(string model, RealtimeSessionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(model, nameof(model));

        return await StartSessionAsync(
            model: model,
            intent: null,
            options: options,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary> Starts a new <see cref="RealtimeSessionClient"/> for multimodal conversation. </summary>
    /// <remarks>
    ///     The <see cref="RealtimeSessionClient"/> abstracts bidirectional communication between the caller and service,
    ///     simultaneously sending and receiving WebSocket messages.
    /// </remarks>
    public RealtimeSessionClient StartConversationSession(string model, RealtimeSessionOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(model, nameof(model));

        return StartSession(
            model: model,
            intent: null,
            options: options,
            cancellationToken: cancellationToken);
    }

    /// <summary> Starts a new <see cref="RealtimeSessionClient"/> for audio transcription.</summary>
    /// <remarks>
    ///     The <see cref="RealtimeSessionClient"/> abstracts bidirectional communication between the caller and service,
    ///     simultaneously sending and receiving WebSocket messages.
    /// </remarks>
    public virtual async Task<RealtimeSessionClient> StartTranscriptionSessionAsync(RealtimeSessionOptions options = null, CancellationToken cancellationToken = default)
    {
        return await StartSessionAsync(
            model: null,
            intent: "transcription",
            options: options,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary> Starts a new <see cref="RealtimeSessionClient"/> for audio transcription.</summary>
    /// <remarks>
    ///     The <see cref="RealtimeSessionClient"/> abstracts bidirectional communication between the caller and service,
    ///     simultaneously sending and receiving WebSocket messages.
    /// </remarks>
    public RealtimeSessionClient StartTranscriptionSession(RealtimeSessionOptions options = null, CancellationToken cancellationToken = default)
    {
        return StartSession(
            model: null,
            intent: "transcription",
            options: options,
            cancellationToken: cancellationToken);
    }

    /// <summary> Starts a new <see cref="RealtimeSessionClient"/>. </summary>
    /// <remarks>
    ///     The <see cref="RealtimeSessionClient"/> abstracts bidirectional communication between the caller and service,
    ///     simultaneously sending and receiving WebSocket messages.
    /// </remarks>
    public virtual async Task<RealtimeSessionClient> StartSessionAsync(string model, string intent, RealtimeSessionOptions options = null, CancellationToken cancellationToken = default)
    {
        options ??= new();

        RealtimeSessionClient provisionalSession = new(_keyCredential, this, _webSocketEndpoint, model, intent);

        try
        {
            await provisionalSession.ConnectAsync(options.QueryString, options.Headers, cancellationToken).ConfigureAwait(false);
            RealtimeSessionClient result = provisionalSession;
            provisionalSession = null;
            return result;
        }
        finally
        {
            provisionalSession?.Dispose();
        }
    }

    /// <summary> Starts a new <see cref="RealtimeSessionClient"/>. </summary>
    /// <remarks>
    ///     The <see cref="RealtimeSessionClient"/> abstracts bidirectional communication between the caller and service,
    ///     simultaneously sending and receiving WebSocket messages.
    /// </remarks>
    public RealtimeSessionClient StartSession(string model, string intent, RealtimeSessionOptions options = null, CancellationToken cancellationToken = default)
    {
        return StartSessionAsync(model, intent, options, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
    }
}