using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Realtime;

[CodeGenType("Realtime")]
[CodeGenSuppress("CreateEphemeralToken", typeof(InternalRealtimeSessionCreateRequest), typeof(CancellationToken))]
[CodeGenSuppress("CreateEphemeralTokenAsync", typeof(InternalRealtimeSessionCreateRequest), typeof(CancellationToken))]
[CodeGenSuppress("CreateEphemeralTranscriptionToken", typeof(TranscriptionSessionOptions), typeof(CancellationToken))]
[CodeGenSuppress("CreateEphemeralTranscriptionTokenAsync", typeof(TranscriptionSessionOptions), typeof(CancellationToken))]
[CodeGenSuppress("StartRealtimeSession", typeof(IEnumerable<InternalRealtimeClientEvent>), typeof(CancellationToken))]
[CodeGenSuppress("StartRealtimeSessionAsync", typeof(IEnumerable<InternalRealtimeClientEvent>), typeof(CancellationToken))]
public partial class RealtimeClient
{
    public event EventHandler<BinaryData> OnSendingCommand;
    public event EventHandler<BinaryData> OnReceivingCommand;

    private readonly ApiKeyCredential _keyCredential;
    private readonly Uri _webSocketEndpoint;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="RealtimeClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public RealtimeClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="RealtimeClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public RealtimeClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="RealtimeClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public RealtimeClient(ApiKeyCredential credential, OpenAIClientOptions options) : this(OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
        _keyCredential = credential;
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="RealtimeClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    [Experimental("OPENAI001")]
    public RealtimeClient(AuthenticationPolicy authenticationPolicy) : this(authenticationPolicy, new OpenAIClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="RealtimeClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    [Experimental("OPENAI001")]
    public RealtimeClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(authenticationPolicy, nameof(authenticationPolicy));
        options ??= new OpenAIClientOptions();

        Pipeline = OpenAIClient.CreatePipeline(authenticationPolicy, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
        _webSocketEndpoint = GetWebSocketEndpoint(options);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="RealtimeClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal RealtimeClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
        _webSocketEndpoint = GetWebSocketEndpoint(options);
    }

    /// <summary>
    /// Starts a new <see cref="RealtimeSession"/> for multimodal conversation.
    /// </summary>
    /// <remarks>
    /// The <see cref="RealtimeSession"/> abstracts bidirectional communication between the caller and service,
    /// simultaneously sending and receiving WebSocket messages.
    /// </remarks>
    /// <param name="model">
    /// The model that the session should use for new conversation items.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns> A new, connected instance of <see cref="RealtimeSession"/> with default configuration. </returns>
    public virtual async Task<RealtimeSession> StartConversationSessionAsync(
        string model,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(model, nameof(model));

        RequestOptions cancellationOptions = cancellationToken.ToRequestOptions();
        RealtimeSession newOperation = await StartConversationSessionAsync(model, cancellationOptions).ConfigureAwait(false);
        return newOperation;
    }

    /// <summary>
    /// Starts a new <see cref="RealtimeSession"/> for multimodal conversation.
    /// </summary>
    /// <remarks>
    /// The <see cref="RealtimeSession"/> abstracts bidirectional communication between the caller and service,
    /// simultaneously sending and receiving WebSocket messages.
    /// </remarks>
    /// <param name="model">
    /// The model that the session should use for new conversation items.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns> A new, connected instance of <see cref="RealtimeSession"/> with default configuration. </returns>
    public RealtimeSession StartConversationSession(string model, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(model, nameof(model));

        return StartConversationSessionAsync(model, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Starts a new <see cref="RealtimeSession"/> for audio transcription.
    /// </summary>
    /// <remarks>
    /// The <see cref="RealtimeSession"/> abstracts bidirectional communication between the caller and service,
    /// simultaneously sending and receiving WebSocket messages.
    /// </remarks>
    /// <param name="cancellationToken"></param>
    /// <returns> A new, connected instance of <see cref="RealtimeSession"/> with default configuration. </returns>
    public virtual async Task<RealtimeSession> StartTranscriptionSessionAsync(
        CancellationToken cancellationToken = default)
    {
        RequestOptions cancellationOptions = cancellationToken.ToRequestOptions();
        RealtimeSession newOperation = await StartTranscriptionSessionAsync(cancellationOptions).ConfigureAwait(false);
        return newOperation;
    }

    /// <summary>
    /// Starts a new <see cref="RealtimeSession"/> for audio transcription.
    /// </summary>
    /// <remarks>
    /// The <see cref="RealtimeSession"/> abstracts bidirectional communication between the caller and service,
    /// simultaneously sending and receiving WebSocket messages.
    /// </remarks>
    /// <param name="cancellationToken"></param>
    /// <returns> A new, connected instance of <see cref="RealtimeSession"/> with default configuration. </returns>
    public RealtimeSession StartTranscriptionSession(CancellationToken cancellationToken = default)
    {
        return StartTranscriptionSessionAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private static Uri GetWebSocketEndpoint(OpenAIClientOptions options)
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

        return uriBuilder.Uri;
    }

    internal void RaiseOnSendingCommand<T>(T session, BinaryData data)
        => OnSendingCommand?.Invoke(session, data);

    internal void RaiseOnReceivingCommand<T>(T session, BinaryData data)
        => OnReceivingCommand?.Invoke(session, data);
}