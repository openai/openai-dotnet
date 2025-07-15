using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.VectorStores;

[CodeGenSuppress("GetVectorStoresAsync", typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetVectorStores", typeof(int?), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetVectorStoreFilesAsync", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetVectorStoreFiles", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CreateVectorStoreFileAsync", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateVectorStoreFile", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateVectorStoreFileBatchAsync", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateVectorStoreFileBatch", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("GetVectorStoreFileBatchAsync", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetVectorStoreFileBatch", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetFilesInVectorStoreBatchAsync", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetFilesInVectorStoreBatch", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("ListFilesInVectorStoreBatchesAsync", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("ListFilesInVectorStoreBatches", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
public partial class VectorStoreClient
{
    /// <summary>
    /// [Protocol Method] Returns a paginated collection of vector-stores.
    /// </summary>
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
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    public virtual AsyncCollectionResult GetVectorStoresAsync(int? limit, string order, string after, string before, RequestOptions options)
    {
        return new AsyncVectorStoreCollectionResult(this, Pipeline, options, limit, order, after, before);
    }

    /// <summary>
    /// [Protocol Method] Returns a paginated collection of vector-stores.
    /// </summary>
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
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    public virtual CollectionResult GetVectorStores(int? limit, string order, string after, string before, RequestOptions options)
    {
        return new VectorStoreCollectionResult(this, Pipeline, options, limit, order, after, before);
    }

    /// <summary>
    /// [Protocol Method] Creates a vector store.
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="CreateVectorStoreOperation"/> that can be used to wait for 
    /// the vector store creation to complete. </returns>
    public virtual async Task<CreateVectorStoreOperation> CreateVectorStoreAsync(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null)
    {
        using PipelineMessage message = CreateCreateVectorStoreRequest(content, options);
        PipelineResponse response = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);
        using JsonDocument document = JsonDocument.Parse(response.Content);
        VectorStore value = VectorStore.DeserializeVectorStore(document.RootElement, ModelSerializationExtensions.WireOptions);

        CreateVectorStoreOperation operation = this.CreateCreateVectorStoreOperation(ClientResult.FromValue(value, response));
        return await operation.WaitUntilAsync(waitUntilCompleted, options).ConfigureAwait(false);
    }

    /// <summary>
    /// [Protocol Method] Creates a vector store.
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="CreateVectorStoreOperation"/> that can be used to wait for 
    /// the vector store creation to complete. </returns>
    public virtual CreateVectorStoreOperation CreateVectorStore(BinaryContent content, bool waitUntilCompleted, RequestOptions options = null)
    {
        using PipelineMessage message = CreateCreateVectorStoreRequest(content, options);
        PipelineResponse response = Pipeline.ProcessMessage(message, options);
        using JsonDocument document = JsonDocument.Parse(response.Content);
        VectorStore value = VectorStore.DeserializeVectorStore(document.RootElement, ModelSerializationExtensions.WireOptions);

        CreateVectorStoreOperation operation = this.CreateCreateVectorStoreOperation(ClientResult.FromValue(value, response));
        return operation.WaitUntil(waitUntilCompleted, options);
    }

    /// <summary>
    /// [Protocol Method] Retrieves a vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    internal virtual async Task<ClientResult> GetVectorStoreAsync(string vectorStoreId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        using PipelineMessage message = CreateGetVectorStoreRequest(vectorStoreId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    internal virtual ClientResult GetVectorStore(string vectorStoreId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        using PipelineMessage message = CreateGetVectorStoreRequest(vectorStoreId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Returns a paginated collection of vector store files.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store that the files belong to. </param>
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
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    public virtual AsyncCollectionResult GetFileAssociationsAsync(string vectorStoreId, int? limit, string order, string after, string before, string filter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        return new AsyncVectorStoreFileCollectionResult(this, Pipeline, options, vectorStoreId, limit, order, after, before, filter);
    }

    /// <summary>
    /// [Protocol Method] Returns a paginated collection of vector store files.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store that the files belong to. </param>
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
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A collection of service responses, each holding a page of values. </returns>
    public virtual CollectionResult GetFileAssociations(string vectorStoreId, int? limit, string order, string after, string before, string filter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        return new VectorStoreFileCollectionResult(this, Pipeline, options, vectorStoreId, limit, order, after, before, filter);
    }


    /// <summary>
    /// [Protocol Method] Create a vector store file by attaching a [File](/docs/api-reference/files) to a [vector store](/docs/api-reference/vector-stores/object).
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStoreId"> The ID of the vector store for which to create a File. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="AddFileToVectorStoreOperation"/> that can be used to wait for 
    /// the vector store file addition to complete. </returns>
    public virtual async Task<AddFileToVectorStoreOperation> AddFileToVectorStoreAsync(string vectorStoreId, BinaryContent content, bool waitUntilCompleted, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateVectorStoreFileRequest(vectorStoreId, content, options);
        PipelineResponse response = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);
        using JsonDocument document = JsonDocument.Parse(response.Content);
        VectorStoreFileAssociation value = VectorStoreFileAssociation.DeserializeVectorStoreFileAssociation(document.RootElement, ModelSerializationExtensions.WireOptions);

        AddFileToVectorStoreOperation operation = this.CreateAddFileToVectorStoreOperation(ClientResult.FromValue(value, response));
        return await operation.WaitUntilAsync(waitUntilCompleted, options).ConfigureAwait(false);
    }

    /// <summary>
    /// [Protocol Method] Create a vector store file by attaching a [File](/docs/api-reference/files) to a [vector store](/docs/api-reference/vector-stores/object).
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStoreId"> The ID of the vector store for which to create a File. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="AddFileToVectorStoreOperation"/> that can be used to wait for 
    /// the vector store file addition to complete. </returns>
    public virtual AddFileToVectorStoreOperation AddFileToVectorStore(string vectorStoreId, BinaryContent content, bool waitUntilCompleted, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateVectorStoreFileRequest(vectorStoreId, content, options);
        PipelineResponse response = Pipeline.ProcessMessage(message, options);
        using JsonDocument document = JsonDocument.Parse(response.Content);
        VectorStoreFileAssociation value = VectorStoreFileAssociation.DeserializeVectorStoreFileAssociation(document.RootElement, ModelSerializationExtensions.WireOptions);

        AddFileToVectorStoreOperation operation = this.CreateAddFileToVectorStoreOperation(ClientResult.FromValue(value, response));
        return operation.WaitUntil(waitUntilCompleted, options);
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
    public virtual AsyncCollectionResult GetFileAssociationsAsync(string vectorStoreId, string batchId, int? limit, string order, string after, string before, string filter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        return new AsyncVectorStoreFileBatchCollectionResult(this, Pipeline, options, vectorStoreId, batchId, limit, order, after, before, filter);
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
    public virtual CollectionResult GetFileAssociations(string vectorStoreId, string batchId, int? limit, string order, string after, string before, string filter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        return new VectorStoreFileBatchCollectionResult(this, Pipeline, options, vectorStoreId, batchId, limit, order, after, before, filter);
    }

    /// <summary>
    /// [Protocol Method] Create a vector store file batch.
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStoreId"> The ID of the vector store for which to create a file batch. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="CreateBatchFileJobOperation"/> that can be used to wait for 
    /// the operation to complete, get information about the batch file job, or cancel the operation. </returns>
    public virtual async Task<CreateBatchFileJobOperation> CreateBatchFileJobAsync(
        string vectorStoreId,
        BinaryContent content,
        bool waitUntilCompleted,
        RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateVectorStoreFileBatchRequest(vectorStoreId, content, options);
        PipelineResponse response = await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);
        using JsonDocument document = JsonDocument.Parse(response.Content);
        VectorStoreBatchFileJob job = VectorStoreBatchFileJob.DeserializeVectorStoreBatchFileJob(document.RootElement, ModelSerializationExtensions.WireOptions);

        CreateBatchFileJobOperation operation = this.CreateBatchFileJobOperation(ClientResult.FromValue(job, response));
        return await operation.WaitUntilAsync(waitUntilCompleted, options).ConfigureAwait(false);
    }

    /// <summary>
    /// [Protocol Method] Create a vector store file batch.
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStoreId"> The ID of the vector store for which to create a file batch. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="CreateBatchFileJobOperation"/> that can be used to wait for 
    /// the operation to complete, get information about the batch file job, or cancel the operation. </returns>
    public virtual CreateBatchFileJobOperation CreateBatchFileJob(
        string vectorStoreId,
        BinaryContent content,
        bool waitUntilCompleted,
        RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateVectorStoreFileBatchRequest(vectorStoreId, content, options);
        PipelineResponse response = Pipeline.ProcessMessage(message, options);
        using JsonDocument document = JsonDocument.Parse(response.Content);
        VectorStoreBatchFileJob job = VectorStoreBatchFileJob.DeserializeVectorStoreBatchFileJob(document.RootElement, ModelSerializationExtensions.WireOptions);

        CreateBatchFileJobOperation operation = this.CreateBatchFileJobOperation(ClientResult.FromValue(job, response));
        return operation.WaitUntil(waitUntilCompleted, options);
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
    internal virtual async Task<ClientResult> GetBatchFileJobAsync(string vectorStoreId, string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateGetVectorStoreFileBatchRequest(vectorStoreId, batchId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
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
    internal virtual ClientResult GetBatchFileJob(string vectorStoreId, string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateGetVectorStoreFileBatchRequest(vectorStoreId, batchId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }
}
