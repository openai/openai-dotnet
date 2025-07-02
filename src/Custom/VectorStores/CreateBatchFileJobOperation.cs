using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.VectorStores;

/// <summary>
/// Long-running operation for creating a vector store file batch.
/// </summary>
[Experimental("OPENAI001")]
public partial class CreateBatchFileJobOperation : OperationResult
{
    internal CreateBatchFileJobOperation(
        VectorStoreClient parentClient,
        Uri endpoint,
        ClientResult<VectorStoreBatchFileJob> result)
        : base(result.GetRawResponse())
    {
        _parentClient = parentClient;
        _endpoint = endpoint;

        Value = result;
        Status = Value.Status;

        _vectorStoreId = Value.VectorStoreId;
        _batchId = Value.BatchId;

        HasCompleted = GetHasCompleted(Value.Status);
        RehydrationToken = new CreateBatchFileJobOperationToken(VectorStoreId, BatchId);
    }

    /// <summary>
    /// The current value of the <see cref="VectorStoreBatchFileJob"/> in progress.
    /// </summary>
    public VectorStoreBatchFileJob? Value { get; private set; }

    /// <summary>
    /// The current status of the <see cref="VectorStoreBatchFileJob"/> in progress.
    /// </summary>
    public VectorStoreBatchFileJobStatus? Status { get; private set; }

    /// <summary>
    /// The ID of the vector store corresponding to this batch file operation.
    /// </summary>
    public string VectorStoreId { get => _vectorStoreId; }

    /// <summary>
    /// The ID of the batch file job represented by this operation.
    /// </summary>
    public string BatchId { get => _batchId; }

    /// <summary>
    /// Recreates a <see cref="CreateBatchFileJobOperation"/> from a rehydration token.
    /// </summary>
    /// <param name="client"> The <see cref="VectorStoreClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="rehydrationToken"> The rehydration token corresponding to 
    /// the operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="client"/> or <paramref name="rehydrationToken"/> is null. </exception>
    public static async Task<CreateBatchFileJobOperation> RehydrateAsync(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(client, nameof(client));
        Argument.AssertNotNull(rehydrationToken, nameof(rehydrationToken));

        CreateBatchFileJobOperationToken token = CreateBatchFileJobOperationToken.FromToken(rehydrationToken);

        ClientResult result = await client.GetBatchFileJobAsync(token.VectorStoreId, token.BatchId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob job = VectorStoreBatchFileJob.FromClientResult(result);

        return client.CreateBatchFileJobOperation(ClientResult.FromValue(job, response));
    }

    /// <summary>
    /// Recreates a <see cref="CreateBatchFileJobOperation"/> from a rehydration token.
    /// </summary>
    /// <param name="client"> The <see cref="VectorStoreClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="rehydrationToken"> The rehydration token corresponding to 
    /// the operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="client"/> or <paramref name="rehydrationToken"/> is null. </exception>
    public static CreateBatchFileJobOperation Rehydrate(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(client, nameof(client));
        Argument.AssertNotNull(rehydrationToken, nameof(rehydrationToken));

        CreateBatchFileJobOperationToken token = CreateBatchFileJobOperationToken.FromToken(rehydrationToken);

        ClientResult result = client.GetBatchFileJob(token.VectorStoreId, token.BatchId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob job = VectorStoreBatchFileJob.FromClientResult(result);

        return client.CreateBatchFileJobOperation(ClientResult.FromValue(job, response));
    }

    /// <inheritdoc/>
    public override async ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options = null)
    {
        ClientResult result = await GetFileBatchAsync(options).ConfigureAwait(false);

        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromClientResult(result);

        ApplyUpdate(response, value);

        return result;
    }

    /// <inheritdoc/>
    public override ClientResult UpdateStatus(RequestOptions? options = null)
    {
        ClientResult result = GetFileBatch(options);

        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromClientResult(result);

        ApplyUpdate(response, value);

        return result;
    }

    internal async Task<CreateBatchFileJobOperation> WaitUntilAsync(bool waitUntilCompleted, RequestOptions? options)
    {
        if (!waitUntilCompleted) return this;
        await WaitForCompletionAsync(options?.CancellationToken ?? default).ConfigureAwait(false);
        return this;
    }

    internal CreateBatchFileJobOperation WaitUntil(bool waitUntilCompleted, RequestOptions? options)
    {
        if (!waitUntilCompleted) return this;
        WaitForCompletion(options?.CancellationToken ?? default);
        return this;
    }

    private void ApplyUpdate(PipelineResponse response, VectorStoreBatchFileJob value)
    {
        Value = value;
        Status = value.Status;

        HasCompleted = GetHasCompleted(value.Status);
        SetRawResponse(response);
    }

    private static bool GetHasCompleted(VectorStoreBatchFileJobStatus status)
    {
        return status == VectorStoreBatchFileJobStatus.Completed ||
            status == VectorStoreBatchFileJobStatus.Cancelled ||
            status == VectorStoreBatchFileJobStatus.Failed;
    }

    // Generated convenience methods

    /// <summary>
    /// Gets an existing vector store batch file ingestion job from a known vector store ID and job ID.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the ingestion operation. </returns>
    public virtual async Task<ClientResult<VectorStoreBatchFileJob>> GetFileBatchAsync(CancellationToken cancellationToken = default)
    {
        ClientResult result = await GetFileBatchAsync(cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromClientResult(result);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Gets an existing vector store batch file ingestion job from a known vector store ID and job ID.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the ingestion operation. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> GetFileBatch(CancellationToken cancellationToken = default)
    {
        ClientResult result = GetFileBatch(cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromClientResult(result);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual async Task<ClientResult<VectorStoreBatchFileJob>> CancelAsync(CancellationToken cancellationToken = default)
    {
        ClientResult result = await CancelAsync(cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromClientResult(result);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> Cancel(CancellationToken cancellationToken = default)
    {
        ClientResult result = Cancel(cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromClientResult(result);
        return ClientResult.FromValue(value, response);
    }
}