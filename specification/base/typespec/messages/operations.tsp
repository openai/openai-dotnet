import "@typespec/http";
import "@typespec/openapi";

import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("/threads/{thread_id}/messages")
interface Messages {
  @post
  @operationId("createMessage")
  @tag("Assistants")
  @summary("Create a message.")
  createMessage(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the [thread](/docs/api-reference/threads) to create a message for. */
    @path thread_id: string,

    @body requestBody: CreateMessageRequest,
  ): MessageObject | MessagesErrorResponse;

  @get
  @operationId("listMessages")
  @tag("Assistants")
  @summary("Returns a list of messages for a given thread.")
  @list
  listMessages(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the [thread](/docs/api-reference/threads) the messages belong to. */
    @path thread_id: string,

    ...MessageCollectionOptions,
  ): ListMessagesResponse | MessagesErrorResponse;

  @route("{message_id}")
  @get
  @operationId("getMessage")
  @tag("Assistants")
  @summary("Retrieve a message.")
  getMessage(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the [thread](/docs/api-reference/threads) to which this message belongs. */
    @path thread_id: string,

    /** The ID of the message to retrieve. */
    @path message_id: string,
  ): MessageObject | MessagesErrorResponse;

  @route("{message_id}")
  @post
  @operationId("modifyMessage")
  @tag("Assistants")
  @summary("Modifies a message.")
  modifyMessage(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the thread to which this message belongs. */
    @path thread_id: string,

    /** The ID of the message to modify. */
    @path message_id: string,

    @body requestBody: ModifyMessageRequest,
  ): MessageObject | MessagesErrorResponse;

  @route("{message_id}")
  @delete
  @operationId("deleteMessage")
  @tag("Assistants")
  @summary("Deletes a message.")
  deleteMessage(
    @header accept: "application/json",
    @header("OpenAI-Beta") openAIBeta: "assistants=v2",

    /** The ID of the thread to which this message belongs. */
    @path thread_id: string,

    /** The ID of the message to delete. */
    @path message_id: string,
  ): DeleteMessageResponse | MessagesErrorResponse;
}
