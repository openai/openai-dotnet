using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Batch;

/// <summary>
/// A long-running operation for executing a batch from an uploaded file of 
/// requests.
/// </summary>
[Experimental("OPENAI001")]
public class CreateBatchOperation : OperationResult
{
    private readonly BatchClient _parentClient;
    private readonly Uri _endpoint;
    private readonly string _batchId;

    internal CreateBatchOperation(
        BatchClient parentClient,
        Uri endpoint,
        string batchId,
        string status,
        PipelineResponse response)
            : base(response)
    {
        _parentClient = parentClient;
        _endpoint = endpoint;
        _batchId = batchId;

        HasCompleted = GetHasCompleted(status);
        RehydrationToken = new CreateBatchOperationToken(batchId);
    }

    public string BatchId => _batchId;

    /// <inheritdoc/>
    public override ContinuationToken? RehydrationToken { get; protected set; }

    /// <summary>
    /// Recreates a <see cref="CreateBatchOperation"/> from a rehydration token.
    /// </summary>
    /// <param name="client"> The <see cref="BatchClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="rehydrationToken"> The rehydration token corresponding to 
    /// the operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="client"/> or <paramref name="rehydrationToken"/> is null. </exception>
    public static async Task<CreateBatchOperation> RehydrateAsync(BatchClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(client, nameof(client));
        Argument.AssertNotNull(rehydrationToken, nameof(rehydrationToken));

        CreateBatchOperationToken token = CreateBatchOperationToken.FromToken(rehydrationToken);

        return await RehydrateAsync(client, token.BatchId, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Recreates a <see cref="CreateBatchOperation"/> from a rehydration token.
    /// </summary>
    /// <param name="client"> The <see cref="BatchClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="rehydrationToken"> The rehydration token corresponding to 
    /// the operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="client"/> or <paramref name="rehydrationToken"/> is null. </exception>
    public static CreateBatchOperation Rehydrate(BatchClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(client, nameof(client));
        Argument.AssertNotNull(rehydrationToken, nameof(rehydrationToken));

        CreateBatchOperationToken token = CreateBatchOperationToken.FromToken(rehydrationToken);

        return Rehydrate(client, token.BatchId, cancellationToken);
    }

    /// <summary>
    /// Recreates a <see cref="CreateBatchOperation"/> from a batch ID.
    /// </summary>
    /// <param name="client"> The <see cref="BatchClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="batchId"> The ID of the batch operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="client"/> is null. </exception>
    public static async Task<CreateBatchOperation> RehydrateAsync(BatchClient client, string batchId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(client, nameof(client));

        ClientResult result = await client.GetBatchAsync(batchId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string status = doc.RootElement.GetProperty("status"u8).GetString()!;

        return client.CreateCreateBatchOperation(batchId, status, response);
    }

    /// <summary>
    /// Recreates a <see cref="CreateBatchOperation"/> from a batch ID.
    /// </summary>
    /// <param name="client"> The <see cref="BatchClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="batchId"> The ID of the batch operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="client"/> is null. </exception>
    public static CreateBatchOperation Rehydrate(BatchClient client, string batchId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(client, nameof(client));

        ClientResult result = client.GetBatch(batchId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string status = doc.RootElement.GetProperty("status"u8).GetString()!;

        return client.CreateCreateBatchOperation(batchId, status, response);
    }

    /// <inheritdoc/>
    public override async ValueTask<ClientResult> UpdateStatusAsync(RequestOptions? options = null)
    {
        ClientResult result = await GetBatchAsync(options).ConfigureAwait(false);

        ApplyUpdate(result);

        return result;
    }

    /// <inheritdoc/>
    public override ClientResult UpdateStatus(RequestOptions? options = null)
    {
        ClientResult result = GetBatch(options);

        ApplyUpdate(result);

        return result;
    }

    internal async Task<CreateBatchOperation> WaitUntilAsync(bool waitUntilCompleted, RequestOptions? options)
    {
        if (!waitUntilCompleted) return this;
        await WaitForCompletionAsync(options?.CancellationToken ?? default).ConfigureAwait(false);
        return this;
    }

    internal CreateBatchOperation WaitUntil(bool waitUntilCompleted, RequestOptions? options)
    {
        if (!waitUntilCompleted) return this;
        WaitForCompletion(options?.CancellationToken ?? default);
        return this;
    }

    private void ApplyUpdate(ClientResult result)
    {
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string? status = doc.RootElement.GetProperty("status"u8).GetString();

        HasCompleted = GetHasCompleted(status);
        SetRawResponse(response);
    }

    private static bool GetHasCompleted(string? status)
    {
        return status == InternalBatchStatus.Completed ||
            status == InternalBatchStatus.Cancelled ||
            status == InternalBatchStatus.Expired ||
            status == InternalBatchStatus.Failed;
    }

    // Generated protocol methods

    /// <summary>
    /// [Protocol Method] Retrieves a batch.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetBatchAsync(RequestOptions? options)
    {
        using PipelineMessage message = _parentClient.CreateGetBatchRequest(_batchId, options);
        return ClientResult.FromResponse(await _parentClient.Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a batch.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetBatch(RequestOptions? options)
    {
        using PipelineMessage message = _parentClient.CreateGetBatchRequest(_batchId, options);
        return ClientResult.FromResponse(_parentClient.Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Cancels an in-progress batch.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CancelAsync(RequestOptions? options)
    {
        using PipelineMessage message = _parentClient.CreateCancelBatchRequest(_batchId, options);
        return ClientResult.FromResponse(await _parentClient.Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Cancels an in-progress batch.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult Cancel(RequestOptions? options)
    {
        using PipelineMessage message = _parentClient.CreateCancelBatchRequest(_batchId, options);
        return ClientResult.FromResponse(_parentClient.Pipeline.ProcessMessage(message, options));
    }
}