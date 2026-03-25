using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[CodeGenType("Realtime")]
public partial class RealtimeClient
{
    private const string DefaultEndpoint = "https://api.openai.com/v1";

    private static class KnownHeaderNames
    {
        public const string OpenAIOrganization = "OpenAI-Organization";
        public const string OpenAIProject = "OpenAI-Project";
        public const string UserAgent = "User-Agent";
    }

    public event EventHandler<BinaryData> OnSendingCommand;
    public event EventHandler<BinaryData> OnReceivingCommand;

    private readonly ApiKeyCredential _keyCredential;
    private readonly Uri _webSocketEndpoint;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="RealtimeClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public RealtimeClient(string apiKey) : this(new ApiKeyCredential(apiKey), new RealtimeClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="RealtimeClient"/>. </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public RealtimeClient(ApiKeyCredential credential) : this(credential, new RealtimeClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="RealtimeClient"/>. </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public RealtimeClient(ApiKeyCredential credential, RealtimeClientOptions options) : this(OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
        _keyCredential = credential;
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="RealtimeClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    [Experimental("OPENAI001")]
    public RealtimeClient(AuthenticationPolicy authenticationPolicy) : this(authenticationPolicy, new RealtimeClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="RealtimeClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    [Experimental("OPENAI001")]
    public RealtimeClient(AuthenticationPolicy authenticationPolicy, RealtimeClientOptions options)
    {
        Argument.AssertNotNull(authenticationPolicy, nameof(authenticationPolicy));
        options ??= new RealtimeClientOptions();

        Pipeline = CreatePipeline(authenticationPolicy, options);
        _endpoint = GetEndpoint(options);
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
    protected internal RealtimeClient(ClientPipeline pipeline, RealtimeClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new RealtimeClientOptions();

        Pipeline = pipeline;
        _endpoint = GetEndpoint(options);
        _webSocketEndpoint = GetWebSocketEndpoint(options);
    }

    [Experimental("SCME0002")]
    public RealtimeClient(RealtimeClientSettings settings)
        : this(AuthenticationPolicy.Create(settings), RealtimeClientOptions.FromClientOptions(settings?.Options))
    {
    }

    /// <summary>
    /// Gets the endpoint URI for the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public Uri Endpoint => _endpoint;

    private static Uri GetEndpoint(RealtimeClientOptions options = null)
    {
        return options?.Endpoint ?? new(DefaultEndpoint);
    }

    private static Uri GetWebSocketEndpoint(RealtimeClientOptions options = null)
    {
        UriBuilder uriBuilder = new(options?.Endpoint ?? new(DefaultEndpoint));
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

    private static ClientPipeline CreatePipeline(AuthenticationPolicy authenticationPolicy, RealtimeClientOptions options)
    {
        return ClientPipeline.Create(
            options: options,
            perCallPolicies: [CreateAddCustomHeadersPolicy(options)],
            perTryPolicies: [authenticationPolicy],
            beforeTransportPolicies: []);
    }

    private static PipelinePolicy CreateAddCustomHeadersPolicy(RealtimeClientOptions options = null)
    {
        TelemetryDetails telemetryDetails = new(typeof(RealtimeClientOptions).Assembly, options?.UserAgentApplicationId);
        return new GenericActionPipelinePolicy((message) =>
        {
            if (message?.Request?.Headers?.TryGetValue(KnownHeaderNames.UserAgent, out string _) == false)
            {
                message.Request.Headers.Set(KnownHeaderNames.UserAgent, telemetryDetails.ToString());
            }

            if (!string.IsNullOrEmpty(options?.OrganizationId))
            {
                message.Request.Headers.Set(KnownHeaderNames.OpenAIOrganization, options.OrganizationId);
            }

            if (!string.IsNullOrEmpty(options?.ProjectId))
            {
                message.Request.Headers.Set(KnownHeaderNames.OpenAIProject, options.ProjectId);
            }
        });
    }

    internal void RaiseOnSendingCommand<T>(T session, BinaryData data)
        => OnSendingCommand?.Invoke(session, data);

    internal void RaiseOnReceivingCommand<T>(T session, BinaryData data)
        => OnReceivingCommand?.Invoke(session, data);
}