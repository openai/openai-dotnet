import "./content_parts.tsp";

using TypeSpec.OpenAPI;

namespace OpenAI;

@discriminator("type")
model RealtimeConversationRequestItem {
  ...RealtimeConversationItemBase;
  type: RealtimeItemType;
  id?: string;
}

@discriminator("role")
model RealtimeRequestMessageItem extends RealtimeConversationRequestItem {
  type: RealtimeItemType.message;
  role: RealtimeMessageRole;
  status?: RealtimeItemStatus;
}

model RealtimeRequestSystemMessageItem extends RealtimeRequestMessageItem {
  role: RealtimeMessageRole.system;
  content: RealtimeRequestTextContentPart[];
}

model RealtimeRequestUserMessageItem extends RealtimeRequestMessageItem {
  role: RealtimeMessageRole.user;
  content: (RealtimeRequestTextContentPart | RealtimeRequestAudioContentPart)[];
}

model RealtimeRequestAssistantMessageItem extends RealtimeRequestMessageItem {
  role: RealtimeMessageRole.assistant;
  content: RealtimeRequestTextContentPart[];
}

model RealtimeRequestFunctionCallItem extends RealtimeConversationRequestItem {
  type: RealtimeItemType.function_call;
  name: string;
  call_id: string;
  arguments: string;
  status?: RealtimeItemStatus;
}

model RealtimeRequestFunctionCallOutputItem
  extends RealtimeConversationRequestItem {
  type: RealtimeItemType.function_call_output;
  call_id: string;
  output: string;
}

// TODO: representation of a doubly-discriminated type with an absent second discriminator
//   (first discriminator: type = message; second discriminator: no role present)

model RealtimeRequestMessageReferenceItem { // extends RealtimeConversationRequestItem {
  type: RealtimeItemType.message;
  id: string;
}

@discriminator("type")
model RealtimeConversationResponseItem {
  ...RealtimeConversationItemBase;
  object: "realtime.item";
  type: RealtimeItemType;
  id: string | null;
}

model RealtimeResponseMessageItem extends RealtimeConversationResponseItem {
  type: RealtimeItemType.message;
  role: RealtimeMessageRole;
  content: RealtimeContentPart[];
  status: RealtimeItemStatus;
}

model RealtimeResponseFunctionCallItem
  extends RealtimeConversationResponseItem {
  type: RealtimeItemType.function_call;
  name: string;
  call_id: string;
  arguments: string;
  status: RealtimeItemStatus;
}

model RealtimeResponseFunctionCallOutputItem
  extends RealtimeConversationResponseItem {
  type: RealtimeItemType.function_call_output;
  call_id: string;
  output: string;
}

/** A Realtime item representing an invocation of a tool on an MCP server. */
model RealtimeRequestMCPCallItem extends RealtimeConversationRequestItem {
  type: RealtimeItemType.mcp_call;
  /** The label of the MCP server running the tool. */
  server_label: string;
  /** The name of the tool that was run. */
  name: string;
  /** A JSON string of the arguments passed to the tool. */
  arguments: string;
  /** The ID of the approval request, if approval was required. */
  approval_request_id?: string | null;
}

/** A Realtime item representing an invocation of a tool on an MCP server. */
model RealtimeResponseMCPCallItem extends RealtimeConversationResponseItem {
  type: RealtimeItemType.mcp_call;
  /** The label of the MCP server running the tool. */
  server_label: string;
  /** The name of the tool that was run. */
  name: string;
  /** A JSON string of the arguments passed to the tool. */
  arguments: string;
  /** The ID of the approval request, if approval was required. */
  approval_request_id?: string | null;
  /** The output of the tool call, if completed. */
  output?: string | null;
  /** Error details if the tool call failed. */
  error?: RealtimeMCPError;
  status: RealtimeItemStatus;
}

/** A Realtime item listing tools available on an MCP server. */
model RealtimeRequestMCPListToolsItem extends RealtimeConversationRequestItem {
  type: RealtimeItemType.mcp_list_tools;
  /** The label of the MCP server. */
  server_label: string;
  /** The tools available on the server. */
  tools: MCPListToolsTool[];
}

/** A Realtime item listing tools available on an MCP server. */
model RealtimeResponseMCPListToolsItem extends RealtimeConversationResponseItem {
  type: RealtimeItemType.mcp_list_tools;
  /** The label of the MCP server. */
  server_label: string;
  /** The tools available on the server. */
  tools: MCPListToolsTool[];
}

/** A Realtime item requesting human approval of a tool invocation. */
model RealtimeRequestMCPApprovalRequestItem extends RealtimeConversationRequestItem {
  type: RealtimeItemType.mcp_approval_request;
  /** The label of the MCP server making the request. */
  server_label: string;
  /** The name of the tool to run. */
  name: string;
  /** A JSON string of arguments for the tool. */
  arguments: string;
}

/** A Realtime item requesting human approval of a tool invocation. */
model RealtimeResponseMCPApprovalRequestItem extends RealtimeConversationResponseItem {
  type: RealtimeItemType.mcp_approval_request;
  /** The label of the MCP server making the request. */
  server_label: string;
  /** The name of the tool to run. */
  name: string;
  /** A JSON string of arguments for the tool. */
  arguments: string;
}

/** A Realtime item responding to an MCP approval request. */
model RealtimeRequestMCPApprovalResponseItem extends RealtimeConversationRequestItem {
  type: RealtimeItemType.mcp_approval_response;
  /** The ID of the approval request being answered. */
  approval_request_id: string;
  /** Whether the request was approved. */
  approve: boolean;
  /** The reason for the approval decision. */
  reason?: string | null;
}

/** A Realtime item responding to an MCP approval request. */
model RealtimeResponseMCPApprovalResponseItem extends RealtimeConversationResponseItem {
  type: RealtimeItemType.mcp_approval_response;
  /** The ID of the approval request being answered. */
  approval_request_id: string;
  /** Whether the request was approved. */
  approve: boolean;
  /** The reason for the approval decision. */
  reason?: string | null;
}

union RealtimeItemType {
  string,
  message: "message",
  function_call: "function_call",
  function_call_output: "function_call_output",
  mcp_call: "mcp_call",
  mcp_call_output: "mcp_call_output",
  mcp_list_tools: "mcp_list_tools",
  mcp_approval_request: "mcp_approval_request",
  mcp_approval_response: "mcp_approval_response",
}

union RealtimeItemStatus {
  string,
  in_progress: "in_progress",
  completed: "completed",
  incomplete: "incomplete",
}

union RealtimeMessageRole {
  string,
  system: "system",
  user: "user",
  assistant: "assistant",
}
