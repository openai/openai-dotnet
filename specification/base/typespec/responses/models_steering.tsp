import "./models_streaming.tsp";

namespace OpenAI;

/** A steering submission owned by the server but not necessarily applied yet. */
model ResponseSteerSubmission {
  id: string;
  previous_response_id: string;
}

/** Accepted steering input is queued; the successor's response.created is the commit point. */
model ResponseSteerAcceptedEvent extends ResponseStreamEvent {
  type: ResponseStreamEventType.response_steer_accepted;
  steer: ResponseSteerSubmission;
  stream_id?: string;
}

union ResponseSteerPendingReason {
  string,
  waiting_for_required_input: "waiting_for_required_input",
}

/** Input still queued after completion. Supply saved tool results once per parent; do not resend accepted input. */
model ResponseSteerPendingEvent extends ResponseStreamEvent {
  type: ResponseStreamEventType.response_steer_pending;
  steer: ResponseSteerSubmission;
  reason: ResponseSteerPendingReason;
  @minItems(1)
  required_input: ResponseSteerRequiredInput[];
  stream_id?: string;
}

union ResponseSteerErrorCode {
  string,
  response_not_found: "response_not_found",
  invalid_input: "invalid_input",
  steering_not_supported: "steering_not_supported",
  too_many_pending_steers: "too_many_pending_steers",
  response_already_completed: "response_already_completed",
  response_not_active: "response_not_active",
  successor_creation_failed: "successor_creation_failed",
}

model ResponseSteerError {
  type: "invalid_request_error";
  code: ResponseSteerErrorCode;
  message: string;
}

model ResponseSteerInputMessage {
  type?: "message";
  role: "user";
  content: string | ItemContent[];
}

model ResponseSteerFailedSubmission {
  id?: string;
  previous_response_id: string;
  input: string | ResponseSteerInputMessage[];
}

/** Returns uncommitted input. A lost acknowledgement does not prove rejection; do not replay automatically. */
model ResponseSteerFailedEvent extends ResponseStreamEvent {
  type: ResponseStreamEventType.response_steer_failed;
  steer: ResponseSteerFailedSubmission;
  error: ResponseSteerError;
  stream_id?: string;
}

union ResponseSteerRequiredInputKind {
  string,
  function_call_output: "function_call_output",
  custom_tool_call_output: "custom_tool_call_output",
  computer_call_output: "computer_call_output",
  shell_call_output: "shell_call_output",
  apply_patch_call_output: "apply_patch_call_output",
  tool_search_output: "tool_search_output",
  mcp_approval_response: "mcp_approval_response",
}

/** Identifies outstanding input; result fields must come from the application. */
@discriminator("type")
model ResponseSteerRequiredInput {
  type: ResponseSteerRequiredInputKind;
}

/** Supply output from the saved function result, without executing the tool again. */
model ResponseSteerFunctionCallOutput extends ResponseSteerRequiredInput {
  type: ResponseSteerRequiredInputKind.function_call_output;
  call_id: string;
  name: string;
}

model ResponseSteerCustomToolCallOutput extends ResponseSteerRequiredInput {
  type: ResponseSteerRequiredInputKind.custom_tool_call_output;
  call_id: string;
}

model ResponseSteerComputerCallOutput extends ResponseSteerRequiredInput {
  type: ResponseSteerRequiredInputKind.computer_call_output;
  call_id: string;
}

model ResponseSteerShellCallOutput extends ResponseSteerRequiredInput {
  type: ResponseSteerRequiredInputKind.shell_call_output;
  call_id: string;
}

model ResponseSteerApplyPatchCallOutput extends ResponseSteerRequiredInput {
  type: ResponseSteerRequiredInputKind.apply_patch_call_output;
  call_id: string;
}

model ResponseSteerToolSearchOutput extends ResponseSteerRequiredInput {
  type: ResponseSteerRequiredInputKind.tool_search_output;
  call_id: string;
  execution: "client";
}

model ResponseSteerMcpApprovalResponse extends ResponseSteerRequiredInput {
  type: ResponseSteerRequiredInputKind.mcp_approval_response;
  approval_request_id: string;
}
