import "@typespec/http";
import "@typespec/openapi";

import "./items.tsp";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("responses")
@tag("Responses")
interface Responses {
  /**
   * Creates a model response.
   */
  @operationId("createResponse")
  @post
  createResponse(
    @header accept: "application/json" | "text/event-stream",
    @body requestBody: CreateResponse,
  ): Response | SseResponseOfResponseStreamEvent | ResponseErrorResponse;

  /**
   * Retrieves a model response with the given ID.
   *
   * @param response_id The ID of the response to retrieve.
   * @param include Specifies additional output data to include in the model response.
   * @param stream If set to true, model response data will be streamed to the client as it is generated using server-sent events.
   * @param starting_after The sequence number of the event after which to start streaming.
   */
  @operationId("getResponse")
  @get
  @route("{response_id}")
  getResponse(
    @path
    response_id: string,

    @query(#{ name: "include[]", explode: true })
    include?: Includable[],

    @query(#{ explode: true })
    stream?: boolean,

    @query(#{ explode: true })
    starting_after?: int32,

    @query(#{ explode: true })
    include_obfuscation?: boolean,
  ): Response | SseResponseOfResponseStreamEvent | ResponseErrorResponse;

  @delete
  @route("{response_id}")
  @operationId("deleteResponse")
  deleteResponse(
    /**
     * The ID of the response to delete.
     */
    @path
    @example("resp_677efb5139a88190b512bc3fef8e535d")
    response_id: string,
  ): DeleteResponseResponse | ResponseErrorResponse;

  @post
  @route("{response_id}/cancel")
  @operationId("cancelResponse")
  cancelResponse(
    /**
     * The ID of the response to cancel.
     */
    @path
    @example("resp_677efb5139a88190b512bc3fef8e535d")
    response_id: string,
  ): Response | ResponseErrorResponse;

  /**
   * Returns a list of input items for a given response.
   *
   * @param response_id The ID of the response to retrieve.
   */
  @operationId("listInputItems")
  @get
  @route("{response_id}/input_items")
  @list
  listInputItems(
    @path
    @example("resp_677efb5139a88190b512bc3fef8e535d")
    response_id: string,

    /**
   * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
   * default is 20.
   */
  @query limit?: int32 = 20,
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: DotNetResponseItemCollectionOrder,
  /**
   * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include after=obj_foo in order to fetch the next page of the list.
   */
  @query @continuationToken after?: string,
  /**
   * A cursor for use in pagination. `before` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include before=obj_foo in order to fetch the previous page of the list.
   */
  @query before?: string,
  ): ResponseItemList | ResponseErrorResponse;

 /** Get input token counts */
 @summary("Get input token counts")
 @post
 @route("input_tokens")
 @operationId("getInputTokenCounts")
 op getInputTokenCounts(
  @header
  contentType: "application/json" | "application/x-www-form-urlencoded",
  @body
  body: TokenCountsBody,
): TokenCountsResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ResponseErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ResponseErrorResponse;
};

  @post
  @route("compact")
  @operationId("compactConversation")
  op compactConversation(
  @header
  contentType: "application/json" | "application/x-www-form-urlencoded",
  @body
  body: CompactResponseMethodPublicBody,
): CompactResource | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ResponseErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ResponseErrorResponse;
};
}
