using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.FineTuning;

/// <summary>
/// A long-running operation for creating a new model from a given dataset.
/// </summary>
public partial class CreateJobOperation : OperationResult
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;

    private readonly string _jobId;

    private PollingInterval? _pollingInterval;

    internal CreateJobOperation(
        ClientPipeline pipeline,
        Uri endpoint,
        string jobId,
        string status,
        PipelineResponse response) : base(response)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
        _jobId = jobId;

        IsCompleted = GetIsCompleted(status);
        RehydrationToken = new CreateJobOperationToken(jobId);
    }

    public string JobId => _jobId;

    /// <inheritdoc/>
    public override ContinuationToken? RehydrationToken { get; protected set; }

    /// <inheritdoc/>
    public override bool IsCompleted { get; protected set; }

    /// <summary>
    /// Recreates a <see cref="CreateJobOperation"/> from a rehydration token.
    /// </summary>
    /// <param name="client"> The <see cref="FineTuningClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="rehydrationToken"> The rehydration token corresponding to 
    /// the operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="client"/> or <paramref name="rehydrationToken"/> is null. </exception>
    public static async Task<CreateJobOperation> RehydrateAsync(FineTuningClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(client, nameof(client));
        Argument.AssertNotNull(rehydrationToken, nameof(rehydrationToken));

        CreateJobOperationToken token = CreateJobOperationToken.FromToken(rehydrationToken);

        ClientResult result = await client.GetJobAsync(token.JobId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string status = doc.RootElement.GetProperty("status"u8).GetString()!;

        return new CreateJobOperation(client.Pipeline, client.Endpoint, token.JobId, status, response);
    }

    /// <summary>
    /// Recreates a <see cref="CreateJobOperation"/> from a rehydration token.
    /// </summary>
    /// <param name="client"> The <see cref="FineTuningClient"/> used to obtain the 
    /// operation status from the service. </param>
    /// <param name="rehydrationToken"> The rehydration token corresponding to 
    /// the operation to rehydrate. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel the 
    /// request. </param>
    /// <returns> The rehydrated operation. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="client"/> or <paramref name="rehydrationToken"/> is null. </exception>
    public static CreateJobOperation Rehydrate(FineTuningClient client, ContinuationToken rehydrationToken, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(client, nameof(client));
        Argument.AssertNotNull(rehydrationToken, nameof(rehydrationToken));

        CreateJobOperationToken token = CreateJobOperationToken.FromToken(rehydrationToken);

        ClientResult result = client.GetJob(token.JobId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string status = doc.RootElement.GetProperty("status"u8).GetString()!;

        return new CreateJobOperation(client.Pipeline, client.Endpoint, token.JobId, status, response);
    }

    /// <inheritdoc/>
    public override async Task WaitForCompletionAsync(CancellationToken cancellationToken = default)
    {
        _pollingInterval ??= new();

        while (!IsCompleted)
        {
            PipelineResponse response = GetRawResponse();

            await _pollingInterval.WaitAsync(response, cancellationToken);

            ClientResult result = await GetJobAsync(cancellationToken.ToRequestOptions()).ConfigureAwait(false);

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

            ClientResult result = GetJob(cancellationToken.ToRequestOptions());

            ApplyUpdate(result);
        }
    }

    internal async Task<CreateJobOperation> WaitUntilAsync(bool waitUntilCompleted, RequestOptions? options)
    {
        if (!waitUntilCompleted) return this;
        await WaitForCompletionAsync(options?.CancellationToken ?? default).ConfigureAwait(false);
        return this;
    }

    internal CreateJobOperation WaitUntil(bool waitUntilCompleted, RequestOptions? options)
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
        return status == InternalFineTuningJobStatus.Succeeded ||
            status == InternalFineTuningJobStatus.Failed ||
            status == InternalFineTuningJobStatus.Cancelled;
    }

    // Generated protocol methods

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Get info about a fine-tuning job.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetJobAsync(RequestOptions? options)
    {
        using PipelineMessage message = CreateRetrieveFineTuningJobRequest(_jobId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Get info about a fine-tuning job.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetJob(RequestOptions? options)
    {
        using PipelineMessage message = CreateRetrieveFineTuningJobRequest(_jobId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Immediately cancel a fine-tune job.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CancelJobAsync(RequestOptions? options)
    {
        using PipelineMessage message = CreateCancelFineTuningJobRequest(_jobId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Immediately cancel a fine-tune job.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CancelJob(RequestOptions? options)
    {
        using PipelineMessage message = CreateCancelFineTuningJobRequest(_jobId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Get status updates for a fine-tuning job.
    /// </summary>
    /// <param name="after"> Identifier for the last event from the previous pagination request. </param>
    /// <param name="limit"> Number of events to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetJobEventsAsync(string after, int? limit, RequestOptions? options)
    {
        using PipelineMessage message = CreateGetFineTuningEventsRequest(_jobId, after, limit, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Get status updates for a fine-tuning job.
    /// </summary>
    /// <param name="after"> Identifier for the last event from the previous pagination request. </param>
    /// <param name="limit"> Number of events to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetJobEvents(string after, int? limit, RequestOptions? options)
    {
        using PipelineMessage message = CreateGetFineTuningEventsRequest(_jobId, after, limit, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] List the checkpoints for a fine-tuning job.
    /// </summary>
    /// <param name="after"> Identifier for the last checkpoint ID from the previous pagination request. </param>
    /// <param name="limit"> Number of checkpoints to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetJobCheckpointsAsync(string after, int? limit, RequestOptions? options)
    {
        using PipelineMessage message = CreateGetFineTuningJobCheckpointsRequest(_jobId, after, limit, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] List the checkpoints for a fine-tuning job.
    /// </summary>
    /// <param name="after"> Identifier for the last checkpoint ID from the previous pagination request. </param>
    /// <param name="limit"> Number of checkpoints to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetJobCheckpoints(string after, int? limit, RequestOptions? options)
    {
        using PipelineMessage message = CreateGetFineTuningJobCheckpointsRequest(_jobId, after, limit, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    internal PipelineMessage CreateRetrieveFineTuningJobRequest(string fineTuningJobId, RequestOptions? options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/fine_tuning/jobs/", false);
        uri.AppendPath(fineTuningJobId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateCancelFineTuningJobRequest(string fineTuningJobId, RequestOptions? options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "POST";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/fine_tuning/jobs/", false);
        uri.AppendPath(fineTuningJobId, true);
        uri.AppendPath("/cancel", false);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateGetFineTuningJobCheckpointsRequest(string fineTuningJobId, string after, int? limit, RequestOptions? options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/fine_tuning/jobs/", false);
        uri.AppendPath(fineTuningJobId, true);
        uri.AppendPath("/checkpoints", false);
        if (after != null)
        {
            uri.AppendQuery("after", after, true);
        }
        if (limit != null)
        {
            uri.AppendQuery("limit", limit.Value, true);
        }
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateGetFineTuningEventsRequest(string fineTuningJobId, string after, int? limit, RequestOptions? options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/fine_tuning/jobs/", false);
        uri.AppendPath(fineTuningJobId, true);
        uri.AppendPath("/events", false);
        if (after != null)
        {
            uri.AppendQuery("after", after, true);
        }
        if (limit != null)
        {
            uri.AppendQuery("limit", limit.Value, true);
        }
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}
