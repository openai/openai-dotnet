using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Decisions;

// CUSTOM: Renamed the client and replaced the endpoint constructor with options-based pipeline configuration.
/// <summary> The service client for OpenAI decision operations. </summary>
[CodeGenType("Decisions")]
[CodeGenSuppress("DecisionsClient", typeof(ClientPipeline), typeof(Uri))]
public partial class DecisionsClient
{
    // CUSTOM: Added an API key string constructor for convenience.
    /// <summary> Initializes a new instance of <see cref="DecisionsClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public DecisionsClient(string apiKey) : this(new ApiKeyCredential(apiKey), new DecisionsClientOptions())
    {
    }

    // CUSTOM: Added a credential constructor that uses default client options.
    /// <summary> Initializes a new instance of <see cref="DecisionsClient"/>. </summary>
    /// <param name="credential"> The credential to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public DecisionsClient(ApiKeyCredential credential) : this(credential, new DecisionsClientOptions())
    {
    }

    // CUSTOM: Use the shared API key authentication policy and configure the endpoint through client options.
    /// <summary> Initializes a new instance of <see cref="DecisionsClient"/>. </summary>
    /// <param name="credential"> The credential to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public DecisionsClient(ApiKeyCredential credential, DecisionsClientOptions options)
        : this(OpenAIClientUtilities.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    // CUSTOM: Added an authentication policy constructor that uses default client options.
    /// <summary> Initializes a new instance of <see cref="DecisionsClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public DecisionsClient(AuthenticationPolicy authenticationPolicy) : this(authenticationPolicy, new DecisionsClientOptions())
    {
    }

    // CUSTOM: Build the standard OpenAI pipeline with configurable authentication, headers, and endpoint.
    /// <summary> Initializes a new instance of <see cref="DecisionsClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public DecisionsClient(AuthenticationPolicy authenticationPolicy, DecisionsClientOptions options)
    {
        Argument.AssertNotNull(authenticationPolicy, nameof(authenticationPolicy));
        options ??= new DecisionsClientOptions();

        Pipeline = OpenAIClientUtilities.CreatePipeline(authenticationPolicy, options, options.UserAgentApplicationId, options.OrganizationId, options.ProjectId);
        _endpoint = OpenAIClientUtilities.GetEndpoint(options.Endpoint);
    }

    // CUSTOM: Allow derived clients and the top-level client factory to reuse an existing pipeline.
    /// <summary> Initializes a new instance of <see cref="DecisionsClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal DecisionsClient(ClientPipeline pipeline, DecisionsClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new DecisionsClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClientUtilities.GetEndpoint(options.Endpoint);
    }

    // CUSTOM: Added settings-based construction for configuration binding and dependency injection.
    [Experimental("SCME0002")]
    public DecisionsClient(DecisionsClientSettings settings) : this(AuthenticationPolicy.Create(settings), settings?.Options)
    {
    }

    // CUSTOM: Expose the configured service endpoint.
    /// <summary> Gets the endpoint URI for the service. </summary>
    public virtual Uri Endpoint => _endpoint;
}
