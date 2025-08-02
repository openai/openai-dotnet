/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "../common";
import "../messages";
import "../runs";
import "../threads";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@doc("""
  Represents an event emitted when streaming a Run.
  
  Each event in a server-sent events stream has an `event` and `data` property:
  
  ```
  event: thread.created
  data: {"id": "thread_123", "object": "thread", ...}
  ```
  
  We emit events whenever a new object is created, transitions to a new state, or is being
  streamed in parts (deltas). For example, we emit `thread.run.created` when a new run
  is created, `thread.run.completed` when a run completes, and so on. When an Assistant chooses
  to create a message during a run, we emit a `thread.message.created event`, a
  `thread.message.in_progress` event, many `thread.message.delta` events, and finally a
  `thread.message.completed` event.
  
  We may add additional events over time, so we recommend handling unknown events gracefully
  in your code. See the [Assistants API quickstart](/docs/assistants/overview) to learn how to
  integrate the Assistants API with streaming.
  """)
union AssistantStreamEvent {
  ThreadStreamEvent,
  RunStreamEvent,
  RunStepStreamEvent,
  MessageStreamEvent,
  ErrorEvent,
  DoneEvent,
}

union ThreadStreamEvent {
  {
    /** Whether to enable input audio transcription. */
    enabled?: boolean,

    event: "thread.created",
    data: ThreadObject,
  },
}

union RunStreamEvent {
  {
    event: "thread.run.created",
    data: RunObject,
  },
  {
    event: "thread.run.queued",
    data: RunObject,
  },
  {
    event: "thread.run.in_progress",
    data: RunObject,
  },
  {
    event: "thread.run.requires_action",
    data: RunObject,
  },
  {
    event: "thread.run.completed",
    data: RunObject,
  },
  {
    event: "thread.run.incomplete",
    data: RunObject,
  },
  {
    event: "thread.run.failed",
    data: RunObject,
  },
  {
    event: "thread.run.cancelling",
    data: RunObject,
  },
  {
    event: "thread.run.cancelled",
    data: RunObject,
  },
  {
    event: "thread.run.expired",
    data: RunObject,
  },
}

union RunStepStreamEvent {
  {
    event: "thread.run.step.created",
    data: RunStepObject,
  },
  {
    event: "thread.run.step.in_progress",
    data: RunStepObject,
  },
  {
    event: "thread.run.step.delta",
    data: RunStepDeltaObject,
  },
  {
    event: "thread.run.step.completed",
    data: RunStepObject,
  },
  {
    event: "thread.run.step.failed",
    data: RunStepObject,
  },
  {
    event: "thread.run.step.cancelled",
    data: RunStepObject,
  },
  {
    event: "thread.run.step.expired",
    data: RunStepObject,
  },
}

union MessageStreamEvent {
  {
    event: "thread.message.created",
    data: MessageObject,
  },
  {
    event: "thread.message.in_progress",
    data: MessageObject,
  },
  {
    event: "thread.message.delta",
    data: MessageDeltaObject,
  },
  {
    event: "thread.message.completed",
    data: MessageObject,
  },
  {
    event: "thread.message.incomplete",
    data: MessageObject,
  },
}

/** Occurs when an [error](/docs/guides/error-codes#api-errors) occurs. This can happen due to an internal server error or a timeout. */
model ErrorEvent {
  event: "error";
  data: Error;
}

/** Occurs when a stream ends. */
model DoneEvent {
  event: "done";
  data: "[DONE]";
}
