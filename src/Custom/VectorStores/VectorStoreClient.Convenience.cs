using OpenAI.Files;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.VectorStores;

public partial class VectorStoreClient
{
    /// <summary>
    /// Modifies an existing vector store.
    /// </summary>
    /// <param name="vectorStore"> The vector store to modify. </param>
    /// <param name="options"> The new options to apply to the vector store. </param>
    /// <returns> The modified vector store instance. </returns>
    public virtual Task<ClientResult<VectorStore>> ModifyVectorStoreAsync(VectorStore vectorStore, VectorStoreModificationOptions options)
        => ModifyVectorStoreAsync(vectorStore?.Id, options);

    /// <summary>
    /// Modifies an existing vector store.
    /// </summary>
    /// <param name="vectorStore"> The vector store to modify. </param>
    /// <param name="options"> The new options to apply to the vector store. </param>
    /// <returns> The modified vector store instance. </returns>
    public virtual ClientResult<VectorStore> ModifyVectorStore(VectorStore vectorStore, VectorStoreModificationOptions options)
        => ModifyVectorStore(vectorStore?.Id, options);

    /// <summary>
    /// Gets an up-to-date instance of an existing vector store.
    /// </summary>
    /// <param name="vectorStore"> The existing vector store instance to get an updated instance of. </param>
    /// <returns> The refreshed vector store instance. </returns>
    public virtual Task<ClientResult<VectorStore>> GetVectorStoreAsync(VectorStore vectorStore)
        => GetVectorStoreAsync(vectorStore?.Id);

    /// <summary>
    /// Gets an up-to-date instance of an existing vector store.
    /// </summary>
    /// <param name="vectorStore"> The existing vector store instance to get an updated instance of. </param>
    /// <returns> The refreshed vector store instance. </returns>
    public virtual ClientResult<VectorStore> GetVectorStore(VectorStore vectorStore)
        => GetVectorStore(vectorStore?.Id);

    /// <summary>
    /// Deletes a vector store.
    /// </summary>
    /// <param name="vectorStore"> The vector store to delete. </param>
    /// <returns> A value indicating whether the deletion operation was successful. </returns>
    public virtual Task<ClientResult<bool>> DeleteVectorStoreAsync(VectorStore vectorStore)
        => DeleteVectorStoreAsync(vectorStore?.Id);

    /// <summary>
    /// Deletes a vector store.
    /// </summary>
    /// <param name="vectorStore"> The vector store to delete. </param>
    /// <returns> A value indicating whether the deletion operation was successful. </returns>
    public virtual ClientResult<bool> DeleteVectorStore(VectorStore vectorStore)
        => DeleteVectorStore(vectorStore?.Id);

    /// <summary>
    /// Associates an uploaded file with a vector store, beginning ingestion of the file into the vector store.
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStore"> The vector store to associate the file with. </param>
    /// <param name="file"> The file to associate with the vector store. </param>
    /// <returns> A <see cref="AddFileToVectorStoreOperation"/> that can be used to wait for 
    /// the vector store file addition to complete. </returns>
    public async virtual Task<AddFileToVectorStoreOperation> AddFileToVectorStoreAsync(bool waitUntilCompleted, VectorStore vectorStore, OpenAIFileInfo file)
        => await AddFileToVectorStoreAsync(waitUntilCompleted, vectorStore?.Id, file?.Id).ConfigureAwait(false);

    /// <summary>
    /// Associates an uploaded file with a vector store, beginning ingestion of the file into the vector store.
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStore"> The vector store to associate the file with. </param>
    /// <param name="file"> The file to associate with the vector store. </param>
    /// <returns> A <see cref="AddFileToVectorStoreOperation"/> that can be used to wait for 
    /// the vector store file addition to complete. </returns>
    public virtual AddFileToVectorStoreOperation AddFileToVectorStore(bool waitUntilCompleted, VectorStore vectorStore, OpenAIFileInfo file)
        => AddFileToVectorStore(waitUntilCompleted, vectorStore?.Id, file?.Id);

    /// <summary>
    /// Gets a page collection holding <see cref="VectorStoreFileAssociation"/> instances that represent file inclusions in the
    /// specified vector store.
    /// </summary>
    /// <param name="vectorStore">
    /// The vector store to enumerate the file associations of.
    /// </param>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <remarks> <see cref="AsyncPageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="AsyncPageCollection{T}.GetAllValuesAsync(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="AsyncPageCollection{T}.GetCurrentPageAsync"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual AsyncPageCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        VectorStore vectorStore,
        VectorStoreFileAssociationCollectionOptions options = default)
            => GetFileAssociationsAsync(vectorStore?.Id, options);

    /// <summary>
    /// Gets a page collection holding <see cref="VectorStoreFileAssociation"/> instances that represent file inclusions in the
    /// specified vector store.
    /// </summary>
    /// <param name="vectorStore">
    /// The ID vector store to enumerate the file associations of.
    /// </param>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <remarks> <see cref="PageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="PageCollection{T}.GetAllValues(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="PageCollection{T}.GetCurrentPage"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual PageCollection<VectorStoreFileAssociation> GetFileAssociations(
        VectorStore vectorStore,
        VectorStoreFileAssociationCollectionOptions options = default)
            => GetFileAssociations(vectorStore?.Id, options);

    /// <summary>
    /// Gets a <see cref="VectorStoreFileAssociation"/> instance representing an existing association between a known
    /// vector store and file.
    /// </summary>
    /// <param name="vectorStore"> The vector store associated with the file. </param>
    /// <param name="file"> The file associated with the vector store. </param>
    /// <returns> A <see cref="VectorStoreFileAssociation"/> instance. </returns>
    public virtual Task<ClientResult<VectorStoreFileAssociation>> GetFileAssociationAsync(
        VectorStore vectorStore,
        OpenAIFileInfo file)
            => GetFileAssociationAsync(vectorStore?.Id, file?.Id);

    /// <summary>
    /// Gets a <see cref="VectorStoreFileAssociation"/> instance representing an existing association between a known
    /// vector store and file.
    /// </summary>
    /// <param name="vectorStore"> The vector store associated with the file. </param>
    /// <param name="file"> The file associated with the vector store. </param>
    /// <returns> A <see cref="VectorStoreFileAssociation"/> instance. </returns>
    public virtual ClientResult<VectorStoreFileAssociation> GetFileAssociation(
        VectorStore vectorStore,
        OpenAIFileInfo file)
            => GetFileAssociation(vectorStore?.Id, file?.Id);

    /// <summary>
    /// Removes the association between a file and vector store, which makes the file no longer available to the vector
    /// store.
    /// </summary>
    /// <remarks>
    /// This does not delete the file. To delete the file, use <see cref="FileClient.DeleteFile(OpenAIFileInfo)"/>.
    /// </remarks>
    /// <param name="vectorStore"> The vector store that the file should be removed from. </param>
    /// <param name="file"> The file to remove from the vector store. </param>
    /// <returns> A value indicating whether the removal operation was successful. </returns>
    public virtual Task<ClientResult<bool>> RemoveFileFromStoreAsync(VectorStore vectorStore, OpenAIFileInfo file)
        => RemoveFileFromStoreAsync(vectorStore?.Id, file?.Id);

    /// <summary>
    /// Removes the association between a file and vector store, which makes the file no longer available to the vector
    /// store.
    /// </summary>
    /// <remarks>
    /// This does not delete the file. To delete the file, use <see cref="FileClient.DeleteFile(OpenAIFileInfo)"/>.
    /// </remarks>
    /// <param name="vectorStore"> The vector store that the file should be removed from. </param>
    /// <param name="file"> The file to remove from the vector store. </param>
    /// <returns> A value indicating whether the removal operation was successful. </returns>
    public virtual ClientResult<bool> RemoveFileFromStore(VectorStore vectorStore, OpenAIFileInfo file)
        => RemoveFileFromStore(vectorStore?.Id, file?.Id);

    /// <summary>
    /// Begins a batch job to associate multiple jobs with a vector store, beginning the ingestion process.
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStore"> The vector store to associate files with. </param>
    /// <param name="files"> The files to associate with the vector store. </param>
    /// <returns> A <see cref="CreateBatchFileJobOperation"/> that can be used to wait for 
    /// the operation to complete, get information about the batch file job, or cancel the operation. </returns>
    public virtual Task<CreateBatchFileJobOperation> CreateBatchFileJobAsync(bool waitUntilCompleted, VectorStore vectorStore, IEnumerable<OpenAIFileInfo> files)
        => CreateBatchFileJobAsync(waitUntilCompleted, vectorStore?.Id, files?.Select(file => file.Id));

    /// <summary>
    /// Begins a batch job to associate multiple jobs with a vector store, beginning the ingestion process.
    /// </summary>
    /// <param name="waitUntilCompleted"> Value indicating whether the method
    /// should return after the operation has been started and is still running
    /// on the service, or wait until the operation has completed to return.
    /// </param>
    /// <param name="vectorStore"> The vector store to associate files with. </param>
    /// <param name="files"> The files to associate with the vector store. </param>
    /// <returns> A <see cref="CreateBatchFileJobOperation"/> that can be used to wait for 
    /// the operation to complete, get information about the batch file job, or cancel the operation. </returns>
    public virtual CreateBatchFileJobOperation CreateBatchFileJob(bool waitUntilCompleted, VectorStore vectorStore, IEnumerable<OpenAIFileInfo> files)
        => CreateBatchFileJob(waitUntilCompleted, vectorStore?.Id, files?.Select(file => file.Id));
}
