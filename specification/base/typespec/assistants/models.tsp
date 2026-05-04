import "../vector-stores/models.tsp";
/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "@typespec/http";
import "@azure-tools/typespec-client-generator-core";

using Http;
using TypeSpec.OpenAPI;
using Azure.ClientGenerator.Core;

namespace OpenAI;

model AssistantsError {
  code: string | null;
  message: string;
  param: string | null;
  type: string;
}

@error
model AssistantsErrorResponse {
  error: AssistantsError;
}

@doc("""
  **Reasoning models only**
  
  Constrains effort on reasoning for
  [reasoning models](https://platform.openai.com/docs/guides/reasoning).
  Currently supported values are `none`, `minimal`, `low`, `medium`, and `high`. Reducing
  reasoning effort can result in faster responses and fewer tokens used
  on reasoning in a response.
  """)
union AssistantsReasoningEffort {
  "none",
  "minimal",
  "low",
  "medium",
  "high",
}

@discriminator("type")
model AssistantsResponseFormat {
  type: "text" | "json_object" | "json_schema";
}


// Tool customization (apply_discriminator): establish a common, discriminated union
@doc("""
  JSON object response format. An older method of generating JSON responses.
  Using `json_schema` is recommended for models that support it. Note that the
  model will not generate JSON without a system or user message instructing it
  to do so.
  """)
model ResponseFormatJsonObject extends AssistantsResponseFormat {
  @doc("""
    The type of response format being defined. Always `json_object`.
    """)
  type: "json_object" ;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
@discriminator("type")
model DotNetAssistantResponseFormat {
  ...AssistantsResponseFormat;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetAssistantResponseFormatText extends DotNetAssistantResponseFormat {
  @doc("""
    The type of response format being defined. Always `text`.
    """)
  type: "text";
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetAssistantResponseFormatJsonSchema
  extends DotNetAssistantResponseFormat {
  type: "json_schema";
  json_schema: {
    description?: string;
    name: string;
    schema?: unknown;
    strict?: boolean | null = false;
  };
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetAssistantResponseFormatJsonObject
  extends DotNetAssistantResponseFormat {
  ...ResponseFormatJsonObject;
}

// Tool customization (apply_discriminator): establish a common, discriminated union
/**
 * JSON Schema response format. Used to generate structured JSON responses.
 * Learn more about [Structured Outputs](/docs/guides/structured-outputs).
 */
model ResponseFormatJsonSchema extends AssistantsResponseFormat {
  @doc("""
    The type of response format being defined. Always `json_schema`.
    """)
  type: "json_schema";

  /** Structured Outputs configuration options, including a JSON Schema. */
  json_schema: {
    /**
     * A description of what the response format is for, used by the model to
     * determine how to respond in the format.
     */
    description?: string;

    /**
     * The name of the response format. Must be a-z, A-Z, 0-9, or contain
     * underscores and dashes, with a maximum length of 64.
     */
    name: string;

    schema?: Record<unknown>;

    @doc("""
      Whether to enable strict schema adherence when generating the output.
      If set to true, the model will always follow the exact schema defined
      in the `schema` field. Only a subset of JSON Schema is supported when
      `strict` is `true`. To learn more, read the [Structured Outputs
      guide](/docs/guides/structured-outputs).
      """)
    strict?: boolean | null = false;
  };
}

@doc("""
  Specifies the format that the model must output. Compatible with [GPT-4o](/docs/models#gpt-4o), [GPT-4 Turbo](/docs/models#gpt-4-turbo-and-gpt-4), and all GPT-3.5 Turbo models since `gpt-3.5-turbo-1106`.
  
  Setting to `{ "type": "json_schema", "json_schema": {...} }` enables Structured Outputs which ensures the model will match your supplied JSON schema. Learn more in the [Structured Outputs guide](/docs/guides/structured-outputs).
  
  Setting to `{ "type": "json_object" }` enables JSON mode, which ensures the message the model generates is valid JSON.
  
  **Important:** when using JSON mode, you **must** also instruct the model to produce JSON yourself via a system or user message. Without this, the model may generate an unending stream of whitespace until the generation reaches the token limit, resulting in a long-running and seemingly "stuck" request. Also note that the message content may be partially cut off if `finish_reason="length"`, which indicates the generation exceeded `max_tokens` or the conversation exceeded the max context length.
  """)
union AssistantsApiResponseFormatOption {
  "auto",
  @doc("""
    The type of response format being defined. Always `text`.
    """)
  type: "text",
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

  reasoning_effort?: AssistantsReasoningEffort | null = "medium";

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

  metadata?: Record<string>;

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

  reasoning_effort?: AssistantsReasoningEffort | null = "medium";

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

  metadata?: Record<string>;

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
  The ranker to use for the file search. If not specified will use the `auto` ranker.
  """)
union FileSearchRanker {
  "auto",
  "default_2024_08_21",
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

model AssistantsFunctionObject {
  /** A description of what the function does, used by the model to choose when and how to call the function. */
  description?: string;

  /** The name of the function to be called. Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 64. */
  name: string;

  /**
   * The parameters the functions accepts, described as a JSON Schema object. See the [guide](/docs/guides/function-calling) for examples, and the [JSON Schema reference](https://json-schema.org/understanding-json-schema/) for documentation about the format.
   *
   * Omitting `parameters` defines a function with an empty parameter list.
   */
  parameters?: unknown;

  @doc("""
    Whether to enable strict schema adherence when generating the function call. If set to true, the model will follow the exact schema defined in the `parameters` field. Only a subset of JSON Schema is supported when `strict` is `true`. Learn more about Structured Outputs in the [function calling guide](docs/guides/function-calling).
    """)
  strict?: boolean | null = false;
}

// Tool customization (apply_discriminator): apply a common model base for tool definitions
model AssistantToolsFunction extends AssistantToolDefinition {
  @doc("""
    The type of tool being defined: `function`
    """)
  type: AssistantToolDefinitionType.function;

  function: AssistantsFunctionObject;
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

  /**
   * Set of 16 key-value pairs that can be attached to an object. This can be
   * useful for storing additional information about the object in a structured
   * format, and querying for objects via API or the dashboard.
   *
   * Keys are strings with a maximum length of 64 characters. Values are strings
   * with a maximum length of 512 characters.
   */
  @extension("x-oaiTypeLabel", "map")
  metadata?: Record<string> | null;

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

union AssistantCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

// This customization allows us to concretely specify that the file_search object must provide
// either ID references --or-- in-line creation helpers, but not both.

model ToolResourcesFileSearch {
  ...ToolResourcesFileSearchIdsOnly;
  ...ToolResourcesFileSearchVectorStoreCreationHelpers;
}

model ToolResourcesFileSearchIdsOnly {
  /**
   * The [vector store](/docs/api-reference/vector-stores/object) attached to this assistant.
   * There can be a maximum of 1 vector store attached to the assistant.
   */
  @maxItems(1)
  vector_store_ids?: string[];
}

model ToolResourcesFileSearchVectorStoreCreationHelpers {
  /**
   * A helper to create a [vector store](/docs/api-reference/vector-stores/object) with
   * file_ids and attach it to this assistant. There can be a maximum of 1 vector store
   * attached to the assistant.
   */
  @maxItems(1)
  vector_stores?: ToolResourcesFileSearchVectorStoreCreationHelper[];
}

alias ToolResourcesFileSearchVectorStoreCreationHelper = {
  /**
   * A list of [file](/docs/api-reference/files) IDs to add to the vector store. There can be
   * a maximum of 10000 files in a vector store.
   */
  @maxItems(10000)
  file_ids?: string[];

  @doc("""
    The chunking strategy used to chunk the file(s). If not set, will use the `auto` strategy. Only applicable if `file_ids` is non-empty.
    """)
  @extension("x-oaiExpandable", true)
  chunking_strategy?: ChunkingStrategyRequestParam;

  metadata?: Record<string>;
};

union AssistantToolDefinitionType {
  code_interpreter: "code_interpreter",
  file_search: "file_search",
  function: "function",
}

@discriminator("type")
model AssistantToolDefinition {
  type: AssistantToolDefinitionType;
}

@access(Access.public)
@usage(Usage.input)
model AssistantCollectionOptions {
  
/**
   * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include after=obj_foo in order to fetch the next page of the list.
   */
  @query @continuationToken afterId?: string;

/**
   * A cursor for use in pagination. `before` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, starting with obj_foo, your
   * subsequent call can include before=obj_foo in order to fetch the previous page of the list.
   */
  @query beforeId?: string;

  /**
   * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
   * default is 20.
   */
  @query pageSizeLimit?: int32 = 20;

  @query order?: AssistantCollectionOrder;
}

union MessageCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model MessageCollectionOptions {
  
/**
   * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include after=obj_foo in order to fetch the next page of the list.
   */
  @query @continuationToken afterId?: string;

  
/**
   * A cursor for use in pagination. `before` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, starting with obj_foo, your
   * subsequent call can include before=obj_foo in order to fetch the previous page of the list.
   */
  @query beforeId?: string;


  /**
   * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
   * default is 20.
   */
  @query pageSizeLimit?: int32 = 20;

  @query order?: MessageCollectionOrder;
}

@access(Access.public)
@usage(Usage.input)
model RunCollectionOptions {
  
/**
   * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include after=obj_foo in order to fetch the next page of the list.
   */
  @query @continuationToken afterId?: string;

  
/**
   * A cursor for use in pagination. `before` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, starting with obj_foo, your
   * subsequent call can include before=obj_foo in order to fetch the previous page of the list.
   */
  @query beforeId?: string;

  
  /**
   * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
   * default is 20.
   */
  @query pageSizeLimit?: int32 = 20;

  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: RunCollectionOrder;
}

@access(Access.public)
@usage(Usage.input)
model RunStepCollectionOptions {
  
/**
   * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
   * subsequent call can include after=obj_foo in order to fetch the next page of the list.
   */
  @query @continuationToken afterId?: string;

  
/**
   * A cursor for use in pagination. `before` is an object ID that defines your place in the list.
   * For instance, if you make a list request and receive 100 objects, starting with obj_foo, your
   * subsequent call can include before=obj_foo in order to fetch the previous page of the list.
   */
  @query beforeId?: string;

  
  /**
   * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
   * default is 20.
   */
  @query pageSizeLimit?: int32 = 20;

  /**
   * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
   * for descending order.
   */
  @query order?: RunStepCollectionOrder;
}

union RunStepCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}
