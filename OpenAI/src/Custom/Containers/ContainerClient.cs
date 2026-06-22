using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Containers;

[CodeGenType("Containers")]
[CodeGenSuppress(nameof(ContainerClient), typeof(ClientPipeline), typeof(Uri))]
[CodeGenSuppress(nameof(GetContainers), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress(nameof(GetContainers), typeof(int?), typeof(ContainerCollectionOrder?), typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress(nameof(GetContainersAsync), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress(nameof(GetContainersAsync), typeof(int?), typeof(ContainerCollectionOrder?), typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress(nameof(GetContainerFiles), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress(nameof(GetContainerFiles), typeof(string), typeof(int?), typeof(ContainerFileCollectionOrder?), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress(nameof(GetContainerFilesAsync), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress(nameof(GetContainerFilesAsync), typeof(string), typeof(int?), typeof(ContainerFileCollectionOrder?), typeof(string), typeof(CancellationToken))]
public partial class ContainerClient
{
    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ContainerClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public ContainerClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ContainerClient"/>. </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public ContainerClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ContainerClient"/>. </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public ContainerClient(ApiKeyCredential credential, OpenAIClientOptions options) : this(OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ContainerClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public ContainerClient(AuthenticationPolicy authenticationPolicy) : this(authenticationPolicy, new OpenAIClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ContainerClient"/>. </summary>
    /// <param name="settings"> The settings to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="settings"/> is null. </exception>
    [Experimental("SCME0002")]
    public ContainerClient(ContainerClientSettings settings) : this(AuthenticationPolicy.Create(settings), settings?.Options)
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="ContainerClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public ContainerClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(authenticationPolicy, nameof(authenticationPolicy));
        options ??= new OpenAIClientOptions();

        Pipeline = OpenAIClient.CreatePipeline(authenticationPolicy, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="ContainerClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal ContainerClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    /// <summary>
    /// Gets the endpoint URI for the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public Uri Endpoint => _endpoint;

    #region GetContainers

    // CUSTOM: Added protocol method that returns ClientResult.
    public virtual ClientResult GetContainerCollectionPage(int? limit, string order, string after, string name, RequestOptions options)
    {
        using PipelineMessage message = CreateGetContainersRequest(limit, order, after, name, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    // CUSTOM: Added protocol method that returns ClientResult.
    public virtual async Task<ClientResult> GetContainerCollectionPageAsync(int? limit, string order, string after, string name, RequestOptions options)
    {
        using PipelineMessage message = CreateGetContainersRequest(limit, order, after, name, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    // CUSTOM: Added protocol model method.
    public virtual ClientResult<ContainerCollectionPage> GetContainerCollectionPage(ContainerCollectionOptions options = default, CancellationToken cancellationToken = default)
    {
        ClientResult result = GetContainerCollectionPage(
            limit: options?.PageSizeLimit,
            order: options?.Order?.ToString(),
            after: options?.AfterId,
            name: options?.Name,
            options: cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((ContainerCollectionPage)result, result.GetRawResponse());
    }

    // CUSTOM: Added protocol model method.
    public virtual async Task<ClientResult<ContainerCollectionPage>> GetContainerCollectionPageAsync(ContainerCollectionOptions options = default, CancellationToken cancellationToken = default)
    {
        ClientResult result = await GetContainerCollectionPageAsync(
            limit: options?.PageSizeLimit,
            order: options?.Order?.ToString(),
            after: options?.AfterId,
            name: options?.Name,
            options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue((ContainerCollectionPage)result, result.GetRawResponse());
    }

    // CUSTOM: Added convenience method with pagination.
    public virtual CollectionResult<ContainerResource> GetContainers(ContainerCollectionOptions options = default, CancellationToken cancellationToken = default)
    {
        return new ContainerClientGetContainersCollectionResultOfT(
            client: this,
            limit: options?.PageSizeLimit,
            order: options?.Order?.ToString(),
            after: options?.AfterId,
            name: options?.Name,
            options: cancellationToken.ToRequestOptions());
    }

    // CUSTOM: Added convenience method with pagination.
    public virtual AsyncCollectionResult<ContainerResource> GetContainersAsync(ContainerCollectionOptions options = default, CancellationToken cancellationToken = default)
    {
        return new ContainerClientGetContainersAsyncCollectionResultOfT(
            client: this,
            limit: options?.PageSizeLimit,
            order: options?.Order?.ToString(),
            after: options?.AfterId,
            name: options?.Name,
            options: cancellationToken.ToRequestOptions());
    }

    #endregion

    #region GetContainerFiles

    // CUSTOM: Added protocol method that returns ClientResult.
    public virtual ClientResult GetContainerFileCollectionPage(string containerId, int? limit, string order, string after, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(containerId, nameof(containerId));

        using PipelineMessage message = CreateGetContainerFilesRequest(containerId, limit, order, after, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    // CUSTOM: Added protocol method that returns ClientResult.
    public virtual async Task<ClientResult> GetContainerFileCollectionPageAsync(string containerId, int? limit, string order, string after, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(containerId, nameof(containerId));

        using PipelineMessage message = CreateGetContainerFilesRequest(containerId, limit, order, after, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    // CUSTOM: Added protocol model method.
    public virtual ClientResult<ContainerFileCollectionPage> GetContainerFileCollectionPage(ContainerFileCollectionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ContainerId, "options.ContainerId");

        ClientResult result = GetContainerFileCollectionPage(
            containerId: options.ContainerId,
            limit: options.PageSizeLimit,
            order: options.Order?.ToString(),
            after: options.AfterId,
            options: cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((ContainerFileCollectionPage)result, result.GetRawResponse());
    }

    // CUSTOM: Added protocol model method.
    public virtual async Task<ClientResult<ContainerFileCollectionPage>> GetContainerFileCollectionPageAsync(ContainerFileCollectionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ContainerId, "options.ContainerId");

        ClientResult result = await GetContainerFileCollectionPageAsync(
            containerId: options.ContainerId,
            limit: options.PageSizeLimit,
            order: options.Order?.ToString(),
            after: options.AfterId,
            options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue((ContainerFileCollectionPage)result, result.GetRawResponse());
    }

    // CUSTOM: Added convenience method with pagination.
    public virtual CollectionResult<ContainerFileResource> GetContainerFiles(ContainerFileCollectionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ContainerId, "options.ContainerId");

        return new ContainerClientGetContainerFilesCollectionResultOfT(
            this,
            options.ContainerId,
            options.PageSizeLimit,
            options.Order?.ToString(),
            options.AfterId,
            cancellationToken.ToRequestOptions());
    }

    // CUSTOM: Added convenience method with pagination.
    public virtual AsyncCollectionResult<ContainerFileResource> GetContainerFilesAsync(ContainerFileCollectionOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(options, nameof(options));
        Argument.AssertNotNullOrEmpty(options.ContainerId, "options.ContainerId");

        return new ContainerClientGetContainerFilesAsyncCollectionResultOfT(
            this,
            options.ContainerId,
            options.PageSizeLimit,
            options.Order?.ToString(),
            options.AfterId,
            cancellationToken.ToRequestOptions());
    }

    #endregion
}
