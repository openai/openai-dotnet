import "@typespec/openapi";
import "@typespec/http";
import "@azure-tools/typespec-client-generator-core";
import "../responses/main.tsp";

using TypeSpec.OpenAPI;
using TypeSpec.Http;
using Azure.ClientGenerator.Core;

namespace OpenAI;

model CreateConversationBody {
  metadata?: Record<string>;
  items?: ItemResource[];
}

model ConversationResource {
  /** The unique ID of the conversation. */
  id: string;

  /** The object type, which is always `conversation`. */
  object: "conversation" = "conversation";

  /**Set of 16 key-value pairs that can be attached to an object. This can be         useful for storing additional information about the object in a structured         format, and querying for objects via API or the dashboard.
  Keys are strings with a maximum length of 64 characters. Values are strings         with a maximum length of 512 characters.*/
  metadata: Record<string> | null;

  /** The time at which the conversation was created, measured in seconds since the Unix epoch. */
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  created_at: utcDateTime;
}

model UpdateConversationBody {
  /** Set of 16 key-value pairs that can be attached to an object. This can be
    * useful for storing additional information about the object in a structured
    * format, and querying for objects via API or the dashboard.
    * Keys are strings with a maximum length of 64 characters. Values are strings
    * with a maximum length of 512 characters.
    */
  metadata: Record<string> | null;
}

model DeletedConversationResource {
  object: "conversation.deleted" = "conversation.deleted";
  deleted: boolean;
  id: string;
}

model CreateConversationItemsParametersBody {
  @maxItems(20)
  items: ItemResource[];
}

/**Specify additional output data to include in the model response. Currently supported values are:
- `web_search_call.action.sources`: Include the sources of the web search tool call.
- `code_interpreter_call.outputs`: Includes the outputs of python code execution in code interpreter tool call items.
- `computer_call_output.output.image_url`: Include image urls from the computer call output.
- `file_search_call.results`: Include the search results of the file search tool call.
- `message.input_image.image_url`: Include image urls from the input message.
- `message.output_text.logprobs`: Include logprobs with assistant messages.
- `reasoning.encrypted_content`: Includes an encrypted version of reasoning tokens in reasoning item outputs. This enables reasoning items to be used in multi-turn conversations when using the Responses API statelessly (like when the `store` parameter is set to `false`, or when an organization is enrolled in the zero data retention program).*/
union IncludeEnum {
  string,
  `file_search_call.results`: "file_search_call.results",
  `web_search_call.results`: "web_search_call.results",
  `web_search_call.action.sources`: "web_search_call.action.sources",
  `message.input_image.image_url`: "message.input_image.image_url",
  `computer_call_output.output.image_url`: "computer_call_output.output.image_url",
  `code_interpreter_call.outputs`: "code_interpreter_call.outputs",
  `reasoning.encrypted_content`: "reasoning.encrypted_content",
  `message.output_text.logprobs`: "message.output_text.logprobs",
}

union ConversationItemCollectionOrder {
  string,
  asc: "asc",
  desc: "desc",
}

@access(Access.public)
@usage(Usage.input)
model ConversationItemCollectionOptions {
  /** The ID of the conversation to list items for. */
  @path
  conversation_id: string,

  /** A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20. */
  @query
  limit?: integer = 20,

  /**
   * The order to return the input items in. Default is `desc`.
   * - `asc`: Return the input items in ascending order.
   * - `desc`: Return the input items in descending order.
   */
  @query
  order?: ConversationItemCollectionOrder,

  /** An item ID to list items after, used in pagination. */
  @query
  @continuationToken
  after?: string,

  /**
   * Specify additional output data to include in the model response. Currently supported values are:
   * - `web_search_call.action.sources`: Include the sources of the web search tool call.
   * - `code_interpreter_call.outputs`: Includes the outputs of python code execution in code interpreter tool call items.
   * - `computer_call_output.output.image_url`: Include image urls from the computer call output.
   * - `file_search_call.results`: Include the search results of the file search tool call.
   * - `message.input_image.image_url`: Include image urls from the input message.
   * - `message.output_text.logprobs`: Include logprobs with assistant messages.
   * - `reasoning.encrypted_content`: Includes an encrypted version of reasoning tokens in reasoning item outputs. 
   * This enables reasoning items to be used in multi-turn conversations when using the Responses API statelessly (like when the `store` parameter is set to `false`, 
   * or when an organization is enrolled in the zero data retention program).
   * */
  @query
  include?: IncludeEnum[],
}

model ConversationItemCollection {
  /** The type of object returned, must be `list`. */
  object: "list";

  /** A list of conversation items. */
  @pageItems
  data: ItemResource[];

  /** The ID of the first item in the list. */
  first_id: string;

  /** The ID of the last item in the list. */
  @continuationToken
  last_id: string;

  /** Whether there are more items available. */
  has_more: boolean;
}