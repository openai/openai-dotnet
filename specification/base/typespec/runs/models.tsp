/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "../assistants";
import "../common";
import "../messages";
import "../threads";
import "./custom.tsp";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

// Tool generated type. Extracts from SubmitToolOutputsRunRequest.tool_outputs
alias SubmitToolOutputsRunRequestToolOutput = {
  @doc("""
    The ID of the tool call in the `required_action` object within the run object the output is being submitted for.
    """)
  tool_call_id?: string;

  /** The output of the tool call to be submitted to continue the run. */
  output?: string;
};

// Tool generated type. Extracts from RunStepDetailsToolCallsFileSearchResultObject.content
alias RunStepDetailsToolCallsFileSearchResultObjectContent = {
  // Tool customization: Address source spec inaccuracy of optionality & use enum instead of string constants to maintain backwards compatibility
  /** The type of the content. */
  type: RunStepDetailsToolCallsFileSearchResultObjectContentType.text;

  // Tool customization: address source spec inaccuracy of optionality
  /** The text content of the file. */
  text: string;
};

model CreateRunRequest {
  /** The ID of the [assistant](/docs/api-reference/assistants) to use to execute this run. */
  assistant_id: string;

  /** The ID of the [Model](/docs/api-reference/models) to be used to execute this run. If a value is provided here, it will override the model associated with the assistant. If not, the model associated with the assistant will be used. */
  @extension("x-oaiTypeLabel", "string")
  `model`?: AssistantSupportedModels | null;

  reasoning_effort?: ReasoningEffort | null = "medium";

  /** Overrides the [instructions](/docs/api-reference/assistants/createAssistant) of the assistant. This is useful for modifying the behavior on a per-run basis. */
  instructions?: string | null;

  /** Appends additional instructions at the end of the instructions for the run. This is useful for modifying the behavior on a per-run basis without overriding other instructions. */
  additional_instructions?: string | null;

  /** Adds additional messages to the thread before creating the run. */
  additional_messages?: CreateMessageRequest[] | null;

  // Tool customization: use common model base for tool definitions
  /** Override the tools the assistant can use for this run. This is useful for modifying the behavior on a per-run basis. */
  // Tool note: can't automatically apply to nullable: @maxItems(20)
  tools?: AssistantToolDefinition[] | null;

  ...MetadataPropertyForRequest;

  /** What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. */
  @minValue(0)
  @maxValue(2)
  temperature?: float32 | null = 1;

  /**
   * An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.
   *
   * We generally recommend altering this or temperature but not both.
   */
  @minValue(0)
  @maxValue(1)
  top_p?: float32 | null = 1;

  @doc("""
    If `true`, returns a stream of events that happen during the Run as server-sent events, terminating when the Run enters a terminal state with a `data: [DONE]` message.
    """)
  stream?: boolean | null;

  @doc("""
    The maximum number of prompt tokens that may be used over the course of the run. The run will make a best effort to use only the number of prompt tokens specified, across multiple turns of the run. If the run exceeds the number of prompt tokens specified, the run will end with status `incomplete`. See `incomplete_details` for more info.
    """)
  @minValue(256)
  max_prompt_tokens?: int32 | null;

  @doc("""
    The maximum number of completion tokens that may be used over the course of the run. The run will make a best effort to use only the number of completion tokens specified, across multiple turns of the run. If the run exceeds the number of completion tokens specified, the run will end with status `incomplete`. See `incomplete_details` for more info.
    """)
  @minValue(256)
  max_completion_tokens?: int32 | null;

  truncation_strategy?: TruncationObject | null;
  tool_choice?: AssistantsApiToolChoiceOption | null;
  parallel_tool_calls?: ParallelToolCalls = true;
  response_format?: AssistantsApiResponseFormatOption | null;
}

model ModifyRunRequest {
  ...MetadataPropertyForRequest;
}

model CreateThreadAndRunRequest {
  /** The ID of the [assistant](/docs/api-reference/assistants) to use to execute this run. */
  assistant_id: string;

  thread?: CreateThreadRequest;

  /** The ID of the [Model](/docs/api-reference/models) to be used to execute this run. If a value is provided here, it will override the model associated with the assistant. If not, the model associated with the assistant will be used. */
  @extension("x-oaiTypeLabel", "string")
  `model`?:
    | string
    | "gpt-4.1"
    | "gpt-4.1-mini"
    | "gpt-4.1-nano"
    | "gpt-4.1-2025-04-14"
    | "gpt-4.1-mini-2025-04-14"
    | "gpt-4.1-nano-2025-04-14"
    | "gpt-4o"
    | "gpt-4o-2024-11-20"
    | "gpt-4o-2024-08-06"
    | "gpt-4o-2024-05-13"
    | "gpt-4o-mini"
    | "gpt-4o-mini-2024-07-18"
    | "gpt-4.5-preview"
    | "gpt-4.5-preview-2025-02-27"
    | "gpt-4-turbo"
    | "gpt-4-turbo-2024-04-09"
    | "gpt-4-0125-preview"
    | "gpt-4-turbo-preview"
    | "gpt-4-1106-preview"
    | "gpt-4-vision-preview"
    | "gpt-4"
    | "gpt-4-0314"
    | "gpt-4-0613"
    | "gpt-4-32k"
    | "gpt-4-32k-0314"
    | "gpt-4-32k-0613"
    | "gpt-3.5-turbo"
    | "gpt-3.5-turbo-16k"
    | "gpt-3.5-turbo-0613"
    | "gpt-3.5-turbo-1106"
    | "gpt-3.5-turbo-0125"
    | "gpt-3.5-turbo-16k-0613"
    | null;

  /** Override the default system message of the assistant. This is useful for modifying the behavior on a per-run basis. */
  instructions?: string | null;

  // Tool customization: use common model base for tool definitions
  /** Override the tools the assistant can use for this run. This is useful for modifying the behavior on a per-run basis. */
  // Tool note: can't automatically apply to nullable: @maxItems(20)
  tools?: AssistantToolDefinition[] | null;

  @doc("""
    A set of resources that are used by the assistant's tools. The resources are specific to the type of tool. For example, the `code_interpreter` tool requires a list of file IDs, while the `file_search` tool requires a list of vector store IDs.
    """)
  tool_resources?: {
    code_interpreter?: {
      @doc("""
        A list of [file](/docs/api-reference/files) IDs made available to the `code_interpreter` tool. There can be a maximum of 20 files associated with the tool.
        """)
      @maxItems(20)
      file_ids?: string[] = #[];
    };

    // Tool customization: use custom type for sophisticated union
    file_search?: ToolResourcesFileSearchIdsOnly;
  } | null;

  ...MetadataPropertyForRequest;

  /** What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. */
  @minValue(0)
  @maxValue(2)
  temperature?: float32 | null = 1;

  /**
   * An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.
   *
   * We generally recommend altering this or temperature but not both.
   */
  @minValue(0)
  @maxValue(1)
  top_p?: float32 | null = 1;

  @doc("""
    If `true`, returns a stream of events that happen during the Run as server-sent events, terminating when the Run enters a terminal state with a `data: [DONE]` message.
    """)
  stream?: boolean | null;

  @doc("""
    The maximum number of prompt tokens that may be used over the course of the run. The run will make a best effort to use only the number of prompt tokens specified, across multiple turns of the run. If the run exceeds the number of prompt tokens specified, the run will end with status `incomplete`. See `incomplete_details` for more info.
    """)
  @minValue(256)
  max_prompt_tokens?: int32 | null;

  @doc("""
    The maximum number of completion tokens that may be used over the course of the run. The run will make a best effort to use only the number of completion tokens specified, across multiple turns of the run. If the run exceeds the number of completion tokens specified, the run will end with status `incomplete`. See `incomplete_details` for more info.
    """)
  @minValue(256)
  max_completion_tokens?: int32 | null;

  truncation_strategy?: TruncationObject | null;
  tool_choice?: AssistantsApiToolChoiceOption | null;
  parallel_tool_calls?: ParallelToolCalls = true;
  response_format?: AssistantsApiResponseFormatOption | null;
}

model SubmitToolOutputsRunRequest {
  /** A list of tools for which the outputs are being submitted. */
  tool_outputs: SubmitToolOutputsRunRequestToolOutput[];

  @doc("""
    If `true`, returns a stream of events that happen during the Run as server-sent events, terminating when the Run enters a terminal state with a `data: [DONE]` message.
    """)
  stream?: boolean | null;
}

model ListRunsResponse {
  // Tool customization: add a clear enum enforcement of constrained 'object' label
  object: "list";

  @pageItems data: RunObject[];
  first_id: string;
  @continuationToken last_id: string;
  has_more: boolean;
}

model ListRunStepsResponse {
  // Tool customization: add a clear enum enforcement of constrained 'object' label
  object: "list";

  @pageItems data: RunStepObject[];
  first_id: string;
  @continuationToken last_id: string;
  has_more: boolean;
}

// Tool customization (apply_discriminator): apply custom, common base type to union items
/** Details of the message creation by the run step. */
model RunStepDetailsMessageCreationObject extends RunStepObjectStepDetails {
  @doc("""
    Always `message_creation`.
    """)
  type: RunStepDetailsType.message_creation;

  message_creation: {
    /** The ID of the message that was created by this run step. */
    message_id: string;
  };
}

// Tool customization (apply_discriminator): apply custom, common base type to union items
/** Details of the tool call. */
model RunStepDetailsToolCallsObject extends RunStepObjectStepDetails {
  @doc("""
    Always `tool_calls`.
    """)
  type: RunStepDetailsType.tool_calls;

  // Tool customization: replace unioned types with a custom, common base
  @doc("""
    An array of tool calls the run step was involved in. These can be associated with one of three types of tools: `code_interpreter`, `file_search`, or `function`.
    """)
  tool_calls: RunStepDetailsToolCallsObjectToolCallsObject[];
}

// Tool customization: Remove parent-relocated 'id' from discriminated type instance
/** Details of the Code Interpreter tool call the run step was involved in. */
model RunStepDetailsToolCallsCodeObject
  extends RunStepDetailsToolCallsObjectToolCallsObject {
  @doc("""
    The type of tool call. This is always going to be `code_interpreter` for this type of tool call.
    """)
  type: RunStepDetailsToolCallType.code_interpreter;

  /** The Code Interpreter tool call definition. */
  code_interpreter: {
    /** The input to the Code Interpreter tool call. */
    input: string;

    // Tool customization: Apply read-only visibility via annotation
    @doc("""
      The outputs from the Code Interpreter tool call. Code Interpreter can output one or more items, including text (`logs`) or images (`image`). Each of these are represented by a different object type.
      """)
    @visibility(Lifecycle.Read)
    outputs: RunStepDetailsToolCallsCodeObjectCodeInterpreterOutputsObject[];
  };
}

// Tool customization (apply_discriminator): apply custom, common base type to union items
/** Text output from the Code Interpreter tool call as part of a run step. */
model RunStepDetailsToolCallsCodeOutputLogsObject
  extends RunStepDetailsToolCallsCodeObjectCodeInterpreterOutputsObject {
  @doc("""
    Always `logs`.
    """)
  type: RunStepDetailsCodeInterpreterOutputType.logs;

  /** The text output from the Code Interpreter tool call. */
  logs: string;
}

// Tool customization (apply_discriminator): apply custom, common base type to union items
model RunStepDetailsToolCallsCodeOutputImageObject
  extends RunStepDetailsToolCallsCodeObjectCodeInterpreterOutputsObject {
  @doc("""
    Always `image`.
    """)
  type: RunStepDetailsCodeInterpreterOutputType.image;

  image: {
    /** The [file](/docs/api-reference/files) ID of the image. */
    file_id: string;
  };
}

// Tool customization: Remove parent-relocated 'id' from discriminated type instance
model RunStepDetailsToolCallsFileSearchObject
  extends RunStepDetailsToolCallsObjectToolCallsObject {
  @doc("""
    The type of tool call. This is always going to be `file_search` for this type of tool call.
    """)
  type: RunStepDetailsToolCallType.file_search;

  /** For now, this is always going to be an empty object. */
  @extension("x-oaiTypeLabel", "map")
  file_search: {
    ranking_options?: RunStepDetailsToolCallsFileSearchRankingOptionsObject;

    // Tool customization: Apply read-only visibility via annotation
    /** The results of the file search. */
    @visibility(Lifecycle.Read)
    results?: RunStepDetailsToolCallsFileSearchResultObject[];
  };
}

/** The ranking options for the file search. */
model RunStepDetailsToolCallsFileSearchRankingOptionsObject {
  ranker: FileSearchRanker;

  /** The score threshold for the file search. All values must be a floating point number between 0 and 1. */
  @minValue(0)
  @maxValue(1)
  score_threshold: float32;
}

/** A result instance of the file search. */
@extension("x-oaiTypeLabel", "map")
model RunStepDetailsToolCallsFileSearchResultObject {
  /** The ID of the file that result was found in. */
  file_id: string;

  /** The name of the file that result was found in. */
  file_name: string;

  /** The score of the result. All values must be a floating point number between 0 and 1. */
  @minValue(0)
  @maxValue(1)
  score: float32;

  /** The content of the result that was found. The content is only included if requested via the include query parameter. */
  content?: RunStepDetailsToolCallsFileSearchResultObjectContent[];
}

// Tool customization: Remove parent-relocated 'id' from discriminated type instance
model RunStepDetailsToolCallsFunctionObject
  extends RunStepDetailsToolCallsObjectToolCallsObject {
  @doc("""
    The type of tool call. This is always going to be `function` for this type of tool call.
    """)
  type: RunStepDetailsToolCallType.function;

  /** The definition of the function that was called. */
  function: {
    /** The name of the function. */
    name: string;

    /** The arguments passed to the function. */
    arguments: string;

    @doc("""
      The output of the function. This will be `null` if the outputs have not been [submitted](/docs/api-reference/runs/submitToolOutputs) yet.
      """)
    output: string | null;
  };
}

@doc("""
  Usage statistics related to the run. This value will be `null` if the run is not in a terminal state (i.e. `in_progress`, `queued`, etc.).
  """)
model RunCompletionUsage {
  /** Number of completion tokens used over the course of the run. */
  completion_tokens: int32;

  /** Number of prompt tokens used over the course of the run. */
  prompt_tokens: int32;

  /** Total number of tokens used (prompt + completion). */
  total_tokens: int32;
}

@doc("""
  Usage statistics related to the run step. This value will be `null` while the run step's status is `in_progress`.
  """)
model RunStepCompletionUsage {
  /** Number of completion tokens used over the course of the run step. */
  completion_tokens: int32;

  /** Number of prompt tokens used over the course of the run step. */
  prompt_tokens: int32;

  /** Total number of tokens used (prompt + completion). */
  total_tokens: int32;
}

/** Represents an execution run on a [thread](/docs/api-reference/threads). */
model RunObject {
  /** The identifier, which can be referenced in API endpoints. */
  id: string;

  @doc("""
    The object type, which is always `thread.run`.
    """)
  object: "thread.run";

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the run was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  /** The ID of the [thread](/docs/api-reference/threads) that was executed on as a part of this run. */
  thread_id: string;

  /** The ID of the [assistant](/docs/api-reference/assistants) used for execution of this run. */
  assistant_id: string;

  @doc("""
    The status of the run, which can be either `queued`, `in_progress`, `requires_action`, `cancelling`, `cancelled`, `failed`, `completed`, `incomplete`, or `expired`.
    """)
  status:
    | "queued"
    | "in_progress"
    | "requires_action"
    | "cancelling"
    | "cancelled"
    | "failed"
    | "completed"
    | "incomplete"
    | "expired";

  @doc("""
    Details on the action required to continue the run. Will be `null` if no action is required.
    """)
  required_action: {
    @doc("""
      For now, this is always `submit_tool_outputs`.
      """)
    type: "submit_tool_outputs";

    /** Details on the tool outputs needed for this run to continue. */
    submit_tool_outputs: {
      // Tool customization: Apply read-only visibility via annotation
      /** A list of the relevant tool calls. */
      @visibility(Lifecycle.Read)
      tool_calls: RunToolCallObject[];
    };
  } | null;

  @doc("""
    The last error associated with this run. Will be `null` if there are no errors.
    """)
  last_error: {
    @doc("""
      One of `server_error`, `rate_limit_exceeded`, or `invalid_prompt`.
      """)
    code: "server_error" | "rate_limit_exceeded" | "invalid_prompt";

    /** A human-readable description of the error. */
    message: string;
  } | null;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the run will expire. */
  @encode("unixTimestamp", int32)
  expires_at: utcDateTime | null;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the run was started. */
  @encode("unixTimestamp", int32)
  started_at: utcDateTime | null;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the run was cancelled. */
  @encode("unixTimestamp", int32)
  cancelled_at: utcDateTime | null;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the run failed. */
  @encode("unixTimestamp", int32)
  failed_at: utcDateTime | null;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the run was completed. */
  @encode("unixTimestamp", int32)
  completed_at: utcDateTime | null;

  @doc("""
    Details on why the run is incomplete. Will be `null` if the run is not incomplete.
    """)
  incomplete_details: {
    /** The reason why the run is incomplete. This will point to which specific token limit was reached over the course of the run. */
    reason?: "max_completion_tokens" | "max_prompt_tokens";
  } | null;

  /** The model that the [assistant](/docs/api-reference/assistants) used for this run. */
  `model`: string;

  /** The instructions that the [assistant](/docs/api-reference/assistants) used for this run. */
  instructions: string;

  // Tool customization: use common model base for tool definitions
  /** The list of tools that the [assistant](/docs/api-reference/assistants) used for this run. */
  @maxItems(20)
  tools: AssistantToolDefinition[] = #[];

  ...MetadataPropertyForResponse;
  usage: RunCompletionUsage | null;

  /** The sampling temperature used for this run. If not set, defaults to 1. */
  temperature?: float32 | null;

  /** The nucleus sampling value used for this run. If not set, defaults to 1. */
  top_p?: float32 | null;

  /** The maximum number of prompt tokens specified to have been used over the course of the run. */
  @minValue(256)
  max_prompt_tokens: int32 | null;

  /** The maximum number of completion tokens specified to have been used over the course of the run. */
  @minValue(256)
  max_completion_tokens: int32 | null;

  truncation_strategy: TruncationObject | null;
  tool_choice: AssistantsApiToolChoiceOption | null;
  parallel_tool_calls: ParallelToolCalls = true;
  response_format: AssistantsApiResponseFormatOption | null;
}

/** Represents a step in execution of a run. */
model RunStepObject {
  /** The identifier of the run step, which can be referenced in API endpoints. */
  id: string;

  @doc("""
    The object type, which is always `thread.run.step`.
    """)
  object: "thread.run.step";

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the run step was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  /** The ID of the [assistant](/docs/api-reference/assistants) associated with the run step. */
  assistant_id: string;

  /** The ID of the [thread](/docs/api-reference/threads) that was run. */
  thread_id: string;

  /** The ID of the [run](/docs/api-reference/runs) that this run step is a part of. */
  run_id: string;

  @doc("""
    The type of run step, which can be either `message_creation` or `tool_calls`.
    """)
  type: "message_creation" | "tool_calls";

  @doc("""
    The status of the run step, which can be either `in_progress`, `cancelled`, `failed`, `completed`, or `expired`.
    """)
  status: "in_progress" | "cancelled" | "failed" | "completed" | "expired";

  // Tool customization: replace unioned types with a custom, common base
  /** The details of the run step. */
  step_details: RunStepObjectStepDetails;

  @doc("""
    The last error associated with this run step. Will be `null` if there are no errors.
    """)
  last_error: {
    @doc("""
      One of `server_error` or `rate_limit_exceeded`.
      """)
    code: "server_error" | "rate_limit_exceeded";

    /** A human-readable description of the error. */
    message: string;
  } | null;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the run step expired. A step is considered expired if the parent run is expired. */
  @encode("unixTimestamp", int32)
  expired_at: utcDateTime | null;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the run step was cancelled. */
  @encode("unixTimestamp", int32)
  cancelled_at: utcDateTime | null;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the run step failed. */
  @encode("unixTimestamp", int32)
  failed_at: utcDateTime | null;

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the run step completed. */
  @encode("unixTimestamp", int32)
  completed_at: utcDateTime | null;

  ...MetadataPropertyForResponse;
  usage: RunStepCompletionUsage | null;
}

/** Tool call objects */
model RunToolCallObject {
  /** The ID of the tool call. This ID must be referenced when you submit the tool outputs in using the [Submit tool outputs to run](/docs/api-reference/runs/submitToolOutputs) endpoint. */
  id: string;

  @doc("""
    The type of tool call the output is required for. For now, this is always `function`.
    """)
  type: "function";

  /** The function definition. */
  function: {
    /** The name of the function. */
    name: string;

    /** The arguments that the model expects you to pass to the function. */
    arguments: string;
  };
}

/** Represents a run step delta i.e. any changed fields on a run step during streaming. */
model RunStepDeltaObject {
  /** The identifier of the run step, which can be referenced in API endpoints. */
  id: string;

  @doc("""
    The object type, which is always `thread.run.step.delta`.
    """)
  object: "thread.run.step.delta";

  /** The delta containing the fields that have changed on the run step. */
  delta: {
    // Tool customization: replace unioned types with a custom, common base
    /** The details of the run step. */
    step_details?: RunStepDeltaStepDetails;
  };
}

// Tool customization (apply_discriminator): apply custom, common base type to union items
/** Details of the message creation by the run step. */
model RunStepDeltaStepDetailsMessageCreationObject
  extends RunStepDeltaStepDetails {
  @doc("""
    Always `message_creation`.
    """)
  type: "message_creation";

  message_creation?: {
    /** The ID of the message that was created by this run step. */
    message_id?: string;
  };
}

// Tool customization (apply_discriminator): apply custom, common base type to union items
/** Details of the tool call. */
model RunStepDeltaStepDetailsToolCallsObject extends RunStepDeltaStepDetails {
  @doc("""
    Always `tool_calls`.
    """)
  type: "tool_calls";

  // Tool customization: replace unioned types with a custom, common base
  @doc("""
    An array of tool calls the run step was involved in. These can be associated with one of three types of tools: `code_interpreter`, `file_search`, or `function`.
    """)
  tool_calls?: RunStepDeltaStepDetailsToolCallsObjectToolCallsObject[];
}

// Tool customization (apply_discriminator): apply custom, common base type to union items
/** Details of the Code Interpreter tool call the run step was involved in. */
model RunStepDeltaStepDetailsToolCallsCodeObject
  extends RunStepDeltaStepDetailsToolCallsObjectToolCallsObject {
  /** The index of the tool call in the tool calls array. */
  index: int32;

  /** The ID of the tool call. */
  id?: string;

  @doc("""
    The type of tool call. This is always going to be `code_interpreter` for this type of tool call.
    """)
  type: RunStepDetailsToolCallType.code_interpreter;

  /** The Code Interpreter tool call definition. */
  code_interpreter?: {
    /** The input to the Code Interpreter tool call. */
    input?: string;

    // Tool customization: Apply read-only visibility via annotation
    @doc("""
      The outputs from the Code Interpreter tool call. Code Interpreter can output one or more items, including text (`logs`) or images (`image`). Each of these are represented by a different object type.
      """)
    @visibility(Lifecycle.Read)
    outputs?: RunStepDeltaStepDetailsToolCallsCodeObjectCodeInterpreterOutputsObject[];
  };
}

// Tool customization (apply_discriminator): apply custom, common base type to union items
/** Text output from the Code Interpreter tool call as part of a run step. */
model RunStepDeltaStepDetailsToolCallsCodeOutputLogsObject
  extends RunStepDeltaStepDetailsToolCallsCodeObjectCodeInterpreterOutputsObject {
  /** The index of the output in the outputs array. */
  index: int32;

  @doc("""
    Always `logs`.
    """)
  type: RunStepDetailsCodeInterpreterOutputType.logs;

  /** The text output from the Code Interpreter tool call. */
  logs?: string;
}

// Tool customization (apply_discriminator): apply custom, common base type to union items
model RunStepDeltaStepDetailsToolCallsCodeOutputImageObject
  extends RunStepDeltaStepDetailsToolCallsCodeObjectCodeInterpreterOutputsObject {
  /** The index of the output in the outputs array. */
  index: int32;

  @doc("""
    Always `image`.
    """)
  type: RunStepDetailsCodeInterpreterOutputType.image;

  image?: {
    /** The [file](/docs/api-reference/files) ID of the image. */
    file_id?: string;
  };
}

// Tool customization (apply_discriminator): apply custom, common base type to union items
model RunStepDeltaStepDetailsToolCallsFileSearchObject
  extends RunStepDeltaStepDetailsToolCallsObjectToolCallsObject {
  /** The index of the tool call in the tool calls array. */
  index: int32;

  /** The ID of the tool call object. */
  id?: string;

  @doc("""
    The type of tool call. This is always going to be `file_search` for this type of tool call.
    """)
  type: RunStepDetailsToolCallType.file_search;

  // Tool customization: add missing and shared file_search definition for streaming
  /** For now, this is always going to be an empty object. */
  @extension("x-oaiTypeLabel", "map")
  file_search: {
    ranking_options?: RunStepDetailsToolCallsFileSearchRankingOptionsObject;

    /** The results of the file search. */
    @visibility(Lifecycle.Read)
    results?: RunStepDetailsToolCallsFileSearchResultObject[];
  };
}

// Tool customization (apply_discriminator): apply custom, common base type to union items
model RunStepDeltaStepDetailsToolCallsFunctionObject
  extends RunStepDeltaStepDetailsToolCallsObjectToolCallsObject {
  /** The index of the tool call in the tool calls array. */
  index: int32;

  /** The ID of the tool call object. */
  id?: string;

  @doc("""
    The type of tool call. This is always going to be `function` for this type of tool call.
    """)
  type: RunStepDetailsToolCallType.function;

  /** The definition of the function that was called. */
  function?: {
    /** The name of the function. */
    name?: string;

    /** The arguments passed to the function. */
    arguments?: string;

    @doc("""
      The output of the function. This will be `null` if the outputs have not been [submitted](/docs/api-reference/runs/submitToolOutputs) yet.
      """)
    output?: string | null;
  };
}
