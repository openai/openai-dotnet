using Microsoft.TypeSpec.Generator.Customizations;
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
[CodeGenSuppress("AddFileToVectorStore", typeof(string), typeof(InternalCreateVectorStoreFileRequest), typeof(CancellationToken))]
[CodeGenSuppress("AddFileToVectorStoreAsync", typeof(string), typeof(InternalCreateVectorStoreFileRequest), typeof(CancellationToken))]
[CodeGenSuppress("AddFileBatchToVectorStore", typeof(string), typeof(InternalCreateVectorStoreFileBatchRequest), typeof(CancellationToken))]
[CodeGenSuppress("AddFileBatchToVectorStoreAsync", typeof(string), typeof(InternalCreateVectorStoreFileBatchRequest), typeof(CancellationToken))]
[CodeGenSuppress("RemoveFileFromVectorStore", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("RemoveFileFromVectorStoreAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetVectorStoreFile", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetVectorStoreFileAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetVectorStoreFileBatch", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("GetVectorStoreFileBatchAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelVectorStoreFileBatchAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("CancelVectorStoreFileBatch", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("RetrieveVectorStoreFileContent", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("RetrieveVectorStoreFileContentAsync", typeof(string), typeof(string), typeof(CancellationToken))]
[CodeGenSuppress("SearchVectorStore", typeof(string), typeof(BinaryData), typeof(bool?), typeof(int?), typeof(BinaryData), typeof(InternalVectorStoreSearchRequestRankingOptions), typeof(CancellationToken))]
[CodeGenSuppress("SearchVectorStoreAsync", typeof(string), typeof(BinaryData), typeof(bool?), typeof(int?), typeof(BinaryData), typeof(InternalVectorStoreSearchRequestRankingOptions), typeof(CancellationToken))]
[CodeGenSuppress("VectorStoreClient", typeof(ClientPipeline), typeof(Uri))]
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
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="credential"/> is null. </exception>
    public VectorStoreClient(ApiKeyCredential credential) : this(credential, new OpenAIClientOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VectorStoreClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="credential"> The <see cref="ApiKeyCredential"/> to authenticate with the service. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public VectorStoreClient(ApiKeyCredential credential, OpenAIClientOptions options) : this(OpenAIClient.CreateApiKeyAuthenticationPolicy(credential), options)
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="VectorStoreClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public VectorStoreClient(AuthenticationPolicy authenticationPolicy) : this(authenticationPolicy, new OpenAIClientOptions())
    {
    }

    // CUSTOM: Added as a convenience.
    /// <summary> Initializes a new instance of <see cref="VectorStoreClient"/>. </summary>
    /// <param name="authenticationPolicy"> The authentication policy used to authenticate with the service. </param>
    /// <param name="options"> The options to configure the client. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="authenticationPolicy"/> is null. </exception>
    public VectorStoreClient(AuthenticationPolicy authenticationPolicy, OpenAIClientOptions options)
    {
        Argument.AssertNotNull(authenticationPolicy, nameof(authenticationPolicy));
        options ??= new OpenAIClientOptions();

        Pipeline = OpenAIClient.CreatePipeline(authenticationPolicy, options);
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

    [Experimental("SCME0002")]
    public VectorStoreClient(VectorStoreClientSettings settings)
        : this(AuthenticationPolicy.Create(settings), settings?.Options)
    {
    }

    /// <summary>
    /// Gets the endpoint URI for the service.
    /// </summary>
    [Experimental("OPENAI001")]
    public Uri Endpoint => _endpoint;

    /// <summary> Creates a vector store. </summary>
    /// <param name="options"> The <see cref="VectorStoreCreationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="options"/> is null. </exception>
    /// <returns> A <see cref="VectorStore"/> instance. </returns>
    public virtual async Task<ClientResult<VectorStore>> CreateVectorStoreAsync(VectorStoreCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        using BinaryContent content = options != null ? options.ToBinaryContent() : new VectorStoreCreationOptions().ToBinaryContent();
        ClientResult result = await CreateVectorStoreAsync(content, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
        return ClientResult.FromValue((VectorStore)result, result.GetRawResponse());
    }

    /// <summary> Creates a vector store. </summary>
    /// <param name="options"> The <see cref="VectorStoreCreationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="options"/> is null. </exception>
    /// <returns> A <see cref="VectorStore"/> instance. </returns>
    public virtual ClientResult<VectorStore> CreateVectorStore(VectorStoreCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        using BinaryContent content = options != null ? options.ToBinaryContent() : new VectorStoreCreationOptions().ToBinaryContent();
        ClientResult result = CreateVectorStore(content, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((VectorStore)result, result.GetRawResponse());
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
            (VectorStore)result, result.GetRawResponse());
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
        return ClientResult.FromValue((VectorStore)result, result.GetRawResponse());
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
        return ClientResult.FromValue((VectorStore)result, result.GetRawResponse());
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
        return ClientResult.FromValue((VectorStore)result, result.GetRawResponse());
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
        VectorStoreDeletionResult value = (VectorStoreDeletionResult)protocolResult;
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
        VectorStoreDeletionResult value = (VectorStoreDeletionResult)protocolResult;
        return ClientResult.FromValue(value, rawProtocolResponse);
    }

    /// <summary>
    /// Associates a single, uploaded file with a vector store, beginning ingestion of the file into the vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to associate the file with. </param>
    /// <param name="fileId"> The ID of the file to associate with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreFile"/> instance. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="fileId"/> is null. </exception>
    public virtual async Task<ClientResult<VectorStoreFile>> AddFileToVectorStoreAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        InternalCreateVectorStoreFileRequest internalRequest = new(fileId);
        ClientResult result = await AddFileToVectorStoreAsync(vectorStoreId, internalRequest.ToBinaryContent(), cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
        return ClientResult.FromValue((VectorStoreFile)result, result.GetRawResponse());
    }

    /// <summary>
    /// Associates a single, uploaded file with a vector store, beginning ingestion of the file into the vector store.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to associate the file with. </param>
    /// <param name="fileId"> The ID of the file to associate with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreFile"/> instance. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="vectorStoreId"/> or <paramref name="fileId"/> is null. </exception>
    public virtual ClientResult<VectorStoreFile> AddFileToVectorStore(string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        InternalCreateVectorStoreFileRequest internalRequest = new(fileId);
        ClientResult result = AddFileToVectorStore(vectorStoreId, internalRequest.ToBinaryContent(), cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((VectorStoreFile)result, result.GetRawResponse());
    }

    public virtual ClientResult<VectorStoreFile> UpdateVectorStoreFileAttributes(string vectorStoreId, string fileId, IDictionary<string, BinaryData> attributes, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        InternalUpdateVectorStoreFileAttributesRequest spreadModel = new InternalUpdateVectorStoreFileAttributesRequest(attributes, null);
        ClientResult result = UpdateVectorStoreFileAttributes(vectorStoreId, fileId, spreadModel, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((VectorStoreFile)result, result.GetRawResponse());
    }

    public virtual async Task<ClientResult<VectorStoreFile>> UpdateVectorStoreFileAttributesAsync(string vectorStoreId, string fileId, IDictionary<string, BinaryData> attributes, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        InternalUpdateVectorStoreFileAttributesRequest spreadModel = new InternalUpdateVectorStoreFileAttributesRequest(attributes, null);
        ClientResult result = await UpdateVectorStoreFileAttributesAsync(vectorStoreId, fileId, spreadModel, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
        return ClientResult.FromValue((VectorStoreFile)result, result.GetRawResponse());
    }

    /// <summary>
    /// Gets a <see cref="VectorStoreFile"/> instance representing an existing association between a known
    /// vector store ID and file ID.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store associated with the file. </param>
    /// <param name="fileId"> The ID of the file associated with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreFile"/> instance. </returns>
    public virtual async Task<ClientResult<VectorStoreFile>> GetVectorStoreFileAsync(
        string vectorStoreId,
        string fileId,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        ClientResult result = await GetVectorStoreFileAsync(vectorStoreId, fileId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreFile value = (VectorStoreFile)result;
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Gets a <see cref="VectorStoreFile"/> instance representing an existing association between a known
    /// vector store ID and file ID.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store associated with the file. </param>
    /// <param name="fileId"> The ID of the file associated with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreFile"/> instance. </returns>
    public virtual ClientResult<VectorStoreFile> GetVectorStoreFile(
        string vectorStoreId,
        string fileId,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileId, nameof(fileId));

        ClientResult result = GetVectorStoreFile(vectorStoreId, fileId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreFile value = (VectorStoreFile)result;
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
    public virtual async Task<ClientResult<FileFromStoreRemovalResult>> RemoveFileFromVectorStoreAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await RemoveFileFromVectorStoreAsync(vectorStoreId, fileId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse protocolResponse = protocolResult?.GetRawResponse();
        FileFromStoreRemovalResult value = (FileFromStoreRemovalResult)protocolResult;
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
    public virtual ClientResult<FileFromStoreRemovalResult> RemoveFileFromVectorStore(string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = RemoveFileFromVectorStore(vectorStoreId, fileId, cancellationToken.ToRequestOptions());
        PipelineResponse protocolResponse = protocolResult?.GetRawResponse();
        FileFromStoreRemovalResult value = (FileFromStoreRemovalResult)protocolResult;
        return ClientResult.FromValue(value, protocolResponse);
    }

    /// <summary>
    /// Adds multiple files in a batch to the vector store, beginning the ingestion process.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to associate files with. </param>
    /// <param name="fileIds"> The IDs of the files to associate with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreFileBatch"/> instance. </returns>
    public virtual async Task<ClientResult<VectorStoreFileBatch>> AddFileBatchToVectorStoreAsync(string vectorStoreId, IEnumerable<string> fileIds, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileIds, nameof(fileIds));

        var createFileBatchRequest = new InternalCreateVectorStoreFileBatchRequest(fileIds);
        ClientResult result = await AddFileBatchToVectorStoreAsync(vectorStoreId, createFileBatchRequest, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
        return ClientResult.FromValue((VectorStoreFileBatch)result, result.GetRawResponse());
    }

    /// <summary>
    /// Begins a batch job to associate multiple jobs with a vector store, beginning the ingestion process.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store to associate files with. </param>
    /// <param name="fileIds"> The IDs of the files to associate with the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreFileBatch"/> instance. </returns>
    public virtual ClientResult<VectorStoreFileBatch> AddFileBatchToVectorStore(string vectorStoreId, IEnumerable<string> fileIds, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(fileIds, nameof(fileIds));

        var createFileBatchRequest = new InternalCreateVectorStoreFileBatchRequest(fileIds);
        ClientResult result = AddFileBatchToVectorStore(vectorStoreId, createFileBatchRequest, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
        return ClientResult.FromValue((VectorStoreFileBatch)result, result.GetRawResponse());
    }

    /// <summary>
    /// Gets an existing vector store batch file ingestion job from a known vector store ID and batch ID.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store into which the batch of files was started. </param>
    /// <param name="batchId"> The ID of the batch operation adding files to the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreFileBatch"/> instance representing the ingestion operation. </returns>
    public virtual async Task<ClientResult<VectorStoreFileBatch>> GetVectorStoreFileBatchAsync(string vectorStoreId, string batchId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        ClientResult result = await GetVectorStoreFileBatchAsync(vectorStoreId, batchId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreFileBatch value = (VectorStoreFileBatch)result;
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Gets an existing vector store batch file ingestion job from a known vector store ID and batch ID.
    /// </summary>
    /// <param name="vectorStoreId"> The ID of the vector store into which the batch of files was started. </param>
    /// <param name="batchId"> The ID of the batch operation adding files to the vector store. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreFileBatch"/> instance representing the ingestion operation. </returns>
    public virtual ClientResult<VectorStoreFileBatch> GetVectorStoreFileBatch(string vectorStoreId, string batchId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        ClientResult result = GetVectorStoreFileBatch(vectorStoreId, batchId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreFileBatch value = (VectorStoreFileBatch)result;
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreFileBatch"/>.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the <see cref="VectorStore"/> that is the ingestion target of the batch job being cancelled.
    /// </param>
    /// <param name="batchId">
    /// The ID of the <see cref="VectorStoreFileBatch"/> that should be canceled.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="VectorStoreFileBatch"/> instance. </returns>
    public virtual async Task<ClientResult<VectorStoreFileBatch>> CancelVectorStoreFileBatchAsync(string vectorStoreId, string batchId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        ClientResult result = await CancelVectorStoreFileBatchAsync(vectorStoreId, batchId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreFileBatch value = (VectorStoreFileBatch)result;
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreFileBatch"/>.
    /// </summary>
    /// <param name="vectorStoreId">
    /// The ID of the <see cref="VectorStore"/> that is the ingestion target of the batch job being cancelled.
    /// </param>
    /// <param name="batchId">
    /// The ID of the <see cref="VectorStoreFileBatch"/> that should be canceled.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="VectorStoreFileBatch"/> instance. </returns>
    public virtual ClientResult<VectorStoreFileBatch> CancelVectorStoreFileBatch(string vectorStoreId, string batchId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(vectorStoreId, nameof(vectorStoreId));
        Argument.AssertNotNullOrEmpty(batchId, nameof(batchId));

        ClientResult result = CancelVectorStoreFileBatch(vectorStoreId, batchId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result?.GetRawResponse();
        VectorStoreFileBatch value = (VectorStoreFileBatch)result;
        return ClientResult.FromValue(value, response);
    }
}
