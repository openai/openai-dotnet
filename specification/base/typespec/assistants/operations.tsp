import "@typespec/http";
import "@typespec/openapi";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("assistants")
@tag("Assistants")
interface Assistants {
  @get
  @operationId("listAssistants")
  @summary("Returns a list of assistants.")
  @list
  listAssistants(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",
    /**
   * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
   * default is 20.
   */
  @query limit?: int32 = 20,
  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: AssistantCollectionOrder,
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
  ): ListAssistantsResponse | AssistantsErrorResponse;

  @post
  @operationId("createAssistant")
  @summary("Create an assistant with a model and instructions.")
  createAssistant(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",
    @body requestBody: CreateAssistantRequest,
  ): AssistantObject | AssistantsErrorResponse;

  @route("{assistant_id}")
  @get
  @operationId("getAssistant")
  @summary("Retrieves an assistant.")
  getAssistant(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the assistant to retrieve. */
    @path assistant_id: string,
  ): AssistantObject | AssistantsErrorResponse;

  @route("{assistant_id}")
  @post
  @operationId("modifyAssistant")
  @summary("Modifies an assistant.")
  modifyAssistant(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the assistant to modify. */
    @path assistant_id: string,

    @body requestBody: ModifyAssistantRequest,
  ): AssistantObject | AssistantsErrorResponse;

  @route("{assistant_id}")
  @delete
  @operationId("deleteAssistant")
  @summary("Delete an assistant.")
  deleteAssistant(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the assistant to delete. */
    @path assistant_id: string,
  ): DeleteAssistantResponse | AssistantsErrorResponse;
}
