using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Suppressed convenience methods for now.
/// <summary> The service client for OpenAI fine-tuning operations. </summary>
[Experimental("OPENAI001")]
[CodeGenClient("FineTuning")]
[CodeGenSuppress("FineTuningClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateFineTuningJobAsync", typeof(FineTuningOptions))]
[CodeGenSuppress("CreateFineTuningJob", typeof(FineTuningOptions))]
[CodeGenSuppress("GetPaginatedFineTuningJobsAsync", typeof(string), typeof(int?))]
[CodeGenSuppress("GetPaginatedFineTuningJobs", typeof(string), typeof(int?))]
[CodeGenSuppress("RetrieveFineTuningJobAsync", typeof(string))]
[CodeGenSuppress("RetrieveFineTuningJob", typeof(string))]
[CodeGenSuppress("CancelFineTuningJobAsync", typeof(string))]
[CodeGenSuppress("CancelFineTuningJob", typeof(string))]
[CodeGenSuppress("GetFineTuningEventsAsync", typeof(string), typeof(string), typeof(int?))]
[CodeGenSuppress("GetFineTuningEvents", typeof(string), typeof(string), typeof(int?))]
[CodeGenSuppress("GetFineTuningJobCheckpointsAsync", typeof(string), typeof(string), typeof(int?))]
[CodeGenSuppress("GetFineTuningJobCheckpoints", typeof(string), typeof(string), typeof(int?))]
public partial class FineTuningClient
{
    // CUSTOM: Remove virtual keyword.
    /// <summary>
    /// The HTTP pipeline for sending and receiving REST requests and responses.
    /// </summary>
    public ClientPipeline Pipeline => _pipeline;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="FineTuningClient">. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public FineTuningClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="FineTuningClient">. </summary>
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

        _pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="FineTuningClient">. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal FineTuningClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        _pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    internal virtual FineTuningJobOperation CreateCreateJobOperation(string jobId, string status, PipelineResponse response)
    {
        return new FineTuningJobOperation(_pipeline, _endpoint, jobId, status, response);
    }
}
