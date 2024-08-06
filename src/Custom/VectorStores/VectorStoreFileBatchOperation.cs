using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.VectorStores;

/// <summary>
/// Long-running operation for creating a vector store file batch.
/// </summary>
public partial class VectorStoreFileBatchOperation : OperationResult
{
    internal VectorStoreFileBatchOperation(
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
        RehydrationToken = new VectorStoreFileBatchOperationToken(VectorStoreId, BatchId);
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
    /// Recreates a <see cref="VectorStoreFileBatchOperation"/> from a rehydration token.
    /// </summary>
    /// <param name="client"> The <see cref="VectorStoreClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="rehydrationToken"> The rehydration token corresponding to 
    /// the operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public static async Task<VectorStoreFileBatchOperation> RehydrateAsync(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default)
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    {
        Argument.AssertNotNull(client, nameof(client));
        Argument.AssertNotNull(rehydrationToken, nameof(rehydrationToken));

        VectorStoreFileBatchOperationToken token = VectorStoreFileBatchOperationToken.FromToken(rehydrationToken);

        ClientResult result = await client.GetBatchFileJobAsync(token.VectorStoreId, token.BatchId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob job = VectorStoreBatchFileJob.FromResponse(response);

        return new VectorStoreFileBatchOperation(client.Pipeline, client.Endpoint, FromValue(job, response));
    }

    /// <summary>
    /// Recreates a <see cref="VectorStoreFileBatchOperation"/> from a rehydration token.
    /// </summary>
    /// <param name="client"> The <see cref="VectorStoreClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="rehydrationToken"> The rehydration token corresponding to 
    /// the operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public static VectorStoreFileBatchOperation Rehydrate(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default)
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    {
        Argument.AssertNotNull(client, nameof(client));
        Argument.AssertNotNull(rehydrationToken, nameof(rehydrationToken));

        VectorStoreFileBatchOperationToken token = VectorStoreFileBatchOperationToken.FromToken(rehydrationToken);

        ClientResult result = client.GetBatchFileJob(token.VectorStoreId, token.BatchId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob job = VectorStoreBatchFileJob.FromResponse(response);

        return new VectorStoreFileBatchOperation(client.Pipeline, client.Endpoint, FromValue(job, response));
    }

    /// <inheritdoc/>
    public override async Task WaitForCompletionAsync(CancellationToken cancellationToken = default)
    {
        _pollingInterval ??= new();

        while (!IsCompleted)
        {
            PipelineResponse response = GetRawResponse();

            await _pollingInterval.WaitAsync(response, cancellationToken);

            ClientResult<VectorStoreBatchFileJob> result = await GetBatchFileJobAsync(cancellationToken).ConfigureAwait(false);

            ApplyUpdate(result);
        }
    }

    /// <summary>
    /// Waits for the operation to complete processing on the service.
    /// </summary>
    /// <param name="pollingInterval"> The time to wait between sending requests
    /// for status updates from the service. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this
    /// method call. </param>
    public async Task WaitForCompletionAsync(TimeSpan pollingInterval, CancellationToken cancellationToken = default)
    {
        _pollingInterval = new(pollingInterval);

        await WaitForCompletionAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override void WaitForCompletion(CancellationToken cancellationToken = default)
    {
        _pollingInterval ??= new();

        while (!IsCompleted)
        {
            PipelineResponse response = GetRawResponse();

            _pollingInterval.Wait(response, cancellationToken);

            ClientResult<VectorStoreBatchFileJob> result = GetBatchFileJob(cancellationToken);

            ApplyUpdate(result);
        }
    }

    /// <summary>
    /// Waits for the operation to complete processing on the service.
    /// </summary>
    /// <param name="pollingInterval"> The time to wait between sending requests
    /// for status updates from the service. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this
    /// method call. </param>
    public void WaitForCompletion(TimeSpan pollingInterval, CancellationToken cancellationToken = default)
    {
        _pollingInterval = new(pollingInterval);

        WaitForCompletion(cancellationToken);
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
    public virtual async Task<ClientResult<VectorStoreBatchFileJob>> GetBatchFileJobAsync(CancellationToken cancellationToken = default)
    {
        ClientResult result = await GetBatchFileJobAsync(_vectorStoreId, _batchId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Gets an existing vector store batch file ingestion job from a known vector store ID and job ID.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the ingestion operation. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> GetBatchFileJob(CancellationToken cancellationToken = default)
    {
        ClientResult result = GetBatchFileJob(_vectorStoreId, _batchId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual async Task<ClientResult<VectorStoreBatchFileJob>> CancelBatchFileJobAsync(CancellationToken cancellationToken = default)
    {
        ClientResult result = await CancelBatchFileJobAsync(_vectorStoreId, _batchId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> CancelBatchFileJob(CancellationToken cancellationToken = default)
    {
        ClientResult result = CancelBatchFileJob(_vectorStoreId, _batchId, cancellationToken.ToRequestOptions());
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
    public virtual AsyncPageCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        VectorStoreFileAssociationCollectionOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        VectorStoreFileBatchesPageEnumerator enumerator = new(_pipeline, _endpoint,
            _vectorStoreId,
            _batchId,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            options?.Filter?.ToString(),
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
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
    public virtual AsyncPageCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreFileBatchesPageToken pageToken = VectorStoreFileBatchesPageToken.FromToken(firstPageToken);

        if (_vectorStoreId != pageToken.VectorStoreId)
        {
            throw new ArgumentException(
                "Invalid page token. 'VectorStoreId' value does not match page token value.",
                nameof(firstPageToken));
        }

        if (_batchId != pageToken.BatchId)
        {
            throw new ArgumentException(
                "Invalid page token. 'BatchId' value does not match page token value.",
                nameof(firstPageToken));
        }

        VectorStoreFileBatchesPageEnumerator enumerator = new(_pipeline, _endpoint,
            pageToken.VectorStoreId,
            pageToken.BatchId,
            pageToken.Limit,
            pageToken.Order,
            pageToken.After,
            pageToken.Before,
            pageToken.Filter,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
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
    public virtual PageCollection<VectorStoreFileAssociation> GetFileAssociations(
        VectorStoreFileAssociationCollectionOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        VectorStoreFileBatchesPageEnumerator enumerator = new(_pipeline, _endpoint,
            _vectorStoreId,
            _batchId,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            options?.Filter?.ToString(),
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
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
    public virtual PageCollection<VectorStoreFileAssociation> GetFileAssociations(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreFileBatchesPageToken pageToken = VectorStoreFileBatchesPageToken.FromToken(firstPageToken);

        if (_vectorStoreId != pageToken.VectorStoreId)
        {
            throw new ArgumentException(
                "Invalid page token. 'VectorStoreId' value does not match page token value.",
                nameof(firstPageToken));
        }

        if (_batchId != pageToken.BatchId)
        {
            throw new ArgumentException(
                "Invalid page token. 'BatchId' value does not match page token value.",
                nameof(firstPageToken));
        }

        VectorStoreFileBatchesPageEnumerator enumerator = new(_pipeline, _endpoint,
            pageToken.VectorStoreId,
            pageToken.BatchId,
            pageToken.Limit,
            pageToken.Order,
            pageToken.After,
            pageToken.Before,
            pageToken.Filter,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
    }
}