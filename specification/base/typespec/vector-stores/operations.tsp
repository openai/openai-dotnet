import "@typespec/http";
import "@typespec/openapi";

import "../common";
import "./custom.tsp";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("vector_stores")
@tag("Vector Stores")
interface VectorStores {
  @get
  @operationId("listVectorStores")
  @summary("Returns a list of vector stores.")
  @list
  listVectorStores(
    ...AcceptJsonHeader,
    ...CommonPageQueryParameters,
  ): ListVectorStoresResponse | ErrorResponse;

  @post
  @operationId("createVectorStore")
  @summary("Creates a vector store.")
  createVectorStore(
    ...AcceptJsonHeader,
    @body requestBody: CreateVectorStoreRequest,
  ): VectorStoreObject | ErrorResponse;

  @route("{vector_store_id}")
  @get
  @operationId("getVectorStore")
  @summary("Retrieves a vector store.")
  getVectorStore(
    ...AcceptJsonHeader,

    /** The ID of the vector store to retrieve. */
    @path vector_store_id: string,
  ): VectorStoreObject | ErrorResponse;

  @route("{vector_store_id}")
  @post
  @operationId("modifyVectorStore")
  @summary("Modifies a vector store.")
  modifyVectorStore(
    ...AcceptJsonHeader,

    /** The ID of the vector store to modify. */
    @path vector_store_id: string,

    @body requestBody: UpdateVectorStoreRequest,
  ): VectorStoreObject | ErrorResponse;

  @route("{vector_store_id}")
  @delete
  @operationId("deleteVectorStore")
  @summary("Delete a vector store.")
  deleteVectorStore(
    ...AcceptJsonHeader,

    /** The ID of the vector store to delete. */
    @path vector_store_id: string,
  ): DeleteVectorStoreResponse | ErrorResponse;

  @route("{vector_store_id}/file_batches")
  @post
  @operationId("createVectorStoreFileBatch")
  @summary("Create a vector store file batch.")
  createVectorStoreFileBatch(
    ...AcceptJsonHeader,

    /** The ID of the vector store for which to create a file batch. */
    @path vector_store_id: string,

    @body requestBody: CreateVectorStoreFileBatchRequest,
  ): VectorStoreFileBatchObject | ErrorResponse;

  @route("{vector_store_id}/file_batches/{batch_id}")
  @get
  @operationId("getVectorStoreFileBatch")
  @summary("Retrieves a vector store file batch.")
  getVectorStoreFileBatch(
    ...AcceptJsonHeader,

    /** The ID of the vector store that the file batch belongs to. */
    @path vector_store_id: string,

    /** The ID of the file batch being retrieved. */
    @path batch_id: string,
  ): VectorStoreFileBatchObject | ErrorResponse;

  @route("{vector_store_id}/file_batches/{batch_id}/cancel")
  @post
  @operationId("cancelVectorStoreFileBatch")
  @summary("Cancel a vector store file batch. This attempts to cancel the processing of files in this batch as soon as possible.")
  cancelVectorStoreFileBatch(
    ...AcceptJsonHeader,

    /** The ID of the vector store that the file batch belongs to. */
    @path vector_store_id: string,

    /** The ID of the file batch to cancel. */
    @path batch_id: string,
  ): VectorStoreFileBatchObject | ErrorResponse;

  @route("{vector_store_id}/file_batches/{batch_id}/files")
  @get
  @operationId("listFilesInVectorStoreBatch")
  @summary("Returns a list of vector store files in a batch.")
  listFilesInVectorStoreBatch(
    ...AcceptJsonHeader,

    /** The ID of the vector store that the file batch belongs to. */
    @path vector_store_id: string,

    /** The ID of the file batch that the files belong to. */
    @path batch_id: string,

    ...VectorStoreFilesPageQueryParameters,
  ): ListVectorStoreFilesResponse | ErrorResponse;

  @route("{vector_store_id}/files")
  @get
  @operationId("listVectorStoreFiles")
  @summary("Returns a list of vector store files.")
  @list
  listVectorStoreFiles(
    ...AcceptJsonHeader,

    /** The ID of the vector store that the files belong to. */
    @path vector_store_id: string,

    ...VectorStoreFilesPageQueryParameters,
  ): ListVectorStoreFilesResponse | ErrorResponse;

  @route("{vector_store_id}/files")
  @post
  @operationId("createVectorStoreFile")
  @summary("Create a vector store file by attaching a [File](/docs/api-reference/files) to a [vector store](/docs/api-reference/vector-stores/object).")
  createVectorStoreFile(
    ...AcceptJsonHeader,

    /** The ID of the vector store for which to create a File. */
    @path vector_store_id: string,

    @body requestBody: CreateVectorStoreFileRequest,
  ): VectorStoreFileObject | ErrorResponse;

  @route("{vector_store_id}/files/{file_id}")
  @get
  @operationId("getVectorStoreFile")
  @summary("Retrieves a vector store file.")
  getVectorStoreFile(
    ...AcceptJsonHeader,

    /** The ID of the vector store that the file belongs to. */
    @path vector_store_id: string,

    /** The ID of the file being retrieved. */
    @path file_id: string,
  ): VectorStoreFileObject | ErrorResponse;

  @route("{vector_store_id}/files/{file_id}")
  @delete
  @operationId("deleteVectorStoreFile")
  @summary("Delete a vector store file. This will remove the file from the vector store but the file itself will not be deleted. To delete the file, use the [delete file](/docs/api-reference/files/delete) endpoint.")
  deleteVectorStoreFile(
    ...AcceptJsonHeader,

    /** The ID of the vector store that the file belongs to. */
    @path vector_store_id: string,

    /** The ID of the file to delete. */
    @path file_id: string,
  ): DeleteVectorStoreFileResponse | ErrorResponse;

  @post
  @route("{vector_store_id}/files/{file_id}")
  @operationId("updateVectorStoreFileAttributes")
  @summary("Update the attributes of a vector store file.")
  updateVectorStoreFileAttributes(
    /** The ID of the vector store the file being updated belongs to. */
    @path vector_store_id: string,

    /** The ID of the file to update attributes for. */
    @path file_id: string,

    ...UpdateVectorStoreFileAttributesRequest,
  ): VectorStoreFileObject | ErrorResponse;

  @get
  @route("{vector_store_id}/files/{file_id}/content")
  @operationId("retrieveVectorStoreFileContent")
  @summary("Retrieves the content of a vector store file.")
  retrieveVectorStoreFileContent(
    /** The ID of the vector store that the file belongs to. */
    @path vector_store_id: string,

    /** The ID of the file with content being retrieved. */
    @path file_id: string,
  ): VectorStoreFileContentResponse | ErrorResponse;

  @post
  @route("{vector_store_id}/search")
  @operationId("searchVectorStore")
  @summary("Searches a vector store for relevant chunks based on a query and file attributes filter.")
  searchVectorStore(
    /** The ID of the vector store to search. */
    @path vector_store_id: string,

    ...VectorStoreSearchRequest,
  ): VectorStoreSearchResultsPage | ErrorResponse;
}

alias VectorStoreFilesPageQueryParameters = {
  ...PageLimitQueryParameter;
  ...PageOrderQueryParameter;
  ...PageAfterQueryParameter;
  ...PageBeforeQueryParameter;

  /**
   * Filter by file status. One of `in_progress`, `completed`, `failed`, `cancelled`.
   */
  @query filter?: ListVectorStoreFilesFilter;
};
