using OpenAI.Files;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.VectorStores;

/// <summary>
/// The service client for OpenAI vector store operations.
/// </summary>
[CodeGenClient("VectorStores")]
[CodeGenSuppress("CreateVectorStoreAsync", typeof(VectorStoreCreationOptions))]
[CodeGenSuppress("CreateVectorStore", typeof(VectorStoreCreationOptions))]
[CodeGenSuppress("GetVectorStoreAsync", typeof(string))]
[CodeGenSuppress("GetVectorStore", typeof(string))]
[CodeGenSuppress("ModifyVectorStoreAsync", typeof(string), typeof(VectorStoreModificationOptions))]
[CodeGenSuppress("ModifyVectorStore", typeof(string), typeof(VectorStoreModificationOptions))]
[CodeGenSuppress("DeleteVectorStoreAsync", typeof(string))]
[CodeGenSuppress("DeleteVectorStore", typeof(string))]
[CodeGenSuppress("GetVectorStoresAsync", typeof(int?), typeof(ListOrder?), typeof(string), typeof(string))]
[CodeGenSuppress("GetVectorStores", typeof(int?), typeof(ListOrder?), typeof(string), typeof(string))]
[CodeGenSuppress("GetVectorStoreFilesAsync", typeof(string), typeof(int?), typeof(ListOrder?), typeof(string), typeof(string), typeof(VectorStoreFileStatusFilter?))]
[CodeGenSuppress("GetVectorStoreFiles", typeof(string), typeof(int?), typeof(ListOrder?), typeof(string), typeof(string), typeof(VectorStoreFileStatusFilter?))]
[CodeGenSuppress("CreateVectorStoreFileAsync", typeof(string), typeof(InternalCreateVectorStoreFileRequest))]
[CodeGenSuppress("CreateVectorStoreFile", typeof(string), typeof(InternalCreateVectorStoreFileRequest))]
[CodeGenSuppress("GetVectorStoreFileAsync", typeof(string), typeof(string))]
[CodeGenSuppress("GetVectorStoreFile", typeof(string), typeof(string))]
[CodeGenSuppress("DeleteVectorStoreFileAsync", typeof(string), typeof(string))]
[CodeGenSuppress("DeleteVectorStoreFile", typeof(string), typeof(string))]
[CodeGenSuppress("CreateVectorStoreFileBatchAsync", typeof(string), typeof(InternalCreateVectorStoreFileBatchRequest))]
[CodeGenSuppress("CreateVectorStoreFileBatch", typeof(string), typeof(InternalCreateVectorStoreFileBatchRequest))]
[CodeGenSuppress("GetVectorStoreFileBatchAsync", typeof(string), typeof(string))]
[CodeGenSuppress("GetVectorStoreFileBatch", typeof(string), typeof(string))]
[CodeGenSuppress("CancelVectorStoreFileBatchAsync", typeof(string), typeof(string))]
[CodeGenSuppress("CancelVectorStoreFileBatch", typeof(string), typeof(string))]
[CodeGenSuppress("GetFilesInVectorStoreBatchesAsync", typeof(string), typeof(string), typeof(int?), typeof(ListOrder?), typeof(string), typeof(string), typeof(VectorStoreFileStatusFilter?))]
[CodeGenSuppress("GetFilesInVectorStoreBatches", typeof(string), typeof(string), typeof(int?), typeof(ListOrder?), typeof(string), typeof(string), typeof(VectorStoreFileStatusFilter?))]
[Experimental("OPENAI001")]
public partial class VectorStoreClient
{
    internal Uri Endpoint => _endpoint;

    /// <summary>
    /// Initializes a new instance of <see cref="VectorStoreClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public VectorStoreClient(ApiKeyCredential credential, OpenAIClientOptions options = null)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(credential, requireExplicitCredential: true), options),
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="VectorStoreClient"/> that will use an API key from the OPENAI_API_KEY
    /// environment variable when authenticating.
    /// </summary>
    /// <remarks>
    /// To provide an explicit credential instead of using the environment variable, use an alternate constructor like
    /// <see cref="VectorStoreClient(ApiKeyCredential,OpenAIClientOptions)"/>.
    /// </remarks>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="InvalidOperationException"> The OPENAI_API_KEY environment variable was not found. </exception>
    public VectorStoreClient(OpenAIClientOptions options = null)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(), options),
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    /// <summary> Initializes a new instance of VectorStoreClient. </summary>
    /// <param name="pipeline"> The HTTP pipeline for sending and receiving REST requests and responses. </param>
    /// <param name="endpoint"> OpenAI Endpoint. </param>
    protected internal VectorStoreClient(ClientPipeline pipeline, Uri endpoint, OpenAIClientOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
    }

    /// <summary> Creates a vector store. </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStore"> The <see cref="VectorStoreCreationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStore"/> is null. </exception>
    /// <returns> A <see cref="CreateVectorStoreOperation"/> that can be used to wait for 
    /// the vector store creation to complete. </returns>
    public virtual async Task<CreateVectorStoreOperation> CreateVectorStoreAsync(bool waitUntilCompleted, VectorStoreCreationOptions vectorStore = null, CancellationToken cancellationToken = default)
    {
        using BinaryContent content = vectorStore?.ToBinaryContent();
        return await CreateVectorStoreAsync(waitUntilCompleted, content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
    }

    /// <summary> Creates a vector store. </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStore"> The <see cref="VectorStoreCreationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStore"/> is null. </exception>
    /// <returns> A <see cref="CreateVectorStoreOperation"/> that can be used to wait for 
    /// the vector store creation to complete. </returns>
    public virtual CreateVectorStoreOperation CreateVectorStore(bool waitUntilCompleted, VectorStoreCreationOptions vectorStore = null, CancellationToken cancellationToken = default)
    {
        using BinaryContent content = vectorStore?.ToBinaryContent();
        return CreateVectorStore(waitUntilCompleted, content, cancellationToken.ToRequestOptions());
    }

    /// <summary>
    /// Gets an instance representing an existing <see cref="VectorStore"/> based on its ID.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to retrieve. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A representation of an existing <see cref="VectorStore"/>. </returns>
    public virtual async Task<ClientResult<VectorStore>> GetVectorStoreAsync(string vectorStoreId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        ClientResult result
            = await GetVectorStoreAsync(vectorStoreId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(
            VectorStore.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary>
    /// Gets an instance representing an existing <see cref="VectorStore"/> based on its ID.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to retrieve. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A representation of an existing <see cref="VectorStore"/>. </returns>
    public virtual ClientResult<VectorStore> GetVectorStore(string vectorStoreId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        ClientResult result = GetVectorStore(vectorStoreId, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(VectorStore.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary>
    /// Modifies an existing <see cref="VectorStore"/>.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the <see cref="VectorStore"/> to modify. </param>
    /// <param name="vectorStore"> The new options to apply to the <see cref="VectorStore"/>. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated representation of the modified <see cref="VectorStore"/>. </returns>
    public virtual async Task<ClientResult<VectorStore>> ModifyVectorStoreAsync(string vectorStoreId, VectorStoreModificationOptions vectorStore, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(vectorStore, nameof(vectorStore));

        using BinaryContent content = vectorStore.ToBinaryContent();
        ClientResult result = await ModifyVectorStoreAsync(vectorStoreId, content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(VectorStore.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary>
    /// Modifies an existing <see cref="VectorStore"/>.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the <see cref="VectorStore"/> to modify. </param>
    /// <param name="vectorStore"> The new options to apply to the <see cref="VectorStore"/>. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated representation of the modified <see cref="VectorStore"/>. </returns>
    public virtual ClientResult<VectorStore> ModifyVectorStore(string vectorStoreId, VectorStoreModificationOptions vectorStore, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(vectorStore, nameof(vectorStore));

        using BinaryContent content = vectorStore.ToBinaryContent();
        ClientResult result = ModifyVectorStore(vectorStoreId, content, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(VectorStore.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary>
    /// Deletes a vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to delete. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A value indicating whether the deletion operation was successful. </returns>
    public virtual async Task<ClientResult<bool>> DeleteVectorStoreAsync(string vectorStoreId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        ClientResult protocolResult = await DeleteVectorStoreAsync(vectorStoreId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse rawProtocolResponse = protocolResult?.GetRawResponse();
        InternalDeleteVectorStoreResponse internalResponse = InternalDeleteVectorStoreResponse.FromResponse(rawProtocolResponse);
        return ClientResult.FromValue(internalResponse.Deleted, rawProtocolResponse);
    }

    /// <summary>
    /// Deletes a vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to delete. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A value indicating whether the deletion operation was successful. </returns>
    public virtual ClientResult<bool> DeleteVectorStore(string vectorStoreId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        ClientResult protocolResult = DeleteVectorStore(vectorStoreId, cancellationToken.ToRequestOptions());
        PipelineResponse rawProtocolResponse = protocolResult?.GetRawResponse();
        InternalDeleteVectorStoreResponse internalResponse = InternalDeleteVectorStoreResponse.FromResponse(rawProtocolResponse);
        return ClientResult.FromValue(internalResponse.Deleted, rawProtocolResponse);
    }

    /// <summary>
    /// Gets a page collection holding <see cref="VectorStore"/> instances for the configured organization.
    /// </summary>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="AsyncPageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="AsyncPageCollection{T}.GetAllValuesAsync(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="AsyncPageCollection{T}.GetCurrentPageAsync"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStore"/>. </returns>
    public virtual AsyncPageCollection<VectorStore> GetVectorStoresAsync(
        VectorStoreCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        return GetVectorStoresAsync(options?.PageSize, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, cancellationToken.ToRequestOptions())
            as AsyncPageCollection<VectorStore>;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="VectorStore"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="AsyncPageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="AsyncPageCollection{T}.GetAllValuesAsync(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="AsyncPageCollection{T}.GetCurrentPageAsync"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStore"/>. </returns>
    public virtual AsyncPageCollection<VectorStore> GetVectorStoresAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoresPageToken pageToken = VectorStoresPageToken.FromToken(firstPageToken);
        return GetVectorStoresAsync(pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, cancellationToken.ToRequestOptions())
            as AsyncPageCollection<VectorStore>;
    }

    /// <summary>
    /// Gets a page collection holding <see cref="VectorStore"/> instances for the configured organization.
    /// </summary>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="PageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="PageCollection{T}.GetAllValues(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="PageCollection{T}.GetCurrentPage"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStore"/>. </returns>
    public virtual PageCollection<VectorStore> GetVectorStores(
        VectorStoreCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        return GetVectorStores(options?.PageSize, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, cancellationToken.ToRequestOptions())
            as PageCollection<VectorStore>;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="VectorStore"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="PageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="PageCollection{T}.GetAllValues(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="PageCollection{T}.GetCurrentPage"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStore"/>. </returns>
    public virtual PageCollection<VectorStore> GetVectorStores(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoresPageToken pageToken = VectorStoresPageToken.FromToken(firstPageToken);
        return GetVectorStores(pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, cancellationToken.ToRequestOptions())
            as PageCollection<VectorStore>;
    }

    /// <summary>
    /// Associates a single, uploaded file with a vector store, beginning ingestion of the file into the vector store.
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStoreId"> The ID of the vector store to associate the file with. </param>
    /// <param name="fileId"> The ID of the file to associate with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="AddFileToVectorStoreOperation"/> that can be used to wait for 
    /// the vector store file addition to complete. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="fileId"/> is null. </exception>
    public virtual async Task<AddFileToVectorStoreOperation> AddFileToVectorStoreAsync(bool waitUntilCompleted, string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        InternalCreateVectorStoreFileRequest internalRequest = new(fileId);
        return await AddFileToVectorStoreAsync(waitUntilCompleted, vectorStoreId, internalRequest.ToBinaryContent(), cancellationToken.ToRequestOptions()).ConfigureAwait(false);
    }

    /// <summary>
    /// Associates a single, uploaded file with a vector store, beginning ingestion of the file into the vector store.
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStoreId"> The ID of the vector store to associate the file with. </param>
    /// <param name="fileId"> The ID of the file to associate with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="AddFileToVectorStoreOperation"/> that can be used to wait for 
    /// the vector store file addition to complete. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="fileId"/> is null. </exception>
    public virtual AddFileToVectorStoreOperation AddFileToVectorStore(bool waitUntilCompleted, string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        InternalCreateVectorStoreFileRequest internalRequest = new(fileId);
        return AddFileToVectorStore(waitUntilCompleted, vectorStoreId, internalRequest.ToBinaryContent(), cancellationToken.ToRequestOptions());
    }

    /// <summary>
    /// Gets a page collection holding <see cref="VectorStoreFileAssociation"/> instances that represent file inclusions in the
    /// specified vector store.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the vector store to enumerate the file associations of.
    /// </param>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="AsyncPageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="AsyncPageCollection{T}.GetAllValuesAsync(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="AsyncPageCollection{T}.GetCurrentPageAsync"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual AsyncPageCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        string vectorStoreId,
        VectorStoreFileAssociationCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        return GetFileAssociationsAsync(vectorStoreId, options?.PageSize, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, options?.Filter?.ToString(), cancellationToken.ToRequestOptions())
            as AsyncPageCollection<VectorStoreFileAssociation>;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="VectorStoreFileAssociation"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="AsyncPageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="AsyncPageCollection{T}.GetAllValuesAsync(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="AsyncPageCollection{T}.GetCurrentPageAsync"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual AsyncPageCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreFilesPageToken pageToken = VectorStoreFilesPageToken.FromToken(firstPageToken);
        return GetFileAssociationsAsync(pageToken?.VectorStoreId, pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, pageToken?.Filter, cancellationToken.ToRequestOptions())
            as AsyncPageCollection<VectorStoreFileAssociation>;
    }

    /// <summary>
    /// Gets a page collection holding <see cref="VectorStoreFileAssociation"/> instances that represent file inclusions in the
    /// specified vector store.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the vector store to enumerate the file associations of.
    /// </param>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="PageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="PageCollection{T}.GetAllValues(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="PageCollection{T}.GetCurrentPage"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual PageCollection<VectorStoreFileAssociation> GetFileAssociations(
        string vectorStoreId,
        VectorStoreFileAssociationCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        return GetFileAssociations(vectorStoreId, options?.PageSize, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, options?.Filter?.ToString(), cancellationToken.ToRequestOptions())
            as PageCollection<VectorStoreFileAssociation>;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="VectorStoreFileAssociation"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="PageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="PageCollection{T}.GetAllValues(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="PageCollection{T}.GetCurrentPage"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual PageCollection<VectorStoreFileAssociation> GetFileAssociations(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreFilesPageToken pageToken = VectorStoreFilesPageToken.FromToken(firstPageToken);
        return GetFileAssociations(pageToken?.VectorStoreId, pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, pageToken?.Filter, cancellationToken.ToRequestOptions())
            as PageCollection<VectorStoreFileAssociation>;
    }

    /// <summary>
    /// Gets a <see cref="VectorStoreFileAssociation"/> instance representing an existing association between a known
    /// vector store ID and file ID.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store associated with the file. </param>
    /// <param name="fileId"> The ID of the file associated with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreFileAssociation"/> instance. </returns>
    public virtual async Task<ClientResult<VectorStoreFileAssociation>> GetFileAssociationAsync(
        string vectorStoreId,
        string fileId,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        ClientResult result = await GetFileAssociationAsync(vectorStoreId, fileId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreFileAssociation value = VectorStoreFileAssociation.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Gets a <see cref="VectorStoreFileAssociation"/> instance representing an existing association between a known
    /// vector store ID and file ID.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store associated with the file. </param>
    /// <param name="fileId"> The ID of the file associated with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreFileAssociation"/> instance. </returns>
    public virtual ClientResult<VectorStoreFileAssociation> GetFileAssociation(
        string vectorStoreId,
        string fileId,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        ClientResult result = GetFileAssociation(vectorStoreId, fileId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreFileAssociation value = VectorStoreFileAssociation.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Removes the association between a file and vector store, which makes the file no longer available to the vector
    /// store.
    /// </summary>
    /// <remarks>
    /// This does not delete the file. To delete the file, use <see cref="FileClient.DeleteFile(string)"/>.
    /// </remarks>
    /// <param name="vectorStoreId"> The ID of the vector store that the file should be removed from. </param>
    /// <param name="fileId"> The ID of the file to remove from the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A value indicating whether the removal operation was successful. </returns>
    public virtual async Task<ClientResult<bool>> RemoveFileFromStoreAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await RemoveFileFromStoreAsync(vectorStoreId, fileId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse protocolResponse = protocolResult?.GetRawResponse();
        InternalDeleteVectorStoreFileResponse internalDeletion = InternalDeleteVectorStoreFileResponse.FromResponse(protocolResponse);
        return ClientResult.FromValue(internalDeletion.Deleted, protocolResponse);
    }

    /// <summary>
    /// Removes the association between a file and vector store, which makes the file no longer available to the vector
    /// store.
    /// </summary>
    /// <remarks>
    /// This does not delete the file. To delete the file, use <see cref="FileClient.DeleteFile(string)"/>.
    /// </remarks>
    /// <param name="vectorStoreId"> The ID of the vector store that the file should be removed from. </param>
    /// <param name="fileId"> The ID of the file to remove from the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A value indicating whether the removal operation was successful. </returns>
    public virtual ClientResult<bool> RemoveFileFromStore(string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = RemoveFileFromStore(vectorStoreId, fileId, cancellationToken.ToRequestOptions());
        PipelineResponse protocolResponse = protocolResult?.GetRawResponse();
        InternalDeleteVectorStoreFileResponse internalDeletion = InternalDeleteVectorStoreFileResponse.FromResponse(protocolResponse);
        return ClientResult.FromValue(internalDeletion.Deleted, protocolResponse);
    }

    /// <summary>
    /// Adds multiple files in a batch to the vector store, beginning the ingestion process.
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStoreId"> The ID of the vector store to associate files with. </param>
    /// <param name="fileIds"> The IDs of the files to associate with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="CreateBatchFileJobOperation"/> that can be used to wait for 
    /// the operation to complete, get information about the batch file job, or cancel the operation. </returns>
    public virtual async Task<CreateBatchFileJobOperation> CreateBatchFileJobAsync(
        bool waitUntilCompleted,
        string vectorStoreId, 
        IEnumerable<string> fileIds,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileIds, nameof(fileIds));

        BinaryContent content = new InternalCreateVectorStoreFileBatchRequest(fileIds).ToBinaryContent();
        RequestOptions options = cancellationToken.ToRequestOptions();

        return await CreateBatchFileJobAsync(waitUntilCompleted, vectorStoreId, content, options).ConfigureAwait(false);
    }

    /// <summary>
    /// Begins a batch job to associate multiple jobs with a vector store, beginning the ingestion process.
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStoreId"> The ID of the vector store to associate files with. </param>
    /// <param name="fileIds"> The IDs of the files to associate with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="CreateBatchFileJobOperation"/> that can be used to wait for 
    /// the operation to complete, get information about the batch file job, or cancel the operation. </returns>
    public virtual CreateBatchFileJobOperation CreateBatchFileJob(
        bool waitUntilCompleted,
        string vectorStoreId,
        IEnumerable<string> fileIds, 
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileIds, nameof(fileIds));

        BinaryContent content = new InternalCreateVectorStoreFileBatchRequest(fileIds).ToBinaryContent();
        RequestOptions options = cancellationToken.ToRequestOptions();

        return CreateBatchFileJob(waitUntilCompleted, vectorStoreId, content, options);
    }
}
