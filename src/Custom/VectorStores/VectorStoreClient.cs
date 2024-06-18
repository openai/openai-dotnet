using OpenAI.Assistants;
using OpenAI.Files;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using static OpenAI.InternalListHelpers;

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
    {}

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
    {}

    /// <summary> Initializes a new instance of VectorStoreClient. </summary>
    /// <param name="pipeline"> The HTTP pipeline for sending and receiving REST requests and responses. </param>
    /// <param name="endpoint"> OpenAI Endpoint. </param>
    protected internal VectorStoreClient(ClientPipeline pipeline, Uri endpoint, OpenAIClientOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
    }

    /// <summary> Creates a vector store. </summary>
    /// <param name="vectorStore"> The <see cref="VectorStoreCreationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStore"/> is null. </exception>
    /// <remarks> Create vector store. </remarks>
    public virtual async Task<ClientResult<VectorStore>> CreateVectorStoreAsync(VectorStoreCreationOptions vectorStore = null, CancellationToken cancellationToken = default)
    {
        using BinaryContent content = vectorStore?.ToBinaryContent();
        ClientResult result = await CreateVectorStoreAsync(content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return ClientResult.FromValue(VectorStore.FromResponse(result.GetRawResponse()), result.GetRawResponse());
    }

    /// <summary> Creates a vector store. </summary>
    /// <param name="vectorStore"> The <see cref="VectorStoreCreationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStore"/> is null. </exception>
    /// <remarks> Create vector store. </remarks>
    public virtual ClientResult<VectorStore> CreateVectorStore(VectorStoreCreationOptions vectorStore = null, CancellationToken cancellationToken = default)
    {
        using BinaryContent content = vectorStore?.ToBinaryContent();
        ClientResult result = CreateVectorStore(content, cancellationToken.ToRequestOptions());
        return ClientResult.FromValue(VectorStore.FromResponse(result.GetRawResponse()), result.GetRawResponse());
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
    /// Gets the collection of <see cref="VectorStore"/> instances for the configured organization.
    /// </summary>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns>
    /// A collection of <see cref="VectorStore"/> instances that can be asynchronously enumerated via
    /// <c>await foreach</c>.
    /// </returns>
    public virtual AsyncPageableCollection<VectorStore> GetVectorStoresAsync(ListOrder? resultOrder = null, CancellationToken cancellationToken = default)
    {
        return CreateAsyncPageable<VectorStore, InternalListVectorStoresResponse>((continuationToken, pageSize)
            => GetVectorStoresAsync(pageSize, resultOrder?.ToString(), continuationToken, null, cancellationToken.ToRequestOptions()));
    }

    /// <summary>
    /// Gets the collection of <see cref="VectorStore"/> instances for the configured organization.
    /// </summary>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns>
    /// A collection of <see cref="VectorStore"/> instances that can be synchronously enumerated via <c>foreach</c>.
    /// </returns>
    public virtual PageableCollection<VectorStore> GetVectorStores(ListOrder? resultOrder = null, CancellationToken cancellationToken = default)
    {
        return CreatePageable<VectorStore, InternalListVectorStoresResponse>((continuationToken, pageSize)
            => GetVectorStores(pageSize, resultOrder?.ToString(), continuationToken, null, cancellationToken.ToRequestOptions()));
    }

    /// <summary>
    /// Associates a single, uploaded file with a vector store, beginning ingestion of the file into the vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to associate the file with. </param>
    /// <param name="fileId"> The ID of the file to associate with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns>
    /// A <see cref="VectorStoreFileAssociation"/> instance that represents the new association.
    /// </returns>
    public virtual async Task<ClientResult<VectorStoreFileAssociation>> AddFileToVectorStoreAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
    {
        InternalCreateVectorStoreFileRequest internalRequest = new(fileId);
        ClientResult protocolResult = await AddFileToVectorStoreAsync(vectorStoreId, internalRequest.ToBinaryContent(), cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse protocolResponse = protocolResult?.GetRawResponse();
        VectorStoreFileAssociation fileAssociation = VectorStoreFileAssociation.FromResponse(protocolResponse);
        return ClientResult.FromValue(fileAssociation, protocolResponse);
    }

    /// <summary>
    /// Associates a single, uploaded file with a vector store, beginning ingestion of the file into the vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to associate the file with. </param>
    /// <param name="fileId"> The ID of the file to associate with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns>
    /// A <see cref="VectorStoreFileAssociation"/> instance that represents the new association.
    /// </returns>
    public virtual ClientResult<VectorStoreFileAssociation> AddFileToVectorStore(string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
    {
        InternalCreateVectorStoreFileRequest internalRequest = new(fileId);
        ClientResult protocolResult = AddFileToVectorStore(vectorStoreId, internalRequest.ToBinaryContent(), cancellationToken.ToRequestOptions());
        PipelineResponse protocolResponse = protocolResult?.GetRawResponse();
        VectorStoreFileAssociation fileAssociation = VectorStoreFileAssociation.FromResponse(protocolResponse);
        return ClientResult.FromValue(fileAssociation, protocolResponse);
    }

    /// <summary>
    /// Gets the collection of <see cref="VectorStoreFileAssociation"/> instances representing file inclusions in the
    /// specified vector store.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the vector store to enumerate the file associations of.
    /// </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="filter">
    /// A status filter that file associations must match to be included in the collection.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns>
    /// A collection of <see cref="VectorStoreFileAssociation"/> instances that can be asynchronously enumerated via
    /// <c>await foreach</c>.
    /// </returns>
    public virtual AsyncPageableCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        string vectorStoreId,
        ListOrder? resultOrder = null,
        VectorStoreFileStatusFilter? filter = null, 
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        return CreateAsyncPageable<VectorStoreFileAssociation, InternalListVectorStoreFilesResponse>(
            (continuationToken, pageSize) => GetFileAssociationsAsync(
                vectorStoreId, pageSize, resultOrder?.ToString(), continuationToken, null, filter?.ToString(), cancellationToken.ToRequestOptions()));
    }

    /// <summary>
    /// Gets the collection of <see cref="VectorStoreFileAssociation"/> instances representing file inclusions in the
    /// specified vector store.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the vector store to enumerate the file associations of.
    /// </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="filter">
    /// A status filter that file associations must match to be included in the collection.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns>
    /// A collection of <see cref="VectorStoreFileAssociation"/> instances that can be synchronously enumerated via
    /// <c>foreach</c>.
    /// </returns>
    public virtual PageableCollection<VectorStoreFileAssociation> GetFileAssociations(string vectorStoreId, ListOrder? resultOrder = null, VectorStoreFileStatusFilter? filter = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        return CreatePageable<VectorStoreFileAssociation, InternalListVectorStoreFilesResponse>(
            (continuationToken, pageSize) => GetFileAssociations(
                vectorStoreId, pageSize, resultOrder?.ToString(), continuationToken, null, filter?.ToString(), cancellationToken.ToRequestOptions()));
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
    /// Begins a batch job to associate multiple jobs with a vector store, beginning the ingestion process.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to associate files with. </param>
    /// <param name="fileIds"> The IDs of the files to associate with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the batch operation. </returns>
    public virtual async Task<ClientResult<VectorStoreBatchFileJob>> CreateBatchFileJobAsync(string vectorStoreId, IEnumerable<string> fileIds, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileIds, nameof(fileIds));

        BinaryContent content = new InternalCreateVectorStoreFileBatchRequest(fileIds).ToBinaryContent();
        ClientResult result = await CreateBatchFileJobAsync(vectorStoreId, content, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Begins a batch job to associate multiple jobs with a vector store, beginning the ingestion process.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to associate files with. </param>
    /// <param name="fileIds"> The IDs of the files to associate with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the batch operation. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> CreateBatchFileJob(string vectorStoreId, IEnumerable<string> fileIds, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileIds, nameof(fileIds));

        BinaryContent content = new InternalCreateVectorStoreFileBatchRequest(fileIds).ToBinaryContent();
        ClientResult result = CreateBatchFileJob(vectorStoreId, content, cancellationToken.ToRequestOptions());
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
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
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
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
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
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
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
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
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Gets the collection of file associations associated with a vector store batch file job, representing the files
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the vector store into which the file batch was scheduled for ingestion.
    /// </param>
    /// <param name="batchJobId">
    /// The ID of the batch file job that was previously scheduled.
    /// </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="filter">
    /// A status filter that file associations must match to be included in the collection.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns>
    /// A collection of <see cref="VectorStoreFileAssociation"/> instances that can be asynchronously enumerated via
    /// <c>await foreach</c>.
    /// </returns>
    public virtual AsyncPageableCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        string vectorStoreId,
        string batchJobId,
        ListOrder? resultOrder = null,
        VectorStoreFileStatusFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchJobId, nameof(batchJobId));

        return CreateAsyncPageable<VectorStoreFileAssociation, InternalListVectorStoreFilesResponse>(
            (continuationToken, pageSize) => GetFileAssociationsAsync
                (vectorStoreId, batchJobId, pageSize, resultOrder?.ToString(), continuationToken, null, filter?.ToString(), cancellationToken.ToRequestOptions()));
    }

    /// <summary>
    /// Gets the collection of file associations associated with a vector store batch file job, representing the files
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the vector store into which the file batch was scheduled for ingestion.
    /// </param>
    /// <param name="batchJobId">
    /// The ID of the batch file job that was previously scheduled.
    /// </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="filter">
    /// A status filter that file associations must match to be included in the collection.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns>
    /// A collection of <see cref="VectorStoreFileAssociation"/> instances that can be synchronously enumerated via
    /// <c>foreach</c>.
    /// </returns>
    public virtual PageableCollection<VectorStoreFileAssociation> GetFileAssociations(
        string vectorStoreId,
        string batchJobId,
        ListOrder? resultOrder = null,
        VectorStoreFileStatusFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchJobId, nameof(batchJobId));

        return CreatePageable<VectorStoreFileAssociation, InternalListVectorStoreFilesResponse>(
            (continuationToken, pageSize) => GetFileAssociations
                (vectorStoreId, batchJobId, pageSize, resultOrder?.ToString(), continuationToken, null, filter?.ToString(), cancellationToken.ToRequestOptions()));
    }
}
