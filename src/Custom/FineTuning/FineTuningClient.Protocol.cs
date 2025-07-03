using System;
using System.ClientModel;
using System.ClientModel.Primitives;
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
    /// <summary>
    /// [Protocol Method] Creates a fine-tuning job which begins the process of creating a new model from a given dataset.
    ///
    /// Response includes details of the enqueued job including job status and the name of the fine-tuned models once complete.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the LRO has been started and is still running
    /// on the service, or wait until the job has completed to return.
    /// </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="FineTuningJob"/> that can be used to wait for 
    /// the LRO to complete, get information about the fine tuning job, or 
    /// cancel the job. </returns>
    public virtual async Task<FineTuningJob> FineTuneAsync(
        BinaryContent content,
        bool waitUntilCompleted,
        RequestOptions options)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = PostJobPipelineMessage(content, options);
        PipelineResponse response = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);

        FineTuningJob job = this.CreateJobFromResponse(response);
        return await job.WaitUntilAsync(waitUntilCompleted, options).ConfigureAwait(false);
    }

    /// <inheritdoc cref="FineTuneAsync(BinaryContent, bool, RequestOptions)"/>
    public virtual FineTuningJob FineTune(
        BinaryContent content,
        bool waitUntilCompleted,
        RequestOptions options)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = PostJobPipelineMessage(content, options);
        PipelineResponse response = Pipeline.ProcessMessage(message, options);

        FineTuningJob job = this.CreateJobFromResponse(response);
        return job.WaitUntil(waitUntilCompleted, options);
    }

    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] List all of your organization's fine-tuning jobs
    /// </summary>
    /// <param name="afterJobId"> Identifier for the last job from the previous pagination request. </param>
    /// <param name="pageSize"> Number of fine-tuning jobs to retrieve at a time. Collection will iterate until _all_ jobs are fetched. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    internal virtual AsyncCollectionResult GetJobsAsync(string afterJobId, int? pageSize, RequestOptions options)
    {
        return new AsyncFineTuningJobCollectionResult(this, Pipeline, options, pageSize, afterJobId);
    }


    // CUSTOM:
    // - Renamed.
    // - Edited doc comment.
    /// <summary>
    /// [Protocol Method] List all of your your organization's fine-tuning jobs
    /// </summary>
    /// <param name="after"> Identifier for the last job from the previous pagination request. </param>
    /// <param name="pageSize"> Number of fine-tuning jobs to retrieve at a time. Collection will iterate until _all_ jobs are fetched. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    internal virtual CollectionResult GetJobs(string after, int? pageSize, RequestOptions options)
    {
        return new FineTuningJobCollectionResult(this, Pipeline, options, pageSize, after);
    }

    /// <summary>
    /// [Protocol Method] Get info about a fine-tuning job.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="JobId"> The ID of the fine-tuning job. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="JobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="JobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    internal async Task<ClientResult> GetJobAsync(string JobId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(JobId, nameof(JobId));

        using PipelineMessage message = GetJobPipelineMessage(Pipeline, _endpoint, JobId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Get info about a fine-tuning job.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="JobId"> The ID of the fine-tuning job. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="JobId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="JobId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    internal ClientResult GetJob(string JobId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(JobId, nameof(JobId));

        using PipelineMessage message = GetJobPipelineMessage(Pipeline, _endpoint, JobId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    internal virtual PipelineMessage PostJobPipelineMessage(BinaryContent content, RequestOptions options)
    {
        var message = Pipeline.CreateMessage();
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

    internal virtual PipelineMessage GetJobsPipelineMessage(string after, int? limit, RequestOptions options)
    {
        var message = Pipeline.CreateMessage();
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

    internal static PipelineMessage GetJobPipelineMessage(ClientPipeline clientPipeline, Uri endpoint, string JobId, RequestOptions options)
    {
        // This is referenced by client.GetJobAsync and client.GetJob, and job.GetJobAsync and job.GetJob.
        // It is static so that FineTuningJob can use it as well.
        var message = clientPipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(endpoint);
        uri.AppendPath("/fine_tuning/jobs/", false);
        uri.AppendPath(JobId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }
}
