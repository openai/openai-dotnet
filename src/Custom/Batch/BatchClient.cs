using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Batch;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Suppressed convenience methods for now.
/// <summary> The service client for OpenAI batch operations. </summary>
[Experimental("OPENAI001")]
[CodeGenClient("Batches")]
[CodeGenSuppress("BatchClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateBatch", typeof(string), typeof(InternalCreateBatchRequestEndpoint), typeof(InternalBatchCompletionTimeframe), typeof(IReadOnlyDictionary<string, string>))]
[CodeGenSuppress("CreateBatchAsync", typeof(string), typeof(InternalCreateBatchRequestEndpoint), typeof(InternalBatchCompletionTimeframe), typeof(IReadOnlyDictionary<string, string>))]
[CodeGenSuppress("CreateBatch", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateBatchAsync", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("RetrieveBatch", typeof(string))]
[CodeGenSuppress("RetrieveBatchAsync", typeof(string))]
[CodeGenSuppress("CancelBatch", typeof(string))]
[CodeGenSuppress("CancelBatchAsync", typeof(string))]
[CodeGenSuppress("CancelBatch", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CancelBatchAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetBatches", typeof(string), typeof(int?))]
[CodeGenSuppress("GetBatchesAsync", typeof(string), typeof(int?))]
public partial class BatchClient
{
    // CUSTOM: Remove virtual keyword.
    /// <summary>
    /// The HTTP pipeline for sending and receiving REST requests and responses.
    /// </summary>
    public ClientPipeline Pipeline => _pipeline;

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="BatchClient">. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public BatchClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="BatchClient">. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public BatchClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BatchClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public BatchClient(ApiKeyCredential credential, OpenAIClientOptions options)
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
    /// <summary> Initializes a new instance of <see cref="BatchClient">. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal BatchClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        _pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    internal virtual CreateBatchOperation CreateCreateBatchOperation(string batchId, string status, PipelineResponse response)
    {
        return new CreateBatchOperation(_pipeline, _endpoint, batchId, status, response);
    }
}
