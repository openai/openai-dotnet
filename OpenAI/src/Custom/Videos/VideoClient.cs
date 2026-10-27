using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Videos;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
/// <summary> The service client for OpenAI video operations. </summary>
[CodeGenType("Videos")]
[CodeGenSuppress("VideoClient", typeof(ClientPipeline), typeof(Uri))]
public partial class VideoClient
{
    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="VideoClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public VideoClient(string apiKey) : this(new ApiKeyCredential(apiKey), new VideoClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="VideoClient"/>. </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public VideoClient(ApiKeyCredential credential) : this(credential, new VideoClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="VideoClient"/>. </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public VideoClient(ApiKeyCredential credential, VideoClientOptions options) : this(OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="VideoClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public VideoClient(AuthenticationPolicy authenticationPolicy) : this(authenticationPolicy, new VideoClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="VideoClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public VideoClient(AuthenticationPolicy authenticationPolicy, VideoClientOptions options)
    {
        Argument.AssertNotNull(authenticationPolicy, nameof(authenticationPolicy));
        options ??= new VideoClientOptions();

        Pipeline = OpenAIClientUtilities.CreatePipeline(authenticationPolicy, options, options.UserAgentApplicationId, options.OrganizationId, options.ProjectId);
        _endpoint = OpenAIClientUtilities.GetEndpoint(options.Endpoint);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="VideoClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal VideoClient(ClientPipeline pipeline, VideoClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new VideoClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClientUtilities.GetEndpoint(options.Endpoint);
    }

    [Experimental("SCME0002")]
    public VideoClient(VideoClientSettings settings)
        : this(AuthenticationPolicy.Create(settings), settings?.Options)
    {
    }

    /// <summary>
    /// Gets the endpoint URI for the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public Uri Endpoint => _endpoint;
}

