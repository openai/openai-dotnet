using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
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
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CreateJobAsync(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateFineTuningJobRequest(content, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
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
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CreateJob(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateFineTuningJobRequest(content, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
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
    public virtual IAsyncEnumerable<ClientResult> GetJobsAsync(string after, int? limit, RequestOptions options)
    {
        FineTuningJobsPageEnumerator enumerator = new FineTuningJobsPageEnumerator(_pipeline, _endpoint, after, limit, options);
        return PageCollectionHelpers.CreateAsync(enumerator);
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
    public virtual IEnumerable<ClientResult> GetJobs(string after, int? limit, RequestOptions options)
    {
        FineTuningJobsPageEnumerator enumerator = new FineTuningJobsPageEnumerator(_pipeline, _endpoint, after, limit, options);
        return PageCollectionHelpers.Create(enumerator);
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
    public virtual async Task<ClientResult> GetJobAsync(string jobId, RequestOptions options)
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
    public virtual ClientResult GetJob(string jobId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(jobId, nameof(jobId));

        using PipelineMessage message = CreateRetrieveFineTuningJobRequest(jobId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Immediately cancel a fine-tune job.
    /// </summary>
    /// <param name="jobId"> The ID of the fine-tuning job to cancel. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="jobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="jobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CancelJobAsync(string jobId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(jobId, nameof(jobId));

        using PipelineMessage message = CreateCancelFineTuningJobRequest(jobId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Immediately cancel a fine-tune job.
    /// </summary>
    /// <param name="jobId"> The ID of the fine-tuning job to cancel. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="jobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="jobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CancelJob(string jobId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(jobId, nameof(jobId));

        using PipelineMessage message = CreateCancelFineTuningJobRequest(jobId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Get status updates for a fine-tuning job.
    /// </summary>
    /// <param name="jobId"> The ID of the fine-tuning job to get events for. </param>
    /// <param name="after"> Identifier for the last event from the previous pagination request. </param>
    /// <param name="limit"> Number of events to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="jobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="jobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual IAsyncEnumerable<ClientResult> GetJobEventsAsync(string jobId, string after, int? limit, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(jobId, nameof(jobId));

        FineTuningJobEventsPageEnumerator enumerator = new FineTuningJobEventsPageEnumerator(_pipeline, _endpoint, jobId, after, limit, options);
        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] Get status updates for a fine-tuning job.
    /// </summary>
    /// <param name="jobId"> The ID of the fine-tuning job to get events for. </param>
    /// <param name="after"> Identifier for the last event from the previous pagination request. </param>
    /// <param name="limit"> Number of events to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="jobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="jobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual IEnumerable<ClientResult> GetJobEvents(string jobId, string after, int? limit, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(jobId, nameof(jobId));

        FineTuningJobEventsPageEnumerator enumerator = new FineTuningJobEventsPageEnumerator(_pipeline, _endpoint, jobId, after, limit, options);
        return PageCollectionHelpers.Create(enumerator);
    }

    /// <summary>
    /// [Protocol Method] List the checkpoints for a fine-tuning job.
    /// </summary>
    /// <param name="jobId"> The ID of the fine-tuning job to get checkpoints for. </param>
    /// <param name="after"> Identifier for the last checkpoint ID from the previous pagination request. </param>
    /// <param name="limit"> Number of checkpoints to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="jobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="jobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual IAsyncEnumerable<ClientResult> GetJobCheckpointsAsync(string jobId, string after, int? limit, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(jobId, nameof(jobId));

        FineTuningJobCheckpointsPageEnumerator enumerator = new FineTuningJobCheckpointsPageEnumerator(_pipeline, _endpoint, jobId, after, limit, options);
        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    /// <summary>
    /// [Protocol Method] List the checkpoints for a fine-tuning job.
    /// </summary>
    /// <param name="jobId"> The ID of the fine-tuning job to get checkpoints for. </param>
    /// <param name="after"> Identifier for the last checkpoint ID from the previous pagination request. </param>
    /// <param name="limit"> Number of checkpoints to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="jobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="jobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual IEnumerable<ClientResult> GetJobCheckpoints(string jobId, string after, int? limit, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(jobId, nameof(jobId));

        FineTuningJobCheckpointsPageEnumerator enumerator = new FineTuningJobCheckpointsPageEnumerator(_pipeline, _endpoint, jobId, after, limit, options);
        return PageCollectionHelpers.Create(enumerator);
    }
}
