import "@typespec/http";
import "@typespec/openapi";

import "./models.tsp";
import "./models_items.tsp";
import "./models_streaming.tsp";
import "./models_tools.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI.Responses;

/**
 * Creates a model response.
 */
@tag("Responses")
@operationId("createResponse")
@post
@route("/responses")
op createResponse(
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
@tag("Responses")
@operationId("getResponse")
@get
@route("/responses/{response_id}")
op getResponse(
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

@tag("Responses")
@operationId("deleteResponse")
@delete
@route("/responses/{response_id}")
op deleteResponse(
  /**
   * The ID of the response to delete.
   */
  @path
  @example("resp_677efb5139a88190b512bc3fef8e535d")
  response_id: string,
): DeleteResponseResponse | ResponseErrorResponse;

@tag("Responses")
@operationId("cancelResponse")
@post
@route("/responses/{response_id}/cancel")
op cancelResponse(
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
@tag("Responses")
@operationId("listInputItems")
@get
@route("/responses/{response_id}/input_items")
@list
op listInputItems(
  ...DotNetResponseItemCollectionOptions,
): ResponseItemList | ResponseErrorResponse;

/**
 * Get input token counts
 */
@tag("Responses")
@operationId("getInputTokenCounts")
@summary("Get input token counts")
@post
@route("/responses/input_tokens")
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

@tag("Responses")
@operationId("compactConversation")
@post
@route("/responses/compact")
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