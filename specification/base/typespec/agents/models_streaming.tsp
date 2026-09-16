import "@typespec/openapi";
import "@typespec/events";
import "@azure-tools/typespec-client-generator-core";

import "./models.tsp";
import "./models_items.tsp";

using Azure.ClientGenerator.Core;
using TypeSpec.Events;
using TypeSpec.OpenAPI;

namespace OpenAI;

// ------------ Session event support models ------------

/** An error payload with the same public fields as Responses API streaming errors. */
model SessionErrorResource {
  /** The error type. */
  type: string;

  /** The machine-readable error code, if any. */
  code: string | null;

  /** A customer-safe explanation of the error. */
  message: string;

  /** The request parameter associated with the error, if any. */
  param: string | null;
}

/** The connection status of a session environment. */
union SessionEnvironmentStatusResource {
  /** The environment is being prepared. */
  pending: "pending",

  /** The environment is ready to connect. */
  ready: "ready",

  /** The environment is connected. */
  connected: "connected",

  /** The environment is disconnected. */
  disconnected: "disconnected",

  /** The environment failed to connect. */
  failed: "failed",
}

/** An error reported while preparing a session environment. */
model SessionEnvironmentErrorResource {
  /** The error type. */
  type: string;

  /** A machine-readable error code. */
  code: string;

  /** A human-readable error message. */
  message: string;
}

/** The current state of a session environment. */
model SessionEnvironmentStateResource {
  /** The public ID of the environment. */
  id: string;

  /** The environment type. */
  type: string;

  /** The environment's connection status. */
  status: SessionEnvironmentStatusResource;

  /** The error reported while preparing the environment, if any. */
  error: SessionEnvironmentErrorResource | null;
}

// ------------ Session events ------------

/** Emitted when a turn or session fails. */
model SessionEventError {
  /** The type of the object. Always `error`. */
  type: "error";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The error that occurred. */
  error: SessionErrorResource;
}

/** Emitted when a hosted session environment is ready to connect. */
model SessionEventAgentSessionEnvironmentReady {
  /** The type of the object. Always `agent.session.environment.ready`. */
  type: "agent.session.environment.ready";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The current environment state. */
  environment: SessionEnvironmentStateResource;
}

/** Emitted when command execution produces an output delta. */
model SessionEventAgentOutputCommandExecutionOutputDelta {
  /** The type of the object. Always `agent.output.command_execution_output.delta`. */
  type: "agent.output.command_execution_output.delta";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The ID of the command execution item. */
  item_id: string;

  /** The index of the item in the turn output. */
  @minValue(0)
  @maxValue(4294967295)
  output_index: int64;

  /** The output text that was appended. */
  delta: string;
}

/** Emitted when a session is created. */
model SessionEventAgentSessionCreated {
  /** The type of the object. Always `agent.session.created`. */
  type: "agent.session.created";

  /** The unique ID of the event. */
  event_id: string;

  /** The session that was created. */
  session: SessionResource;
}

/** Emitted when a turn is created. */
model SessionEventAgentSessionTurnCreated {
  /** The type of the object. Always `agent.session.turn.created`. */
  type: "agent.session.turn.created";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event. */
  turn_id: string;

  /** The turn at the time it was created. */
  turn: TurnResource;
}

/** Emitted when a turn starts running. */
model SessionEventAgentSessionTurnInProgress {
  /** The type of the object. Always `agent.session.turn.in_progress`. */
  type: "agent.session.turn.in_progress";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event. */
  turn_id: string;

  /** The turn at the time it started running. */
  turn: TurnResource;
}

/** Emitted when a turn completes. */
model SessionEventAgentSessionTurnCompleted {
  /** The type of the object. Always `agent.session.turn.completed`. */
  type: "agent.session.turn.completed";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event. */
  turn_id: string;

  /** The completed turn. */
  turn: TurnResource;

  /** Token usage by the root agent during the turn, when available. */
  usage: TokenUsageResource | null;
}

/** Emitted when a turn fails. */
model SessionEventAgentSessionTurnFailed {
  /** The type of the object. Always `agent.session.turn.failed`. */
  type: "agent.session.turn.failed";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event. */
  turn_id: string;

  /** The failed turn. */
  turn: TurnResource;

  /** Token usage by the root agent during the turn, when available. */
  usage: TokenUsageResource | null;
}

/** Emitted when a turn is cancelled. */
model SessionEventAgentSessionTurnCancelled {
  /** The type of the object. Always `agent.session.turn.cancelled`. */
  type: "agent.session.turn.cancelled";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event. */
  turn_id: string;

  /** The cancelled turn. */
  turn: TurnResource;

  /** Token usage by the root agent during the turn, when available. */
  usage: TokenUsageResource | null;
}

/** Emitted when an item is added to a turn. */
model SessionEventAgentSessionTurnItemAdded {
  /** The type of the object. Always `agent.session.turn.item.added`. */
  type: "agent.session.turn.item.added";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The index of the item in the turn output, when the item is agent output. */
  @minValue(0)
  @maxValue(4294967295)
  output_index: int64 | null;

  /** The item that was added. */
  item: SessionTurnItemResource;
}

/** Emitted when a session becomes idle. */
model SessionEventAgentSessionIdle {
  /** The type of the object. Always `agent.session.idle`. */
  type: "agent.session.idle";

  /** The unique ID of the event. */
  event_id: string;

  /** The session that became idle. */
  session: SessionResource;
}

/** Emitted when a session starts processing a turn. */
model SessionEventAgentSessionInProgress {
  /** The type of the object. Always `agent.session.in_progress`. */
  type: "agent.session.in_progress";

  /** The unique ID of the event. */
  event_id: string;

  /** The session that started processing. */
  session: SessionResource;
}

/** Emitted when a session is waiting for one or more required actions. */
model SessionEventAgentSessionRequiresAction {
  /** The type of the object. Always `agent.session.requires_action`. */
  type: "agent.session.requires_action";

  /** The unique ID of the event. */
  event_id: string;

  /** The session and its current required actions. */
  session: SessionResource;
}

/** Emitted when a session fails. */
model SessionEventAgentSessionFailed {
  /** The type of the object. Always `agent.session.failed`. */
  type: "agent.session.failed";

  /** The unique ID of the event. */
  event_id: string;

  /** The failed session. */
  session: SessionResource;
}

/** Emitted while a session environment is being prepared. */
model SessionEventAgentSessionEnvironmentPending {
  /** The type of the object. Always `agent.session.environment.pending`. */
  type: "agent.session.environment.pending";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The current environment state. */
  environment: SessionEnvironmentStateResource;
}

/** Emitted when a session environment connects. */
model SessionEventAgentSessionEnvironmentConnected {
  /** The type of the object. Always `agent.session.environment.connected`. */
  type: "agent.session.environment.connected";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The current environment state. */
  environment: SessionEnvironmentStateResource;
}

/** Emitted when a session environment disconnects. */
model SessionEventAgentSessionEnvironmentDisconnected {
  /** The type of the object. Always `agent.session.environment.disconnected`. */
  type: "agent.session.environment.disconnected";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The current environment state. */
  environment: SessionEnvironmentStateResource;
}

/** Emitted when a session environment fails. */
model SessionEventAgentSessionEnvironmentFailed {
  /** The type of the object. Always `agent.session.environment.failed`. */
  type: "agent.session.environment.failed";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The current environment state. */
  environment: SessionEnvironmentStateResource;
}

/** Emitted when a subagent is created. */
model SessionEventAgentSessionSubagentCreated {
  /** The type of the object. Always `agent.session.subagent.created`. */
  type: "agent.session.subagent.created";

  /** The unique ID of the event. */
  event_id: string;

  /** The subagent that was created. */
  subagent: SubagentResource;
}

/** Emitted when a closed subagent successfully resumes. */
model SessionEventAgentSessionSubagentActive {
  /** The type of the object. Always `agent.session.subagent.active`. */
  type: "agent.session.subagent.active";

  /** The unique ID of the event. */
  event_id: string;

  /** The subagent that resumed. */
  subagent: SubagentResource;
}

/** Emitted when a subagent is closed. */
model SessionEventAgentSessionSubagentClosed {
  /** The type of the object. Always `agent.session.subagent.closed`. */
  type: "agent.session.subagent.closed";

  /** The unique ID of the event. */
  event_id: string;

  /** The subagent that was closed. */
  subagent: SubagentResource;
}

/** Emitted when an output item is complete. */
model SessionEventAgentSessionTurnItemDone {
  /** The type of the object. Always `agent.session.turn.item.done`. */
  type: "agent.session.turn.item.done";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The index of the output item in the turn output. */
  @minValue(0)
  @maxValue(4294967295)
  output_index: int64;

  // note: spec types this as AgentOutputItemResource, a parallel discriminated union that reuses
  // the same variant schemas already parented to SessionTurnItemResource. TypeSpec `extends` is
  // single-inheritance, so the superset base SessionTurnItemResource is used here instead.
  /** The completed output item. */
  item: SessionTurnItemResource;
}

/** Emitted when an output text content part is added. */
model SessionEventAgentSessionTurnContentPartAdded {
  /** The type of the object. Always `agent.session.turn.content_part.added`. */
  type: "agent.session.turn.content_part.added";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The ID of the message item. */
  item_id: string;

  /** The index of the item in the turn output. */
  @minValue(0)
  @maxValue(4294967295)
  output_index: int64;

  /** The index of the content part in the message. */
  @minValue(0)
  @maxValue(4294967295)
  content_index: int64;

  /** The initial content part. */
  part: OutputTextResource;
}

/** Emitted when an output content part is complete. */
model SessionEventAgentSessionTurnContentPartDone {
  /** The type of the object. Always `agent.session.turn.content_part.done`. */
  type: "agent.session.turn.content_part.done";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The ID of the message item. */
  item_id: string;

  /** The index of the item in the turn output. */
  @minValue(0)
  @maxValue(4294967295)
  output_index: int64;

  /** The index of the content part in the message. */
  @minValue(0)
  @maxValue(4294967295)
  content_index: int64;

  /** The completed content part. */
  part: OutputTextResource;
}

/** Emitted when text is appended to an output text content part. */
model SessionEventAgentSessionTurnOutputTextDelta {
  /** The type of the object. Always `agent.session.turn.output_text.delta`. */
  type: "agent.session.turn.output_text.delta";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The ID of the message item. */
  item_id: string;

  /** The index of the item in the turn output. */
  @minValue(0)
  @maxValue(4294967295)
  output_index: int64;

  /** The index of the content part in the message. */
  @minValue(0)
  @maxValue(4294967295)
  content_index: int64;

  /** The text that was appended. */
  delta: string;
}

/** Emitted when an output text content part is complete. */
model SessionEventAgentSessionTurnOutputTextDone {
  /** The type of the object. Always `agent.session.turn.output_text.done`. */
  type: "agent.session.turn.output_text.done";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The ID of the message item. */
  item_id: string;

  /** The index of the item in the turn output. */
  @minValue(0)
  @maxValue(4294967295)
  output_index: int64;

  /** The index of the content part in the message. */
  @minValue(0)
  @maxValue(4294967295)
  content_index: int64;

  /** The complete output text. */
  text: string;
}

/** Emitted when a reasoning summary content part is added. */
model SessionEventAgentSessionTurnReasoningSummaryPartAdded {
  /** The type of the object. Always `agent.session.turn.reasoning_summary_part.added`. */
  type: "agent.session.turn.reasoning_summary_part.added";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The ID of the reasoning item. */
  item_id: string;

  /** The index of the item in the turn output. */
  @minValue(0)
  @maxValue(4294967295)
  output_index: int64;

  /** The index of the summary content part. */
  @minValue(0)
  @maxValue(4294967295)
  summary_index: int64;

  /** The initial summary part. */
  part: SummaryTextResource;
}

/** Emitted when a reasoning summary part is complete. */
model SessionEventAgentSessionTurnReasoningSummaryPartDone {
  /** The type of the object. Always `agent.session.turn.reasoning_summary_part.done`. */
  type: "agent.session.turn.reasoning_summary_part.done";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The ID of the reasoning item. */
  item_id: string;

  /** The index of the item in the turn output. */
  @minValue(0)
  @maxValue(4294967295)
  output_index: int64;

  /** The index of the summary part. */
  @minValue(0)
  @maxValue(4294967295)
  summary_index: int64;

  /** The completed summary part. */
  part: SummaryTextResource;

  /** Present as `incomplete` when summary generation was interrupted. */
  status: "incomplete" | null;
}

/** Emitted when text is appended to a reasoning summary. */
model SessionEventAgentSessionTurnReasoningSummaryTextDelta {
  /** The type of the object. Always `agent.session.turn.reasoning_summary_text.delta`. */
  type: "agent.session.turn.reasoning_summary_text.delta";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The ID of the reasoning item. */
  item_id: string;

  /** The index of the item in the turn output. */
  @minValue(0)
  @maxValue(4294967295)
  output_index: int64;

  /** The index of the summary content part. */
  @minValue(0)
  @maxValue(4294967295)
  summary_index: int64;

  /** The summary text that was appended. */
  delta: string;
}

/** Emitted when a reasoning summary content part is complete. */
model SessionEventAgentSessionTurnReasoningSummaryTextDone {
  /** The type of the object. Always `agent.session.turn.reasoning_summary_text.done`. */
  type: "agent.session.turn.reasoning_summary_text.done";

  /** The unique ID of the event. */
  event_id: string;

  /** The ID of the session associated with the event. */
  session_id: string;

  /** The ID of the turn associated with the event, when applicable. */
  turn_id: string | null;

  /** The ID of the reasoning item. */
  item_id: string;

  /** The index of the item in the turn output. */
  @minValue(0)
  @maxValue(4294967295)
  output_index: int64;

  /** The index of the summary content part. */
  @minValue(0)
  @maxValue(4294967295)
  summary_index: int64;

  /** The complete reasoning summary text. */
  text: string;
}

/** A live stream of events emitted for an agent session. */
@events
union SessionEvent {
  error: SessionEventError,
  `agent.session.environment.ready`: SessionEventAgentSessionEnvironmentReady,
  `agent.output.command_execution_output.delta`: SessionEventAgentOutputCommandExecutionOutputDelta,
  `agent.session.created`: SessionEventAgentSessionCreated,
  `agent.session.turn.created`: SessionEventAgentSessionTurnCreated,
  `agent.session.turn.in_progress`: SessionEventAgentSessionTurnInProgress,
  `agent.session.turn.completed`: SessionEventAgentSessionTurnCompleted,
  `agent.session.turn.failed`: SessionEventAgentSessionTurnFailed,
  `agent.session.turn.cancelled`: SessionEventAgentSessionTurnCancelled,
  `agent.session.turn.item.added`: SessionEventAgentSessionTurnItemAdded,
  `agent.session.idle`: SessionEventAgentSessionIdle,
  `agent.session.in_progress`: SessionEventAgentSessionInProgress,
  `agent.session.requires_action`: SessionEventAgentSessionRequiresAction,
  `agent.session.failed`: SessionEventAgentSessionFailed,
  `agent.session.environment.pending`: SessionEventAgentSessionEnvironmentPending,
  `agent.session.environment.connected`: SessionEventAgentSessionEnvironmentConnected,
  `agent.session.environment.disconnected`: SessionEventAgentSessionEnvironmentDisconnected,
  `agent.session.environment.failed`: SessionEventAgentSessionEnvironmentFailed,
  `agent.session.subagent.created`: SessionEventAgentSessionSubagentCreated,
  `agent.session.subagent.active`: SessionEventAgentSessionSubagentActive,
  `agent.session.subagent.closed`: SessionEventAgentSessionSubagentClosed,
  `agent.session.turn.item.done`: SessionEventAgentSessionTurnItemDone,
  `agent.session.turn.content_part.added`: SessionEventAgentSessionTurnContentPartAdded,
  `agent.session.turn.content_part.done`: SessionEventAgentSessionTurnContentPartDone,
  `agent.session.turn.output_text.delta`: SessionEventAgentSessionTurnOutputTextDelta,
  `agent.session.turn.output_text.done`: SessionEventAgentSessionTurnOutputTextDone,
  `agent.session.turn.reasoning_summary_part.added`: SessionEventAgentSessionTurnReasoningSummaryPartAdded,
  `agent.session.turn.reasoning_summary_part.done`: SessionEventAgentSessionTurnReasoningSummaryPartDone,
  `agent.session.turn.reasoning_summary_text.delta`: SessionEventAgentSessionTurnReasoningSummaryTextDelta,
  `agent.session.turn.reasoning_summary_text.done`: SessionEventAgentSessionTurnReasoningSummaryTextDone,
}

// ------------ Session input events ------------

/** The type of an input event submitted to a session. */
union SessionInputType {
  string,
  message: "agent.session.input.message",
  cancel: "agent.session.input.cancel",
  tool_result: "agent.session.input.tool_result",
}

@discriminator("type")
model SessionInputParam {
  type: SessionInputType;
}

/** Adds one or more user messages and starts a turn. */
model SessionInputParamAgentSessionInputMessage extends SessionInputParam {
  /** The type of the object. Always `agent.session.input.message`. */
  type: SessionInputType.message;

  /** The user messages to add to the session. */
  @maxItems(16384)
  input: InputMessageParam[];
}

/** Cancels the session's active turn. */
model SessionInputParamAgentSessionInputCancel extends SessionInputParam {
  /** The type of the object. Always `agent.session.input.cancel`. */
  type: SessionInputType.cancel;
}

/** Submits the result of a function call. */
model SessionInputParamAgentSessionInputToolResult extends SessionInputParam {
  /** The type of the object. Always `agent.session.input.tool_result`. */
  type: SessionInputType.tool_result;

  /** The ID of the turn that requested the function call. */
  @maxLength(1048576)
  turn_id: string;

  /** The ID of the function call. */
  @maxLength(1048576)
  call_id: string;

  /** Whether the function call succeeded. */
  success: boolean;

  /** The function result when the call succeeded, as text or supported model-input content. */
  output?: (string | InputContentParam[]) | null;

  /** The error message when the call failed. */
  @maxLength(1048576)
  error?: string | null;
}

/** Input events submitted to an existing session. */
model CreateSessionEventsParams {
  /** The input events to submit to the session. */
  @maxItems(16384)
  events: SessionInputParam[];
}
