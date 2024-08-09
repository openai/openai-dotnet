using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Batch;

public partial class BatchClient
{
    /// <summary>
    /// [Protocol Method] Creates and executes a batch from an uploaded file of requests
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="CreateBatchOperation"/> that can be used to wait for 
    /// the operation to complete, or cancel the operation. </returns>
    public virtual async Task<CreateBatchOperation> CreateBatchAsync(bool waitUntilCompleted, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateBatchRequest(content, options);

        PipelineResponse response = await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string batchId = doc.RootElement.GetProperty("id"u8).GetString();
        string status = doc.RootElement.GetProperty("status"u8).GetString();

        CreateBatchOperation operation = new(_pipeline, _endpoint, batchId, status, response);
        return await operation.WaitUntilAsync(waitUntilCompleted, options).ConfigureAwait(false);
    }

    /// <summary>
    /// [Protocol Method] Creates and executes a batch from an uploaded file of requests
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="CreateBatchOperation"/> that can be used to wait for 
    /// the operation to complete, or cancel the operation. </returns>
    public virtual CreateBatchOperation CreateBatch(bool waitUntilCompleted, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateBatchRequest(content, options);
        PipelineResponse response = _pipeline.ProcessMessage(message, options);

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string batchId = doc.RootElement.GetProperty("id"u8).GetString();
        string status = doc.RootElement.GetProperty("status"u8).GetString();

        CreateBatchOperation operation = new(_pipeline, _endpoint, batchId, status, response);
        return operation.WaitUntil(waitUntilCompleted, options);
    }

    /// <summary>
    /// [Protocol Method] List your organization's batches.
    /// </summary>
    /// <param name="after"> A cursor for use in pagination. `after` is an object ID that defines your place in the list. For instance, if you make a list request and receive 100 objects, ending with obj_foo, your subsequent call can include after=obj_foo in order to fetch the next page of the list. </param>
    /// <param name="limit"> A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetBatchesAsync(string after, int? limit, RequestOptions options)
    {
        using PipelineMessage message = CreateGetBatchesRequest(after, limit, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] List your organization's batches.
    /// </summary>
    /// <param name="after"> A cursor for use in pagination. `after` is an object ID that defines your place in the list. For instance, if you make a list request and receive 100 objects, ending with obj_foo, your subsequent call can include after=obj_foo in order to fetch the next page of the list. </param>
    /// <param name="limit"> A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetBatches(string after, int? limit, RequestOptions options)
    {
        using PipelineMessage message = CreateGetBatchesRequest(after, limit, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a batch.
    /// </summary>
    /// <param name="batchId"> The ID of the batch to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    internal virtual async Task<ClientResult> GetBatchAsync(string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateRetrieveBatchRequest(batchId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a batch.
    /// </summary>
    /// <param name="batchId"> The ID of the batch to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    internal virtual ClientResult GetBatch(string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateRetrieveBatchRequest(batchId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }
}
