import "@typespec/openapi";
import "../../common";
import "../models.tsp";
import "./items.messages.tsp";

using TypeSpec.OpenAPI;

namespace OpenAI;

union ItemType {
  message: "message",
  file_search_call: "file_search_call",
  function_call: "function_call",
  function_call_output: "function_call_output",
  computer_call: "computer_call",
  computer_call_output: "computer_call_output",
  web_search_call: "web_search_call",
  reasoning: "reasoning",
  item_reference: "item_reference",
  image_generation_call: "image_generation_call",
  code_interpreter_call: "code_interpreter_call",
  local_shell_call: "local_shell_call",
  local_shell_call_output: "local_shell_call_output",
  mcp_list_tools: "mcp_list_tools",
  mcp_approval_request: "mcp_approval_request",
  mcp_approval_response: "mcp_approval_response",
  mcp_call: "mcp_call",
}

model FileSearchToolCallItemParam extends ItemParam {
  type: ItemType.file_search_call;
  ...FileSearchToolCallItemBase;
}

model FileSearchToolCallItemResource extends ItemResource {
  type: ItemType.file_search_call;

  @doc("""
    The status of the file search tool call. One of `in_progress`, 
    `searching`, `incomplete` or `failed`,
    """)
  status: "in_progress" | "searching" | "completed" | "incomplete" | "failed";

  ...FileSearchToolCallItemBase;
}

model ComputerToolCallItemParam extends ItemParam {
  type: ItemType.computer_call;
  ...ComputerToolCallItemBase;
}

model ComputerToolCallItemResource extends ItemResource {
  type: ItemType.computer_call;

  @doc("""
    The status of the item. One of `in_progress`, `completed`, or
    `incomplete`. Populated when items are returned via API.
    """)
  status: "in_progress" | "completed" | "incomplete";

  ...ComputerToolCallItemBase;
}

model ComputerToolCallOutputItemParam extends ItemParam {
  type: ItemType.computer_call_output;
  ...ComputerToolCallOutputItemBase;
}

model ComputerToolCallOutputItemResource extends ItemResource {
  type: ItemType.computer_call_output;

  @doc("""
    The status of the item. One of `in_progress`, `completed`, or
    `incomplete`. Populated when items are returned via API.
    """)
  status: "in_progress" | "completed" | "incomplete";

  ...ComputerToolCallOutputItemBase;
}

/**The results of a web search tool call. See the
[web search guide](/docs/guides/tools-web-search) for more information.*/
@summary("Web search tool call")
model WebSearchToolCallItemResource extends ItemResource {
  /** The type of the web search tool call. Always `web_search_call`. */
  type: ItemType.web_search_call;

  /** The status of the web search tool call. */
  status: "in_progress" | "searching" | "completed" | "failed";

  // CUSTOM: Based on service behavior, "action" is optional.
  /**An object describing the specific action taken in this web search call.
  Includes details on how the model used the web (search, open_page, find_in_page).*/
  action?: WebSearchActionBase;
}

union WebSearchActionSearchSourcesType {
  string,
  url: "url",
}

@discriminator("type")
model WebSearchActionSearchSourceBase {
  type: WebSearchActionSearchSourcesType;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model WebSearchActionSearchSourceUrl extends WebSearchActionSearchSourceBase {
  type: WebSearchActionSearchSourcesType.url;
  
  url: url;
}

union WebSearchActionType {
  string,
  search: "search",
  open_page: "open_page",
  find_in_page: "find_in_page",
}

@discriminator("type")
model WebSearchActionBase {
  type: WebSearchActionType;
}

/** Action type "search" - Performs a web search query. */
@summary("Search action")
model WebSearchActionSearch extends WebSearchActionBase {
  /** The action type. */
  type: WebSearchActionType.search;

  // CUSTOM: Based on service behavior, "query" became optional after the introduction of "queries"
  /** [DEPRECATED] The search query. */
  #deprecated "deprecated"
  query?: string;

  /** The search queries. */
  @summary("Search queries")
  queries?: string[];

  /** The sources used in the search. */
  @summary("Web search sources")
  sources?: WebSearchActionSearchSourceBase[];
}

/** Action type "open_page" - Opens a specific URL from search results. */
@summary("Open page action")
model WebSearchActionOpenPage extends WebSearchActionBase {
  /** The action type. */
  type: WebSearchActionType.open_page;

  /** The URL opened by the model. */
  url?: url | null;
}

/** Action type "find_in_page": Searches for a pattern within a loaded page. */
@summary("Find action")
model WebSearchActionFind extends WebSearchActionBase {
  /** The action type. */
  type: WebSearchActionType.find_in_page;

  /** The URL of the page searched for the pattern. */
  url: url;

  /** The pattern or text to search for within the page. */
  pattern: string;
}

model FunctionToolCallItemParam extends ItemParam {
  type: ItemType.function_call;
  ...FunctionToolCallItemBase;
}

model FunctionToolCallItemResource extends ItemResource {
  type: ItemType.function_call;

  @doc("""
    The status of the item. One of `in_progress`, `completed`, or
    `incomplete`. Populated when items are returned via API.
    """)
  status: "in_progress" | "completed" | "incomplete";

  ...FunctionToolCallItemBase;
}

model FunctionToolCallOutputItemParam extends ItemParam {
  type: ItemType.function_call_output;
  ...FunctionToolCallOutputItemBase;
}

model FunctionToolCallOutputItemResource extends ItemResource {
  type: ItemType.function_call_output;

  @doc("""
    The status of the item. One of `in_progress`, `completed`, or
    `incomplete`. Populated when items are returned via API.
    """)
  status: "in_progress" | "completed" | "incomplete";

  ...FunctionToolCallOutputItemBase;
}

model ReasoningItemParam extends ItemParam {
  type: ItemType.reasoning;
  ...ReasoningItemBase;
}

model ReasoningItemResource extends ItemResource {
  type: ItemType.reasoning;

  @doc("""
    The status of the item. One of `in_progress`, `completed`, or
    `incomplete`. Populated when items are returned via API.
    """)
  status: "in_progress" | "completed" | "incomplete";

  ...ReasoningItemBase;
}

model ItemReferenceItemParam extends ItemParam {
  type: ItemType.item_reference;

  /** The service-originated ID of the previously generated response item being referenced. */
  id: string;

  ...ItemReferenceItemBase;
}

// Note: there's no current "resource" representation of item references (they're instead resolved)

model ImageGenToolCallItemParam extends ItemParam {
  type: ItemType.image_generation_call;
  ...ImageGenToolCallItemBase;
}

model ImageGenToolCallItemResource extends ItemResource {
  type: ItemType.image_generation_call;
  status: "in_progress" | "completed" | "generating" | "failed";
  action: ImageGenerationToolAction;
  background: "transparent" | "opaque" | "auto" = "auto";
  output_format?: "png" | "jpeg" | "webp" | null = "png";
  quality?:
    | "standard"
    | "hd"
    | "low"
    | "medium"
    | "high"
    | "auto"
    | null = "auto";
  size?:
    | "auto"
    | "1024x1024"
    | "1536x1024"
    | "1024x1536"
    | "256x256"
    | "512x512"
    | "1792x1024"
    | "1024x1792"
    | null = "auto";
  revised_prompt?: string;
  ...ImageGenToolCallItemBase;
}

model CodeInterpreterToolCallItemParam extends ItemParam {
  type: ItemType.code_interpreter_call;
  ...CodeInterpreterToolCallItemBase;
}

model CodeInterpreterToolCallItemResource extends ItemResource {
  type: ItemType.code_interpreter_call;
  status: "in_progress" | "interpreting" | "completed";
  ...CodeInterpreterToolCallItemBase;
}

model LocalShellToolCallItemParam extends ItemParam {
  type: ItemType.local_shell_call;
  ...LocalShellToolCallItemBase;
}

model LocalShellToolCallItemResource extends ItemResource {
  type: ItemType.local_shell_call;
  status: "in_progress" | "completed" | "incomplete";
  ...LocalShellToolCallItemBase;
}

model LocalShellToolCallOutputItemParam extends ItemParam {
  type: ItemType.local_shell_call_output;
  ...LocalShellToolCallOutputItemBase;
}

model LocalShellToolCallOutputItemResource extends ItemResource {
  type: ItemType.local_shell_call_output;
  status: "in_progress" | "completed" | "incomplete";
  ...LocalShellToolCallOutputItemBase;
}

model MCPListToolsItemParam extends ItemParam {
  type: ItemType.mcp_list_tools;
  ...MCPListToolsItemBase;
}

model MCPListToolsItemResource extends ItemResource {
  type: ItemType.mcp_list_tools;
  ...MCPListToolsItemBase;
}

model MCPApprovalRequestItemParam extends ItemParam {
  type: ItemType.mcp_approval_request;
  ...MCPApprovalRequestItemBase;
}

model MCPApprovalRequestItemResource extends ItemResource {
  type: ItemType.mcp_approval_request;
  ...MCPApprovalRequestItemBase;
}

model MCPApprovalResponseItemParam extends ItemParam {
  type: ItemType.mcp_approval_response;
  ...MCPApprovalResponseItemBase;
}

model MCPApprovalResponseItemResource extends ItemResource {
  type: ItemType.mcp_approval_response;
  ...MCPApprovalResponseItemBase;
}

model MCPCallItemParam extends ItemParam {
  type: ItemType.mcp_call;
  ...MCPCallItemBase;
}

model MCPCallItemResource extends ItemResource {
  type: ItemType.mcp_call;
  ...MCPCallItemBase;
}
