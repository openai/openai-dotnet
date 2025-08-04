import "@typespec/http";
import "@typespec/openapi";

import "../common";
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
    ...MetadataPropertyForRequest,
  ): Batch | ErrorResponse;

  @get
  @operationId("listBatches")
  @tag("Batch")
  @summary("List your organization's batches.")
  listBatches(
    @header accept: "application/json",

    /**
     * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
     * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
     * subsequent call can include after=obj_foo in order to fetch the next page of the list.
     */
    @query after?: string,

    /**
     * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
     * default is 20.
     */
    @query limit?: int32 = 20,
  ): ListBatchesResponse | ErrorResponse;

  @route("{batch_id}")
  @get
  @operationId("retrieveBatch")
  @tag("Batch")
  @summary("Retrieves a batch.")
  retrieveBatch(
    @header accept: "application/json",

    /** The ID of the batch to retrieve. */
    @path batch_id: string,
  ): Batch | ErrorResponse;

  @route("{batch_id}/cancel")
  @post
  @operationId("cancelBatch")
  @tag("Batch")
  @summary("Cancels an in-progress batch.")
  cancelBatch(
    @header accept: "application/json",

    /** The ID of the batch to cancel. */
    @path batch_id: string,
  ): Batch | ErrorResponse;
}
