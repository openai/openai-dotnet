using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.VectorStores;

[Experimental("OPENAI001")]
public partial class AddFileToVectorStoreOperation : OperationResult
{
    internal AddFileToVectorStoreOperation(
        VectorStoreClient parentClient,
        Uri endpoint,
        ClientResult<VectorStoreFileAssociation> result)
        : base(result.GetRawResponse())
    {
        _parentClient = parentClient;
        _endpoint = endpoint;

        Value = result;
        Status = Value.Status;

        _vectorStoreId = Value.VectorStoreId;
        _fileId = Value.FileId;

        HasCompleted = GetHasCompleted(Value.Status);
        RehydrationToken = new AddFileToVectorStoreOperationToken(VectorStoreId, FileId);
    }

    /// <summary>
    /// The current value of the add file to vector store operation in progress.
    /// </summary>
    public VectorStoreFileAssociation? Value { get; private set; }

    /// <summary>
    /// The current status of the add file to vector store operation in progress.
    /// </summary>
    public VectorStoreFileAssociationStatus? Status { get; private set; }

    /// <summary>
    /// The ID of the vector store the file is being added to.
    /// </summary>
    public string VectorStoreId { get => _vectorStoreId; }

    /// <summary>
    /// The ID of the file being added to the vector store.
    /// </summary>
    public string FileId { get => _fileId; }

    /// <summary>
    /// Recreates a <see cref="AddFileToVectorStoreOperation"/> from a rehydration token.
    /// </summary>
    /// <param name="client"> The <see cref="VectorStoreClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="rehydrationToken"> The rehydration token corresponding to 
    /// the operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="client"/> or <paramref name="rehydrationToken"/> is null. </exception>
    public static async Task<AddFileToVectorStoreOperation> RehydrateAsync(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(client, nameof(client));
        Argument.AssertNotNull(rehydrationToken, nameof(rehydrationToken));

        AddFileToVectorStoreOperationToken token = AddFileToVectorStoreOperationToken.FromToken(rehydrationToken);

        ClientResult result = await client.GetFileAssociationAsync(token.VectorStoreId, token.FileId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        VectorStoreFileAssociation value = VectorStoreFileAssociation.FromClientResult(result);

        return client.CreateAddFileToVectorStoreOperation(ClientResult.FromValue(value, response));
    }

    /// <summary>
    /// Recreates a <see cref="AddFileToVectorStoreOperation"/> from a rehydration token.
    /// </summary>
    /// <param name="client"> The <see cref="VectorStoreClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="rehydrationToken"> The rehydration token corresponding to 
    /// the operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="client"/> or <paramref name="rehydrationToken"/> is null. </exception>
    public static AddFileToVectorStoreOperation Rehydrate(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(client, nameof(client));
        Argument.AssertNotNull(rehydrationToken, nameof(rehydrationToken));

        AddFileToVectorStoreOperationToken token = AddFileToVectorStoreOperationToken.FromToken(rehydrationToken);

        ClientResult result = client.GetFileAssociation(token.VectorStoreId, token.FileId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();
        VectorStoreFileAssociation value = VectorStoreFileAssociation.FromClientResult(result);

        return client.CreateAddFileToVectorStoreOperation(ClientResult.FromValue(value, response));
    }

    /// <inheritdoc/>
    public override async ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options = null)
    {
        ClientResult result = await GetFileAssociationAsync(options).ConfigureAwait(false);

        PipelineResponse response = result.GetRawResponse();
        VectorStoreFileAssociation value = VectorStoreFileAssociation.FromClientResult(result);

        ApplyUpdate(response, value);

        return result;
    }

    /// <inheritdoc/>
    public override ClientResult UpdateStatus(RequestOptions? options = null)
    {
        ClientResult result = GetFileAssociation(options);

        PipelineResponse response = result.GetRawResponse();
        VectorStoreFileAssociation value = VectorStoreFileAssociation.FromClientResult(result);

        ApplyUpdate(response, value);

        return result;
    }

    internal async Task<AddFileToVectorStoreOperation> WaitUntilAsync(bool waitUntilCompleted, RequestOptions? options)
    {
        if (!waitUntilCompleted) return this;
        await WaitForCompletionAsync(options?.CancellationToken ?? default).ConfigureAwait(false);
        return this;
    }

    internal AddFileToVectorStoreOperation WaitUntil(bool waitUntilCompleted, RequestOptions? options)
    {
        if (!waitUntilCompleted) return this;
        WaitForCompletion(options?.CancellationToken ?? default);
        return this;
    }

    private void ApplyUpdate(PipelineResponse response, VectorStoreFileAssociation value)
    {
        Value = value;
        Status = value.Status;

        HasCompleted = GetHasCompleted(value.Status);
        SetRawResponse(response);
    }

    private static bool GetHasCompleted(VectorStoreFileAssociationStatus status)
    {
        return status == VectorStoreFileAssociationStatus.Completed ||
            status == VectorStoreFileAssociationStatus.Cancelled ||
            status == VectorStoreFileAssociationStatus.Failed;
    }

    /// <summary>
    /// Gets a <see cref="VectorStoreFileAssociation"/> instance representing an existing association between a known
    /// vector store ID and file ID.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreFileAssociation"/> instance. </returns>
    public virtual async Task<ClientResult<VectorStoreFileAssociation>> GetFileAssociationAsync(CancellationToken cancellationToken = default)
    {
        ClientResult result = await GetFileAssociationAsync(cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        VectorStoreFileAssociation value = VectorStoreFileAssociation.FromClientResult(result);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Gets a <see cref="VectorStoreFileAssociation"/> instance representing an existing association between a known
    /// vector store ID and file ID.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreFileAssociation"/> instance. </returns>
    public virtual ClientResult<VectorStoreFileAssociation> GetFileAssociation(CancellationToken cancellationToken = default)
    {
        ClientResult result = GetFileAssociation(cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();
        VectorStoreFileAssociation value = VectorStoreFileAssociation.FromClientResult(result);
        return ClientResult.FromValue(value, response);
    }
}
