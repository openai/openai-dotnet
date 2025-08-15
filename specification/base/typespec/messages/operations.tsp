import "@typespec/http";
import "@typespec/openapi";

import "../common";
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
    ...AcceptJsonHeader,
    ...AssistantsBetaHeader,

    /** The ID of the [thread](/docs/api-reference/threads) to create a message for. */
    @path thread_id: string,

    @body requestBody: CreateMessageRequest,
  ): MessageObject | ErrorResponse;

  @get
  @operationId("listMessages")
  @tag("Assistants")
  @summary("Returns a list of messages for a given thread.")
  listMessages(
    ...AcceptJsonHeader,
    ...AssistantsBetaHeader,

    /** The ID of the [thread](/docs/api-reference/threads) the messages belong to. */
    @path thread_id: string,

    ...CommonPageQueryParameters,
  ): ListMessagesResponse | ErrorResponse;

  @route("{message_id}")
  @get
  @operationId("getMessage")
  @tag("Assistants")
  @summary("Retrieve a message.")
  getMessage(
    ...AcceptJsonHeader,
    ...AssistantsBetaHeader,

    /** The ID of the [thread](/docs/api-reference/threads) to which this message belongs. */
    @path thread_id: string,

    /** The ID of the message to retrieve. */
    @path message_id: string,
  ): MessageObject | ErrorResponse;

  @route("{message_id}")
  @post
  @operationId("modifyMessage")
  @tag("Assistants")
  @summary("Modifies a message.")
  modifyMessage(
    ...AcceptJsonHeader,
    ...AssistantsBetaHeader,

    /** The ID of the thread to which this message belongs. */
    @path thread_id: string,

    /** The ID of the message to modify. */
    @path message_id: string,

    @body requestBody: ModifyMessageRequest,
  ): MessageObject | ErrorResponse;

  @route("{message_id}")
  @delete
  @operationId("deleteMessage")
  @tag("Assistants")
  @summary("Deletes a message.")
  deleteMessage(
    ...AcceptJsonHeader,
    ...AssistantsBetaHeader,

    /** The ID of the thread to which this message belongs. */
    @path thread_id: string,

    /** The ID of the message to delete. */
    @path message_id: string,
  ): DeleteMessageResponse | ErrorResponse;
}
