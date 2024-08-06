using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.VectorStores;

/// <summary>
/// Long-running operation for creating a vector store file batch.
/// </summary>
public partial class VectorStoreFileBatchOperation : OperationResult
{
    private readonly ClientPipeline _pipeline;
    private readonly Uri _endpoint;
    
    private readonly string _vectorStoreId;
    private readonly string _batchId;
    
    private PollingInterval? _pollingInterval;

    /// <inheritdoc/>
    public override ContinuationToken? RehydrationToken { get; protected set; }

    /// <inheritdoc/>
    public override bool IsCompleted { get; protected set; }

    // Generated protocol methods

    /// <summary>
    /// [Protocol Method] Retrieves a vector store file batch.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store that the file batch belongs to. </param>
    /// <param name="batchId"> The ID of the file batch being retrieved. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> or <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> GetBatchFileJobAsync(string vectorStoreId, string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateGetVectorStoreFileBatchRequest(vectorStoreId, batchId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a vector store file batch.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store that the file batch belongs to. </param>
    /// <param name="batchId"> The ID of the file batch being retrieved. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> or <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult GetBatchFileJob(string vectorStoreId, string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateGetVectorStoreFileBatchRequest(vectorStoreId, batchId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Cancel a vector store file batch. This attempts to cancel the processing of files in this batch as soon as possible.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store that the file batch belongs to. </param>
    /// <param name="batchId"> The ID of the file batch to cancel. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> or <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> CancelBatchFileJobAsync(string vectorStoreId, string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateCancelVectorStoreFileBatchRequest(vectorStoreId, batchId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Cancel a vector store file batch. This attempts to cancel the processing of files in this batch as soon as possible.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store that the file batch belongs to. </param>
    /// <param name="batchId"> The ID of the file batch to cancel. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> or <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult CancelBatchFileJob(string vectorStoreId, string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateCancelVectorStoreFileBatchRequest(vectorStoreId, batchId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Returns a paginated collection of vector store files in a batch.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store that the file batch belongs to. </param>
    /// <param name="batchId"> The ID of the file batch that the files belong to. </param>
    /// <param name="limit">
    /// A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
    /// default is 20.
    /// </param>
    /// <param name="order">
    /// Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
    /// for descending order. Allowed values: "asc" | "desc"
    /// </param>
    /// <param name="after">
    /// A cursor for use in pagination. `after` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include after=obj_foo in order to fetch the next page of the list.
    /// </param>
    /// <param name="before">
    /// A cursor for use in pagination. `before` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include before=obj_foo in order to fetch the previous page of the list.
    /// </param>
    /// <param name="filter"> Filter by file status. One of `in_progress`, `completed`, `failed`, `cancelled`. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> or <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual IAsyncEnumerable<ClientResult> GetFileAssociationsAsync(string vectorStoreId, string batchId, int? limit, string order, string after, string before, string filter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        VectorStoreFileBatchesPageEnumerator enumerator = new VectorStoreFileBatchesPageEnumerator(_pipeline, _endpoint, vectorStoreId, batchId, limit, order, after, before, filter, options);
        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    /// <summary>
    /// [Protocol Method] Returns a paginated collection of vector store files in a batch.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store that the file batch belongs to. </param>
    /// <param name="batchId"> The ID of the file batch that the files belong to. </param>
    /// <param name="limit">
    /// A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
    /// default is 20.
    /// </param>
    /// <param name="order">
    /// Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
    /// for descending order. Allowed values: "asc" | "desc"
    /// </param>
    /// <param name="after">
    /// A cursor for use in pagination. `after` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include after=obj_foo in order to fetch the next page of the list.
    /// </param>
    /// <param name="before">
    /// A cursor for use in pagination. `before` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include before=obj_foo in order to fetch the previous page of the list.
    /// </param>
    /// <param name="filter"> Filter by file status. One of `in_progress`, `completed`, `failed`, `cancelled`. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="batchId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> or <paramref name="batchId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual IEnumerable<ClientResult> GetFileAssociations(string vectorStoreId, string batchId, int? limit, string order, string after, string before, string filter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        VectorStoreFileBatchesPageEnumerator enumerator = new VectorStoreFileBatchesPageEnumerator(_pipeline, _endpoint, vectorStoreId, batchId, limit, order, after, before, filter, options);
        return PageCollectionHelpers.Create(enumerator);
    }

    internal PipelineMessage CreateGetVectorStoreFileBatchRequest(string vectorStoreId, string batchId, RequestOptions options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/vector_stores/", false);
        uri.AppendPath(vectorStoreId, true);
        uri.AppendPath("/file_batches/", false);
        uri.AppendPath(batchId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateCancelVectorStoreFileBatchRequest(string vectorStoreId, string batchId, RequestOptions options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "POST";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/vector_stores/", false);
        uri.AppendPath(vectorStoreId, true);
        uri.AppendPath("/file_batches/", false);
        uri.AppendPath(batchId, true);
        uri.AppendPath("/cancel", false);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateGetFilesInVectorStoreBatchesRequest(string vectorStoreId, string batchId, int? limit, string order, string after, string before, string filter, RequestOptions options)
    {
        var message = _pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/vector_stores/", false);
        uri.AppendPath(vectorStoreId, true);
        uri.AppendPath("/file_batches/", false);
        uri.AppendPath(batchId, true);
        uri.AppendPath("/files", false);
        if (limit != null)
        {
            uri.AppendQuery("limit", limit.Value, true);
        }
        if (order != null)
        {
            uri.AppendQuery("order", order, true);
        }
        if (after != null)
        {
            uri.AppendQuery("after", after, true);
        }
        if (before != null)
        {
            uri.AppendQuery("before", before, true);
        }
        if (filter != null)
        {
            uri.AppendQuery("filter", filter, true);
        }
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}