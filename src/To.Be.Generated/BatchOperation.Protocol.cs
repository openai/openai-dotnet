using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Batch;

// Protocol version
public partial class BatchOperation : OperationResult
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;

    private readonly string _batchId;

    private PollingInterval _pollingInterval;

    // For use with protocol methods where the response has been obtained prior
    // to creation of the LRO instance.
    internal BatchOperation(
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

        _pollingInterval = new();

        RehydrationToken = new BatchOperationToken(batchId);
    }

    public override ContinuationToken? RehydrationToken { get; protected set; }

    public override bool IsCompleted { get; protected set; }

    // These are replaced when LRO is evolved to have conveniences
    public override async Task WaitForCompletionAsync(CancellationToken cancellationToken = default)
    {
        while (!IsCompleted)
        {
            await _pollingInterval.WaitAsync(cancellationToken);

            ClientResult result = await GetBatchAsync(_batchId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);

            ApplyUpdate(result);
        }
    }

    public override void WaitForCompletion(CancellationToken cancellationToken = default)
    {
        while (!IsCompleted)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _pollingInterval.Wait();

            ClientResult result = GetBatch(_batchId, cancellationToken.ToRequestOptions());

            ApplyUpdate(result);
        }
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
        return status == "completed" ||
            status == "cancelled" ||
            status == "expired" ||
            status == "failed";
    }

    // Generated protocol methods

    /// <summary>
    /// [Protocol Method] Retrieves a batch.
    /// </summary>
    /// <param name="batchId"> The ID of the batch to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetBatchAsync(string batchId, RequestOptions options)
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
    public virtual ClientResult GetBatch(string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateRetrieveBatchRequest(batchId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Cancels an in-progress batch.
    /// </summary>
    /// <param name="batchId"> The ID of the batch to cancel. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CancelBatchAsync(string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateCancelBatchRequest(batchId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Cancels an in-progress batch.
    /// </summary>
    /// <param name="batchId"> The ID of the batch to cancel. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CancelBatch(string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateCancelBatchRequest(batchId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }
    internal PipelineMessage CreateRetrieveBatchRequest(string batchId, RequestOptions options)
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

    internal PipelineMessage CreateCancelBatchRequest(string batchId, RequestOptions options)
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