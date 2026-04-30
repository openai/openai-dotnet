import "@typespec/openapi";
import "@azure-tools/typespec-client-generator-core";
import "./models.tsp";

using Azure.ClientGenerator.Core;
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
  apply_patch_call: "apply_patch_call",
  apply_patch_call_output: "apply_patch_call_output",
}

// Tool customization: Establish a discriminated type base
/** Content item used to generate a response. */
@discriminator("type")
model ItemParam {
  type: ItemType;
}

// Tool customization: Establish a discriminated type base
/** Content item used to generate a response. */
@discriminator("type")
model ItemResource {
  type: ItemType;
  id: string;
}

// Tool generated type. Extracts from FileSearchToolCallItemBase.results
alias FileSearchToolCallItemBaseResult = {
  /** The unique ID of the file. */
  file_id?: string;

  /** The text that was retrieved from the file. */
  text?: string;

  /** The name of the file. */
  filename?: string;

  attributes?: ResponsesVectorStoreFileAttributes;

  /** The relevance score of the file - a value between 0 and 1. */
  score?: float32;
};

union ReasoningItemSummaryPartType {
  string,
  summary_text: "summary_text",
}

@discriminator("type")
model ReasoningItemSummaryPart {
  type: ReasoningItemSummaryPartType;
}

model ReasoningItemSummaryTextPart extends ReasoningItemSummaryPart {
  type: ReasoningItemSummaryPartType.summary_text;
  text: string;
}

/** A pending safety check for the computer call. */
model ComputerCallSafetyCheckParam {
  /** The ID of the pending safety check. */
  id: string;

  /** The type of the pending safety check. */
  code?: string | null;

  /** Details about the pending safety check. */
  message?: string | null;
}

/** A pending safety check for the computer call. */
model ComputerToolCallSafetyCheck {
  /** The ID of the pending safety check. */
  id: string;

  /** The type of the pending safety check. */
  code: string;

  /** Details about the pending safety check. */
  message: string;
}

// Tool customization (apply_discriminator): Apply base discriminator type for ComputerAction and align naming
/** A click action. */
model ComputerActionClick extends ComputerAction {
  @doc("""
    Specifies the event type. For a click action, this property is
    always set to `click`.
    """)
  type: ComputerActionType.click;

  @doc("""
    Indicates which mouse button was pressed during the click. One of `left`, `right`, `wheel`, `back`, or `forward`.
    """)
  button: "left" | "right" | "wheel" | "back" | "forward";

  /** The x-coordinate where the click occurred. */
  x: int32;

  /** The y-coordinate where the click occurred. */
  y: int32;
}

// Tool customization: Establish discriminated type base
union ComputerActionType {
  screenshot: "screenshot",
  click: "click",
  double_click: "double_click",
  scroll: "scroll",
  type: "type",
  wait: "wait",
  keypress: "keypress",
  drag: "drag",
  move: "move",
}
@discriminator("type")
model ComputerAction {
  type: ComputerActionType;
}

// Tool customization: Establish a discriminated type basis for computer call outputs
/** A computer screenshot image used with the computer use tool. */
union ComputerToolCallOutputItemOutputType {
  string,
  screenshot: "computer_screenshot",
}
@discriminator("type")
model ComputerToolCallOutputItemOutput {
  type: ComputerToolCallOutputItemOutputType;
}
model ComputerToolCallOutputItemOutputComputerScreenshot
  extends ComputerToolCallOutputItemOutput {
  type: ComputerToolCallOutputItemOutputType.screenshot;
  image_url?: string;
  file_id?: string;
}

// Tool customization (apply_discriminator): Apply base discriminator type for ComputerAction and align naming
/** A double click action. */
model ComputerActionDoubleClick extends ComputerAction {
  @doc("""
    Specifies the event type. For a double click action, this property is
    always set to `double_click`.
    """)
  type: ComputerActionType.double_click;

  /** The x-coordinate where the double click occurred. */
  x: int32;

  /** The y-coordinate where the double click occurred. */
  y: int32;
}

// Tool customization (apply_discriminator): Apply base discriminator type for ComputerAction and align naming
/** A drag action. */
model ComputerActionDrag extends ComputerAction {
  @doc("""
    Specifies the event type. For a drag action, this property is
    always set to `drag`.
    """)
  type: ComputerActionType.drag;

  @doc("""
    An array of coordinates representing the path of the drag action. Coordinates will appear as an array
    of objects, eg
    ```
    [
      { x: 100, y: 200 },
      { x: 200, y: 300 }
    ]
    ```
    """)
  path: Coordinate[];
}

// Tool customization (apply_discriminator): Apply base discriminator type for ComputerAction and align naming
/** A mouse move action. */
model ComputerActionMove extends ComputerAction {
  @doc("""
    Specifies the event type. For a move action, this property is
    always set to `move`.
    """)
  type: ComputerActionType.move;

  /** The x-coordinate to move to. */
  x: int32;

  /** The y-coordinate to move to. */
  y: int32;
}

// Tool customization (apply_discriminator): Apply base discriminator type for ComputerAction and align naming
/** A screenshot action. */
model ComputerActionScreenshot extends ComputerAction {
  @doc("""
    Specifies the event type. For a screenshot action, this property is
    always set to `screenshot`.
    """)
  type: ComputerActionType.screenshot;
}

// Tool customization (apply_discriminator): Apply base discriminator type for ComputerAction and align naming
/** A scroll action. */
model ComputerActionScroll extends ComputerAction {
  @doc("""
    Specifies the event type. For a scroll action, this property is
    always set to `scroll`.
    """)
  type: ComputerActionType.scroll;

  /** The x-coordinate where the scroll occurred. */
  x: int32;

  /** The y-coordinate where the scroll occurred. */
  y: int32;

  /** The horizontal scroll distance. */
  scroll_x: int32;

  /** The vertical scroll distance. */
  scroll_y: int32;
}

// Tool customization (apply_discriminator): Apply base discriminator type for ComputerAction and align naming
/** An action to type in text. */
model ComputerActionTypeKeys extends ComputerAction {
  @doc("""
    Specifies the event type. For a type action, this property is
    always set to `type`.
    """)
  type: ComputerActionType.type;

  /** The text to type. */
  text: string;
}

// Tool customization (apply_discriminator): Apply base discriminator type for ComputerAction and align naming
/** A wait action. */
model ComputerActionWait extends ComputerAction {
  @doc("""
    Specifies the event type. For a wait action, this property is
    always set to `wait`.
    """)
  type: ComputerActionType.wait;
}

// Tool customization (apply_discriminator): Apply base discriminator type for ComputerAction and align naming
/** A collection of keypresses the model would like to perform. */
model ComputerActionKeyPress extends ComputerAction {
  @doc("""
    Specifies the event type. For a keypress action, this property is
    always set to `keypress`.
    """)
  type: ComputerActionType.keypress;

  /**
   * The combination of keys the model is requesting to be pressed. This is an
   * array of strings, each representing a key.
   */
  keys: string[];
}

@doc("""
  An x/y coordinate pair, e.g. `{ x: 100, y: 200 }`.
  """)
model Coordinate {
  /** The x-coordinate. */
  x: int32;

  /** The y-coordinate. */
  y: int32;
}

union CodeInterpreterToolOutputType {
  string,
  logs: "logs",
  image: "image",
}

@discriminator("type")
model CodeInterpreterToolOutput {
  @doc("The type of the output.")
  type: CodeInterpreterToolOutputType;
}

model CodeInterpreterToolLogsOutput extends CodeInterpreterToolOutput {
  @doc("The type of the output. Always 'logs'.")
  type: CodeInterpreterToolOutputType.logs;

  @doc("The logs output from the code interpreter.")
  logs: string;
}

model CodeInterpreterToolImageOutput extends CodeInterpreterToolOutput {
  @doc("The type of the output. Always 'image'.")
  type: CodeInterpreterToolOutputType.image;

  @doc("The URL of the image output from the code interpreter.")
  url: url;
}

// Tool customization: Rename to supply base for split input/output models
/**
 * The results of a file search tool call. See the
 * [file search guide](/docs/guides/tools-file-search) for more information.
 */
alias FileSearchToolCallItemBase = {
  /** The queries used to search for files. */
  queries: string[];

  /** The results of the file search tool call. */
  results?: FileSearchToolCallItemBaseResult[] | null;
};

@@doc(FileSearchToolCallItemResource,
  """
    The results of a file search tool call. See the
    [file search guide](/docs/guides/tools-file-search) for more information.
    
    """
);
@@doc(FileSearchToolCallItemParam,
  """
    The results of a file search tool call. See the
    [file search guide](/docs/guides/tools-file-search) for more information.
    
    """
);
// Tool customization: Rename to supply base for split input/output models
/**
 * A tool call to run a function. See the
 * [function calling guide](/docs/guides/function-calling) for more information.
 */
alias FunctionToolCallItemBase = {
  /** The unique ID of the function tool call generated by the model. */
  call_id: string;

  /** The name of the function to run. */
  name: string;

  /** A JSON string of the arguments to pass to the function. */
  arguments: string;
};

@@doc(FunctionToolCallItemResource,
  """
    A tool call to run a function. See the
    [function calling guide](/docs/guides/function-calling) for more information.
    
    """
);
@@doc(FunctionToolCallItemParam,
  """
    A tool call to run a function. See the
    [function calling guide](/docs/guides/function-calling) for more information.
    
    """
);
// Tool customization: Rename to supply base for split input/output models
/** The output of a function tool call. */
alias FunctionToolCallOutputItemBase = {
  /** The unique ID of the function tool call generated by the model. */
  call_id: string;

  /** A JSON string of the output of the function tool call. */
  output: string;
};

@@doc(FunctionToolCallOutputItemResource,
  """
    The output of a function tool call.
    
    """
);
@@doc(FunctionToolCallOutputItemParam,
  """
    The output of a function tool call.
    
    """
);

// Tool customization: Rename to supply base for split input/output models
/**
 * A tool call to a computer use tool. See the
 * [computer use guide](/docs/guides/tools-computer-use) for more information.
 */
alias ComputerToolCallItemBase = {
  /** An identifier used when responding to the tool call with output. */
  call_id: string;

  action: ComputerAction;

  /** The pending safety checks for the computer call. */
  pending_safety_checks: ComputerToolCallSafetyCheck[];
};

@@doc(ComputerToolCallItemResource,
  """
    A tool call to a computer use tool. See the
    [computer use guide](/docs/guides/tools-computer-use) for more information.
    
    """
);
@@doc(ComputerToolCallItemParam,
  """
    A tool call to a computer use tool. See the
    [computer use guide](/docs/guides/tools-computer-use) for more information.
    
    """
);
// Tool customization: Rename to supply base for split input/output models
/** The output of a computer tool call. */
alias ComputerToolCallOutputItemBase = {
  /** The ID of the computer tool call that produced the output. */
  call_id: string;

  /**
   * The safety checks reported by the API that have been acknowledged by the
   * developer.
   */
  acknowledged_safety_checks?: ComputerToolCallSafetyCheck[];

  // Tool customization: Apply discriminated type
  output: ComputerToolCallOutputItemOutput;
};

@@doc(ComputerToolCallOutputItemResource,
  """
    The output of a computer tool call.
    
    """
);
@@doc(ComputerToolCallOutputItemParam,
  """
    The output of a computer tool call.
    
    """
);
// Tool customization: Rename to supply base for split input/output models
alias ReasoningItemBase = {
  @doc("""
    The encrypted content of the reasoning item - populated when a response is
    generated with `reasoning.encrypted_content` in the `include` parameter.
    """)
  encrypted_content?: string | null;

  // Tool customization: Use customized discriminated type for summary parts
  /** Reasoning text contents. */
  summary: ReasoningItemSummaryPart[];
};

@@doc(ReasoningItemResource,
  """
    A description of the chain of thought used by a reasoning model while generating
    a response. Be sure to include these items in your `input` to the Responses API
    for subsequent turns of a conversation if you are manually
    [managing context](/docs/guides/conversation-state).
    
    """
);
@@doc(ReasoningItemParam,
  """
    A description of the chain of thought used by a reasoning model while generating
    a response. Be sure to include these items in your `input` to the Responses API
    for subsequent turns of a conversation if you are manually
    [managing context](/docs/guides/conversation-state).
    
    """
);
// Tool customization: Rename to supply base for split input/output models
/** An internal identifier for an item to reference. */
alias ItemReferenceItemBase = {};

@@doc(ItemReferenceItemParam,
  """
    An internal identifier for an item to reference.
    """
);

// Tool customization: Rename to supply base for split input/output models
/** An image generation request made by the model. */
alias ImageGenToolCallItemBase = {
  /** The generated image encoded in base64. */
  @encode("base64", string)
  result: bytes | null;
};

@@doc(ImageGenToolCallItemResource,
  """
    An image generation request made by the model.
    
    """
);
@@doc(ImageGenToolCallItemParam,
  """
    An image generation request made by the model.
    
    """
);

// Tool customization: Rename to supply base for split input/output models
/** A tool call to run a command on the local shell. */
alias LocalShellToolCallItemBase = {
  /** The unique ID of the local shell tool call generated by the model. */
  call_id: string;

  action: LocalShellExecAction;
};

@@doc(LocalShellToolCallItemResource,
  """
    A tool call to run a command on the local shell.
    
    """
);
@@doc(LocalShellToolCallItemParam,
  """
    A tool call to run a command on the local shell.
    
    """
);
// Tool customization: Rename to supply base for split input/output models
/** The output of a local shell tool call. */
alias LocalShellToolCallOutputItemBase = {
  /** A JSON string of the output of the local shell tool call. */
  output: string;
};

@@doc(LocalShellToolCallOutputItemResource,
  """
    The output of a local shell tool call.
    
    """
);
@@doc(LocalShellToolCallOutputItemParam,
  """
    The output of a local shell tool call.
    
    """
);
// Tool customization: Rename to supply base for split input/output models
/** A request for human approval of a tool invocation. */
alias MCPApprovalRequestItemBase = {
  /** The label of the MCP server making the request. */
  server_label: string;

  /** The name of the tool to run. */
  name: string;

  /** A JSON string of arguments for the tool. */
  arguments: string;
};

@@doc(MCPApprovalRequestItemResource,
  """
    A request for human approval of a tool invocation.
    
    """
);
@@doc(MCPApprovalRequestItemParam,
  """
    A request for human approval of a tool invocation.
    
    """
);
// Tool customization: Rename to supply base for split input/output models
/** A response to an MCP approval request. */
alias MCPApprovalResponseItemBase = {
  /** The ID of the approval request being answered. */
  approval_request_id: string;

  /** Whether the request was approved. */
  approve: boolean;

  /** Optional reason for the decision. */
  reason?: string | null;
};

@@doc(MCPApprovalResponseItemResource,
  """
    A response to an MCP approval request.
    
    """
);
@@doc(MCPApprovalResponseItemParam,
  """
    A response to an MCP approval request.
    
    """
);
// Tool customization: Stub for already-split type
alias MCPApprovalResponseResource = MCPApprovalResponseItemResource;

// Tool customization: Rename to supply base for split input/output models
/** A list of tools available on an MCP server. */
alias MCPListToolsItemBase = {
  /** The label of the MCP server. */
  server_label: string;

  /** The tools available on the server. */
  tools: MCPListToolsTool[];

  /** Error message if the server could not list tools. */
  error?: string | null;
};

@@doc(MCPListToolsItemResource,
  """
    A list of tools available on an MCP server.
    
    """
);
@@doc(MCPListToolsItemParam,
  """
    A list of tools available on an MCP server.
    
    """
);

// Tool customization: Rename to supply base for split input/output models
/** An invocation of a tool on an MCP server. */
alias MCPCallItemBase = {
  /** The label of the MCP server running the tool. */
  server_label: string;

  /** The name of the tool that was run. */
  name: string;

  /** A JSON string of the arguments passed to the tool. */
  arguments: string;

  /** The output from the tool call. */
  output?: string | null;

  /** The error from the tool call, if any. */
  error?: string | null;
};

@@doc(MCPCallItemResource,
  """
    An invocation of a tool on an MCP server.
    
    """
);
@@doc(MCPCallItemParam,
  """
    An invocation of a tool on an MCP server.
    
    """
);

// Tool customization: Rename to supply base for split input/output models
/** A tool call to run code. */
alias CodeInterpreterToolCallItemBase = {
  /** The ID of the container used to run the code. */
  container_id?: string;

  /** The code to run. */
  code: string;

  /** The outputs of the code interpreter tool call. */
  outputs?: CodeInterpreterToolOutput[];
};

@@doc(CodeInterpreterToolCallItemResource,
  """
    A tool call to run code.
    
    """
);
@@doc(CodeInterpreterToolCallItemParam,
  """
    A tool call to run code.
    
    """
);

// Tool customization: Stub/remove deeply customized type
alias InputItem = ItemParam;

// Tool customization: Stub/remove deeply customized type
alias InputContent = ItemContent;

// Tool customization: Stub/remove deeply customized type
alias OutputContent = ItemContent;

// Tool customization: Stub/remove deeply customized type
alias OutputItem = ItemResource;

// Tool customization: Stub/remove deeply customized type
alias OutputMessage = ResponsesMessageItemResource;

// Tool customization: Stub/remove deeply customized type
alias InputMessage = ResponsesMessageItemResource;

// Tool customization: Stub/remove deeply customized type
alias InputMessageResource = ResponsesMessageItemParam;

// Tool customization: Stub/remove deeply customized type
alias EasyInputMessage = unknown;

// Tool customization: Stub/remove deeply customized type
alias ComputerCallOutputItemParam = ItemParam;

// Tool customization: Stub/remove deeply customized type
alias ComputerToolCallOutputResource = ComputerToolCallOutputItemResource;

// Tool customization: Stub/remove deeply customized type
alias FunctionToolCallResource = FunctionToolCallItemResource;

// Tool customization: Stub/remove deeply customized type
alias FunctionToolCallOutputResource = FunctionToolCallOutputItemResource;

// Tool customization: Stub/remove deeply customized type
alias FunctionCallOutputItemParam = FunctionToolCallOutputItemParam;

/** A list of Response items. */
model ResponseItemList {
  @doc("""
    The type of object returned, must be `list`.
    """)
  object: "list";

  /** A list of items used to generate this response. */
  @pageItems data: ItemResource[];

  /** Whether there are more items available. */
  has_more: boolean;

  /** The ID of the first item in the list. */
  first_id: string;

  /** The ID of the last item in the list. */
  @continuationToken last_id: string;
}

model FileSearchToolCallItemParam extends ItemParam {
  type: ItemType.file_search_call;
  ...FileSearchToolCallItemBase;
}

model FileSearchToolCallItemResource extends ItemResource {
  type: ItemType.file_search_call;

  @doc("""
    The status of the file search tool call. One of `in_progress`,
    `searching`, `completed`, `incomplete`, or `failed`.
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
  action?: ImageGenerationToolAction;
  background?: ImageGenerationBackground;
  output_format?: ImageGenerationOutputFormat | null;
  quality?: ImageGenerationQuality | null;
  size?: ImageGenerationSize | null;
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

/** A tool call that applies file diffs by creating, deleting, or updating files. */
@summary("Apply patch tool call")
model ApplyPatchToolCallItemResource extends ItemResource {
  /** The type of the item. Always `apply_patch_call`. */
  type: ItemType.apply_patch_call;

  /** The unique ID of the apply patch tool call generated by the model. */
  call_id: string;

  /** The status of the apply patch tool call. One of `in_progress` or `completed`. */
  status: ApplyPatchCallStatus;

  /** One of the create_file, delete_file, or update_file operations applied via apply_patch. */
  @summary("Apply patch operation")
  operation: ApplyPatchOperation;

  /** The ID of the entity that created this tool call. */
  created_by?: string;
}

/** Status values reported for apply_patch tool calls. */
@summary("Apply patch call status")
union ApplyPatchCallStatus {
  string,
  in_progress: "in_progress",
  completed: "completed",
}

union ApplyPatchOperationType {
  string,
  create_file: "create_file",
  delete_file: "delete_file",
  update_file: "update_file",
}

/** One of the create_file, delete_file, or update_file operations supplied to the apply_patch tool. */
@summary("Apply patch operation")
@discriminator("type")
model ApplyPatchOperation  {
  type: ApplyPatchOperationType;
}

/** Instruction describing how to create a file via the apply_patch tool. */
@summary("Apply patch create file operation")
model ApplyPatchCreateFileOperation extends ApplyPatchOperation {
  /** Create a new file with the provided diff. */
  type: ApplyPatchOperationType.create_file;

  /** Path of the file to create. */
  path: string;

  /** Diff to apply. */
  diff: string;
}

/** Instruction describing how to delete a file via the apply_patch tool. */
@summary("Apply patch delete file operation")
model ApplyPatchDeleteFileOperation extends ApplyPatchOperation {
  /** Delete the specified file. */
  type: ApplyPatchOperationType.delete_file;

  /** Path of the file to delete. */
  path: string;
}

/** Instruction describing how to update a file via the apply_patch tool. */
@summary("Apply patch update file operation")
model ApplyPatchUpdateFileOperation extends ApplyPatchOperation {
  /** Update an existing file with the provided diff. */
  type: ApplyPatchOperationType.update_file;

  /** Path of the file to update. */
  path: string;

  /** Diff to apply. */
  diff: string;
}

/** The output emitted by an apply patch tool call. */
@summary("Apply patch tool call output")
model ApplyPatchToolCallOutputItemResource extends ItemResource {
  /** The type of the item. Always `apply_patch_call_output`. */
  type: ItemType.apply_patch_call_output;

  /** The unique ID of the apply patch tool call generated by the model. */
  call_id: string;

  /** The status of the apply patch tool call output. One of `completed` or `failed`. */
  status: ApplyPatchCallOutputStatus;

  output?: string | null;

  /** The ID of the entity that created this tool call output. */
  created_by?: string;
}

union ApplyPatchCallOutputStatus {
  string,
  completed: "completed",
  failed: "failed",
}

// ---------------------------------------------------------------------------
// Message items (moved from custom/items.messages.tsp)
// ---------------------------------------------------------------------------

/** The collection of valid roles for responses message items. */
union ResponsesMessageRole {
  system: "system",
  developer: "developer",
  user: "user",
  assistant: "assistant",
}

/** A response message item, representing a role and content, as provided as client request parameters. */
@discriminator("role")
model ResponsesMessageItemParam extends ItemParam {
  /** The type of the responses item, which is always 'message'. */
  type: ItemType.message;

  /** The role associated with the message. */
  role: ResponsesMessageRole;
}

/** A message parameter item with the `user` role. */
model ResponsesUserMessageItemParam extends ResponsesMessageItemParam {
  /** The role of the message, which is always `user`. */
  role: ResponsesMessageRole.user;

  /** The content associated with the message. */
  content: InputContent[];
}

/** A message parameter item with the `system` role. */
model ResponsesSystemMessageItemParam extends ResponsesMessageItemParam {
  /** The role of the message, which is always `system`. */
  role: ResponsesMessageRole.system;

  /** The content associated with the message. */
  content: InputContent[];
}

/** A message parameter item with the `developer` role. */
model ResponsesDeveloperMessageItemParam extends ResponsesMessageItemParam {
  /** The role of the message, which is always `developer`. */
  role: ResponsesMessageRole.developer;

  /** The content associated with the message. */
  content: InputContent[];
}

/** A message parameter item with the `assistant` role. */
model ResponsesAssistantMessageItemParam extends ResponsesMessageItemParam {
  /** The role of the message, which is always `assistant`. */
  role: ResponsesMessageRole.assistant;

  /** The content associated with the message. */
  content: OutputContent[];
}

/** A response message resource item, representing a role and content, as provided on service responses. */
@discriminator("role")
model ResponsesMessageItemResource extends ItemResource {
  /** The type of the responses item, which is always 'message'. */
  type: ItemType.message;

  @doc("""
    The status of the item. One of `in_progress`, `completed`, or
    `incomplete`. Populated when items are returned via API.
    """)
  status: "in_progress" | "completed" | "incomplete";

  /** The role associated with the message. */
  role: ResponsesMessageRole;
}

/** A message resource item with the `user` role. */
model ResponsesUserMessageItemResource extends ResponsesMessageItemResource {
  /** The role of the message, which is always `user`. */
  role: ResponsesMessageRole.user;

  /** The content associated with the message. */
  content: InputContent[];
}

/** A message resource item with the `system` role. */
model ResponsesSystemMessageItemResource extends ResponsesMessageItemResource {
  /** The role of the message, which is always `system`. */
  role: ResponsesMessageRole.system;

  /** The content associated with the message. */
  content: InputContent[];
}

/** A message resource item with the `developer` role. */
model ResponsesDeveloperMessageItemResource
  extends ResponsesMessageItemResource {
  /** The role of the message, which is always `developer`. */
  role: ResponsesMessageRole.developer;

  /** The content associated with the message. */
  content: InputContent[];
}

/** A message resource item with the `assistant` role. */
model ResponsesAssistantMessageItemResource
  extends ResponsesMessageItemResource {
  /** The role of the message, which is always `assistant`. */
  role: ResponsesMessageRole.assistant;

  /** The content associated with the message. */
  content: OutputContent[];
}

model ImplicitUserMessage {
  content: string | ItemContent[];
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetItemReferenceItemResource extends ItemResource {
  type: ItemType.item_reference;
  ...ItemReferenceItemBase;
}
