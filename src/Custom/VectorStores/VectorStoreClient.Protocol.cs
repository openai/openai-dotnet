using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OpenAI.VectorStores;

[CodeGenSuppress("GetVectorStoreFilesAsync", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetVectorStoreFiles", typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CreateVectorStoreFileAsync", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateVectorStoreFile", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("GetVectorStoreFileAsync", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetVectorStoreFile", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("DeleteVectorStoreFileAsync", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("DeleteVectorStoreFile", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CreateVectorStoreFileBatchAsync", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateVectorStoreFileBatch", typeof(string), typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("GetVectorStoreFileBatchAsync", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetVectorStoreFileBatch", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CancelVectorStoreFileBatchAsync", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("CancelVectorStoreFileBatch", typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetFilesInVectorStoreBatchesAsync", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
[CodeGenSuppress("GetFilesInVectorStoreBatches", typeof(string), typeof(string), typeof(int?), typeof(string), typeof(string), typeof(string), typeof(string), typeof(RequestOptions))]
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual IAsyncEnumerable<ClientResult> GetVectorStoresAsync(int? limit, string order, string after, string before, RequestOptions options)
    {
        VectorStoresPageEnumerator enumerator = new VectorStoresPageEnumerator(_pipeline, _endpoint, limit, order, after, before, options);
        return PageCollectionHelpers.CreateAsync(enumerator);
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual IEnumerable<ClientResult> GetVectorStores(int? limit, string order, string after, string before, RequestOptions options)
    {
        VectorStoresPageEnumerator enumerator = new VectorStoresPageEnumerator(_pipeline, _endpoint, limit, order, after, before, options);
        return PageCollectionHelpers.Create(enumerator);
    }

    /// <summary>
    /// [Protocol Method] Creates a vector store.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CreateVectorStoreAsync(BinaryContent content, RequestOptions options = null)
    {
        using PipelineMessage message = CreateCreateVectorStoreRequest(content, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Creates a vector store.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult CreateVectorStore(BinaryContent content, RequestOptions options = null)
    {
        using PipelineMessage message = CreateCreateVectorStoreRequest(content, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> GetVectorStoreAsync(string vectorStoreId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        using PipelineMessage message = CreateGetVectorStoreRequest(vectorStoreId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult GetVectorStore(string vectorStoreId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        using PipelineMessage message = CreateGetVectorStoreRequest(vectorStoreId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Modifies a vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to modify. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> ModifyVectorStoreAsync(string vectorStoreId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyVectorStoreRequest(vectorStoreId, content, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Modifies a vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to modify. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult ModifyVectorStore(string vectorStoreId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyVectorStoreRequest(vectorStoreId, content, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Delete a vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> DeleteVectorStoreAsync(string vectorStoreId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        using PipelineMessage message = CreateDeleteVectorStoreRequest(vectorStoreId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Delete a vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult DeleteVectorStore(string vectorStoreId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        using PipelineMessage message = CreateDeleteVectorStoreRequest(vectorStoreId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual IAsyncEnumerable<ClientResult> GetFileAssociationsAsync(string vectorStoreId, int? limit, string order, string after, string before, string filter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        VectorStoreFilesPageEnumerator enumerator = new VectorStoreFilesPageEnumerator(_pipeline, _endpoint, vectorStoreId, limit, order, after, before, filter, options);
        return PageCollectionHelpers.CreateAsync(enumerator);
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual IEnumerable<ClientResult> GetFileAssociations(string vectorStoreId, int? limit, string order, string after, string before, string filter, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        VectorStoreFilesPageEnumerator enumerator = new VectorStoreFilesPageEnumerator(_pipeline, _endpoint, vectorStoreId, limit, order, after, before, filter, options);
        return PageCollectionHelpers.Create(enumerator);
    }

    /// <summary>
    /// [Protocol Method] Create a vector store file by attaching a [File](/docs/api-reference/files) to a [vector store](/docs/api-reference/vector-stores/object).
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store for which to create a File. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> AddFileToVectorStoreAsync(string vectorStoreId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateVectorStoreFileRequest(vectorStoreId, content, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Create a vector store file by attaching a [File](/docs/api-reference/files) to a [vector store](/docs/api-reference/vector-stores/object).
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store for which to create a File. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult AddFileToVectorStore(string vectorStoreId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateVectorStoreFileRequest(vectorStoreId, content, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a vector store file.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store that the file belongs to. </param>
    /// <param name="fileId"> The ID of the file being retrieved. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> or <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> GetFileAssociationAsync(string vectorStoreId, string fileId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        using PipelineMessage message = CreateGetVectorStoreFileRequest(vectorStoreId, fileId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a vector store file.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store that the file belongs to. </param>
    /// <param name="fileId"> The ID of the file being retrieved. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> or <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult GetFileAssociation(string vectorStoreId, string fileId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        using PipelineMessage message = CreateGetVectorStoreFileRequest(vectorStoreId, fileId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Delete a vector store file. This will remove the file from the vector store but the file itself will not be deleted. To delete the file, use the [delete file](/docs/api-reference/files/delete) endpoint.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store that the file belongs to. </param>
    /// <param name="fileId"> The ID of the file to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> or <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ClientResult> RemoveFileFromStoreAsync(string vectorStoreId, string fileId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        using PipelineMessage message = CreateDeleteVectorStoreFileRequest(vectorStoreId, fileId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Delete a vector store file. This will remove the file from the vector store but the file itself will not be deleted. To delete the file, use the [delete file](/docs/api-reference/files/delete) endpoint.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store that the file belongs to. </param>
    /// <param name="fileId"> The ID of the file to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="fileId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> or <paramref name="fileId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ClientResult RemoveFileFromStore(string vectorStoreId, string fileId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        using PipelineMessage message = CreateDeleteVectorStoreFileRequest(vectorStoreId, fileId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Create a vector store file batch.
    /// </summary>
    /// <param name="returnWhen"> <see cref="ReturnWhen.Completed"/> if the
    /// method should return when the service has finished running the 
    /// operation, or <see cref="ReturnWhen.Started"/> if it should return 
    /// after the operation has been created but may not have completed 
    /// processing. </param>
    /// <param name="vectorStoreId"> The ID of the vector store for which to create a file batch. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="VectorStoreFileBatchOperation"/> that can be used to wait for 
    /// the operation to complete, get information about the batch file job, or cancel the operation. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<VectorStoreFileBatchOperation> CreateBatchFileJobAsync(
        ReturnWhen returnWhen,
        string vectorStoreId,
        BinaryContent content,
        RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateVectorStoreFileBatchRequest(vectorStoreId, content, options);
        PipelineResponse response = await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false);
        VectorStoreBatchFileJob job = VectorStoreBatchFileJob.FromResponse(response);

        VectorStoreFileBatchOperation operation = new(_pipeline, _endpoint, ClientResult.FromValue(job, response));
        if (returnWhen == ReturnWhen.Started)
        {
            return operation;
        }

        await operation.WaitForCompletionAsync(options?.CancellationToken ?? default).ConfigureAwait(false);
        return operation;
    }

    /// <summary>
    /// [Protocol Method] Create a vector store file batch.
    /// </summary>
    /// <param name="returnWhen"> <see cref="ReturnWhen.Completed"/> if the
    /// method should return when the service has finished running the 
    /// operation, or <see cref="ReturnWhen.Started"/> if it should return 
    /// after the operation has been created but may not have completed 
    /// processing. </param>
    /// <param name="vectorStoreId"> The ID of the vector store for which to create a file batch. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="vectorStoreId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> A <see cref="VectorStoreFileBatchOperation"/> that can be used to wait for 
    /// the operation to complete, get information about the batch file job, or cancel the operation. </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual VectorStoreFileBatchOperation CreateBatchFileJob(
        ReturnWhen returnWhen,
        string vectorStoreId,
        BinaryContent content,
        RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateVectorStoreFileBatchRequest(vectorStoreId, content, options);
        PipelineResponse response = _pipeline.ProcessMessage(message, options);
        VectorStoreBatchFileJob job = VectorStoreBatchFileJob.FromResponse(response);

        VectorStoreFileBatchOperation operation = new(_pipeline, _endpoint, ClientResult.FromValue(job, response));
        if (returnWhen == ReturnWhen.Started)
        {
            return operation;
        }

        operation.WaitForCompletion(options?.CancellationToken ?? default);
        return operation;
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
    internal virtual async Task<ClientResult> GetBatchFileJobAsync(string vectorStoreId, string batchId, RequestOptions options)
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
    internal virtual ClientResult GetBatchFileJob(string vectorStoreId, string batchId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        using PipelineMessage message = CreateGetVectorStoreFileBatchRequest(vectorStoreId, batchId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }
}
