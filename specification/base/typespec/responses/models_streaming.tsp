import "@typespec/openapi";
import "@azure-tools/typespec-client-generator-core";
import "./models.tsp";
import "./models_items.tsp";

using Azure.ClientGenerator.Core;
using TypeSpec.OpenAPI;

namespace OpenAI;

union ResponseStreamEventType {
  string,
  response_audio_delta: "response.audio.delta",
  response_audio_done: "response.audio.done",
  response_audio_transcript_delta: "response.audio_transcript.delta",
  response_audio_transcript_done: "response.audio_transcript.done",
  response_code_interpreter_call_code_delta: "response.code_interpreter_call_code.delta",
  response_code_interpreter_call_code_done: "response.code_interpreter_call_code.done",
  response_code_interpreter_call_completed: "response.code_interpreter_call.completed",
  response_code_interpreter_call_in_progress: "response.code_interpreter_call.in_progress",
  response_code_interpreter_call_interpreting: "response.code_interpreter_call.interpreting",
  response_completed: "response.completed",
  response_content_part_added: "response.content_part.added",
  response_content_part_done: "response.content_part.done",
  response_created: "response.created",
  error: "error",
  response_file_search_call_completed: "response.file_search_call.completed",
  response_file_search_call_in_progress: "response.file_search_call.in_progress",
  response_file_search_call_searching: "response.file_search_call.searching",
  response_function_call_arguments_delta: "response.function_call_arguments.delta",
  response_function_call_arguments_done: "response.function_call_arguments.done",
  response_in_progress: "response.in_progress",
  response_failed: "response.failed",
  response_incomplete: "response.incomplete",
  response_output_item_added: "response.output_item.added",
  response_output_item_done: "response.output_item.done",
  response_reasoning_summary_part_added: "response.reasoning_summary_part.added",
  response_reasoning_summary_part_done: "response.reasoning_summary_part.done",
  response_reasoning_summary_text_delta: "response.reasoning_summary_text.delta",
  response_reasoning_summary_text_done: "response.reasoning_summary_text.done",
  response_reasoning_text_delta: "response.reasoning_text.delta",
  response_reasoning_text_done: "response.reasoning_text.done",
  response_refusal_delta: "response.refusal.delta",
  response_refusal_done: "response.refusal.done",
  response_output_text_delta: "response.output_text.delta",
  response_output_text_done: "response.output_text.done",
  response_web_search_call_completed: "response.web_search_call.completed",
  response_web_search_call_in_progress: "response.web_search_call.in_progress",
  response_web_search_call_searching: "response.web_search_call.searching",
  response_image_generation_call_completed: "response.image_generation_call.completed",
  response_image_generation_call_generating: "response.image_generation_call.generating",
  response_image_generation_call_in_progress: "response.image_generation_call.in_progress",
  response_image_generation_call_partial_image: "response.image_generation_call.partial_image",
  response_mcp_call_arguments_delta: "response.mcp_call_arguments.delta",
  response_mcp_call_arguments_done: "response.mcp_call_arguments.done",
  response_mcp_call_completed: "response.mcp_call.completed",
  response_mcp_call_failed: "response.mcp_call.failed",
  response_mcp_call_in_progress: "response.mcp_call.in_progress",
  response_mcp_list_tools_completed: "response.mcp_list_tools.completed",
  response_mcp_list_tools_failed: "response.mcp_list_tools.failed",
  response_mcp_list_tools_in_progress: "response.mcp_list_tools.in_progress",
  response_output_text_annotation_added: "response.output_text.annotation.added",
  response_queued: "response.queued",
  response_custom_tool_call_input_delta: "response.custom_tool_call_input.delta",
  response_custom_tool_call_input_done: "response.custom_tool_call_input.done",
}

@discriminator("type")
model ResponseStreamEvent {
  /** The type of the event. */
  type: ResponseStreamEventType;

  /** The sequence number of this event, used to order streaming events. */
  sequence_number: int32;
}

/** Emitted when there is a partial audio response. */
model ResponseAudioDeltaEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.audio.delta`. */
  type: ResponseStreamEventType.response_audio_delta;

  /** A chunk of Base64 encoded response audio bytes. */
  delta: string;
}

/** Emitted when the audio response is complete. */
model ResponseAudioDoneEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.audio.done`. */
  type: ResponseStreamEventType.response_audio_done;
}

/** Emitted when there is a partial transcript of audio. */
model ResponseAudioTranscriptDeltaEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.audio.transcript.delta`. */
  type: ResponseStreamEventType.response_audio_transcript_delta;

  /** The partial transcript of the audio response. */
  delta: string;
}

/** Emitted when the full audio transcript is completed. */
model ResponseAudioTranscriptDoneEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.audio.transcript.done`. */
  type: ResponseStreamEventType.response_audio_transcript_done;
}

/** Emitted when a partial code snippet is streamed by the code interpreter. */
model ResponseCodeInterpreterCallCodeDeltaEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.code_interpreter_call_code.delta`. */
  type: ResponseStreamEventType.response_code_interpreter_call_code_delta;

  /** The index of the output item in the response for which the code is being streamed. */
  output_index: int32;

  /** The unique identifier of the code interpreter tool call item. */
  item_id: string;

  /** The partial code snippet being streamed by the code interpreter. */
  delta: string;
}

/** Emitted when the code snippet is finalized by the code interpreter. */
model ResponseCodeInterpreterCallCodeDoneEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.code_interpreter_call_code.done`. */
  type: ResponseStreamEventType.response_code_interpreter_call_code_done;

  /** The index of the output item in the response for which the code is finalized. */
  output_index: int32;

  /** The unique identifier of the code interpreter tool call item. */
  item_id: string;

  /** The final code snippet output by the code interpreter. */
  code: string;
}

/** Emitted when the code interpreter call is completed. */
model ResponseCodeInterpreterCallCompletedEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.code_interpreter_call.completed`. */
  type: ResponseStreamEventType.response_code_interpreter_call_completed;

  /** The index of the output item in the response for which the code interpreter call is completed. */
  output_index: int32;

  /** The unique identifier of the code interpreter tool call item. */
  item_id: string;
}

/** Emitted when a code interpreter call is in progress. */
model ResponseCodeInterpreterCallInProgressEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.code_interpreter_call.in_progress`. */
  type: ResponseStreamEventType.response_code_interpreter_call_in_progress;

  /** The index of the output item in the response for which the code interpreter call is in progress. */
  output_index: int32;

  /** The unique identifier of the code interpreter tool call item. */
  item_id: string;
}

/** Emitted when the code interpreter is actively interpreting the code snippet. */
model ResponseCodeInterpreterCallInterpretingEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.code_interpreter_call.interpreting`. */
  type: ResponseStreamEventType.response_code_interpreter_call_interpreting;

  /** The index of the output item in the response for which the code interpreter is interpreting code. */
  output_index: int32;

  /** The unique identifier of the code interpreter tool call item. */
  item_id: string;
}

/** Emitted when the model response is complete. */
model ResponseCompletedEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.completed`. */
  type: ResponseStreamEventType.response_completed;

   /** Properties of the completed response. */
  response: Response;
}

/** Emitted when a new content part is added. */
model ResponseContentPartAddedEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.content_part.added`. */
  type: ResponseStreamEventType.response_content_part_added;

  /** The ID of the output item that the content part was added to. */
  item_id: string;

  /** The index of the output item that the content part was added to. */
  output_index: int32;

  /** The index of the content part that was added. */
  content_index: int32;

  /** The content part that was added. */
  part: OutputContent;
}

/** Emitted when a content part is done. */
model ResponseContentPartDoneEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.content_part.done`. */
  type: ResponseStreamEventType.response_content_part_done;

  /** The ID of the output item that the content part was added to. */
  item_id: string;

  /** The index of the output item that the content part was added to. */
  output_index: int32;

  /** The index of the content part that is done. */
  content_index: int32;

  /** The content part that is done. */
  part: OutputContent;
}

/** Emitted when a response is created. */
model ResponseCreatedEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.created`. */
  type: ResponseStreamEventType.response_created;

  /** The response that was created. */
  response: Response;
}

/** Emitted when an error occurs. */
model ResponseErrorEvent extends ResponseStreamEvent {
  /** The type of the event. Always `error`. */
  type: ResponseStreamEventType.error;

  /** The error code. */
  code: string | null;

  /** The error message. */
  message: string;

  /** The error parameter. */
  param: string | null;
}

/** Emitted when a file search call is completed (results found). */
model ResponseFileSearchCallCompletedEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.file_search_call.completed`. */
  type: ResponseStreamEventType.response_file_search_call_completed;

  /** The index of the output item that the file search call is initiated. */
  output_index: int32;

  /** The ID of the output item that the file search call is initiated. */
  item_id: string;
}

/** Emitted when a file search call is initiated. */
model ResponseFileSearchCallInProgressEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.file_search_call.in_progress`. */
  type: ResponseStreamEventType.response_file_search_call_in_progress;

  /** The index of the output item that the file search call is initiated. */
  output_index: int32;

  /** The ID of the output item that the file search call is initiated. */
  item_id: string;
}

/** Emitted when a file search is currently searching. */
model ResponseFileSearchCallSearchingEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.file_search_call.searching`. */
  type: ResponseStreamEventType.response_file_search_call_searching;

  /** The index of the output item that the file search call is searching. */
  output_index: int32;

  /** The ID of the output item that the file search call is initiated. */
  item_id: string;
}

/** Emitted when there is a partial function call arguments delta. */
model ResponseFunctionCallArgumentsDeltaEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.function_call_arguments.delta`. */
  type: ResponseStreamEventType.response_function_call_arguments_delta;

  /** The ID of the output item that the function call arguments delta is added to. */
  item_id: string;

  /** The index of the output item that the function call arguments delta is added to. */
  output_index: int32;

  /** The function call arguments delta that is added. */
  delta: string;
}

/** Emitted when function call arguments are finalized. */
model ResponseFunctionCallArgumentsDoneEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.function_call_arguments.done`. */
  type: ResponseStreamEventType.response_function_call_arguments_done;

  /** The ID of the item. */
  item_id: string;

  /** The name of the function that was called. */
  name: string;

  /** The index of the output item. */
  output_index: int32;

   /** The function call arguments. */
  arguments: string;
}

/** Emitted when the response is in progress. */
model ResponseInProgressEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.in_progress`. */
  type: ResponseStreamEventType.response_in_progress;

  /** The response that is in progress. */
  response: Response;
}

/** Emitted when a response fails. */
model ResponseFailedEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.failed`. */
  type: ResponseStreamEventType.response_failed;

  /** The response that failed. */
  response: Response;
}

/** Emitted when a response finishes as incomplete. */
model ResponseIncompleteEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.incomplete`. */
  type: ResponseStreamEventType.response_incomplete;

  /** The response that was incomplete. */
  response: Response;
}

/** Emitted when a new output item is added. */
model ResponseOutputItemAddedEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.output_item.added`. */
  type: ResponseStreamEventType.response_output_item_added;

  /** The index of the output item that was added. */
  output_index: int32;

  /** The output item that was added. */
  item: OutputItem;
}

/** Emitted when an output item is marked done. */
model ResponseOutputItemDoneEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.output_item.done`. */
  type: ResponseStreamEventType.response_output_item_done;

  /** The index of the output item that was marked done. */
  output_index: int32;

  /** The output item that was marked done. */
  item: OutputItem;
}

/** Emitted when a new reasoning summary part is added. */
model ResponseReasoningSummaryPartAddedEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.reasoning_summary_part.added`. */
  type: ResponseStreamEventType.response_reasoning_summary_part_added;

  /** The ID of the item this summary part is associated with. */
  item_id: string;

  /** The index of the output item this summary part is associated with. */
  output_index: int32;

  /** The index of the summary part within the reasoning summary. */
  summary_index: int32;

  /** The summary part that was added. */
  part: ReasoningItemSummaryPart;
}

/** Emitted when a reasoning summary part is completed. */
model ResponseReasoningSummaryPartDoneEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.reasoning_summary_part.done`. */
  type: ResponseStreamEventType.response_reasoning_summary_part_done;

  /** The ID of the item this summary part is associated with. */
  item_id: string;

  /** The index of the output item this summary part is associated with. */
  output_index: int32;

  /** The index of the summary part within the reasoning summary. */
  summary_index: int32;

  /** The completed summary part. */
  part: ReasoningItemSummaryPart;
}

/** Emitted when a delta is added to a reasoning summary text. */
model ResponseReasoningSummaryTextDeltaEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.reasoning_summary_text.delta`. */
  type: ResponseStreamEventType.response_reasoning_summary_text_delta;

  /** The ID of the item this summary text delta is associated with. */
  item_id: string;

  /** The index of the output item this summary text delta is associated with. */
  output_index: int32;

  /** The index of the summary part within the reasoning summary. */
  summary_index: int32;

  /** The text delta that was added to the summary. */
  delta: string;
}

/** Emitted when a reasoning summary text is completed. */
model ResponseReasoningSummaryTextDoneEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.reasoning_summary_text.done`. */
  type: ResponseStreamEventType.response_reasoning_summary_text_done;

  /** The ID of the item this summary text is associated with. */
  item_id: string;

  /** The index of the output item this summary text is associated with. */
  output_index: int32;

  /** The index of the summary part within the reasoning summary. */
  summary_index: int32;

  /** The full text of the completed reasoning summary. */
  text: string;
}

/** Emitted when a delta is added to a reasoning text. */
model ResponseReasoningTextDeltaEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.reasoning_text.delta`. */
  type: ResponseStreamEventType.response_reasoning_text_delta;

  /** The ID of the item this reasoning text delta is associated with. */
  item_id: string;

  /** The index of the output item this reasoning text delta is associated with. */
  output_index: int32;

  /** The index of the reasoning content part this delta is associated with. */
  content_index: int32;

  /** The text delta that was added to the reasoning content. */
  delta: string;
}

/** Emitted when a reasoning text is completed. */
model ResponseReasoningTextDoneEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.reasoning_text.done`. */
  type: ResponseStreamEventType.response_reasoning_text_done;

  /** The ID of the item this reasoning text is associated with. */
  item_id: string;

  /** The index of the output item this reasoning text is associated with. */
  output_index: int32;

  /** The index of the reasoning content part. */
  content_index: int32;

  /** The full text of the completed reasoning content. */
  text: string;
}

/** Emitted when there is a partial refusal text. */
model ResponseRefusalDeltaEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.refusal.delta`. */
  type: ResponseStreamEventType.response_refusal_delta;

  /** The ID of the output item that the refusal text is added to. */
  item_id: string;

  /** The index of the output item that the refusal text is added to. */
  output_index: int32;

  /** The index of the content part that the refusal text is added to. */
  content_index: int32;

  /** The refusal text that is added. */
  delta: string;
}

/** Emitted when refusal text is finalized. */
model ResponseRefusalDoneEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.refusal.done`. */
  type: ResponseStreamEventType.response_refusal_done;

  /** The ID of the output item that the refusal text is finalized. */
  item_id: string;

  /** The index of the output item that the refusal text is finalized. */
  output_index: int32;

  /** The index of the content part that the refusal text is finalized. */
  content_index: int32;

  /** The refusal text that is finalized. */
  refusal: string;
}

/** Emitted when there is an additional text delta. */
model ResponseOutputTextDeltaEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.output_text.delta`. */
  type: ResponseStreamEventType.response_output_text_delta;

  /** The ID of the output item that the text delta was added to. */
  item_id: string;

  /** The index of the output item that the text delta was added to. */
  output_index: int32;

  /** The index of the content part that the text delta was added to. */
  content_index: int32;

  /** The text delta that was added. */
  delta: string;

  /** The log probabilities of the tokens in the delta. */
  logprobs: LogProb[];
}

/** Emitted when text content is finalized. */
model ResponseOutputTextDoneEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.output_text.done`. */
  type: ResponseStreamEventType.response_output_text_done;

  /** The ID of the output item that the text content is finalized. */
  item_id: string;

  /** The index of the output item that the text content is finalized. */
  output_index: int32;

  /** The index of the content part that the text content is finalized. */
  content_index: int32;

  /** The text content that is finalized. */
  text: string;

  /** The log probabilities of the tokens in the delta. */
  logprobs: LogProb[];
}

/** Emitted when a web search call is completed. */
model ResponseWebSearchCallCompletedEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.web_search_call.completed`. */
  type: ResponseStreamEventType.response_web_search_call_completed;

  /** The index of the output item that the web search call is associated with. */
  output_index: int32;

  /** Unique ID for the output item associated with the web search call. */
  item_id: string;
}

/** Emitted when a web search call is initiated. */
model ResponseWebSearchCallInProgressEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.web_search_call.in_progress`. */
  type: ResponseStreamEventType.response_web_search_call_in_progress;

  /** The index of the output item that the web search call is associated with. */
  output_index: int32;

  /** Unique ID for the output item associated with the web search call. */
  item_id: string;
}

/** Emitted when a web search call is executing. */
model ResponseWebSearchCallSearchingEvent extends ResponseStreamEvent {
  /** The type of the event. Always `response.web_search_call.searching`. */
  type: ResponseStreamEventType.response_web_search_call_searching;

  /** The index of the output item that the web search call is associated with. */
  output_index: int32;

  /** Unique ID for the output item associated with the web search call. */
  item_id: string;
}

/** Emitted when an image generation tool call has completed and the final image is available. */
model ResponseImageGenerationCallCompletedEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.image_generation_call.completed'. */
  type: ResponseStreamEventType.response_image_generation_call_completed;

  /** The index of the output item in the response's output array. */
  output_index: int32;

  /** The unique identifier of the image generation item being processed. */
  item_id: string;
}

/** Emitted when an image generation tool call is actively generating an image (intermediate state). */
model ResponseImageGenerationCallGeneratingEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.image_generation_call.generating'. */
  type: ResponseStreamEventType.response_image_generation_call_generating;

  /** The index of the output item in the response's output array. */
  output_index: int32;

  /** The unique identifier of the image generation item being processed. */
  item_id: string;
}

/** Emitted when an image generation tool call is in progress. */
model ResponseImageGenerationCallInProgressEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.image_generation_call.in_progress'. */
  type: ResponseStreamEventType.response_image_generation_call_in_progress;

  /** The index of the output item in the response's output array. */
  output_index: int32;

  /** The unique identifier of the image generation item being processed. */
  item_id: string;
}

/** Emitted when a partial image is available during image generation streaming. */
model ResponseImageGenerationCallPartialImageEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.image_generation_call.partial_image'. */
  type: ResponseStreamEventType.response_image_generation_call_partial_image;

  /** The index of the output item in the response's output array. */
  output_index: int32;

  /** The unique identifier of the image generation item being processed. */
  item_id: string;

  /** 0-based index for the partial image (backend is 1-based, but this is 0-based for the user). */
  partial_image_index: int32;

  /** Base64-encoded partial image data, suitable for rendering as an image. */
  @encode("base64", string)
  partial_image_b64: bytes;
}

/** Emitted when there is a delta (partial update) to the arguments of an MCP tool call. */
model ResponseMcpCallArgumentsDeltaEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.mcp_call_arguments.delta'. */
  type: ResponseStreamEventType.response_mcp_call_arguments_delta;

  /** The index of the output item in the response's output array. */
  output_index: int32;

  /** The unique identifier of the MCP tool call item being processed. */
  item_id: string;

  /** A JSON string containing the partial update to the arguments for the MCP tool call. */
  delta: string;
}

/** Emitted when the arguments for an MCP tool call are finalized. */
model ResponseMcpCallArgumentsDoneEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.mcp_call_arguments.done'. */
  type: ResponseStreamEventType.response_mcp_call_arguments_done;

  /** The index of the output item in the response's output array. */
  output_index: int32;

  /** The unique identifier of the MCP tool call item being processed. */
  item_id: string;

  /** A JSON string containing the finalized arguments for the MCP tool call. */
  arguments: string;
}

/** Emitted when an MCP tool call has completed successfully. */
model ResponseMcpCallCompletedEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.mcp_call.completed'. */
  type: ResponseStreamEventType.response_mcp_call_completed;

  /** The ID of the MCP tool call item that completed. */
  item_id: string;

  /** The index of the output item that completed. */
  output_index: int32;
}

/** Emitted when an MCP tool call has failed. */
model ResponseMcpCallFailedEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.mcp_call.failed'. */
  type: ResponseStreamEventType.response_mcp_call_failed;

  /** The ID of the MCP tool call item that failed. */
  item_id: string;

  /** The index of the output item that failed. */
  output_index: int32;
}

/** Emitted when an MCP tool call is in progress. */
model ResponseMcpCallInProgressEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.mcp_call.in_progress'. */
  type: ResponseStreamEventType.response_mcp_call_in_progress;

  /** The index of the output item in the response's output array. */
  output_index: int32;

  /** The unique identifier of the MCP tool call item being processed. */
  item_id: string;
}

/** Emitted when the list of available MCP tools has been successfully retrieved.*/
model ResponseMcpListToolsCompletedEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.mcp_list_tools.completed'. */
  type: ResponseStreamEventType.response_mcp_list_tools_completed;

  /** The ID of the MCP tool call item that produced this output. */
  item_id: string;

  /** The index of the output item that was processed. */
  output_index: int32;
}

/** Emitted when the attempt to list available MCP tools has failed. */
model ResponseMcpListToolsFailedEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.mcp_list_tools.failed'. */
  type: ResponseStreamEventType.response_mcp_list_tools_failed;

  /** The ID of the MCP tool call item that failed. */
  item_id: string;

  /** The index of the output item that failed. */
  output_index: int32;
}

/** Emitted when the system is in the process of retrieving the list of available MCP tools. */
model ResponseMcpListToolsInProgressEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.mcp_list_tools.in_progress'. */
  type: ResponseStreamEventType.response_mcp_list_tools_in_progress;

  /** The ID of the MCP tool call item that is being processed. */
  item_id: string;

  /** The index of the output item that is being processed. */
  output_index: int32;
}

/** Emitted when an annotation is added to output text content. */
model ResponseOutputTextAnnotationAddedEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.output_text.annotation.added'. */
  type: ResponseStreamEventType.response_output_text_annotation_added;

  /** The unique identifier of the item to which the annotation is being added. */
  item_id: string;

  /** The index of the output item in the response's output array. */
  output_index: int32;

  /** The index of the content part within the output item. */
  content_index: int32;

  /** The index of the annotation within the content part. */
  annotation_index: int32;

  /** The annotation object being added. (See annotation schema for details.) */
  annotation: Annotation;
}

/** Emitted when a response is queued and waiting to be processed. */
model ResponseQueuedEvent extends ResponseStreamEvent {
  /** The type of the event. Always 'response.queued'. */
  type: ResponseStreamEventType.response_queued;

  /** The full response object that is queued. */
  response: Response;
}

/** Emitted when there is a delta (partial update) to the input of a custom tool call. */
model ResponseCustomToolCallInputDeltaEvent extends ResponseStreamEvent {
  /** The event type identifier. */
  type: ResponseStreamEventType.response_custom_tool_call_input_delta;

  /** The index of the output this delta applies to. */
  output_index: integer;

  /** Unique identifier for the API item associated with this event. */
  item_id: string;

  /** The incremental input data (delta) for the custom tool call. */
  delta: string;
}

/** Emitted when the input for a custom tool call is complete. */
model ResponseCustomToolCallInputDoneEvent extends ResponseStreamEvent {
  /** The event type identifier. */
  type: ResponseStreamEventType.response_custom_tool_call_input_done;

  /** The index of the output this event applies to. */
  output_index: integer;

  /** Unique identifier for the API item associated with this event. */
  item_id: string;

  /** The complete input data for the custom tool call. */
  input: string;
}