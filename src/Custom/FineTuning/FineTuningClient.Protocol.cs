using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.FineTuning;

[CodeGenSuppress("CreateFineTuningJobAsync", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateFineTuningJob", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("GetPaginatedFineTuningJobsAsync", typeof(string), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("GetPaginatedFineTuningJobs", typeof(string), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("RetrieveFineTuningJobAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("RetrieveFineTuningJob", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CancelFineTuningJobAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CancelFineTuningJob", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetFineTuningEventsAsync", typeof(string), typeof(string), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("GetFineTuningEvents", typeof(string), typeof(string), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("GetFineTuningJobCheckpointsAsync", typeof(string), typeof(string), typeof(int?), typeof(RequestOptions))]
[CodeGenSuppress("GetFineTuningJobCheckpoints", typeof(string), typeof(string), typeof(int?), typeof(RequestOptions))]
public partial class FineTuningClient
{
    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Creates a fine-tuning job which begins the process of creating a new model from a given dataset.
    ///
    /// Response includes details of the enqueued job including job status and the name of the fine-tuned models once complete.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="FineTuningJobOperation"/> that can be used to wait for 
    /// the operation to complete, get information about the fine tuning job, or 
    /// cancel the operation. </returns>
    public virtual async Task<FineTuningJobOperation> CreateFineTuningJobAsync(
        BinaryContent content,
        bool waitUntilCompleted,
        RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateFineTuningJobRequest(content, options);
        PipelineResponse response = await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string jobId = doc.RootElement.GetProperty("id"u8).GetString();
        string status = doc.RootElement.GetProperty("status"u8).GetString();

        FineTuningJobOperation operation = this.CreateCreateJobOperation(jobId, status, response);
        return await operation.WaitUntilAsync(waitUntilCompleted, options).ConfigureAwait(false);
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Creates a fine-tuning job which begins the process of creating a new model from a given dataset.
    ///
    /// Response includes details of the enqueued job including job status and the name of the fine-tuned models once complete.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="FineTuningJobOperation"/> that can be used to wait for 
    /// the operation to complete, get information about the fine tuning job, or 
    /// cancel the operation. </returns>
    public virtual FineTuningJobOperation CreateFineTuningJob(
        BinaryContent content,
        bool waitUntilCompleted,
        RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateFineTuningJobRequest(content, options);
        PipelineResponse response = _pipeline.ProcessMessage(message, options);

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string jobId = doc.RootElement.GetProperty("id"u8).GetString();
        string status = doc.RootElement.GetProperty("status"u8).GetString();

        FineTuningJobOperation operation = this.CreateCreateJobOperation(jobId, status, response);
        return operation.WaitUntil(waitUntilCompleted, options);
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] List your organization's fine-tuning jobs
    /// </summary>
    /// <param name="after"> Identifier for the last job from the previous pagination request. </param>
    /// <param name="limit"> Number of fine-tuning jobs to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual AsyncCollectionResult GetJobsAsync(string after, int? limit, RequestOptions options)
    {
        return new AsyncFineTuningJobCollectionResult(this, _pipeline, options, limit, after);
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] List your organization's fine-tuning jobs
    /// </summary>
    /// <param name="after"> Identifier for the last job from the previous pagination request. </param>
    /// <param name="limit"> Number of fine-tuning jobs to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual CollectionResult GetJobs(string after, int? limit, RequestOptions options)
    {
        return new FineTuningJobCollectionResult(this, _pipeline, options, limit, after);
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Get info about a fine-tuning job.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="fineTuningJobId"> The ID of the fine-tuning job. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fineTuningJobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fineTuningJobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetJobAsync(string fineTuningJobId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(fineTuningJobId, nameof(fineTuningJobId));

        using PipelineMessage message = CreateRetrieveFineTuningJobRequest(fineTuningJobId, options);
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
    /// <param name="fineTuningJobId"> The ID of the fine-tuning job. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="fineTuningJobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="fineTuningJobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetJob(string fineTuningJobId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(fineTuningJobId, nameof(fineTuningJobId));

        using PipelineMessage message = CreateRetrieveFineTuningJobRequest(fineTuningJobId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    internal virtual PipelineMessage CreateCreateFineTuningJobRequest(BinaryContent content, RequestOptions options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "POST";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/fine_tuning/jobs", false);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        request.Headers.Set("Content-Type", "application/json");
        request.Content = content;
        message.Apply(options);
        return message;
    }

    internal virtual PipelineMessage CreateGetPaginatedFineTuningJobsRequest(string after, int? limit, RequestOptions options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/fine_tuning/jobs", false);
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

    internal virtual PipelineMessage CreateRetrieveFineTuningJobRequest(string fineTuningJobId, RequestOptions options)
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
}
