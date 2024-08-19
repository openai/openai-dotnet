using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

namespace OpenAI.Models;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Renamed convenience methods.
/// <summary> The service client for OpenAI model operations. </summary>
[CodeGenClient("ModelsOps")]
[CodeGenSuppress("ModelClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("RetrieveAsync", typeof(string))]
[CodeGenSuppress("Retrieve", typeof(string))]
[CodeGenSuppress("DeleteAsync", typeof(string))]
[CodeGenSuppress("Delete", typeof(string))]
public partial class ModelClient
{
    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ModelClient">. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public ModelClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="ModelClient">. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public ModelClient(ApiKeyCredential credential, OpenAIClientOptions options)
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
    /// <summary> Initializes a new instance of <see cref="ModelClient">. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal ModelClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        _pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    /// <summary> Gets basic information about each of the models that are currently available, such as their corresponding owner and availability. </summary>
    public virtual async Task<ClientResult<OpenAIModelInfoCollection>> GetModelsAsync()
    {
        ClientResult result = await GetModelsAsync(null).ConfigureAwait(false);
        return ClientResult.FromValue(OpenAIModelInfoCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Gets basic information about each of the models that are currently available, such as their corresponding owner and availability. </summary>
    public virtual ClientResult<OpenAIModelInfoCollection> GetModels()
    {
        ClientResult result = GetModels(null);
        return ClientResult.FromValue(OpenAIModelInfoCollection.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Gets basic information about the specified model, such as its owner and availability. </summary>
    /// <param name="model"> The name of the desired model. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<OpenAIModelInfo>> GetModelAsync(string model)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        ClientResult result = await GetModelAsync(model, (RequestOptions)null).ConfigureAwait(false);
        return ClientResult.FromValue(OpenAIModelInfo.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Gets basic information about the specified model, such as its owner and availability. </summary>
    /// <param name="model"> The name of the desired model. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<OpenAIModelInfo> GetModel(string model)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        ClientResult result = GetModel(model, (RequestOptions)null);
        return ClientResult.FromValue(OpenAIModelInfo.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Deletes the specified fine-tuned model. </summary>
    /// <remarks> You must have the role of "owner" within your organization in order to be able to delete a model. </remarks>
    /// <param name="model"> The name of the model to delete. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual async Task<ClientResult<bool>> DeleteModelAsync(string model)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        ClientResult result = await DeleteModelAsync(model, null).ConfigureAwait(false);
        PipelineResponse response = result?.GetRawResponse();
        InternalDeleteModelResponse value = InternalDeleteModelResponse.FromResponse(response);
        return ClientResult.FromValue(value.Deleted, response);
    }

    /// <summary> Deletes the specified fine-tuned model. </summary>
    /// <remarks> You must have the role of "owner" within your organization in order to be able to delete a model. </remarks>
    /// <param name="model"> The name of the model to delete. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    public virtual ClientResult<bool> DeleteModel(string model)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        ClientResult result = DeleteModel(model, null);
        PipelineResponse response = result?.GetRawResponse();
        InternalDeleteModelResponse value = InternalDeleteModelResponse.FromResponse(response);
        return ClientResult.FromValue(value.Deleted, response);
    }
}
