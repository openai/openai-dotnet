/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "../common";
import "./custom.tsp";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@doc("""
  Specifies the format that the model must output. Compatible with [GPT-4o](/docs/models#gpt-4o), [GPT-4 Turbo](/docs/models#gpt-4-turbo-and-gpt-4), and all GPT-3.5 Turbo models since `gpt-3.5-turbo-1106`.
  
  Setting to `{ "type": "json_schema", "json_schema": {...} }` enables Structured Outputs which ensures the model will match your supplied JSON schema. Learn more in the [Structured Outputs guide](/docs/guides/structured-outputs).
  
  Setting to `{ "type": "json_object" }` enables JSON mode, which ensures the message the model generates is valid JSON.
  
  **Important:** when using JSON mode, you **must** also instruct the model to produce JSON yourself via a system or user message. Without this, the model may generate an unending stream of whitespace until the generation reaches the token limit, resulting in a long-running and seemingly "stuck" request. Also note that the message content may be partially cut off if `finish_reason="length"`, which indicates the generation exceeded `max_tokens` or the conversation exceeded the max context length.
  """)
union AssistantsApiResponseFormatOption {
  "auto",
  ResponseFormatText,
  ResponseFormatJsonObject,
  ResponseFormatJsonSchema,
}

model CreateAssistantRequest {
  /** ID of the model to use. You can use the [List models](/docs/api-reference/models/list) API to see all of your available models, or see our [Model overview](/docs/models) for descriptions of them. */
  @extension("x-oaiTypeLabel", "string")
  `model`: AssistantSupportedModels;

  /** The name of the assistant. The maximum length is 256 characters. */
  @maxLength(256)
  name?: string | null;

  /** The description of the assistant. The maximum length is 512 characters. */
  @maxLength(512)
  description?: string | null;

  /** The system instructions that the assistant uses. The maximum length is 256,000 characters. */
  @maxLength(256000)
  instructions?: string | null;

  reasoning_effort?: ReasoningEffort | null = "medium";

  // Tool customization: use common model base for tool definitions
  @doc("""
    A list of tool enabled on the assistant. There can be a maximum of 128 tools per assistant. Tools can be of types `code_interpreter`, `file_search`, or `function`.
    """)
  @maxItems(128)
  tools?: AssistantToolDefinition[] = #[];

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
    file_search?: ToolResourcesFileSearch;
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

  response_format?: AssistantsApiResponseFormatOption | null;
}

model ModifyAssistantRequest {
  /** ID of the model to use. You can use the [List models](/docs/api-reference/models/list) API to see all of your available models, or see our [Model overview](/docs/models) for descriptions of them. */
  `model`?: AssistantSupportedModels;

  reasoning_effort?: ReasoningEffort | null = "medium";

  /** The name of the assistant. The maximum length is 256 characters. */
  @maxLength(256)
  name?: string | null;

  /** The description of the assistant. The maximum length is 512 characters. */
  @maxLength(512)
  description?: string | null;

  /** The system instructions that the assistant uses. The maximum length is 256,000 characters. */
  @maxLength(256000)
  instructions?: string | null;

  // Tool customization: use common model base for tool definitions
  @doc("""
    A list of tool enabled on the assistant. There can be a maximum of 128 tools per assistant. Tools can be of types `code_interpreter`, `file_search`, or `function`.
    """)
  @maxItems(128)
  tools?: AssistantToolDefinition[] = #[];

  @doc("""
    A set of resources that are used by the assistant's tools. The resources are specific to the type of tool. For example, the `code_interpreter` tool requires a list of file IDs, while the `file_search` tool requires a list of vector store IDs.
    """)
  tool_resources?: {
    code_interpreter?: {
      @doc("""
        Overrides the list of [file](/docs/api-reference/files) IDs made available to the `code_interpreter` tool. There can be a maximum of 20 files associated with the tool.
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

  response_format?: AssistantsApiResponseFormatOption | null;
}

model ListAssistantsResponse {
  // Tool customization: add a clear enum enforcement of constrained 'object' label
  object: "list";

  @pageItems data: AssistantObject[];
  first_id: string;
  @continuationToken last_id: string;
  has_more: boolean;
}

model DeleteAssistantResponse {
  id: string;
  deleted: boolean;
  object: "assistant.deleted";
}

// Tool customization (apply_discriminator): apply a common model base for tool definitions
model AssistantToolsCode extends AssistantToolDefinition {
  @doc("""
    The type of tool being defined: `code_interpreter`
    """)
  type: AssistantToolDefinitionType.code_interpreter;
}

// Tool customization (apply_discriminator): apply a common model base for tool definitions
model AssistantToolsFileSearch extends AssistantToolDefinition {
  @doc("""
    The type of tool being defined: `file_search`
    """)
  type: AssistantToolDefinitionType.file_search;

  /** Overrides for the file search tool. */
  file_search?: {
    @doc("""
      The maximum number of results the file search tool should output. The default is 20 for `gpt-4*` models and 5 for `gpt-3.5-turbo`. This number should be between 1 and 50 inclusive.
      
      Note that the file search tool may output fewer than `max_num_results` results. See the [file search tool documentation](/docs/assistants/tools/file-search#customizing-file-search-settings) for more information.
      """)
    @minValue(1)
    @maxValue(50)
    max_num_results?: int32;

    ranking_options?: FileSearchRankingOptions;
  };
}

@doc("""
  The ranking options for the file search. If not specified, the file search tool will use the `auto` ranker and a score_threshold of 0.
  
  See the [file search tool documentation](/docs/assistants/tools/file-search#customizing-file-search-settings) for more information.
  """)
model FileSearchRankingOptions {
  ranker?: FileSearchRanker;

  /** The score threshold for the file search. All values must be a floating point number between 0 and 1. */
  @minValue(0)
  @maxValue(1)
  score_threshold: float32;
}

model AssistantToolsFileSearchTypeOnly {
  @doc("""
    The type of tool being defined: `file_search`
    """)
  type: "file_search";
}

// Tool customization (apply_discriminator): apply a common model base for tool definitions
model AssistantToolsFunction extends AssistantToolDefinition {
  @doc("""
    The type of tool being defined: `function`
    """)
  type: AssistantToolDefinitionType.function;

  function: FunctionObject;
}

@doc("""
  Represents an `assistant` that can call the model and use tools.
  """)
model AssistantObject {
  /** The identifier, which can be referenced in API endpoints. */
  id: string;

  @doc("""
    The object type, which is always `assistant`.
    """)
  object: "assistant";

  // Tool customization: 'created' and fields ending in '_at' are Unix encoded utcDateTime
  /** The Unix timestamp (in seconds) for when the assistant was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  /** The name of the assistant. The maximum length is 256 characters. */
  @maxLength(256)
  name: string | null;

  /** The description of the assistant. The maximum length is 512 characters. */
  @maxLength(512)
  description: string | null;

  /** ID of the model to use. You can use the [List models](/docs/api-reference/models/list) API to see all of your available models, or see our [Model overview](/docs/models) for descriptions of them. */
  `model`: string;

  /** The system instructions that the assistant uses. The maximum length is 256,000 characters. */
  @maxLength(256000)
  instructions: string | null;

  // Tool customization: use common model base for tool definitions
  @doc("""
    A list of tool enabled on the assistant. There can be a maximum of 128 tools per assistant. Tools can be of types `code_interpreter`, `file_search`, or `function`.
    """)
  @maxItems(128)
  tools: AssistantToolDefinition[] = #[];

  @doc("""
    A set of resources that are used by the assistant's tools. The resources are specific to the type of tool. For example, the `code_interpreter` tool requires a list of file IDs, while the `file_search` tool requires a list of vector store IDs.
    """)
  tool_resources?: {
    code_interpreter?: {
      @doc("""
        A list of [file](/docs/api-reference/files) IDs made available to the `code_interpreter`` tool. There can be a maximum of 20 files associated with the tool.
        """)
      @maxItems(20)
      file_ids?: string[] = #[];
    };

    // Tool customization: use custom type for sophisticated union
    file_search?: ToolResourcesFileSearchIdsOnly;
  } | null;

  ...MetadataPropertyForResponse;

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

  response_format?: AssistantsApiResponseFormatOption | null;
}

/** Controls for how a thread will be truncated prior to the run. Use this to control the intial context window of the run. */
model TruncationObject {
  @doc("""
    The truncation strategy to use for the thread. The default is `auto`. If set to `last_messages`, the thread will be truncated to the n most recent messages in the thread. When set to `auto`, messages in the middle of the thread will be dropped to fit the context length of the model, `max_prompt_tokens`.
    """)
  type: "auto" | "last_messages";

  /** The number of most recent messages from the thread when constructing the context for the run. */
  @minValue(1)
  last_messages?: int32 | null;
}

@doc("""
  Controls which (if any) tool is called by the model.
  `none` means the model will not call any tools and instead generates a message.
  `auto` is the default value and means the model can pick between generating a message or calling one or more tools.
  `required` means the model must call one or more tools before responding to the user.
  Specifying a particular tool like `{"type": "file_search"}` or `{"type": "function", "function": {"name": "my_function"}}` forces the model to call that tool.
  """)
union AssistantsApiToolChoiceOption {
  "none" | "auto" | "required",
  AssistantsNamedToolChoice,
}

/** Specifies a tool the model should use. Use to force the model to call a specific tool. */
model AssistantsNamedToolChoice {
  @doc("""
    The type of the tool. If type is `function`, the function name must be set
    """)
  type: "function" | "code_interpreter" | "file_search";

  function?: {
    /** The name of the function to call. */
    name: string;
  };
}

union AssistantSupportedModels {
  "gpt-4.1",
  "gpt-4.1-mini",
  "gpt-4.1-nano",
  "gpt-4.1-2025-04-14",
  "gpt-4.1-mini-2025-04-14",
  "gpt-4.1-nano-2025-04-14",
  "o3-mini",
  "o3-mini-2025-01-31",
  "o1",
  "o1-2024-12-17",
  "gpt-4o",
  "gpt-4o-2024-11-20",
  "gpt-4o-2024-08-06",
  "gpt-4o-2024-05-13",
  "gpt-4o-mini",
  "gpt-4o-mini-2024-07-18",
  "gpt-4.5-preview",
  "gpt-4.5-preview-2025-02-27",
  "gpt-4-turbo",
  "gpt-4-turbo-2024-04-09",
  "gpt-4-0125-preview",
  "gpt-4-turbo-preview",
  "gpt-4-1106-preview",
  "gpt-4-vision-preview",
  "gpt-4",
  "gpt-4-0314",
  "gpt-4-0613",
  "gpt-4-32k",
  "gpt-4-32k-0314",
  "gpt-4-32k-0613",
  "gpt-3.5-turbo",
  "gpt-3.5-turbo-16k",
  "gpt-3.5-turbo-0613",
  "gpt-3.5-turbo-1106",
  "gpt-3.5-turbo-0125",
  "gpt-3.5-turbo-16k-0613",
}
