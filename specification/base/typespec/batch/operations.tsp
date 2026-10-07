import "@typespec/http";
import "@typespec/openapi";

import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("/batches")
interface Batches {
  @post
  @operationId("createBatch")
  @tag("Batch")
  @summary("Creates and executes a batch from an uploaded file of requests")
  createBatch(
    @header accept: "application/json",

    /**
     * The ID of an uploaded file that contains requests for the new batch.
     *
     * See [upload file](/docs/api-reference/files/create) for how to upload a file.
     *
     * Your input file must be formatted as a [JSONL file](/docs/api-reference/batch/requestInput),
     * and must be uploaded with the purpose `batch`.
     */
    input_file_id: string,

    /**
     * The endpoint to be used for all requests in the batch. Currently `/v1/chat/completions` and
     * `/v1/embeddings` are supported.
     */
    endpoint: "/v1/chat/completions" | "/v1/embeddings",

    /**
     * The time frame within which the batch should be processed. Currently only `24h` is supported.
     */
    completion_window: "24h",

    /** Optional custom metadata for the batch. */
    metadata?: Record<string>,
  ): Batch | BatchErrorResponse;

  @get
  @operationId("listBatches")
  @tag("Batch")
  @summary("List your organization's batches.")
  @list
  listBatches(
    @header accept: "application/json",
    ...BatchCollectionOptions,
  ): ListBatchesResponse | BatchErrorResponse;

  @route("{batch_id}")
  @get
  @operationId("retrieveBatch")
  @tag("Batch")
  @summary("Retrieves a batch.")
  retrieveBatch(
    @header accept: "application/json",

    /** The ID of the batch to retrieve. */
    @path batch_id: string,
  ): Batch | BatchErrorResponse;

  @route("{batch_id}/cancel")
  @post
  @operationId("cancelBatch")
  @tag("Batch")
  @summary("Cancels an in-progress batch.")
  cancelBatch(
    @header accept: "application/json",

    /** The ID of the batch to cancel. */
    @path batch_id: string,
  ): Batch | BatchErrorResponse;
}
