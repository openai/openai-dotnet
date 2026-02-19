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
  shell_call: "shell_call",
  shell_call_output: "shell_call_output",
  apply_patch_call: "apply_patch_call",
  apply_patch_call_output: "apply_patch_call_output",
  custom_tool_call: "custom_tool_call",
  custom_tool_call_output: "custom_tool_call_output",
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

model WebSearchToolCallItemParam extends ItemParam {
  type: ItemType.web_search_call;
  ...WebSearchToolCallItemBase;
}

model WebSearchToolCallItemResource extends ItemResource {
  type: ItemType.web_search_call;

  /** The status of the web search tool call. */
  status: "in_progress" | "searching" | "completed" | "failed";

  ...WebSearchToolCallItemBase;
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

model FunctionShellCallItemParam extends ItemParam {
  type: ItemType.shell_call;
  ...FunctionShellCallItemBase;
}

model FunctionShellCallItemResource extends ItemResource {
  type: ItemType.shell_call;
  status: FunctionShellCallItemStatus;
  ...FunctionShellCallItemBase;
}

model FunctionShellCallOutputItemParam extends ItemParam {
  type: ItemType.shell_call_output;
  ...FunctionShellCallOutputItemBase;
}

model FunctionShellCallOutputItemResource extends ItemResource {
  type: ItemType.shell_call_output;
  status: "in_progress" | "completed" | "incomplete";
  ...FunctionShellCallOutputItemBase;
}

model ApplyPatchToolCallItemParam extends ItemParam {
  type: ItemType.apply_patch_call;
  ...ApplyPatchToolCallItemBase;
}

model ApplyPatchToolCallItemResource extends ItemResource {
  type: ItemType.apply_patch_call;
  status: ApplyPatchCallStatusParam;
  ...ApplyPatchToolCallItemBase;
}

model ApplyPatchToolCallOutputItemParam extends ItemParam {
  type: ItemType.apply_patch_call_output;
  ...ApplyPatchToolCallOutputItemBase;
}

model ApplyPatchToolCallOutputItemResource extends ItemResource {
  type: ItemType.apply_patch_call_output;
  status: ApplyPatchCallOutputStatusParam;
  ...ApplyPatchToolCallOutputItemBase;
}

model CustomToolCallItemParam extends ItemParam {
  type: ItemType.custom_tool_call;
  ...CustomToolCallItemBase;
}

model CustomToolCallItemResource extends ItemResource {
  type: ItemType.custom_tool_call;
  status: "in_progress" | "completed" | "incomplete";
  ...CustomToolCallItemBase;
}

model CustomToolCallOutputItemParam extends ItemParam {
  type: ItemType.custom_tool_call_output;
  ...CustomToolCallOutputItemBase;
}

model CustomToolCallOutputItemResource extends ItemResource {
  type: ItemType.custom_tool_call_output;
  status: "in_progress" | "completed" | "incomplete";
  ...CustomToolCallOutputItemBase;
}
