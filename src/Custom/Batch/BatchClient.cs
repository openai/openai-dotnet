using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Batch;

[CodeGenClient("Batches")]
[CodeGenSuppress("BatchClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateBatch", typeof(string), typeof(InternalBatchOperationEndpoint), typeof(InternalBatchCompletionTimeframe), typeof(IDictionary<string, string>))]
[CodeGenSuppress("CreateBatchAsync", typeof(string), typeof(InternalBatchOperationEndpoint), typeof(InternalBatchCompletionTimeframe), typeof(IDictionary<string, string>))]
[CodeGenSuppress("RetrieveBatch", typeof(string))]
[CodeGenSuppress("RetrieveBatchAsync", typeof(string))]
[CodeGenSuppress("RetrieveBatch", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("RetrieveBatchAsync", typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CancelBatch", typeof(string))]
[CodeGenSuppress("CancelBatchAsync", typeof(string))]
[CodeGenSuppress("GetBatches", typeof(string), typeof(int?))]
[CodeGenSuppress("GetBatchesAsync", typeof(string), typeof(int?))]
public partial class BatchClient
{
    /// <summary>
    /// Initializes a new instance of <see cref="BatchClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public BatchClient(ApiKeyCredential credential, OpenAIClientOptions options = null)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(credential, requireExplicitCredential: true), options),
              OpenAIClient.GetEndpoint(options),
              options) 
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="BatchClient"/> that will use an API key from the OPENAI_API_KEY
    /// environment variable when authenticating.
    /// </summary>
    /// <remarks>
    /// To provide an explicit credential instead of using the environment variable, use an alternate constructor like
    /// <see cref="BatchClient(ApiKeyCredential,OpenAIClientOptions)"/>.
    /// </remarks>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="InvalidOperationException"> The OPENAI_API_KEY environment variable was not found. </exception>
    public BatchClient(OpenAIClientOptions options = null)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(), options),
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="BatchClient"/>.
    /// </summary>
    /// <param name="pipeline"> The client pipeline to use. </param>
    /// <param name="endpoint"> The endpoint to use. </param>
    protected internal BatchClient(ClientPipeline pipeline, Uri endpoint, OpenAIClientOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
    }
}
