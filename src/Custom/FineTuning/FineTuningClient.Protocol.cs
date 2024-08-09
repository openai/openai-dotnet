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
    /// <returns> A <see cref="CreateJobOperation"/> that can be used to wait for 
    /// the operation to complete, get information about the fine tuning job, or 
    /// cancel the operation. </returns>
    public virtual async Task<CreateJobOperation> CreateJobAsync(
        bool waitUntilCompleted,
        BinaryContent content,
        RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateFineTuningJobRequest(content, options);
        PipelineResponse response = await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string jobId = doc.RootElement.GetProperty("id"u8).GetString();
        string status = doc.RootElement.GetProperty("status"u8).GetString();

        CreateJobOperation operation = new(_pipeline, _endpoint, jobId, status, response);
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
    /// <returns> A <see cref="CreateJobOperation"/> that can be used to wait for 
    /// the operation to complete, get information about the fine tuning job, or 
    /// cancel the operation. </returns>
    public virtual CreateJobOperation CreateJob(
        bool waitUntilCompleted,
        BinaryContent content,
        RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateFineTuningJobRequest(content, options);
        PipelineResponse response = _pipeline.ProcessMessage(message, options);

        using JsonDocument doc = JsonDocument.Parse(response.Content);
        string jobId = doc.RootElement.GetProperty("id"u8).GetString();
        string status = doc.RootElement.GetProperty("status"u8).GetString();

        CreateJobOperation operation = new(_pipeline, _endpoint, jobId, status, response);
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
    public virtual async Task<ClientResult> GetJobsAsync(string after, int? limit, RequestOptions options)
    {
        using PipelineMessage message = CreateGetPaginatedFineTuningJobsRequest(after, limit, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
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
    public virtual ClientResult GetJobs(string after, int? limit, RequestOptions options)
    {
        using PipelineMessage message = CreateGetPaginatedFineTuningJobsRequest(after, limit, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Get info about a fine-tuning job.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="jobId"> The ID of the fine-tuning job. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="jobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="jobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    internal virtual async Task<ClientResult> GetJobAsync(string jobId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(jobId, nameof(jobId));

        using PipelineMessage message = CreateRetrieveFineTuningJobRequest(jobId, options);
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
    /// <param name="jobId"> The ID of the fine-tuning job. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="jobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="jobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    internal virtual ClientResult GetJob(string jobId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(jobId, nameof(jobId));

        using PipelineMessage message = CreateRetrieveFineTuningJobRequest(jobId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }
}
