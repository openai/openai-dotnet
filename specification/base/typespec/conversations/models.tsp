import "@typespec/openapi";
import "../responses/models.tsp";
import "../common/models.tsp";
using TypeSpec.OpenAPI;
namespace OpenAI;
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model ConversationResource {
  /** The unique ID of the conversation. */
  id: string;

  /** The object type, which is always `conversation`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "conversation" = "conversation";

  /** Set of 16 key-value pairs that can be attached to an object. This can be useful for storing additional information about the object in a structured format, and querying for objects via API or the dashboard.
  Keys are strings with a maximum length of 64 characters. Values are strings with a maximum length of 512 characters. */
  #suppress "@azure-tools/typespec-azure-core/no-unknown" "Auto-suppressed warnings non-applicable rules during import."
  metadata: unknown;

  /** The time at which the conversation was created, measured in seconds since the Unix epoch. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  created_at: integer;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("The conversation object")
@extension(
  "x-oaiMeta",
  #{ name: "The conversation object", group: "conversations" }
)
model Conversation {
  ...ConversationResource;
}

/** A list of Conversation items. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("The conversation item list")
@extension("x-oaiMeta", #{ name: "The item list", group: "conversations" })
model ConversationItemList {
  /** The type of object returned, must be `list`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "list";

  /** A list of conversation items. */
  @pageItems data: ItemResource[];

  /** Whether there are more items available. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  has_more: boolean;

  /** The ID of the first item in the list. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  first_id: string;

  /** The ID of the last item in the list. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @continuationToken last_id: string;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model DeletedConversationResource {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "conversation.deleted" = "conversation.deleted";

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  deleted: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  id: string;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("The deleted conversation object")
@extension(
  "x-oaiMeta",
  #{ name: "The deleted conversation object", group: "conversations" }
)
model DeletedConversation {
  ...DeletedConversationResource;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateConversationBody {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  metadata?: Metadata | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  items?: ItemResource[] | null;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model UpdateConversationBody {
  /** Set of 16 key-value pairs that can be attached to an object. This can be useful for storing additional information about the object in a structured format, and querying for objects via API or the dashboard.
  Keys are strings with a maximum length of 64 characters. Values are strings with a maximum length of 512 characters. */
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  metadata: Metadata | null;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateConversationItemsParametersBody {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
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