using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace OpenAI.Batch;

// CUSTOM:
// - Renamed.
// - Suppressed constructor that takes endpoint parameter; endpoint is now a property in the options class.
// - Suppressed convenience methods for now.
/// <summary> The service client for OpenAI batch operations. </summary>
[CodeGenType("Batches")]
[CodeGenSuppress("BatchClient", typeof(ClientPipeline), typeof(Uri))]
[CodeGenSuppress("CreateBatch", typeof(string), typeof(InternalCreateBatchRequestEndpoint), typeof(string), typeof(IDictionary<string, string>), typeof(CancellationToken))]
[CodeGenSuppress("CreateBatchAsync", typeof(string), typeof(InternalCreateBatchRequestEndpoint), typeof(string), typeof(IDictionary<string, string>), typeof(CancellationToken))]
[CodeGenSuppress("CreateBatch", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateBatchAsync", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("GetBatch", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetBatchAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelBatch", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelBatchAsync", typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelBatch", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CancelBatchAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetBatches", typeof(string), typeof(int?), typeof(CancellationToken))]
[CodeGenSuppress("GetBatchesAsync", typeof(string), typeof(int?), typeof(CancellationToken))]
public partial class BatchClient
{
    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="BatchClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public BatchClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="BatchClient"/>. </summary>
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

        Pipeline = OpenAIClient.CreatePipeline(credential, options);
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    // - Made protected.
    /// <summary> Initializes a new instance of <see cref="BatchClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal BatchClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    internal virtual CreateBatchOperation CreateCreateBatchOperation(string batchId, string status, PipelineResponse response)
    {
        return new CreateBatchOperation(this, _endpoint, batchId, status, response);
    }
}
