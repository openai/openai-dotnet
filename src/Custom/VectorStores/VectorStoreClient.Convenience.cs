using OpenAI.Files;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.VectorStores;

public partial class VectorStoreClient
{
    /// <summary>
    /// Modifies an existing vector store.
    /// </summary>
    /// <param name="vectorStore"> The vector store to modify. </param>
    /// <param name="options"> The new options to apply to the vector store. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> The modified vector store instance. </returns>
    public virtual Task<ClientResult<VectorStore>> ModifyVectorStoreAsync(VectorStore vectorStore, VectorStoreModificationOptions options, CancellationToken cancellationToken = default)
        => ModifyVectorStoreAsync(vectorStore?.Id, options, cancellationToken);

    /// <summary>
    /// Modifies an existing vector store.
    /// </summary>
    /// <param name="vectorStore"> The vector store to modify. </param>
    /// <param name="options"> The new options to apply to the vector store. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> The modified vector store instance. </returns>
    public virtual ClientResult<VectorStore> ModifyVectorStore(VectorStore vectorStore, VectorStoreModificationOptions options, CancellationToken cancellationToken = default)
        => ModifyVectorStore(vectorStore?.Id, options, cancellationToken);

    /// <summary>
    /// Gets an up-to-date instance of an existing vector store.
    /// </summary>
    /// <param name="vectorStore"> The existing vector store instance to get an updated instance of. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> The refreshed vector store instance. </returns>
    public virtual Task<ClientResult<VectorStore>> GetVectorStoreAsync(VectorStore vectorStore, CancellationToken cancellationToken = default)
        => GetVectorStoreAsync(vectorStore?.Id, cancellationToken);

    /// <summary>
    /// Gets an up-to-date instance of an existing vector store.
    /// </summary>
    /// <param name="vectorStore"> The existing vector store instance to get an updated instance of. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> The refreshed vector store instance. </returns>
    public virtual ClientResult<VectorStore> GetVectorStore(VectorStore vectorStore, CancellationToken cancellationToken = default)
        => GetVectorStore(vectorStore?.Id, cancellationToken);

    /// <summary>
    /// Deletes a vector store.
    /// </summary>
    /// <param name="vectorStore"> The vector store to delete. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A value indicating whether the deletion operation was successful. </returns>
    public virtual Task<ClientResult<bool>> DeleteVectorStoreAsync(VectorStore vectorStore, CancellationToken cancellationToken = default)
        => DeleteVectorStoreAsync(vectorStore?.Id, cancellationToken);

    /// <summary>
    /// Deletes a vector store.
    /// </summary>
    /// <param name="vectorStore"> The vector store to delete. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A value indicating whether the deletion operation was successful. </returns>
    public virtual ClientResult<bool> DeleteVectorStore(VectorStore vectorStore, CancellationToken cancellationToken = default)
        => DeleteVectorStore(vectorStore?.Id, cancellationToken);

    /// <summary>
    /// Associates an uploaded file with a vector store, beginning ingestion of the file into the vector store.
    /// </summary>
    /// <param name="vectorStore"> The vector store to associate the file with. </param>
    /// <param name="file"> The file to associate with the vector store. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns>
    /// A <see cref="VectorStoreFileAssociation"/> instance that represents the new association.
    /// </returns>
    public virtual Task<ClientResult<VectorStoreFileAssociation>> AddFileToVectorStoreAsync(VectorStore vectorStore, OpenAIFileInfo file, CancellationToken cancellationToken = default)
        => AddFileToVectorStoreAsync(vectorStore?.Id, file?.Id, cancellationToken);

    /// <summary>
    /// Associates an uploaded file with a vector store, beginning ingestion of the file into the vector store.
    /// </summary>
    /// <param name="vectorStore"> The vector store to associate the file with. </param>
    /// <param name="file"> The file to associate with the vector store. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns>
    /// A <see cref="VectorStoreFileAssociation"/> instance that represents the new association.
    /// </returns>
    public virtual ClientResult<VectorStoreFileAssociation> AddFileToVectorStore(VectorStore vectorStore, OpenAIFileInfo file, CancellationToken cancellationToken = default)
        => AddFileToVectorStore(vectorStore?.Id, file?.Id, cancellationToken);

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
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns>
    /// A collection of <see cref="VectorStoreFileAssociation"/> instances that can be asynchronously enumerated via
    /// <c>await foreach</c>.
    /// </returns>
    public virtual AsyncPageableCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        VectorStore vectorStore,
        ListOrder? resultOrder = null,
        VectorStoreFileStatusFilter? filter = null,
        CancellationToken cancellationToken = default)
            => GetFileAssociationsAsync(vectorStore?.Id, resultOrder, filter, cancellationToken);

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
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns>
    /// A collection of <see cref="VectorStoreFileAssociation"/> instances that can be synchronously enumerated via
    /// <c>foreach</c>.
    /// </returns>
    public virtual PageableCollection<VectorStoreFileAssociation> GetFileAssociations(
        VectorStore vectorStore,
        ListOrder? resultOrder = null,
        VectorStoreFileStatusFilter? filter = null,
        CancellationToken cancellationToken = default)
            => GetFileAssociations(vectorStore?.Id, resultOrder, filter, cancellationToken);

    /// <summary>
    /// Gets a <see cref="VectorStoreFileAssociation"/> instance representing an existing association between a known
    /// vector store and file.
    /// </summary>
    /// <param name="vectorStore"> The vector store associated with the file. </param>
    /// <param name="file"> The file associated with the vector store. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A <see cref="VectorStoreFileAssociation"/> instance. </returns>
    public virtual Task<ClientResult<VectorStoreFileAssociation>> GetFileAssociationAsync(
        VectorStore vectorStore,
        OpenAIFileInfo file, CancellationToken cancellationToken = default)
            => GetFileAssociationAsync(vectorStore?.Id, file?.Id, cancellationToken);

    /// <summary>
    /// Gets a <see cref="VectorStoreFileAssociation"/> instance representing an existing association between a known
    /// vector store and file.
    /// </summary>
    /// <param name="vectorStore"> The vector store associated with the file. </param>
    /// <param name="file"> The file associated with the vector store. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A <see cref="VectorStoreFileAssociation"/> instance. </returns>
    public virtual ClientResult<VectorStoreFileAssociation> GetFileAssociation(
        VectorStore vectorStore,
        OpenAIFileInfo file, CancellationToken cancellationToken = default)
            => GetFileAssociation(vectorStore?.Id, file?.Id, cancellationToken);

    /// <summary>
    /// Removes the association between a file and vector store, which makes the file no longer available to the vector
    /// store.
    /// </summary>
    /// <remarks>
    /// This does not delete the file. To delete the file, use <see cref="FileClient.DeleteFile(OpenAIFileInfo)"/>.
    /// </remarks>
    /// <param name="vectorStore"> The vector store that the file should be removed from. </param>
    /// <param name="file"> The file to remove from the vector store. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A value indicating whether the removal operation was successful. </returns>
    public virtual Task<ClientResult<bool>> RemoveFileFromStoreAsync(VectorStore vectorStore, OpenAIFileInfo file, CancellationToken cancellationToken = default)
        => RemoveFileFromStoreAsync(vectorStore?.Id, file?.Id, cancellationToken);

    /// <summary>
    /// Removes the association between a file and vector store, which makes the file no longer available to the vector
    /// store.
    /// </summary>
    /// <remarks>
    /// This does not delete the file. To delete the file, use <see cref="FileClient.DeleteFile(OpenAIFileInfo)"/>.
    /// </remarks>
    /// <param name="vectorStore"> The vector store that the file should be removed from. </param>
    /// <param name="file"> The file to remove from the vector store. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A value indicating whether the removal operation was successful. </returns>
    public virtual ClientResult<bool> RemoveFileFromStore(VectorStore vectorStore, OpenAIFileInfo file, CancellationToken cancellationToken = default)
        => RemoveFileFromStore(vectorStore?.Id, file?.Id, cancellationToken);

    /// <summary>
    /// Begins a batch job to associate multiple jobs with a vector store, beginning the ingestion process.
    /// </summary>
    /// <param name="vectorStore"> The vector store to associate files with. </param>
    /// <param name="files"> The files to associate with the vector store. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the batch operation. </returns>
    public virtual Task<ClientResult<VectorStoreBatchFileJob>> CreateBatchFileJobAsync(VectorStore vectorStore, IEnumerable<OpenAIFileInfo> files, CancellationToken cancellationToken = default)
        => CreateBatchFileJobAsync(vectorStore?.Id, files?.Select(file => file.Id), cancellationToken);

    /// <summary>
    /// Begins a batch job to associate multiple jobs with a vector store, beginning the ingestion process.
    /// </summary>
    /// <param name="vectorStore"> The vector store to associate files with. </param>
    /// <param name="files"> The files to associate with the vector store. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the batch operation. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> CreateBatchFileJob(VectorStore vectorStore, IEnumerable<OpenAIFileInfo> files, CancellationToken cancellationToken = default)
        => CreateBatchFileJob(vectorStore?.Id, files?.Select(file => file.Id), cancellationToken);

    /// <summary>
    /// Gets an updated instance of an existing <see cref="VectorStoreBatchFileJob"/>, refreshing its status.
    /// </summary>
    /// <param name="batchJob"> The job to refresh. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> The refreshed instance of <see cref="VectorStoreBatchFileJob"/>. </returns>
    public virtual Task<ClientResult<VectorStoreBatchFileJob>> GetBatchFileJobAsync(VectorStoreBatchFileJob batchJob, CancellationToken cancellationToken = default)
        => GetBatchFileJobAsync(batchJob?.VectorStoreId, batchJob?.BatchId, cancellationToken);

    /// <summary>
    /// Gets an updated instance of an existing <see cref="VectorStoreBatchFileJob"/>, refreshing its status.
    /// </summary>
    /// <param name="batchJob"> The job to refresh. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> The refreshed instance of <see cref="VectorStoreBatchFileJob"/>. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> GetBatchFileJob(VectorStoreBatchFileJob batchJob, CancellationToken cancellationToken = default)
        => GetBatchFileJob(batchJob?.VectorStoreId, batchJob?.BatchId, cancellationToken);

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="batchJob"> The <see cref="VectorStoreBatchFileJob"/> that should be canceled. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual Task<ClientResult<VectorStoreBatchFileJob>> CancelBatchFileJobAsync(VectorStoreBatchFileJob batchJob, CancellationToken cancellationToken = default)
        => CancelBatchFileJobAsync(batchJob?.VectorStoreId, batchJob?.BatchId, cancellationToken);

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="batchJob"> The <see cref="VectorStoreBatchFileJob"/> that should be canceled. </param>
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> CancelBatchFileJob(VectorStoreBatchFileJob batchJob, CancellationToken cancellationToken = default)
        => CancelBatchFileJob(batchJob?.VectorStoreId, batchJob?.BatchId, cancellationToken);

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
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns>
    /// A collection of <see cref="VectorStoreFileAssociation"/> instances that can be asynchronously enumerated via
    /// <c>await foreach</c>.
    /// </returns>
    public virtual AsyncPageableCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        VectorStoreBatchFileJob batchJob,
        ListOrder? resultOrder = null,
        VectorStoreFileStatusFilter? filter = null, CancellationToken cancellationToken = default)
            => GetFileAssociationsAsync(batchJob?.VectorStoreId, batchJob?.BatchId, resultOrder, filter, cancellationToken);

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
    /// <param name="cancellationToken"> A token that can be used to cancel this method call. </param>
    /// <returns>
    /// A collection of <see cref="VectorStoreFileAssociation"/> instances that can be synchronously enumerated via
    /// <c>foreach</c>.
    /// </returns>
    public virtual PageableCollection<VectorStoreFileAssociation> GetFileAssociations(
        VectorStoreBatchFileJob batchJob,
        ListOrder? resultOrder = null,
        VectorStoreFileStatusFilter? filter = null, CancellationToken cancellationToken = default)
            => GetFileAssociations(batchJob?.VectorStoreId, batchJob?.BatchId, resultOrder, filter, cancellationToken);
}
