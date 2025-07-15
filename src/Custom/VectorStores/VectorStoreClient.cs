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
[CodeGenType("VectorStores")]
[CodeGenSuppress("CancelBatchFileJob", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelBatchFileJobAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CreateVectorStore", typeof(VectorStoreCreationOptions), typeof(CancellationToken))]
[CodeGenSuppress("CreateVectorStoreAsync", typeof(VectorStoreCreationOptions), typeof(CancellationToken))]
[CodeGenSuppress("CreateVectorStoreFile", typeof(string), typeof(InternalCreateVectorStoreFileRequest), typeof(CancellationToken))]
[CodeGenSuppress("CreateVectorStore", typeof(VectorStoreCreationOptions), typeof(CancellationToken))]
[CodeGenSuppress("GetVectorStoresAsync", typeof(int?), typeof(VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetVectorStores", typeof(int?), typeof(VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetVectorStoreFilesAsync", typeof(string), typeof(int?), typeof(VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(VectorStoreFileStatusFilter?), typeof(CancellationToken))]
[CodeGenSuppress("GetVectorStoreFiles", typeof(string), typeof(int?), typeof(VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(VectorStoreFileStatusFilter?), typeof(CancellationToken))]
[CodeGenSuppress("CreateVectorStoreFileAsync", typeof(string), typeof(InternalCreateVectorStoreFileRequest), typeof(CancellationToken))]
[CodeGenSuppress("CreateVectorStoreFileBatch", typeof(string), typeof(InternalCreateVectorStoreFileBatchRequest), typeof(CancellationToken))]
[CodeGenSuppress("CreateVectorStoreFileBatchAsync", typeof(string), typeof(InternalCreateVectorStoreFileBatchRequest), typeof(CancellationToken))]
[CodeGenSuppress("RemoveFileFromStore", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("RemoveFileFromStoreAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetFileAssociation", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetFileAssociationAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetVectorStoreFileBatch", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetVectorStoreFileBatchAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("RetrieveVectorStoreFileContent", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("RetrieveVectorStoreFileContentAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("SearchVectorStore", typeof(string), typeof(BinaryData), typeof(bool?), typeof(int?), typeof(BinaryData), typeof(InternalVectorStoreSearchRequestRankingOptions), typeof(CancellationToken))]
[CodeGenSuppress("SearchVectorStoreAsync", typeof(string), typeof(BinaryData), typeof(bool?), typeof(int?), typeof(BinaryData), typeof(InternalVectorStoreSearchRequestRankingOptions), typeof(CancellationToken))]
[CodeGenSuppress("VectorStoreClient", typeof(ClientPipeline), typeof(Uri))]
[CodeGenSuppress("GetVectorStoreFileBatch", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelBatchFileJobAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelBatchFileJob", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetFilesInVectorStoreBatchAsync", typeof(string), typeof(string), typeof(int?), typeof(VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(VectorStoreFileStatusFilter?), typeof(CancellationToken))]
[CodeGenSuppress("GetFilesInVectorStoreBatch", typeof(string), typeof(string), typeof(int?), typeof(VectorStoreCollectionOrder?), typeof(string), typeof(string), typeof(VectorStoreFileStatusFilter?), typeof(CancellationToken))]
public partial class VectorStoreClient
{
    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="VectorStoreClient"/>. </summary>
    /// <param name="apiKey"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="apiKey"/> is null. </exception>
    public VectorStoreClient(string apiKey) : this(new ApiKeyCredential(apiKey), new OpenAIClientOptions())
    {
    }

    // CUSTOM:
    // - Used a custom pipeline.
    // - Demoted the endpoint parameter to be a property in the options class.
    /// <summary> Initializes a new instance of <see cref="VectorStoreClient"/>. </summary>
    /// <param name="credential"> The API key to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public VectorStoreClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VectorStoreClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public VectorStoreClient(ApiKeyCredential credential, OpenAIClientOptions options)
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
    /// <summary> Initializes a new instance of <see cref="VectorStoreClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline to send and receive REST requests and responses. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="pipeline"/> is null. </exception>
    protected internal VectorStoreClient(ClientPipeline pipeline, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(pipeline, nameof(pipeline));
        options ??= new OpenAIClientOptions();

        Pipeline = pipeline;
        _endpoint = OpenAIClient.GetEndpoint(options);
    }

    internal virtual CreateVectorStoreOperation CreateCreateVectorStoreOperation(ClientResult<VectorStore> result)
    {
        return new CreateVectorStoreOperation(this, _endpoint, result);
    }

    internal virtual AddFileToVectorStoreOperation CreateAddFileToVectorStoreOperation(ClientResult<VectorStoreFileAssociation> result)
    {
        return new AddFileToVectorStoreOperation(this, _endpoint, result);
    }

    internal virtual CreateBatchFileJobOperation CreateBatchFileJobOperation(ClientResult<VectorStoreBatchFileJob> result)
    {
        return new CreateBatchFileJobOperation(this, _endpoint, result);
    }

    /// <summary> Creates a vector store. </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="options"> The <see cref="VectorStoreCreationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="options"/> is null. </exception>
    /// <returns> A <see cref="CreateVectorStoreOperation"/> that can be used to wait for
    /// the vector store creation to complete. </returns>
    public virtual async Task<CreateVectorStoreOperation> CreateVectorStoreAsync(bool waitUntilCompleted, VectorStoreCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        using BinaryContent content = options?.ToBinaryContent();
        return await CreateVectorStoreAsync(content, waitUntilCompleted, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
    }

    /// <summary> Creates a vector store. </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="options"> The <see cref="VectorStoreCreationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="options"/> is null. </exception>
    /// <returns> A <see cref="CreateVectorStoreOperation"/> that can be used to wait for
    /// the vector store creation to complete. </returns>
    public virtual CreateVectorStoreOperation CreateVectorStore(bool waitUntilCompleted, VectorStoreCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        using BinaryContent content = options?.ToBinaryContent();
        return CreateVectorStore(content, waitUntilCompleted, cancellationToken.ToRequestOptions());
    }

    /// <summary>
    /// Gets an instance representing an existing <see cref="VectorStore"/> based on its ID.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to retrieve. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A representation of an existing <see cref="VectorStore"/>. </returns>
    internal virtual async Task<ClientResult<VectorStore>> GetVectorStoreAsync(string vectorStoreId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        ClientResult result
            = await GetVectorStoreAsync(vectorStoreId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(
            VectorStore.FromClientResult(result), result.GetRawResponse());
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
        return ClientResult.FromValue(VectorStore.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary>
    /// Modifies an existing <see cref="VectorStore"/>.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the <see cref="VectorStore"/> to modify. </param>
    /// <param name="options"> The new options to apply to the <see cref="VectorStore"/>. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated representation of the modified <see cref="VectorStore"/>. </returns>
    public virtual async Task<ClientResult<VectorStore>> ModifyVectorStoreAsync(string vectorStoreId, VectorStoreModificationOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(options, nameof(options));

        using BinaryContent content = options?.ToBinaryContent();
        ClientResult result = await ModifyVectorStoreAsync(vectorStoreId, content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(VectorStore.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary>
    /// Modifies an existing <see cref="VectorStore"/>.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the <see cref="VectorStore"/> to modify. </param>
    /// <param name="options"> The new options to apply to the <see cref="VectorStore"/>. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated representation of the modified <see cref="VectorStore"/>. </returns>
    public virtual ClientResult<VectorStore> ModifyVectorStore(string vectorStoreId, VectorStoreModificationOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNull(options, nameof(options));

        using BinaryContent content = options?.ToBinaryContent();
        ClientResult result = ModifyVectorStore(vectorStoreId, content, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(VectorStore.FromClientResult(result), result.GetRawResponse());
    }

    /// <summary>
    /// Deletes a vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to delete. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreDeletionResult"/> instance. </returns>
    public virtual async Task<ClientResult<VectorStoreDeletionResult>> DeleteVectorStoreAsync(string vectorStoreId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        ClientResult protocolResult = await DeleteVectorStoreAsync(vectorStoreId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse rawProtocolResponse = protocolResult?.GetRawResponse();
        VectorStoreDeletionResult value = VectorStoreDeletionResult.FromClientResult(protocolResult);
        return ClientResult.FromValue(value, rawProtocolResponse);
    }

    /// <summary>
    /// Deletes a vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to delete. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreDeletionResult"/> instance. </returns>
    public virtual ClientResult<VectorStoreDeletionResult> DeleteVectorStore(string vectorStoreId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        ClientResult protocolResult = DeleteVectorStore(vectorStoreId, cancellationToken.ToRequestOptions());
        PipelineResponse rawProtocolResponse = protocolResult?.GetRawResponse();
        VectorStoreDeletionResult value = VectorStoreDeletionResult.FromClientResult(protocolResult);
        return ClientResult.FromValue(value, rawProtocolResponse);
    }

    /// <summary>
    /// Gets a page collection holding <see cref="VectorStore"/> instances for the configured organization.
    /// </summary>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="VectorStore"/>. </returns>
    public virtual AsyncCollectionResult<VectorStore> GetVectorStoresAsync(
        VectorStoreCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        return GetVectorStoresAsync(options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, cancellationToken.ToRequestOptions())
            as AsyncCollectionResult<VectorStore>;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="VectorStore"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="VectorStore"/>. </returns>
    public virtual AsyncCollectionResult<VectorStore> GetVectorStoresAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreCollectionPageToken pageToken = VectorStoreCollectionPageToken.FromToken(firstPageToken);
        return GetVectorStoresAsync(pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, cancellationToken.ToRequestOptions())
            as AsyncCollectionResult<VectorStore>;
    }

    /// <summary>
    /// Gets a page collection holding <see cref="VectorStore"/> instances for the configured organization.
    /// </summary>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="VectorStore"/>. </returns>
    public virtual CollectionResult<VectorStore> GetVectorStores(
        VectorStoreCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        return GetVectorStores(options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, cancellationToken.ToRequestOptions())
            as CollectionResult<VectorStore>;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="VectorStore"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="VectorStore"/>. </returns>
    public virtual CollectionResult<VectorStore> GetVectorStores(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreCollectionPageToken pageToken = VectorStoreCollectionPageToken.FromToken(firstPageToken);
        return GetVectorStores(pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, cancellationToken.ToRequestOptions())
            as CollectionResult<VectorStore>;
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
    public virtual async Task<AddFileToVectorStoreOperation> AddFileToVectorStoreAsync(string vectorStoreId, string fileId, bool waitUntilCompleted, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        InternalCreateVectorStoreFileRequest internalRequest = new(fileId);
        return await AddFileToVectorStoreAsync(vectorStoreId, internalRequest.ToBinaryContent(), waitUntilCompleted, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
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
    public virtual AddFileToVectorStoreOperation AddFileToVectorStore(string vectorStoreId, string fileId, bool waitUntilCompleted, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        InternalCreateVectorStoreFileRequest internalRequest = new(fileId);
        return AddFileToVectorStore(vectorStoreId, internalRequest.ToBinaryContent(), waitUntilCompleted, cancellationToken.ToRequestOptions());
    }

    public virtual ClientResult<VectorStoreFileAssociation> UpdateVectorStoreFileAttributes(string vectorStoreId, string fileId, IDictionary<string, BinaryData> attributes, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        InternalUpdateVectorStoreFileAttributesRequest spreadModel = new InternalUpdateVectorStoreFileAttributesRequest(attributes, null);
        ClientResult result = UpdateVectorStoreFileAttributes(vectorStoreId, fileId, spreadModel, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue(VectorStoreFileAssociation.FromClientResult(result), result.GetRawResponse());
    }

    public virtual async Task<ClientResult<VectorStoreFileAssociation>> UpdateVectorStoreFileAttributesAsync(string vectorStoreId, string fileId, IDictionary<string, BinaryData> attributes, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        InternalUpdateVectorStoreFileAttributesRequest spreadModel = new InternalUpdateVectorStoreFileAttributesRequest(attributes, null);
        ClientResult result = await UpdateVectorStoreFileAttributesAsync(vectorStoreId, fileId, spreadModel, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
        return ClientResult.FromValue(VectorStoreFileAssociation.FromClientResult(result), result.GetRawResponse());
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
    /// <returns> A collection of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual AsyncCollectionResult<VectorStoreFileAssociation> GetFileAssociationsAsync(
        string vectorStoreId,
        VectorStoreFileAssociationCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        return GetFileAssociationsAsync(vectorStoreId, options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, options?.Filter?.ToString(), cancellationToken.ToRequestOptions())
            as AsyncCollectionResult<VectorStoreFileAssociation>;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="VectorStoreFileAssociation"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual AsyncCollectionResult<VectorStoreFileAssociation> GetFileAssociationsAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreFileCollectionPageToken pageToken = VectorStoreFileCollectionPageToken.FromToken(firstPageToken);
        return GetFileAssociationsAsync(pageToken?.VectorStoreId, pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, pageToken?.Filter, cancellationToken.ToRequestOptions())
            as AsyncCollectionResult<VectorStoreFileAssociation>;
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
    /// <returns> A collection of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual CollectionResult<VectorStoreFileAssociation> GetFileAssociations(
        string vectorStoreId,
        VectorStoreFileAssociationCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));

        return GetFileAssociations(vectorStoreId, options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, options?.Filter?.ToString(), cancellationToken.ToRequestOptions())
            as CollectionResult<VectorStoreFileAssociation>;
    }

    /// <summary>
    /// Rehydrates a page collection holding <see cref="VectorStoreFileAssociation"/> instances from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual CollectionResult<VectorStoreFileAssociation> GetFileAssociations(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreFileCollectionPageToken pageToken = VectorStoreFileCollectionPageToken.FromToken(firstPageToken);
        return GetFileAssociations(pageToken?.VectorStoreId, pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, pageToken?.Filter, cancellationToken.ToRequestOptions())
            as CollectionResult<VectorStoreFileAssociation>;
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
        VectorStoreFileAssociation value = VectorStoreFileAssociation.FromClientResult(result);
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
        VectorStoreFileAssociation value = VectorStoreFileAssociation.FromClientResult(result);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Removes the association between a file and vector store, which makes the file no longer available to the vector
    /// store.
    /// </summary>
    /// <remarks>
    /// This does not delete the file. To delete the file, use <see cref="OpenAIFileClient.DeleteFile(string)"/>.
    /// </remarks>
    /// <param name="vectorStoreId"> The ID of the vector store that the file should be removed from. </param>
    /// <param name="fileId"> The ID of the file to remove from the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="FileFromStoreRemovalResult"/> instance. </returns>
    public virtual async Task<ClientResult<FileFromStoreRemovalResult>> RemoveFileFromStoreAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await RemoveFileFromStoreAsync(vectorStoreId, fileId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse protocolResponse = protocolResult?.GetRawResponse();
        FileFromStoreRemovalResult value = FileFromStoreRemovalResult.FromClientResult(protocolResult);
        return ClientResult.FromValue(value, protocolResponse);
    }

    /// <summary>
    /// Removes the association between a file and vector store, which makes the file no longer available to the vector
    /// store.
    /// </summary>
    /// <remarks>
    /// This does not delete the file. To delete the file, use <see cref="OpenAIFileClient.DeleteFile(string)"/>.
    /// </remarks>
    /// <param name="vectorStoreId"> The ID of the vector store that the file should be removed from. </param>
    /// <param name="fileId"> The ID of the file to remove from the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="FileFromStoreRemovalResult"/> instance. </returns>
    public virtual ClientResult<FileFromStoreRemovalResult> RemoveFileFromStore(string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = RemoveFileFromStore(vectorStoreId, fileId, cancellationToken.ToRequestOptions());
        PipelineResponse protocolResponse = protocolResult?.GetRawResponse();
        FileFromStoreRemovalResult value = FileFromStoreRemovalResult.FromClientResult(protocolResult);
        return ClientResult.FromValue(value, protocolResponse);
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
        string vectorStoreId,
        IEnumerable<string> fileIds,
        bool waitUntilCompleted,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileIds, nameof(fileIds));

        var createFileBatchRequest = new InternalCreateVectorStoreFileBatchRequest(fileIds);
        BinaryContent content = BinaryContent.Create(createFileBatchRequest, ModelSerializationExtensions.WireOptions);
        RequestOptions options = cancellationToken.ToRequestOptions();

        return await CreateBatchFileJobAsync(vectorStoreId, content, waitUntilCompleted, options).ConfigureAwait(false);
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
        string vectorStoreId,
        IEnumerable<string> fileIds,
        bool waitUntilCompleted,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileIds, nameof(fileIds));

        var createFileBatchRequest = new InternalCreateVectorStoreFileBatchRequest(fileIds);
        BinaryContent content = BinaryContent.Create(createFileBatchRequest, ModelSerializationExtensions.WireOptions);
        RequestOptions options = cancellationToken.ToRequestOptions();

        return CreateBatchFileJob(vectorStoreId, content, waitUntilCompleted, options);
    }

    /// <summary>
    /// Gets a page collection of file associations associated with a vector store batch file job, representing the files
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the vector store into which the file batch was scheduled for ingestion.
    /// </param>
    /// <param name="batchJobId">
    /// The ID of the batch file job that was previously scheduled.
    /// </param>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual AsyncCollectionResult<VectorStoreFileAssociation> GetFileAssociationsAsync(
        string vectorStoreId,
        string batchJobId,
        VectorStoreFileAssociationCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchJobId, nameof(batchJobId));

        return GetFileAssociationsAsync(vectorStoreId, batchJobId, options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, options?.Filter?.ToString(), cancellationToken.ToRequestOptions())
            as AsyncCollectionResult<VectorStoreFileAssociation>;
    }

    /// <summary>
    /// Rehydrates a page collection of file associations from a page token.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the vector store into which the file batch was scheduled for ingestion.
    /// </param>
    /// <param name="batchJobId">
    /// The ID of the batch file job that was previously scheduled.
    /// </param>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual AsyncCollectionResult<VectorStoreFileAssociation> GetFileAssociationsAsync(
        string vectorStoreId,
        string batchJobId,
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreFileBatchCollectionPageToken pageToken = VectorStoreFileBatchCollectionPageToken.FromToken(firstPageToken);

        if (vectorStoreId != pageToken.VectorStoreId)
        {
            throw new ArgumentException(
                "Invalid page token. 'vectorStoreId' value does not match page token value.",
                nameof(vectorStoreId));
        }

        if (batchJobId != pageToken.BatchId)
        {
            throw new ArgumentException(
                "Invalid page token. 'batchJobId' value does not match page token value.",
                nameof(vectorStoreId));
        }

        return GetFileAssociationsAsync(vectorStoreId, batchJobId, pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, pageToken?.Filter, cancellationToken.ToRequestOptions())
            as AsyncCollectionResult<VectorStoreFileAssociation>;
    }

    /// <summary>
    /// Gets a page collection of file associations associated with a vector store batch file job, representing the files
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the vector store into which the file batch was scheduled for ingestion.
    /// </param>
    /// <param name="batchJobId">
    /// The ID of the batch file job that was previously scheduled.
    /// </param>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual CollectionResult<VectorStoreFileAssociation> GetFileAssociations(
        string vectorStoreId,
        string batchJobId,
        VectorStoreFileAssociationCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchJobId, nameof(batchJobId));

        return GetFileAssociations(vectorStoreId, batchJobId, options?.PageSizeLimit, options?.Order?.ToString(), options?.AfterId, options?.BeforeId, options?.Filter?.ToString(), cancellationToken.ToRequestOptions())
            as CollectionResult<VectorStoreFileAssociation>;
    }

    /// <summary>
    /// Rehydrates a page collection of file associations from a page token.
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the vector store into which the file batch was scheduled for ingestion.
    /// </param>
    /// <param name="batchJobId">
    /// The ID of the batch file job that was previously scheduled.
    /// </param>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A collection of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual CollectionResult<VectorStoreFileAssociation> GetFileAssociations(
        string vectorStoreId,
        string batchJobId,
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreFileBatchCollectionPageToken pageToken = VectorStoreFileBatchCollectionPageToken.FromToken(firstPageToken);

        if (vectorStoreId != pageToken.VectorStoreId)
        {
            throw new ArgumentException(
                "Invalid page token. 'vectorStoreId' value does not match page token value.",
                nameof(vectorStoreId));
        }

        if (batchJobId != pageToken.BatchId)
        {
            throw new ArgumentException(
                "Invalid page token. 'batchJobId' value does not match page token value.",
                nameof(vectorStoreId));
        }

        return GetFileAssociations(vectorStoreId, batchJobId, pageToken?.Limit, pageToken?.Order, pageToken?.After, pageToken?.Before, pageToken?.Filter, cancellationToken.ToRequestOptions())
            as CollectionResult<VectorStoreFileAssociation>;
    }

    /// <summary>
    /// Gets an existing vector store batch file ingestion job from a known vector store ID and job ID.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store into which the batch of files was started. </param>
    /// <param name="batchJobId"> The ID of the batch operation adding files to the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the ingestion operation. </returns>
    public virtual async Task<ClientResult<VectorStoreBatchFileJob>> GetBatchFileJobAsync(string vectorStoreId, string batchJobId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchJobId, nameof(batchJobId));

        ClientResult result = await GetBatchFileJobAsync(vectorStoreId, batchJobId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromClientResult(result);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Gets an existing vector store batch file ingestion job from a known vector store ID and job ID.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store into which the batch of files was started. </param>
    /// <param name="batchJobId"> The ID of the batch operation adding files to the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the ingestion operation. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> GetBatchFileJob(string vectorStoreId, string batchJobId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchJobId, nameof(batchJobId));

        ClientResult result = GetBatchFileJob(vectorStoreId, batchJobId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromClientResult(result);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the <see cref="VectorStore"/> that is the ingestion target of the batch job being cancelled.
    /// </param>
    /// <param name="batchJobId">
    /// The ID of the <see cref="VectorStoreBatchFileJob"/> that should be canceled.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual async Task<ClientResult<VectorStoreBatchFileJob>> CancelBatchFileJobAsync(string vectorStoreId, string batchJobId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchJobId, nameof(batchJobId));

        ClientResult result = await CancelBatchFileJobAsync(vectorStoreId, batchJobId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromClientResult(result);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the <see cref="VectorStore"/> that is the ingestion target of the batch job being cancelled.
    /// </param>
    /// <param name="batchJobId">
    /// The ID of the <see cref="VectorStoreBatchFileJob"/> that should be canceled.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> CancelBatchFileJob(string vectorStoreId, string batchJobId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchJobId, nameof(batchJobId));

        ClientResult result = CancelBatchFileJob(vectorStoreId, batchJobId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromClientResult(result);
        return ClientResult.FromValue(value, response);
    }
}
