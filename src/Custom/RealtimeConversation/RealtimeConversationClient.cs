using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.RealtimeConversation;

[CodeGenClient("Realtime")]
[CodeGenSuppress("StartRealtimeSessionAsync", typeof(IEnumerable<InternalRealtimeClientEvent>))]
[CodeGenSuppress("StartRealtimeSessionAsync", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("StartRealtimeSession", typeof(IEnumerable<InternalRealtimeClientEvent>))]
[CodeGenSuppress("StartRealtimeSession", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateStartRealtimeSessionRequest", typeof(BinaryContent), typeof(RequestOptions))]
[Experimental("OPENAI002")]
public partial class RealtimeConversationClient
{
    public event EventHandler<BinaryData> OnSendingCommand;
    public event EventHandler<BinaryData> OnReceivingCommand;

    private readonly Uri _endpoint;
    private readonly ApiKeyCredential _credential;

    /// <summary>
    /// Creates a new instance of <see cref="RealtimeConversationClient"/> using an API key for authentication.
    /// </summary>
    /// <param name="credential"> The API key to use for authentication. </param>
    public RealtimeConversationClient(string model, ApiKeyCredential credential) : this(model, credential, new OpenAIClientOptions())
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="RealtimeConversationClient"/> using an API key for authentication.
    /// </summary>
    /// <param name="credential"> The API key to use for authentication. </param>
    /// <param name="options"> Additional options for configuring the client. </param>
    public RealtimeConversationClient(string model, ApiKeyCredential credential, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(credential, nameof(credential));
        Argument.AssertNotNull(options, nameof(options));

        _credential = credential;
        _endpoint = GetEndpoint(model, options);
    }

    protected internal RealtimeConversationClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        throw new NotImplementedException("Pipeline-based initialization of WS-based client not available");
    }

    /// <summary>
    /// Starts a new <see cref="RealtimeConversationSession"/>, optionally applying initial configuration to the session and
    /// default conversation for the session.
    /// </summary>
    /// <remarks>
    /// The <see cref="RealtimeConversationSession"/> abstracts bidirectional communication between the caller and service,
    /// simultaneously sending and receiving WebSocket messages.
    /// </remarks>
    /// <param name="cancellationToken"></param>
    /// <returns> A new, connected, configured instance of <see cref="RealtimeConversationSession"/>. </returns>
    public virtual async Task<RealtimeConversationSession> StartConversationSessionAsync(
        CancellationToken cancellationToken = default)
    {
        RequestOptions cancellationOptions = cancellationToken.ToRequestOptions();
        RealtimeConversationSession newOperation = await StartConversationSessionAsync(cancellationOptions).ConfigureAwait(false);
        return newOperation;
    }

    public RealtimeConversationSession StartConversationSession(CancellationToken cancellationToken = default)
    {
        return StartConversationSessionAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private static Uri GetEndpoint(string model, OpenAIClientOptions options)
    {
        UriBuilder uriBuilder = new(options?.Endpoint ?? new("https://api.openai.com/v1"));
        uriBuilder.Scheme = uriBuilder.Scheme.ToLowerInvariant() switch
        {
            "http" => "ws",
            "https" => "wss",
            _ => uriBuilder.Scheme
        };
        uriBuilder.Query = "";
        if (!uriBuilder.Path.EndsWith("/realtime"))
        {
            uriBuilder.Path += uriBuilder.Path[uriBuilder.Path.Length - 1] == '/' ? "realtime" : "/realtime";
        }

        uriBuilder.Query = $"?model={model}";

        return uriBuilder.Uri;
    }

    internal void RaiseOnSendingCommand(RealtimeConversationSession session, BinaryData data)
    {
        OnSendingCommand?.Invoke(session, data);
    }

    internal void RaiseOnReceivingCommand(RealtimeConversationSession session, BinaryData data)
    {
        OnReceivingCommand?.Invoke(session, data);
    }
}