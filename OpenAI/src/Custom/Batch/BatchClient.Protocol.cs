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
    public virtual async Task<CreateBatchOperation> CreateBatchAsync(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateBatchRequest(content, options);

        PipelineResponse response = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string batchId = doc.RootElement.GetProperty("id"u8).GetString();
        string status = doc.RootElement.GetProperty("status"u8).GetString();

        CreateBatchOperation operation = this.CreateCreateBatchOperation(batchId, status, response);
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
    public virtual CreateBatchOperation CreateBatch(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateBatchRequest(content, options);
        PipelineResponse response = Pipeline.ProcessMessage(message, options);

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string batchId = doc.RootElement.GetProperty("id"u8).GetString();
        string status = doc.RootElement.GetProperty("status"u8).GetString();

        CreateBatchOperation operation = this.CreateCreateBatchOperation(batchId, status, response);
        return operation.WaitUntil(waitUntilCompleted, options);
    }
}
