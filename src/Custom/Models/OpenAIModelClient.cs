using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Models;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Renamed convenience methods.
/// <summary> The service client for OpenAI model operations. </summary>
[CodeGenClient("ModelsOps")]
[CodeGenSuppress("OpenAIModelClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("GetModelsAsync")]
[CodeGenSuppress("GetModels")]
[CodeGenSuppress("RetrieveModelAsync", typeof(string))]
[CodeGenSuppress("RetrieveModel", typeof(string))]
[CodeGenSuppress("DeleteModelAsync", typeof(string))]
[CodeGenSuppress("DeleteModel", typeof(string))]

public partial class OpenAIModelClient
{
    // CUSTOM: Remove virtual keyword.
    /// <summary>
    /// The HTTP pipeline for sending and receiving REST requests and responses.
    /// </summary>
    public ClientPipeline Pipeline => _pipeline;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="OpenAIModelClient">. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public OpenAIModelClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="OpenAIModelClient">. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public OpenAIModelClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="OpenAIModelClient">. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public OpenAIModelClient(ApiKeyCredential credential, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(credential, nameof(credential));
        options ??= new OpenAIClientOptions();

        _pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="OpenAIModelClient">. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal OpenAIModelClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        _pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    /// <summary> Gets basic information about each of the models that are currently available, such as their corresponding owner and availability. </summary>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    public virtual async Task<ClientResult<OpenAIModelCollection>> GetModelsAsync(CancellationToken cancellationToken = default)
    {
        ClientResult result = await GetModelsAsync(cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(OpenAIModelCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Gets basic information about each of the models that are currently available, such as their corresponding owner and availability. </summary>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    public virtual ClientResult<OpenAIModelCollection> GetModels(CancellationToken cancellationToken = default)
    {
        ClientResult result = GetModels(cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(OpenAIModelCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
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
        return ClientResult.FromValue(OpenAIModel.FromResponse(result.GetRawResponse()), result.GetRawResponse());
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
        return ClientResult.FromValue(OpenAIModel.FromResponse(result.GetRawResponse()), result.GetRawResponse());
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
        return ClientResult.FromValue(ModelDeletionResult.FromResponse(result.GetRawResponse()), result.GetRawResponse());
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
        return ClientResult.FromValue(ModelDeletionResult.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }
}
