using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.VectorStores;

namespace OpenAI.FineTuning;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Suppressed convenience methods for now.
/// <summary> The service client for OpenAI fine-tuning jobs. </summary>
[CodeGenType("FineTuning")]
[CodeGenSuppress("CancelFineTuningJob", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelFineTuningJobAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CreateFineTuningCheckpointPermission", typeof(string), typeof(IEnumerable<string>), typeof(CancellationToken))]
[CodeGenSuppress("CreateFineTuningCheckpointPermissionAsync", typeof(string), typeof(IEnumerable<string>), typeof(CancellationToken))]
[CodeGenSuppress("CreateFineTuningJob", typeof(FineTuningOptions), typeof(CancellationToken))]
[CodeGenSuppress("CreateFineTuningJobAsync", typeof(FineTuningOptions), typeof(CancellationToken))]
[CodeGenSuppress("CreateFineTuningJob", typeof(FineTuningOptions), typeof(CancellationToken))]
[CodeGenSuppress("GetPaginatedFineTuningJobsAsync", typeof(string), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("GetPaginatedFineTuningJobs", typeof(string), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("RetrieveFineTuningJobAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("RetrieveFineTuningJob", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelFineTuningJobAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelFineTuningJob", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetFineTuningEventsAsync", typeof(string), typeof(string), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("GetFineTuningEvents", typeof(string), typeof(string), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("GetFineTuningJobCheckpointsAsync", typeof(string), typeof(string), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("GetFineTuningJobCheckpoints", typeof(string), typeof(string), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("DeleteFineTuningCheckpointPermission", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("DeleteFineTuningCheckpointPermissionAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("FineTuningClient", typeof(ClientPipeline), typeof(Uri))]
[CodeGenSuppress("PauseFineTuningJob", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("PauseFineTuningJobAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("ResumeFineTuningJob", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("ResumeFineTuningJobAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("RetrieveFineTuningJob", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("RetrieveFineTuningJobAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetFineTuningCheckpointPermissions", typeof(string), typeof(string), typeof(int?), typeof(VectorStoreCollectionOrder?), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetFineTuningCheckpointPermissionsAsync", typeof(string), typeof(string), typeof(int?), typeof(VectorStoreCollectionOrder?), typeof(string), typeof(CancellationToken))]

public partial class FineTuningClient
{
    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="FineTuningClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public FineTuningClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="FineTuningClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public FineTuningClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FineTuningClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public FineTuningClient(ApiKeyCredential credential, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(credential, nameof(credential));
        options ??= new OpenAIClientOptions();

        Pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="FineTuningClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal FineTuningClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    protected internal FineTuningClient(ClientPipeline pipeline, Uri endpoint)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        Argument.AssertNotNull(endpoint, nameof(endpoint));

        Pipeline = pipeline;
        _endpoint = endpoint;
    }

    /// <summary> Creates a job with a training file and base model. </summary>
    /// <param name="baseModel"> The original model to use as a starting base to fine-tune. String such as "gpt-3.5-turbo" </param>
    /// <param name="trainingFileId"> The training file Id that is already uploaded. String should match pattern '^file-[a-zA-Z0-9]{24}$'. </param>
    /// <param name="waitUntilCompleted"> Whether to wait for the job to complete before returning. </param>
    /// <param name="options"> Additional options (<see cref="FineTuningOptions"/>) to customize the request. </param>
    /// <returns>A <see cref="ClientResult{FineTuningJob}"/> containing the newly started fine-tuning job.</returns>
    /// <param name="cancellationToken"> The cancellation token. </param>
    public virtual FineTuningJob FineTune(
        string baseModel,
        string trainingFileId,
        bool waitUntilCompleted, FineTuningOptions options = default, CancellationToken cancellationToken = default
        )
    {
        options ??= new FineTuningOptions();
        options.Model = baseModel;
        options.TrainingFile = trainingFileId;

        return FineTune(options.ToBinaryContent(), waitUntilCompleted, cancellationToken.ToRequestOptions());
    }

    /// <inheritdoc cref="FineTune(string, string, bool, FineTuningOptions, CancellationToken)"/>
    public virtual async Task<FineTuningJob> FineTuneAsync(
        string baseModel,
        string trainingFileId,
        bool waitUntilCompleted, FineTuningOptions options = default, CancellationToken cancellationToken = default
        )
    {

        options ??= new FineTuningOptions();
        options.Model = baseModel;
        options.TrainingFile = trainingFileId;

        return await FineTuneAsync(options.ToBinaryContent(), waitUntilCompleted, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
    }
    /// <summary>
    /// Get FineTuningJob for a previously started fine-tuning job.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="jobId"> The ID of the fine-tuning job. </param>
    /// <param name="cancellationToken"> The cancellation token. </param>
    public virtual FineTuningJob GetJob(string jobId, CancellationToken cancellationToken = default)
    {
        return FineTuningJob.Rehydrate(this, jobId, cancellationToken.ToRequestOptions());
    }

    /// <summary>
    /// Get FineTuningJob for a previously started fine-tuning job.
    ///
    /// [Learn more about fine-tuning](/docs/guides/fine-tuning)
    /// </summary>
    /// <param name="jobId"> The ID of the fine-tuning job. </param>
    /// <param name="cancellationToken"> The cancellation token. </param>
    public async virtual Task<FineTuningJob> GetJobAsync(string jobId, CancellationToken cancellationToken = default)
    {
        return await FineTuningJob.RehydrateAsync(this, jobId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a list of fine-tuning jobs.
    /// </summary>
    /// <param name="options"> Additional options: <see cref="FineTuningJobCollectionOptions"/> to customize the request. </param>
    /// <param name="cancellationToken"> The cancellation token. </param>
    /// <returns> A <see cref="CollectionResult{FineTuningJob}"/> containing the list of fine-tuning jobs. </returns>
    public virtual CollectionResult<FineTuningJob> GetJobs(FineTuningJobCollectionOptions options = default, CancellationToken cancellationToken = default)
    {
        options ??= new FineTuningJobCollectionOptions();
        return GetJobs(options.AfterJobId, options.PageSize, cancellationToken.ToRequestOptions()) as CollectionResult<FineTuningJob>;
    }

    /// <inheritdoc cref="GetJobs(FineTuningJobCollectionOptions, CancellationToken)"/>
    /// <returns> A <see cref="AsyncCollectionResult{FineTuningJob}"/> containing the list of fine-tuning jobs. </returns>
    public virtual AsyncCollectionResult<FineTuningJob> GetJobsAsync(
    FineTuningJobCollectionOptions options = default,
    CancellationToken cancellationToken = default)
    {
        options ??= new FineTuningJobCollectionOptions();
        AsyncCollectionResult jobs = GetJobsAsync(options.AfterJobId, options.PageSize, cancellationToken.ToRequestOptions());
        return (AsyncCollectionResult<FineTuningJob>)jobs;
    }

    internal virtual FineTuningJob CreateJobFromResponse(PipelineResponse response)
    {
        return new FineTuningJob(Pipeline, _endpoint, response);
    }

    internal virtual IEnumerable<FineTuningJob> CreateJobsFromPageResponse(PipelineResponse response)
    {
        InternalListPaginatedFineTuningJobsResponse jobs = ModelReaderWriter.Read<InternalListPaginatedFineTuningJobsResponse>(response.Content)!;
        return jobs.Data.Select(job => new FineTuningJob(Pipeline, _endpoint, job, response));
    }
}
