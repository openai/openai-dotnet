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
    ...AssistantCollectionOptions,
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
