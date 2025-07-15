using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.VectorStores;

/// <summary>
/// Long-running operation for creating a vector store file batch.
/// </summary>
public partial class CreateBatchFileJobOperation : OperationResult
{
    private readonly VectorStoreClient _parentClient;
    private readonly Uri _endpoint;

    private readonly string _vectorStoreId;
    private readonly string _batchId;

    /// <inheritdoc/>
    public override ContinuationToken? RehydrationToken { get; protected set; }

    // Generated protocol methods

    /// <summary>
    /// [Protocol Method] Retrieves a vector store file batch.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetFileBatchAsync(RequestOptions? options)
    {
        using PipelineMessage message = _parentClient.CreateGetVectorStoreFileBatchRequest(_vectorStoreId, _batchId, options);
        return ClientResult.FromResponse(await _parentClient.Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a vector store file batch.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetFileBatch(RequestOptions? options)
    {
        using PipelineMessage message = _parentClient.CreateGetVectorStoreFileBatchRequest(_vectorStoreId, _batchId, options);
        return ClientResult.FromResponse(_parentClient.Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Cancel a vector store file batch. This attempts to cancel the processing of files in this batch as soon as possible.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CancelAsync(RequestOptions? options)
    {
        using PipelineMessage message = _parentClient.CreateCancelBatchFileJobRequest(_vectorStoreId, _batchId, options);
        return ClientResult.FromResponse(await _parentClient.Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Cancel a vector store file batch. This attempts to cancel the processing of files in this batch as soon as possible.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult Cancel(RequestOptions? options)
    {
        using PipelineMessage message = _parentClient.CreateCancelBatchFileJobRequest(_vectorStoreId, _batchId, options);
        return ClientResult.FromResponse(_parentClient.Pipeline.ProcessMessage(message, options));
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}