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
        ClientPipeline pipeline,
        Uri endpoint,
        ClientResult<VectorStoreBatchFileJob> result)
        : base(result.GetRawResponse())
    {
        _pipeline = pipeline;
        _endpoint = endpoint;

        Value = result;
        Status = Value.Status;

        _vectorStoreId = Value.VectorStoreId;
        _batchId = Value.BatchId;

        IsCompleted = GetIsCompleted(Value.Status);
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
        VectorStoreBatchFileJob job = VectorStoreBatchFileJob.FromResponse(response);

        return new CreateBatchFileJobOperation(client.Pipeline, client.Endpoint, FromValue(job, response));
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
        VectorStoreBatchFileJob job = VectorStoreBatchFileJob.FromResponse(response);

        return new CreateBatchFileJobOperation(client.Pipeline, client.Endpoint, FromValue(job, response));
    }

    /// <inheritdoc/>
    public override async Task WaitForCompletionAsync(CancellationToken cancellationToken = default)
    {
        _pollingInterval ??= new();

        while (!IsCompleted)
        {
            PipelineResponse response = GetRawResponse();

            await _pollingInterval.WaitAsync(response, cancellationToken);

            ClientResult<VectorStoreBatchFileJob> result = await GetFileBatchAsync(cancellationToken).ConfigureAwait(false);

            ApplyUpdate(result);
        }
    }

    /// <inheritdoc/>
    public override void WaitForCompletion(CancellationToken cancellationToken = default)
    {
        _pollingInterval ??= new();

        while (!IsCompleted)
        {
            PipelineResponse response = GetRawResponse();

            _pollingInterval.Wait(response, cancellationToken);

            ClientResult<VectorStoreBatchFileJob> result = GetFileBatch(cancellationToken);

            ApplyUpdate(result);
        }
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

    private void ApplyUpdate(ClientResult<VectorStoreBatchFileJob> update)
    {
        Value = update;
        Status = Value.Status;

        IsCompleted = GetIsCompleted(Value.Status);
        SetRawResponse(update.GetRawResponse());
    }

    private static bool GetIsCompleted(VectorStoreBatchFileJobStatus status)
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
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
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
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual async Task<ClientResult<VectorStoreBatchFileJob>> CancelFileBatchAsync(CancellationToken cancellationToken = default)
    {
        ClientResult result = await CancelFileBatchAsync(cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> CancelFileBatch(CancellationToken cancellationToken = default)
    {
        ClientResult result = CancelFileBatch(cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Gets a page collection of file associations associated with a vector store batch file job, representing the files
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="AsyncPageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="AsyncPageCollection{T}.GetAllValuesAsync(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="AsyncPageCollection{T}.GetCurrentPageAsync"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual AsyncPageCollection<VectorStoreFileAssociation> GetFilesInBatchAsync(
        VectorStoreFileAssociationCollectionOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        return GetFilesInBatchAsync(options?.PageSize, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, options?.Filter?.ToString(), cancellationToken.ToRequestOptions()) is not AsyncPageCollection<VectorStoreFileAssociation> pages
            ? throw new NotSupportedException("Failed to cast protocol method return type to AsyncPageCollection<VectorStoreFileAssociation>.")
            : pages;
    }

    /// <summary>
    /// Rehydrates a page collection of file associations from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="AsyncPageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="AsyncPageCollection{T}.GetAllValuesAsync(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="AsyncPageCollection{T}.GetCurrentPageAsync"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual AsyncPageCollection<VectorStoreFileAssociation> GetFilesInBatchAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreFileBatchesPageToken pageToken = VectorStoreFileBatchesPageToken.FromToken(firstPageToken);

        if (_vectorStoreId != pageToken.VectorStoreId)
        {
            throw new ArgumentException(
                "Invalid page token. 'VectorStoreId' value does not match page token value.",
                nameof(VectorStoreId));
        }

        if (_batchId != pageToken.BatchId)
        {
            throw new ArgumentException(
                "Invalid page token. 'BatchId' value does not match page token value.",
                nameof(BatchId));
        }

        return GetFilesInBatchAsync(pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, pageToken?.Filter, cancellationToken.ToRequestOptions()) is not AsyncPageCollection<VectorStoreFileAssociation> pages
            ? throw new NotSupportedException("Failed to cast protocol method return type to PageCollection<VectorStoreFileAssociation>.")
            : pages;
    }

    /// <summary>
    /// Gets a page collection of file associations associated with a vector store batch file job, representing the files
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="PageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="PageCollection{T}.GetAllValues(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="PageCollection{T}.GetCurrentPage"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual PageCollection<VectorStoreFileAssociation> GetFilesInBatch(
        VectorStoreFileAssociationCollectionOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        return GetFilesInBatch(options?.PageSize, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, options?.Filter?.ToString(), cancellationToken.ToRequestOptions()) is not PageCollection<VectorStoreFileAssociation> pages
            ? throw new NotSupportedException("Failed to cast protocol method return type to AsyncPageCollection<VectorStoreFileAssociation>.")
            : pages;
    }

    /// <summary>
    /// Rehydrates a page collection of file associations from a page token.
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="PageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="PageCollection{T}.GetAllValues(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="PageCollection{T}.GetCurrentPage"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual PageCollection<VectorStoreFileAssociation> GetFilesInBatch(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreFileBatchesPageToken pageToken = VectorStoreFileBatchesPageToken.FromToken(firstPageToken);

        if (VectorStoreId != pageToken.VectorStoreId)
        {
            throw new ArgumentException(
                "Invalid page token. 'VectorStoreId' value does not match page token value.",
                nameof(VectorStoreId));
        }

        if (BatchId != pageToken.BatchId)
        {
            throw new ArgumentException(
                "Invalid page token. 'BatchId' value does not match page token value.",
                nameof(BatchId));
        }

        return GetFilesInBatch(pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, pageToken?.Filter, cancellationToken.ToRequestOptions()) is not PageCollection<VectorStoreFileAssociation> pages
            ? throw new NotSupportedException("Failed to cast protocol method return type to PageCollection<VectorStoreFileAssociation>.")
            : pages;
    }
}