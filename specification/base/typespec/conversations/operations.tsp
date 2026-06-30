import "@typespec/http";
import "@typespec/openapi";
import "./models.tsp";
import "../responses/main.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

/** List all items for a conversation with the given ID. */
@summary("List items")
@get
@route("/conversations/{conversation_id}/items")
@tag("Conversations")
@list
op listConversationItems(
  ...ConversationItemCollectionOptions,
): ConversationItemCollection;

/** Create items in a conversation with the given ID. */
@summary("Create items")
@post
@route("/conversations/{conversation_id}/items")
@tag("Conversations")
op createConversationItems(
  /** The ID of the conversation to add the item to. */
  @path
  conversation_id: string,

  /** Additional fields to include in the response. See the `include`
    * parameter for [listing Conversation items above](https://platform.openai.com/docs/api-reference/conversations/list-items#conversations_list_items-include)
    * for more information. */
  @query
  include?: IncludeEnum[],

  @body
  options: CreateConversationItemsParametersBody,
): ConversationItemCollection;

/** Delete an item from a conversation with the given IDs. */
@summary("Delete an item")
@delete
@route("/conversations/{conversation_id}/items/{item_id}")
@tag("Conversations")
op deleteConversationItem(
  /** The ID of the conversation that contains the item. */
  @path
  conversation_id: string,

  /** The ID of the item to delete. */
  @path
  item_id: string,
): ConversationResource;

/** Get a single item from a conversation with the given IDs. */
@summary("Retrieve an item")
@get
@route("/conversations/{conversation_id}/items/{item_id}")
@tag("Conversations")
op getConversationItem(
  /** The ID of the conversation that contains the item. */
  @path
  conversation_id: string,

  /** The ID of the item to retrieve. */
  @path
  item_id: string,

  /** Additional fields to include in the response. See the `include`
    * parameter for [listing Conversation items above](https://platform.openai.com/docs/api-reference/conversations/list-items#conversations_list_items-include)
    * for more information. */
  @query
  include?: IncludeEnum[],
): ItemResource;

/** Create a conversation. */
@summary("Create a conversation")
@post
@route("/conversations")
@tag("Conversations")
op createConversation(
  @body
  options: CreateConversationBody,
): ConversationResource;

/** Delete a conversation. Items in the conversation will not be deleted. */
@summary("Delete a conversation")
@delete
@route("/conversations/{conversation_id}")
@tag("Conversations")
op deleteConversation(
  /** The ID of the conversation to delete. */
  @path
  conversation_id: string,
): DeletedConversationResource;

/** Get a conversation */
@summary("Retrieve a conversation")
@get
@route("/conversations/{conversation_id}")
@tag("Conversations")
op getConversation(
  /** The ID of the conversation to retrieve. */
  @path
  conversation_id: string,
): ConversationResource;

/** Update a conversation */
@summary("Update a conversation")
@post
@route("/conversations/{conversation_id}")
@tag("Conversations")
op updateConversation(
  /** The ID of the conversation to update. */
  @path
  conversation_id: string,

  @body
  options: UpdateConversationBody,
): ConversationResource;