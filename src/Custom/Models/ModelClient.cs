using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

namespace OpenAI.Models;

/// <summary>
/// The service client for OpenAI model operations.
/// </summary>
[CodeGenClient("ModelsOps")]
[CodeGenSuppress("ModelClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("GetModelsAsync")]
[CodeGenSuppress("GetModels")]
[CodeGenSuppress("RetrieveAsync", typeof(string))]
[CodeGenSuppress("Retrieve", typeof(string))]
[CodeGenSuppress("DeleteAsync", typeof(string))]
[CodeGenSuppress("Delete", typeof(string))]
public partial class ModelClient
{
    /// <summary>
    /// Initializes a new instance of <see cref="ModelClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public ModelClient(ApiKeyCredential credential, OpenAIClientOptions options = null)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(credential, requireExplicitCredential: true), options),
              OpenAIClient.GetEndpoint(options),
              options)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ModelClient"/> that will use an API key from the OPENAI_API_KEY
    /// environment variable when authenticating.
    /// </summary>
    /// <remarks>
    /// To provide an explicit credential instead of using the environment variable, use an alternate constructor like
    /// <see cref="ModelClient(ApiKeyCredential,OpenAIClientOptions)"/>.
    /// </remarks>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="InvalidOperationException"> The OPENAI_API_KEY environment variable was not found. </exception>
    public ModelClient(OpenAIClientOptions options = null)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(), options),
              OpenAIClient.GetEndpoint(options),
              options)
    {
    }

    /// <summary> Initializes a new instance of <see cref="ModelClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline for sending and receiving REST requests and responses. </param>
    /// <param name="endpoint"> OpenAI Endpoint. </param>
    protected internal ModelClient(ClientPipeline pipeline, Uri endpoint, OpenAIClientOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
    }

    /// <summary>
    /// Lists the currently available models, and provides basic information about each one such as the
    /// owner and availability.
    /// </summary>
    /// <remarks> List models. </remarks>
    public virtual async Task<ClientResult<OpenAIModelInfoCollection>> GetModelsAsync()
    {
        ClientResult result = await GetModelsAsync(null).ConfigureAwait(false);
        return ClientResult.FromValue(OpenAIModelInfoCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary>
    /// Lists the currently available models, and provides basic information about each one such as the
    /// owner and availability.
    /// </summary>
    /// <remarks> List models. </remarks>
    public virtual ClientResult<OpenAIModelInfoCollection> GetModels()
    {
        ClientResult result = GetModels(null);
        return ClientResult.FromValue(OpenAIModelInfoCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary>
    /// Retrieves a model instance, providing basic information about the model such as the owner and
    /// permissioning.
    /// </summary>
    /// <param name="model"> The ID of the model to use for this request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    /// <remarks> Retrieve. </remarks>
    public virtual async Task<ClientResult<OpenAIModelInfo>> GetModelAsync(string model)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        ClientResult result = await GetModelAsync(model, (RequestOptions)null).ConfigureAwait(false);
        return ClientResult.FromValue(OpenAIModelInfo.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary>
    /// Retrieves a model instance, providing basic information about the model such as the owner and
    /// permissioning.
    /// </summary>
    /// <param name="model"> The ID of the model to use for this request. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    /// <remarks> Retrieve. </remarks>
    public virtual ClientResult<OpenAIModelInfo> GetModel(string model)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        ClientResult result = GetModel(model, (RequestOptions)null);
        return ClientResult.FromValue(OpenAIModelInfo.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Delete a fine-tuned model. You must have the Owner role in your organization to delete a model. </summary>
    /// <param name="model"> The model to delete. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    /// <remarks> A value indicating whether the deletion operation was successful. </remarks>
    public virtual async Task<ClientResult<bool>> DeleteModelAsync(string model)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        ClientResult result = await DeleteModelAsync(model, null).ConfigureAwait(false);
        PipelineResponse response = result?.GetRawResponse();
        InternalDeleteModelResponse value = InternalDeleteModelResponse.FromResponse(response);
        return ClientResult.FromValue(value.Deleted, response);
    }

    /// <summary> Delete a fine-tuned model. You must have the Owner role in your organization to delete a model. </summary>
    /// <param name="model"> The model to delete. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    /// <remarks> A value indicating whether the deletion operation was successful. </remarks>
    public virtual ClientResult<bool> DeleteModel(string model)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        ClientResult result = DeleteModel(model, null);
        PipelineResponse response = result?.GetRawResponse();
        InternalDeleteModelResponse value = InternalDeleteModelResponse.FromResponse(response);
        return ClientResult.FromValue(value.Deleted, response);
    }
}
