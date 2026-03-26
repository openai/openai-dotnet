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
  apply_patch_call: "apply_patch_call",
  apply_patch_call_output: "apply_patch_call_output",
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