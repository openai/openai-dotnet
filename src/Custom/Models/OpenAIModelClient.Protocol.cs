using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OpenAI.Models;

[CodeGenSuppress("ListModelsAsync", typeof(RequestOptions))]
[CodeGenSuppress("ListModels", typeof(RequestOptions))]
[CodeGenSuppress("RetrieveModelAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("RetrieveModel", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("DeleteModelAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("DeleteModel", typeof(string), typeof(RequestOptions))]
public partial class OpenAIModelClient
{
    /// <summary>
    /// [Protocol Method] Lists the currently available models, and provides basic information about each one such as the
    /// owner and availability.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetModelsAsync(RequestOptions options)
    {
        using PipelineMessage message = CreateGetModelsRequest(options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Lists the currently available models, and provides basic information about each one such as the
    /// owner and availability.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetModels(RequestOptions options)
    {
        using PipelineMessage message = CreateGetModelsRequest(options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a model instance, providing basic information about the model such as the owner and
    /// permissioning.
    /// </summary>
    /// <param name="model"> The ID of the model to use for this request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetModelAsync(string model, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        using PipelineMessage message = CreateRetrieveModelRequest(model, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a model instance, providing basic information about the model such as the owner and
    /// permissioning.
    /// </summary>
    /// <param name="model"> The ID of the model to use for this request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetModel(string model, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        using PipelineMessage message = CreateRetrieveModelRequest(model, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Delete a fine-tuned model. You must have the Owner role in your organization to delete a model.
    /// </summary>
    /// <param name="model"> The model to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> DeleteModelAsync(string model, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        using PipelineMessage message = CreateDeleteModelRequest(model, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Delete a fine-tuned model. You must have the Owner role in your organization to delete a model.
    /// </summary>
    /// <param name="model"> The model to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="model"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult DeleteModel(string model, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));

        using PipelineMessage message = CreateDeleteModelRequest(model, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }
}
