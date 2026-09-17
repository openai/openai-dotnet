import "./models.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
namespace OpenAI;

union ResponseWebSocketCommandType {
  string,
  response_create: "response.create",
  response_steer: "response.steer",
}

/** A command sent over a Responses WebSocket connection. */
@discriminator("type")
@usage(Usage.input | Usage.json)
model ResponseWebSocketCommand {
  type: ResponseWebSocketCommandType;
}

/** Creates a response without closing the connection on completion. */
model ResponseWebSocketCreateCommand extends ResponseWebSocketCommand {
  type: ResponseWebSocketCommandType.response_create;
  stream_id?: string;
  ...OmitProperties<OmitProperties<CreateResponse, "stream">, "background">;

  /** Whether to generate a response; false warms the connection. */
  generate?: boolean;
}

/** Queues user input for the response identified by previous_response_id. */
model ResponseWebSocketSteerCommand extends ResponseWebSocketCommand {
  type: ResponseWebSocketCommandType.response_steer;
  previous_response_id: string;

  /** The target response determines the lane; steering does not accept stream_id. */
  input: ResponseWebSocketSteerMessage[];
}

/** A user message queued for a response continuation. */
model ResponseWebSocketSteerMessage {
  type?: "message";
  role: "user";
  /** Input text, image, or file content parts. */
  content: ItemContent[];
}

union ResponseWebSocketServerEventType {
  string,
  error: "error",
}

/** An event received over a Responses WebSocket connection. */
@discriminator("type")
@usage(Usage.output | Usage.json)
model ResponseWebSocketServerEvent {
  type: ResponseWebSocketServerEventType;
  stream_id?: string;
}

/** A protocol error. It does not by itself close the connection. */
model ResponseWebSocketErrorEvent extends ResponseWebSocketServerEvent {
  type: ResponseWebSocketServerEventType.error;
  error: ResponseWebSocketErrorDetails;
  status?: int32;
  sequence_number?: int32;
}

/** Details of a Responses WebSocket protocol error. */
model ResponseWebSocketErrorDetails {
  type: string;
  message: string;
  code?: string;
  param?: string;
  headers?: Record<string>;
  misalignment?: ResponseWebSocketMisalignment;
}

/** Optional explanation and continuation guidance accompanying a protocol error. */
model ResponseWebSocketMisalignment {
  detailed_explanation?: string;
  error_type?: string;
  steer?: ResponseWebSocketContinuation;
}

/** A public continuation instruction. */
model ResponseWebSocketContinuation {
  message: string;
}
