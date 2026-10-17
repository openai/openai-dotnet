using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Decisions;

/// <summary> The service client for OpenAI decision operations. </summary>
[CodeGenType("Decisions")]
[CodeGenSuppress("DecisionsClient", typeof(ClientPipeline), typeof(Uri))]
public partial class DecisionsClient
{
    /// <summary> Initializes a new instance of <see cref="DecisionsClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public DecisionsClient(string apiKey) : this(new ApiKeyCredential(apiKey), new DecisionsClientOptions())
    {
    }

    /// <summary> Initializes a new instance of <see cref="DecisionsClient"/>. </summary>
    /// <param name="credential"> The credential to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public DecisionsClient(ApiKeyCredential credential) : this(credential, new DecisionsClientOptions())
    {
    }

    /// <summary> Initializes a new instance of <see cref="DecisionsClient"/>. </summary>
    /// <param name="credential"> The credential to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public DecisionsClient(ApiKeyCredential credential, DecisionsClientOptions options)
        : this(OpenAIClientUtilities.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    /// <summary> Initializes a new instance of <see cref="DecisionsClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public DecisionsClient(AuthenticationPolicy authenticationPolicy) : this(authenticationPolicy, new DecisionsClientOptions())
    {
    }

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

    [Experimental("SCME0002")]
    public DecisionsClient(DecisionsClientSettings settings) : this(AuthenticationPolicy.Create(settings), settings?.Options)
    {
    }

    /// <summary> Gets the endpoint URI for the service. </summary>
    public virtual Uri Endpoint => _endpoint;
}
