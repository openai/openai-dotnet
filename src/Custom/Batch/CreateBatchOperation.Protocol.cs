using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Batch;

/// <summary>
/// A long-running operation for executing a batch from an uploaded file of 
/// requests.
/// </summary>
public partial class CreateBatchOperation : OperationResult
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;

    private readonly string _batchId;

    private PollingInterval? _pollingInterval;

    internal CreateBatchOperation(
        ClientPipeline pipeline,
        Uri endpoint,
        string batchId,
        string status,
        PipelineResponse response)
        : base(response)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
        _batchId = batchId;

        IsCompleted = GetIsCompleted(status);
        RehydrationToken = new CreateBatchOperationToken(batchId);
    }

    public string BatchId => _batchId;

    /// <inheritdoc/>
    public override ContinuationToken? RehydrationToken { get; protected set; }

    /// <inheritdoc/>
    public override bool IsCompleted { get; protected set; }

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

        ClientResult result = await client.GetBatchAsync(token.BatchId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string status = doc.RootElement.GetProperty("status"u8).GetString()!;

        return new CreateBatchOperation(client.Pipeline, client.Endpoint, token.BatchId, status, response);
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

        ClientResult result = client.GetBatch(token.BatchId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string status = doc.RootElement.GetProperty("status"u8).GetString()!;

        return new CreateBatchOperation(client.Pipeline, client.Endpoint, token.BatchId, status, response);
    }

    /// <inheritdoc/>
    public override async Task WaitForCompletionAsync(CancellationToken cancellationToken = default)
    {
        _pollingInterval ??= new();

        while (!IsCompleted)
        {
            PipelineResponse response = GetRawResponse();

            await _pollingInterval.WaitAsync(response, cancellationToken);

            ClientResult result = await GetBatchAsync(cancellationToken.ToRequestOptions()).ConfigureAwait(false);

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

            ClientResult result = GetBatch(cancellationToken.ToRequestOptions());

            ApplyUpdate(result);
        }
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

        IsCompleted = GetIsCompleted(status);
        SetRawResponse(response);
    }

    private static bool GetIsCompleted(string? status)
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
        using PipelineMessage message = CreateRetrieveBatchRequest(_batchId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a batch.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetBatch(RequestOptions? options)
    {
        using PipelineMessage message = CreateRetrieveBatchRequest(_batchId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Cancels an in-progress batch.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CancelBatchAsync( RequestOptions? options)
    {
        using PipelineMessage message = CreateCancelBatchRequest(_batchId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Cancels an in-progress batch.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CancelBatch(RequestOptions? options)
    {
        using PipelineMessage message = CreateCancelBatchRequest(_batchId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    internal PipelineMessage CreateRetrieveBatchRequest(string batchId, RequestOptions? options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/batches/", false);
        uri.AppendPath(batchId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateCancelBatchRequest(string batchId, RequestOptions? options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "POST";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/batches/", false);
        uri.AppendPath(batchId, true);
        uri.AppendPath("/cancel", false);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}