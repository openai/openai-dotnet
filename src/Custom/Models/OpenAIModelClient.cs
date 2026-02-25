using Microsoft.TypeSpec.Generator.Customizations;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Models;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Renamed convenience methods.
/// <summary> The service client for OpenAI model operations. </summary>
[CodeGenType("Models")]
[CodeGenSuppress("OpenAIModelClient", typeof(ClientPipeline), typeof(Uri))]
[CodeGenSuppress("GetModelsAsync", typeof(CancellationToken))]
[CodeGenSuppress("GetModels", typeof(CancellationToken))]
[CodeGenSuppress("GetModelAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetModel", typeof(string), typeof(CancellationToken))]

public partial class OpenAIModelClient
{
    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="OpenAIModelClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public OpenAIModelClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="OpenAIModelClient"/>. </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public OpenAIModelClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="OpenAIModelClient"/>. </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public OpenAIModelClient(ApiKeyCredential credential, OpenAIClientOptions options) : this(OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="OpenAIModelClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    [Experimental("OPENAI001")]
    public OpenAIModelClient(AuthenticationPolicy authenticationPolicy) : this(authenticationPolicy, new OpenAIClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="OpenAIModelClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    [Experimental("OPENAI001")]
    public OpenAIModelClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options)
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
    /// <summary> Initializes a new instance of <see cref="OpenAIModelClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal OpenAIModelClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    [Experimental("SCME0002")]
    public OpenAIModelClient(OpenAIModelClientSettings settings)
        : this(AuthenticationPolicy.Create(settings), settings?.Options)
    {
    }

    /// <summary>
    /// Gets the endpoint URI for the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public Uri Endpoint => _endpoint;

    /// <summary> Gets basic information about each of the models that are currently available, such as their corresponding owner and availability. </summary>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    public virtual async Task<ClientResult<OpenAIModelCollection>> GetModelsAsync(CancellationToken cancellationToken = default)
    {
        ClientResult result = await GetModelsAsync(cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue((OpenAIModelCollection)result, result.GetRawResponse());
    }

    /// <summary> Gets basic information about each of the models that are currently available, such as their corresponding owner and availability. </summary>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    public virtual ClientResult<OpenAIModelCollection> GetModels(CancellationToken cancellationToken = default)
    {
        ClientResult result = GetModels(cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((OpenAIModelCollection)result, result.GetRawResponse());
    }

    /// <summary> Gets basic information about the specified model, such as its owner and availability. </summary>
    /// <param name="model"> The name of the desired model. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<OpenAIModel>> GetModelAsync(string model, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        ClientResult result = await GetModelAsync(model, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue((OpenAIModel)result, result.GetRawResponse());
    }

    /// <summary> Gets basic information about the specified model, such as its owner and availability. </summary>
    /// <param name="model"> The name of the desired model. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<OpenAIModel> GetModel(string model, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        ClientResult result = GetModel(model, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((OpenAIModel)result, result.GetRawResponse());
    }

    /// <summary> Deletes the specified fine-tuned model. </summary>
    /// <remarks> You must have the role of "owner" within your organization in order to be able to delete a model. </remarks>
    /// <param name="model"> The name of the model to delete. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<ModelDeletionResult>> DeleteModelAsync(string model, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        ClientResult result = await DeleteModelAsync(model, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue((ModelDeletionResult)result, result.GetRawResponse());
    }

    /// <summary> Deletes the specified fine-tuned model. </summary>
    /// <remarks> You must have the role of "owner" within your organization in order to be able to delete a model. </remarks>
    /// <param name="model"> The name of the model to delete. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<ModelDeletionResult> DeleteModel(string model, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        ClientResult result = DeleteModel(model, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue((ModelDeletionResult)result, result.GetRawResponse());
    }
}
