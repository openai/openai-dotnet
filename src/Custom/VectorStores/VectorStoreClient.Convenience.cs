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
    /// <param name="vectorStore"> The vector store to associate the file with. </param>
    /// <param name="file"> The file to associate with the vector store. </param>
    /// <returns>
    /// A <see cref="VectorStoreFileAssociation"/> instance that represents the new association.
    /// </returns>
    public virtual Task<ClientResult<VectorStoreFileAssociation>> AddFileToVectorStoreAsync(VectorStore vectorStore, OpenAIFileInfo file)
        => AddFileToVectorStoreAsync(vectorStore?.Id, file?.Id);

    /// <summary>
    /// Associates an uploaded file with a vector store, beginning ingestion of the file into the vector store.
    /// </summary>
    /// <param name="vectorStore"> The vector store to associate the file with. </param>
    /// <param name="file"> The file to associate with the vector store. </param>
    /// <returns>
    /// A <see cref="VectorStoreFileAssociation"/> instance that represents the new association.
    /// </returns>
    public virtual ClientResult<VectorStoreFileAssociation> AddFileToVectorStore(VectorStore vectorStore, OpenAIFileInfo file)
        => AddFileToVectorStore(vectorStore?.Id, file?.Id);

    /// <summary>
    /// Gets the collection of <see cref="VectorStoreFileAssociation"/> instances representing file inclusions in the
    /// specified vector store.
    /// </summary>
    /// <param name="vectorStore">
    /// The vector store to enumerate the file associations of.
    /// </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="filter">
    /// A status filter that file associations must match to be included in the collection.
    /// </param>
    /// <returns>
    /// A collection of <see cref="VectorStoreFileAssociation"/> instances that can be asynchronously enumerated via
    /// <c>await foreach</c>.
    /// </returns>
    public virtual AsyncPageableCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        VectorStore vectorStore,
        ListOrder? resultOrder = null,
        VectorStoreFileStatusFilter? filter = null)
            => GetFileAssociationsAsync(vectorStore?.Id, resultOrder, filter);

    /// <summary>
    /// Gets the collection of <see cref="VectorStoreFileAssociation"/> instances representing file inclusions in the
    /// specified vector store.
    /// </summary>
    /// <param name="vectorStore">
    /// The ID vector store to enumerate the file associations of.
    /// </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="filter">
    /// A status filter that file associations must match to be included in the collection.
    /// </param>
    /// <returns>
    /// A collection of <see cref="VectorStoreFileAssociation"/> instances that can be synchronously enumerated via
    /// <c>foreach</c>.
    /// </returns>
    public virtual PageableCollection<VectorStoreFileAssociation> GetFileAssociations(
        VectorStore vectorStore,
        ListOrder? resultOrder = null,
        VectorStoreFileStatusFilter? filter = null)
            => GetFileAssociations(vectorStore?.Id, resultOrder);

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
    /// <param name="vectorStore"> The vector store to associate files with. </param>
    /// <param name="files"> The files to associate with the vector store. </param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the batch operation. </returns>
    public virtual Task<ClientResult<VectorStoreBatchFileJob>> CreateBatchFileJobAsync(VectorStore vectorStore, IEnumerable<OpenAIFileInfo> files)
        => CreateBatchFileJobAsync(vectorStore?.Id, files?.Select(file => file.Id));

    /// <summary>
    /// Begins a batch job to associate multiple jobs with a vector store, beginning the ingestion process.
    /// </summary>
    /// <param name="vectorStore"> The vector store to associate files with. </param>
    /// <param name="files"> The files to associate with the vector store. </param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the batch operation. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> CreateBatchFileJob(VectorStore vectorStore, IEnumerable<OpenAIFileInfo> files)
        => CreateBatchFileJob(vectorStore?.Id, files?.Select(file => file.Id));

    /// <summary>
    /// Gets an updated instance of an existing <see cref="VectorStoreBatchFileJob"/>, refreshing its status.
    /// </summary>
    /// <param name="batchJob"> The job to refresh. </param>
    /// <returns> The refreshed instance of <see cref="VectorStoreBatchFileJob"/>. </returns>
    public virtual Task<ClientResult<VectorStoreBatchFileJob>> GetBatchFileJobAsync(VectorStoreBatchFileJob batchJob)
        => GetBatchFileJobAsync(batchJob?.VectorStoreId, batchJob?.BatchId);

    /// <summary>
    /// Gets an updated instance of an existing <see cref="VectorStoreBatchFileJob"/>, refreshing its status.
    /// </summary>
    /// <param name="batchJob"> The job to refresh. </param>
    /// <returns> The refreshed instance of <see cref="VectorStoreBatchFileJob"/>. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> GetBatchFileJob(VectorStoreBatchFileJob batchJob)
        => GetBatchFileJob(batchJob?.VectorStoreId, batchJob?.BatchId);

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="batchJob"> The <see cref="VectorStoreBatchFileJob"/> that should be canceled. </param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual Task<ClientResult<VectorStoreBatchFileJob>> CancelBatchFileJobAsync(VectorStoreBatchFileJob batchJob)
        => CancelBatchFileJobAsync(batchJob?.VectorStoreId, batchJob?.BatchId);

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="batchJob"> The <see cref="VectorStoreBatchFileJob"/> that should be canceled. </param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> CancelBatchFileJob(VectorStoreBatchFileJob batchJob)
        => CancelBatchFileJob(batchJob?.VectorStoreId, batchJob?.BatchId);

    /// <summary>
    /// Gets the collection of file associations associated with a vector store batch file job, representing the files
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="filter">
    /// A status filter that file associations must match to be included in the collection.
    /// </param>
    /// <returns>
    /// A collection of <see cref="VectorStoreFileAssociation"/> instances that can be asynchronously enumerated via
    /// <c>await foreach</c>.
    /// </returns>
    public virtual AsyncPageableCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        VectorStoreBatchFileJob batchJob,
        ListOrder? resultOrder = null,
        VectorStoreFileStatusFilter? filter = null)
            => GetFileAssociationsAsync(batchJob?.VectorStoreId, batchJob?.BatchId, resultOrder, filter);

    /// <summary>
    /// Gets the collection of file associations associated with a vector store batch file job, representing the files
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="batchJob"> The vector store batch file job to retrieve file associations from. </param>
    /// <param name="resultOrder">
    /// The <c>order</c> that results should appear in the list according to their <c>created_at</c>
    /// timestamp.
    /// </param>
    /// <param name="filter">
    /// A status filter that file associations must match to be included in the collection.
    /// </param>
    /// <returns>
    /// A collection of <see cref="VectorStoreFileAssociation"/> instances that can be synchronously enumerated via
    /// <c>foreach</c>.
    /// </returns>
    public virtual PageableCollection<VectorStoreFileAssociation> GetFileAssociations(
        VectorStoreBatchFileJob batchJob,
        ListOrder? resultOrder = null,
        VectorStoreFileStatusFilter? filter = null)
            => GetFileAssociations(batchJob?.VectorStoreId, batchJob?.BatchId, resultOrder, filter);

}
