using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.VectorStores;

[Experimental("OPENAI001")]
public partial class CreateVectorStoreOperation : OperationResult
{
    internal CreateVectorStoreOperation(
        VectorStoreClient parentClient,
        Uri endpoint,
        ClientResult<VectorStore> result)
        : base(result.GetRawResponse())
    {
        _parentClient = parentClient;
        _endpoint = endpoint;

        Value = result;
        Status = Value.Status;

        _vectorStoreId = Value.Id;

        HasCompleted = GetHasCompleted(Value.Status);
        RehydrationToken = new CreateVectorStoreOperationToken(VectorStoreId);
    }

    /// <summary>
    /// The current value of the create <see cref="VectorStore"/> operation in progress.
    /// </summary>
    public VectorStore? Value { get; private set; }

    /// <summary>
    /// The current status of the create <see cref="VectorStore"/> operation in progress.
    /// </summary>
    public VectorStoreStatus? Status { get; private set; }

    /// <summary>
    /// The ID of the vector store being created.
    /// </summary>
    public string VectorStoreId { get => _vectorStoreId; }


    /// <summary>
    /// Recreates a <see cref="CreateVectorStoreOperation"/> from a rehydration token.
    /// </summary>
    /// <param name="client"> The <see cref="VectorStoreClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="rehydrationToken"> The rehydration token corresponding to 
    /// the operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="client"/> or <paramref name="rehydrationToken"/> is null. </exception>
    public static async Task<CreateVectorStoreOperation> RehydrateAsync(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(client, nameof(client));
        Argument.AssertNotNull(rehydrationToken, nameof(rehydrationToken));

        CreateVectorStoreOperationToken token = CreateVectorStoreOperationToken.FromToken(rehydrationToken);

        ClientResult result = await client.GetVectorStoreAsync(token.VectorStoreId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        VectorStore vectorStore = VectorStore.FromClientResult(result);

        return client.CreateCreateVectorStoreOperation(ClientResult.FromValue(vectorStore, response));
    }

    /// <summary>
    /// Recreates a <see cref="CreateVectorStoreOperation"/> from a rehydration token.
    /// </summary>
    /// <param name="client"> The <see cref="VectorStoreClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="rehydrationToken"> The rehydration token corresponding to 
    /// the operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="client"/> or <paramref name="rehydrationToken"/> is null. </exception>
    public static CreateVectorStoreOperation Rehydrate(VectorStoreClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(client, nameof(client));
        Argument.AssertNotNull(rehydrationToken, nameof(rehydrationToken));

        CreateVectorStoreOperationToken token = CreateVectorStoreOperationToken.FromToken(rehydrationToken);

        ClientResult result = client.GetVectorStore(token.VectorStoreId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();
        VectorStore vectorStore = VectorStore.FromClientResult(result);

        return client.CreateCreateVectorStoreOperation(ClientResult.FromValue(vectorStore, response));
    }

    /// <inheritdoc/>
    public override async ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options = null)
    {
        ClientResult result = await GetVectorStoreAsync(options).ConfigureAwait(false);

        PipelineResponse response = result.GetRawResponse();
        VectorStore value = VectorStore.FromClientResult(result);

        ApplyUpdate(response, value);

        return result;
    }

    /// <inheritdoc/>
    public override ClientResult UpdateStatus(RequestOptions? options = null)
    {
        ClientResult result = GetVectorStore(options);

        PipelineResponse response = result.GetRawResponse();
        VectorStore value = VectorStore.FromClientResult(result);

        ApplyUpdate(response, value);

        return result;
    }

    internal async Task<CreateVectorStoreOperation> WaitUntilAsync(bool waitUntilCompleted, RequestOptions? options)
    {
        if (!waitUntilCompleted) return this;
        await WaitForCompletionAsync(options?.CancellationToken ?? default).ConfigureAwait(false);
        return this;
    }

    internal CreateVectorStoreOperation WaitUntil(bool waitUntilCompleted, RequestOptions? options)
    {
        if (!waitUntilCompleted) return this;
        WaitForCompletion(options?.CancellationToken ?? default);
        return this;
    }

    private void ApplyUpdate(PipelineResponse response, VectorStore value)
    {
        Value = value;
        Status = value.Status;

        HasCompleted = GetHasCompleted(value.Status);
        SetRawResponse(response);
    }

    private static bool GetHasCompleted(VectorStoreStatus status)
    {
        return status == VectorStoreStatus.Completed ||
            status == VectorStoreStatus.Expired;
    }

    /// <summary>
    /// Gets an instance representing an existing <see cref="VectorStore"/>.
    /// </summary>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A representation of an existing <see cref="VectorStore"/>. </returns>
    public virtual async Task<ClientResult<VectorStore>> GetVectorStoreAsync(CancellationToken cancellationToken = default)
    {
        ClientResult result
            = await GetVectorStoreAsync(cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(
            VectorStore.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary>
    /// Gets an instance representing an existing <see cref="VectorStore"/>.
    /// </summary>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A representation of an existing <see cref="VectorStore"/>. </returns>
    public virtual ClientResult<VectorStore> GetVectorStore(CancellationToken cancellationToken = default)
    {
        ClientResult result = GetVectorStore(cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(VectorStore.FromClientResult(result), result.GetRawResponse());
    }
}
