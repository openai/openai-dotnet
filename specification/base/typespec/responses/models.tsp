/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@doc("""
  **Reasoning models only**
  
  Constrains effort on reasoning for
  [reasoning models](https://platform.openai.com/docs/guides/reasoning).
  Currently supported values are `none`, `minimal`, `low`, `medium`, and `high`. Reducing
  reasoning effort can result in faster responses and fewer tokens used
  on reasoning in a response.
  """)
union ResponsesReasoningEffort {
  "none",
  "minimal",
  "low",
  "medium",
  "high",
}

union ResponsesCompoundFilterType {
  string,
  AndValue: "and",
  OrValue: "or",
}

/** A filter used to compare a specified attribute key to a given value using a defined comparison operation. */
model ResponsesComparisonFilter {
  @doc("""
    Specifies the comparison operator: `eq`, `ne`, `gt`, `gte`, `lt`, `lte`.
    - `eq`: equals
    - `ne`: not equal
    - `gt`: greater than
    - `gte`: greater than or equal
    - `lt`: less than
    - `lte`: less than or equal
    """)
  type: "eq" | "ne" | "gt" | "gte" | "lt" | "lte" = "eq";

  /** The key to compare against the value. */
  key: string;

  /** The value to compare against the attribute key; supports string, number, or boolean types. */
  value: string | float32 | boolean;
}

@doc("""
  Combine multiple filters using `and` or `or`.
  """)
model ResponsesCompoundFilter {
  @doc("""
    Type of operation: `and` or `or`.
    """)
  type: ResponsesCompoundFilterType;

  @doc("""
    Array of filters to combine. Items can be `ResponsesComparisonFilter` or `ResponsesCompoundFilter`.
    """)
  filters: (ResponsesComparisonFilter | ResponsesCompoundFilter)[];
}

alias ResponsesVectorStoreFileAttributes = Record<unknown>;

// Tool generated type. Extracts from CodeInterpreterFileOutput.files
alias CodeInterpreterFileOutputFile = {
  /** The MIME type of the file. */
  mime_type: string;

  /** The ID of the file. */
  file_id: string;
};

@doc("""
  Specifies the latency tier to use for processing the request. This parameter is relevant for customers subscribed to the scale tier service:
    - If set to 'auto', and the Project is Scale tier enabled, the system
      will utilize scale tier credits until they are exhausted.
    - If set to 'auto', and the Project is not Scale tier enabled, the request will be processed using the default service tier with a lower uptime SLA and no latency guarantee.
    - If set to 'default', the request will be processed using the default service tier with a lower uptime SLA and no latency guarantee.
    - If set to 'flex', the request will be processed with the Flex Processing service tier. [Learn more](/docs/guides/flex-processing).
    - When not set, the default behavior is 'auto'.
  
    When this parameter is set, the response body will include the `service_tier` utilized.
  """)
union DotNetResponseServiceTier {
  "auto",
  "default",
  "flex",
  "scale",
}

model CreateResponse {
  metadata?: Record<string>;

  @doc("""
    What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.
    We generally recommend altering this or `top_p` but not both.
    """)
  @minValue(0)
  @maxValue(2)
  temperature?: float32 | null = 1;
  
  /** An integer between 0 and 20 specifying the number of most likely tokens to return at each token position, each with an associated log probability. */
  top_logprobs?: int32 | null;

  @doc("""
    An alternative to sampling with temperature, called nucleus sampling,
    where the model considers the results of the tokens with top_p probability
    mass. So 0.1 means only the tokens comprising the top 10% probability mass
    are considered.
    
    We generally recommend altering this or `temperature` but not both.
    """)
  @minValue(0)
  @maxValue(1)
  top_p?: float32 | null = 1;

  /** A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse. [Learn more](/docs/guides/safety-best-practices#end-user-ids). */
  user?: string;

  /**A stable identifier used to help detect users of your application that may be violating OpenAI's usage policies.
  The IDs should be a string that uniquely identifies each user. We recommend hashing their username or email address, in order to avoid sending us any identifying information. [Learn more](https://platform.openai.com/docs/guides/safety-best-practices#safety-identifiers).*/
  safety_identifier?: string;

  service_tier?: DotNetResponseServiceTier | null = "auto";
  
  ...ResponseProperties;

  // Tool customization: Apply renamed Item -> ItemParam
  /**
   * Text, image, or file inputs to the model, used to generate a response.
   *
   * Learn more:
   * - [Text inputs and outputs](/docs/guides/text)
   * - [Image inputs](/docs/guides/images)
   * - [File inputs](/docs/guides/pdf-files)
   * - [Conversation state](/docs/guides/conversation-state)
   * - [Function calling](/docs/guides/function-calling)
   */
  input?: string | (ImplicitUserMessage | ItemParam)[];

  @doc("""
    Specify additional output data to include in the model response. Currently
    supported values are:
    - `file_search_call.results`: Include the search results of
      the file search tool call.
    - `message.input_image.image_url`: Include image urls from the input message.
    - `computer_call_output.output.image_url`: Include image urls from the computer call output.
    - `reasoning.encrypted_content`: Includes an encrypted version of reasoning
      tokens in reasoning item outputs. This enables reasoning items to be used in
      multi-turn conversations when using the Responses API statelessly (like
      when the `store` parameter is set to `false`, or when an organization is
      enrolled in the zero data retention program).
    - `code_interpreter_call.outputs`: Includes the outputs of python code execution
      in code interpreter tool call items.
    """)
  include?: Includable[] | null;

  /** Whether to allow the model to run tool calls in parallel. */
  parallel_tool_calls?: boolean | null = true;

  /**
   * Whether to store the generated model response for later retrieval via
   * API.
   */
  store?: boolean | null = true;

  /**
   * A system (or developer) message inserted into the model's context.
   * When using along with previous_response_id, the instructions from a previous response will not be carried over to the next response.
   * This makes it simple to swap out system (or developer) messages in new responses.
   */
  instructions?: string | null;

  /**
   * If set to true, the model response data will be streamed to the client
   * as it is generated using [server-sent events](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events#Event_stream_format).
   * See the [Streaming section below](/docs/api-reference/responses-streaming)
   * for more information.
   */
  stream?: boolean | null = false;

  /** The conversation that this response belongs to.
   *  Items from this conversation are prepended to input_items for this response request.
   *  Input items and output items from this response are automatically added to this conversation after this response completes. */
  conversation?: ConversationParam | null;

  /** Used by OpenAI to cache responses for similar requests to optimize your cache hit rates. Replaces the `user` field. [Learn more](/docs/guides/prompt-caching). */
  prompt_cache_key?: string;

  prompt_cache_retention?: PromptCacheRetention;
}

union PromptCacheRetention {
  string,
  in_memory: "in_memory",
  `24h`: "24h",
}

/**The conversation that this response belongs to. Items from this conversation are prepended to `input_items` for this response request.
Input items and output items from this response are automatically added to this conversation after this response completes.*/
union ConversationParam {
  string,
  `ConversationParam-2`,
}

/** The conversation that this response belongs to. */
@summary("Conversation object")
model `ConversationParam-2` {
  /** The unique ID of the conversation. */
  id: string;
}

model Response {
  /**
   * Set of 16 key-value pairs that can be attached to an object. This can be
   * useful for storing additional information about the object in a structured
   * format, and querying for objects via API or the dashboard.
   *
   * Keys are strings with a maximum length of 64 characters. Values are strings
   * with a maximum length of 512 characters.
   */
  @extension("x-oaiTypeLabel", "map")
  metadata: Record<string> | null;

  @doc("""
    What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.
    We generally recommend altering this or `top_p` but not both.
    """)
  @minValue(0)
  @maxValue(2)
  temperature: float32 | null;

  /** An integer between 0 and 20 specifying the number of most likely tokens to return at each token position, each with an associated log probability. */
  top_logprobs?: int32 | null;

  @doc("""
    An alternative to sampling with temperature, called nucleus sampling,
    where the model considers the results of the tokens with top_p probability
    mass. So 0.1 means only the tokens comprising the top 10% probability mass
    are considered.
    
    We generally recommend altering this or `temperature` but not both.
    """)
  @minValue(0)
  @maxValue(1)
  top_p: float32 | null;

  /** A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse. [Learn more](/docs/guides/safety-best-practices#end-user-ids). */
  user: string | null;

  /**A stable identifier used to help detect users of your application that may be violating OpenAI's usage policies.
  The IDs should be a string that uniquely identifies each user. We recommend hashing their username or email address, in order to avoid sending us any identifying information. [Learn more](https://platform.openai.com/docs/guides/safety-best-practices#safety-identifiers).*/
  safety_identifier?: string;

  service_tier?: DotNetResponseServiceTier;
  ...ResponseProperties;

  /** Unique identifier for this Response. */
  id: string;

  @doc("""
    The object type of this resource - always set to `response`.
    """)
  object: "response";

  @doc("""
    The status of the response generation. One of `completed`, `failed`,
    `in_progress`, `cancelled`, `queued`, or `incomplete`.
    """)
  status?:
    | "completed"
    | "failed"
    | "in_progress"
    | "cancelled"
    | "queued"
    | "incomplete";

  // Tool customization: Fully qualify as an integer-based unix timestamp
  /** Unix timestamp (in seconds) of when this Response was created. */
  @encode("unixTimestamp", int32)
  created_at: utcDateTime;

  error: ResponseError | null;

  /** Details about why the response is incomplete. */
  incomplete_details: {
    /** The reason why the response is incomplete. */
    reason?: "max_output_tokens" | "content_filter";
  } | null;

  @doc("""
    An array of content items generated by the model.
    
    - The length and order of items in the `output` array is dependent
      on the model's response.
    - Rather than accessing the first item in the `output` array and
      assuming it's an `assistant` message with the content generated by
      the model, you might consider using the `output_text` property where
      supported in SDKs.
    """)
  output: OutputItem[];

  /**
   * A collection of system (or developer) messages inserted into the model's context.
   * When using along with previous_response_id, the instructions from a previous response will not be carried over to the next response.
   * This makes it simple to swap out system (or developer) messages in new responses.
   */
  instructions: string | InputItem[] | null;

  @doc("""
    SDK-only convenience property that contains the aggregated text output
    from all `output_text` items in the `output` array, if any are present.
    Supported in the Python and JavaScript SDKs.
    """)
  output_text?: string | null;

  usage?: ResponseUsage;

  /** Whether to allow the model to run tool calls in parallel. */
  parallel_tool_calls: boolean = true;

  /** The conversation that this response belongs to. Input items and output items from this response are automatically added to this conversation. */
  conversation?: `ConversationParam-2` | null;

  /** Used by OpenAI to cache responses for similar requests to optimize your cache hit rates. Replaces the `user` field. [Learn more](/docs/guides/prompt-caching). */
  prompt_cache_key?: string;

  prompt_cache_retention?: PromptCacheRetention;
}

model ResponseProperties {
  /**
   * The unique ID of the previous response to the model. Use this to
   * create multi-turn conversations. Learn more about
   * [conversation state](/docs/guides/conversation-state).
   */
  previous_response_id?: string | null;

  @doc("""
    Model ID used to generate the response, like `gpt-4o` or `o3`. OpenAI
    offers a wide range of models with different capabilities, performance
    characteristics, and price points. Refer to the [model guide](/docs/models)
    to browse and compare available models.
    """)
  `model`?: ModelIdsResponses;

  reasoning?: Reasoning | null;

  /**
   * Whether to run the model response in the background.
   * [Learn more](/docs/guides/background).
   */
  background?: boolean | null = false;

  /** An upper bound for the number of tokens that can be generated for a response, including visible output tokens and [reasoning tokens](/docs/guides/reasoning). */
  max_output_tokens?: int32 | null;

  /** The maximum number of total calls to built-in tools that can be processed in a response. This maximum number applies across all built-in tool calls, not per individual tool. Any further attempts to call a tool by the model will be ignored. */
  max_tool_calls?: int32 | null;

  /**
   * Configuration options for a text response from the model. Can be plain
   * text or structured JSON data. Learn more:
   * - [Text inputs and outputs](/docs/guides/text)
   * - [Structured Outputs](/docs/guides/structured-outputs)
   */
  text?: {
    // Tool customization: Employ discriminated type base
    format?: ResponseTextFormatConfiguration;
  };

  @doc("""
    An array of tools the model may call while generating a response. You
    can specify which tool to use by setting the `tool_choice` parameter.
    
    The two categories of tools you can provide the model are:
    
    - **Built-in tools**: Tools that are provided by OpenAI that extend the
      model's capabilities, like [web search](/docs/guides/tools-web-search)
      or [file search](/docs/guides/tools-file-search). Learn more about
      [built-in tools](/docs/guides/tools).
    - **Function calls (custom tools)**: Functions that are defined by you,
      enabling the model to call your own code. Learn more about
      [function calling](/docs/guides/function-calling).
    """)
  tools?: Tool[];

  // Tool customization: Employ discriminated type base for tool objects
  @doc("""
    How the model should select which tool (or tools) to use when generating
    a response. See the `tools` parameter to see how to specify which tools
    the model can call.
    """)
  tool_choice?: ToolChoiceOptions | ToolChoiceObject;

  @doc("""
    The truncation strategy to use for the model response.
    - `auto`: If the context of this response and previous ones exceeds
      the model's context window size, the model will truncate the
      response to fit the context window by dropping input items in the
      middle of the conversation.
    - `disabled` (default): If a model response will exceed the context window
      size for a model, the request will fail with a 400 error.
    """)
  truncation?: "auto" | "disabled" | null = "disabled";
}

// Tool customization: Alias to avoid circular reference
alias ModelIds = ModelIdsResponses;

union ModelIdsResponses {
  "gpt-4.1"
  | "gpt-4.1-mini"
  | "gpt-4.1-nano"
  | "gpt-4.1-2025-04-14"
  | "gpt-4.1-mini-2025-04-14"
  | "gpt-4.1-nano-2025-04-14"
  | "o4-mini"
  | "o4-mini-2025-04-16"
  | "o3"
  | "o3-2025-04-16"
  | "o3-mini"
  | "o3-mini-2025-01-31"
  | "o1"
  | "o1-2024-12-17"
  | "o1-preview"
  | "o1-preview-2024-09-12"
  | "o1-mini"
  | "o1-mini-2024-09-12"
  | "gpt-4o"
  | "gpt-4o-2024-11-20"
  | "gpt-4o-2024-08-06"
  | "gpt-4o-2024-05-13"
  | "gpt-4o-audio-preview"
  | "gpt-4o-audio-preview-2024-10-01"
  | "gpt-4o-audio-preview-2024-12-17"
  | "gpt-4o-audio-preview-2025-06-03"
  | "gpt-4o-mini-audio-preview"
  | "gpt-4o-mini-audio-preview-2024-12-17"
  | "gpt-4o-search-preview"
  | "gpt-4o-mini-search-preview"
  | "gpt-4o-search-preview-2025-03-11"
  | "gpt-4o-mini-search-preview-2025-03-11"
  | "chatgpt-4o-latest"
  | "codex-mini-latest"
  | "gpt-4o-mini"
  | "gpt-4o-mini-2024-07-18"
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
  | "gpt-3.5-turbo-0301"
  | "gpt-3.5-turbo-0613"
  | "gpt-3.5-turbo-1106"
  | "gpt-3.5-turbo-0125"
  | "gpt-3.5-turbo-16k-0613"
    | "o1-pro"
    | "o1-pro-2025-03-19"
    | "o3-pro"
    | "o3-pro-2025-06-10"
    | "o3-deep-research"
    | "o3-deep-research-2025-06-26"
    | "o4-mini-deep-research"
    | "o4-mini-deep-research-2025-06-26"
    | "computer-use-preview"
    | "computer-use-preview-2025-03-11"
    | "gpt-5-codex"
    | "gpt-5-pro"
    | "gpt-5-pro-2025-10-06"
    | "gpt-5.1-codex-max",
}

// Tool customization: Establish a discriminated type base for tools
/** A tool that can be used to generate a response. */
union ToolType {
  string,
  function: "function",
  file_search: "file_search",
  computer: "computer",
  computer_use_preview: "computer_use_preview",
  web_search: "web_search",
  mcp: "mcp",
  code_interpreter: "code_interpreter",
  image_generation: "image_generation",
  local_shell: "local_shell",
  shell: "shell",
  custom: "custom",
  `namespace`: "namespace",
  tool_search: "tool_search",
  web_search_preview: "web_search_preview",
  apply_patch: "apply_patch",
}

@discriminator("type")
model Tool {
  type: ToolType;
}

@doc("""
  Controls which (if any) tool is called by the model.
  
  `none` means the model will not call any tool and instead generates a message.
  
  `auto` means the model can pick between generating a message or calling one or
  more tools.
  
  `required` means the model must call one or more tools.
  """)
union ToolChoiceOptions {
  "none",
  "auto",
  "required",
}

// Tool customization: Establish a discriminated type base
/**
 * Indicates that the model should use a built-in tool to generate a response.
 * [Learn more about built-in tools](/docs/guides/tools).
 */
union ToolChoiceObjectType {
  string,
  file_search: "file_search",
  function: "function",
  computer: "computer_use_preview",
  web_search: "web_search_preview",
  image_generation: "image_generation",
  code_interpreter: "code_interpreter",
  mcp: "mcp",
}
@discriminator("type")
model ToolChoiceObject {
  type: ToolChoiceObjectType;
}
model ToolChoiceObjectFileSearch extends ToolChoiceObject {
  type: ToolChoiceObjectType.file_search;
}
model ToolChoiceObjectComputer extends ToolChoiceObject {
  type: ToolChoiceObjectType.computer;
}
model ToolChoiceObjectWebSearch extends ToolChoiceObject {
  type: ToolChoiceObjectType.web_search;
}
model ToolChoiceObjectImageGen extends ToolChoiceObject {
  type: ToolChoiceObjectType.image_generation;
}
model ToolChoiceObjectCodeInterpreter extends ToolChoiceObject {
  type: ToolChoiceObjectType.code_interpreter;
}
model ToolChoiceObjectMCP extends ToolChoiceObject {
  type: ToolChoiceObjectType.mcp;
}

// Tool customization (apply_discriminator): Apply discriminated type base
/** Use this option to force the model to call a specific function. */
model ToolChoiceObjectFunction extends ToolChoiceObject {
  @doc("""
    For function calling, the type is always `function`.
    """)
  type: ToolChoiceObjectType.function;

  /** The name of the function to call. */
  name: string;
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** Defines a function in your own code the model can choose to call. Learn more about [function calling](https://platform.openai.com/docs/guides/function-calling). */
model FunctionTool extends Tool {
  @doc("""
    The type of the function tool. Always `function`.
    """)
  type: ToolType.function;

  /** The name of the function to call. */
  name: string;

  /** A description of the function. Used by the model to determine whether or not to call the function. */
  description?: string | null;

  /** A JSON schema object describing the parameters of the function. */
  parameters: unknown | null;

  @doc("""
    Whether to enforce strict parameter validation. Default `true`.
    """)
  strict: boolean | null;
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** A tool that searches for relevant content from uploaded files. Learn more about the [file search tool](https://platform.openai.com/docs/guides/tools-file-search). */
model FileSearchTool extends Tool {
  @doc("""
    The type of the file search tool. Always `file_search`.
    """)
  type: ToolType.file_search;

  /** The IDs of the vector stores to search. */
  vector_store_ids: string[];

  /** The maximum number of results to return. This number should be between 1 and 50 inclusive. */
  max_num_results?: int32;

  /** Ranking options for search. */
  ranking_options?: RankingOptions;

  /** A filter to apply. */
  filters?: Filters | null;
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** A tool that controls a virtual computer. Learn more about the [computer tool](https://platform.openai.com/docs/guides/tools-computer-use). */
model ComputerUsePreviewTool extends Tool {
  @doc("""
    The type of the computer use tool. Always `computer_use_preview`.
    """)
  type: ToolType.computer_use_preview;

  /** The type of computer environment to control. */
  environment: "windows" | "mac" | "linux" | "ubuntu" | "browser";

  /** The width of the computer display. */
  display_width: int32;

  /** The height of the computer display. */
  display_height: int32;
}

@doc("""
  High level guidance for the amount of context window space to use for the
  search. One of `low`, `medium`, or `high`. `medium` is the default.
  """)
union WebSearchContextSize {
  "low",
  "medium",
  "high",
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** This tool searches the web for relevant results to use in a response. Learn more about the [web search tool](https://platform.openai.com/docs/guides/tools-web-search). */
model WebSearchPreviewTool extends Tool {
  @doc("""
    The type of the web search tool. One of `web_search_preview` or `web_search_preview_2025_03_11`.
    """)
  type: ToolType.web_search_preview;

  // Tool customization: Use a custom discriminated type basis for web search locations
  /** The user's location. */
  user_location?: Location | null;

  @doc("""
    High level guidance for the amount of context window space to use for the search. One of `low`, `medium`, or `high`. `medium` is the default.
    """)
  search_context_size?: WebSearchContextSize;
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** This tool searches the web for relevant results to use in a response. Learn more about the [web search tool](https://platform.openai.com/docs/guides/tools-web-search). */
model WebSearchTool extends Tool {
  /** The type of the web search tool. One of `web_search` or `web_search_2025_08_26`. */
  type: ToolType.web_search;

  filters?: WebSearchToolFilters | null;
  user_location?: Location | null;

  /** High level guidance for the amount of context window space to use for the search. One of `low`, `medium`, or `high`. `medium` is the default. */
  search_context_size?: WebSearchContextSize;
}

model WebSearchToolFilters {
  allowed_domains?: string[] | null;
}

// Tool customization: Establish a discriminated base for location approximation
union LocationType {
  string,
  approximate: "approximate",
}
@discriminator("type")
model Location {
  type: LocationType;
}
model ApproximateLocation extends Location {
  type: LocationType.approximate;
  country?: string | null;
  region?: string | null;
  city?: string | null;
  timezone?: string | null;
}

/**
 * **o-series models only**
 *
 * Configuration options for
 * [reasoning models](https://platform.openai.com/docs/guides/reasoning).
 */
model Reasoning {
  effort?: ResponsesReasoningEffort | null = "medium";

  @doc("""
    A summary of the reasoning performed by the model. This can be
    useful for debugging and understanding the model's reasoning process.
    One of `auto`, `concise`, or `detailed`.
    """)
  summary?: "auto" | "concise" | "detailed" | null;

  // Tool customization: Correct inapplicable default
  #suppress "deprecated" "Warning unnecessary for spec description of deprecation"
  #deprecated "This field is marked as deprecated."
  @doc("""
    **Deprecated:** use `summary` instead.
    
    A summary of the reasoning performed by the model. This can be
    useful for debugging and understanding the model's reasoning process.
    One of `auto`, `concise`, or `detailed`.
    """)
  generate_summary?: "auto" | "concise" | "detailed" | null = null;
}

// Tool customization: Establish discriminated type base to increase union precision, rename for clarity
@doc("""
  An object specifying the format that the model must output.
  
  Configuring `{ "type": "json_schema" }` enables Structured Outputs,
  which ensures the model will match your supplied JSON schema. Learn more in the
  [Structured Outputs guide](/docs/guides/structured-outputs).
  
  The default format is `{ "type": "text" }` with no additional options.
  
  **Not recommended for gpt-4o and newer models:**
  
  Setting to `{ "type": "json_object" }` enables the older JSON mode, which
  ensures the message the model generates is valid JSON. Using `json_schema`
  is preferred for models that support it.
  """)
union ResponseTextFormatConfigurationType {
  string,
  text: "text",
  json_schema: "json_schema",
  json_object: "json_object",
}
@discriminator("type")
model ResponseTextFormatConfiguration {
  type: ResponseTextFormatConfigurationType;
}
model ResponseTextFormatConfigurationText
  extends ResponseTextFormatConfiguration {
  type: ResponseTextFormatConfigurationType.text;
}
model ResponseTextFormatConfigurationJsonObject
  extends ResponseTextFormatConfiguration {
  type: ResponseTextFormatConfigurationType.json_object;
}

// Tool customization (apply_discriminator): Apply base type
/**
 * JSON Schema response format. Used to generate structured JSON responses.
 * Learn more about [Structured Outputs](/docs/guides/structured-outputs).
 */
model ResponseTextFormatConfigurationJsonSchema
  extends ResponseTextFormatConfiguration {
  @doc("""
    The type of response format being defined. Always `json_schema`.
    """)
  type: ResponseTextFormatConfigurationType.json_schema;

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

  schema: Record<unknown>;

  @doc("""
    Whether to enable strict schema adherence when generating the output.
    If set to true, the model will always follow the exact schema defined
    in the `schema` field. Only a subset of JSON Schema is supported when
    `strict` is `true`. To learn more, read the [Structured Outputs
    guide](/docs/guides/structured-outputs).
    """)
  strict?: boolean | null = false;
}

/**Specify additional output data to include in the model response. Currently supported values are:
- `web_search_call.action.sources`: Include the sources of the web search tool call.
- `code_interpreter_call.outputs`: Includes the outputs of python code execution in code interpreter tool call items.
- `computer_call_output.output.image_url`: Include image urls from the computer call output.
- `file_search_call.results`: Include the search results of the file search tool call.
- `message.input_image.image_url`: Include image urls from the input message.
- `message.output_text.logprobs`: Include logprobs with assistant messages.
- `reasoning.encrypted_content`: Includes an encrypted version of reasoning tokens in reasoning item outputs. This enables reasoning items to be used in multi-turn conversations when using the Responses API statelessly (like when the `store` parameter is set to `false`, or when an organization is enrolled in the zero data retention program).*/
union Includable {
  string,
  `file_search_call.results`: "file_search_call.results",
  `web_search_call.results`: "web_search_call.results",
  `web_search_call.action.sources`: "web_search_call.action.sources",
  `message.input_image.image_url`: "message.input_image.image_url",
  `computer_call_output.output.image_url`: "computer_call_output.output.image_url",
  `code_interpreter_call.outputs`: "code_interpreter_call.outputs",
  `reasoning.encrypted_content`: "reasoning.encrypted_content",
  `message.output_text.logprobs`: "message.output_text.logprobs",
}

// Tool customization: Establish discriminated type hierarchy
/** Multi-modal input and output contents. */
union ItemContentType {
  input_text: "input_text",
  input_audio: "input_audio",
  input_image: "input_image",
  input_file: "input_file",
  output_text: "output_text",
  output_audio: "output_audio",
  refusal: "refusal",
}
@discriminator("type")
model ItemContent {
  type: ItemContentType;
}

// Tool customization (apply_discriminator): Apply discriminated type base, rename for clarity
/** An audio input to the model. */
model ItemContentInputAudio extends ItemContent {
  @doc("""
    The type of the input item. Always `input_audio`.
    """)
  type: ItemContentType.input_audio;

  /** Base64-encoded audio data. */
  data: string;

  @doc("""
    The format of the audio data. Currently supported formats are `mp3` and
    `wav`.
    """)
  format: "mp3" | "wav";
}

/**
 * A list of one or many input items to the model, containing different content
 * types.
 */
model InputMessageContentList is InputContent[];

// Tool customization (apply_discriminator): Apply discriminated type base, rename for clarity
/** An audio output from the model. */
model ItemContentOutputAudio extends ItemContent {
  @doc("""
    The type of the output audio. Always `output_audio`.
    """)
  type: ItemContentType.output_audio;

  /** Base64-encoded audio data from the model. */
  data: string;

  /** The transcript of the audio data from the model. */
  transcript: string;
}

// Tool customization (apply_discriminator): Apply discriminated type base, rename for clarity
/** A refusal from the model. */
model ItemContentRefusal extends ItemContent {
  @doc("""
    The type of the refusal. Always `refusal`.
    """)
  type: ItemContentType.refusal;

  /** The refusal explanationfrom the model. */
  refusal: string;
}

// Tool customization: Establish discriminated type hierarchy
union AnnotationType {
  string,
  file_citation: "file_citation",
  url_citation: "url_citation",
  file_path: "file_path",
  container_file_citation: "container_file_citation",
}
@discriminator("type")
model Annotation {
  type: AnnotationType;
}

// Tool customization (apply_discriminator): Apply discriminated base type, rename for consistency and clarity
/** A citation to a file. */
model AnnotationFileCitation extends Annotation {
  @doc("""
    The type of the file citation. Always `file_citation`.
    """)
  type: AnnotationType.file_citation;

  /** The ID of the file. */
  file_id: string;

  /** The index of the file in the list of files. */
  index: int32;

  /** The filename of the file cited. */
  filename: string;
}

// Tool customization (apply_discriminator): Apply discriminated base type, rename for consistency and clarity
/** A citation for a web resource used to generate a model response. */
model AnnotationUrlCitation extends Annotation {
  @doc("""
    The type of the URL citation. Always `url_citation`.
    """)
  type: AnnotationType.url_citation;

  // Tool customization: url uses the url type
  /** The URL of the web resource. */
  url: url;

  /** The index of the first character of the URL citation in the message. */
  start_index: int32;

  /** The index of the last character of the URL citation in the message. */
  end_index: int32;

  /** The title of the web resource. */
  title: string;
}

/** A citation for a container file used to generate a model response. */
model ContainerFileCitationBody extends Annotation {
  @doc("""
    The type of the container file citation. Always `container_file_citation`.
    """)
  type: AnnotationType.container_file_citation;

  /** The ID of the container file. */
  container_id: string;

  /** The ID of the file. */
  file_id: string;

  /** The index of the first character of the container file citation in the message. */
  start_index: int32;

  /** The index of the last character of the container file citation in the message. */
  end_index: int32;

  /** The filename of the container file cited. */
  filename: string;
}

// Tool customization (apply_discriminator): Apply discriminated base type, rename for consistency and clarity
/** A path to a file. */
model AnnotationFilePath extends Annotation {
  @doc("""
    The type of the file path. Always `file_path`.
    """)
  type: AnnotationType.file_path;

  /** The ID of the file. */
  file_id: string;

  /** The index of the file in the list of files. */
  index: int32;
}

// Tool customization (apply_discriminator): Apply discriminated type base, rename for clarity
/** A text input to the model. */
model ItemContentInputText extends ItemContent {
  @doc("""
    The type of the input item. Always `input_text`.
    """)
  type: ItemContentType.input_text;

  /** The text input to the model. */
  text: string;
}

// Tool customization (apply_discriminator): Apply discriminated type base, rename for clarity
/** An image input to the model. Learn about [image inputs](/docs/guides/vision). */
model ItemContentInputImage extends ItemContent {
  @doc("""
    The type of the input item. Always `input_image`.
    """)
  type: ItemContentType.input_image;

  /** The URL of the image to be sent to the model. A fully qualified URL or base64 encoded image in a data URL. */
  image_url?: url | null;

  /** The ID of the file to be sent to the model. */
  file_id?: string | null;

  // Tool customization: Fix optionality, default of property
  @doc("""
    The detail level of the image to be sent to the model. One of `high`, `low`, or `auto`. Defaults to `auto`.
    """)
  detail?: "low" | "high" | "auto" = "auto";
}

// Tool customization (apply_discriminator): Apply discriminated type base, rename for clarity
/** A file input to the model. */
model ItemContentInputFile extends ItemContent {
  @doc("""
    The type of the input item. Always `input_file`.
    """)
  type: ItemContentType.input_file;

  /** The ID of the file to be sent to the model. */
  file_id?: string | null;

  /** The URL of the file to be sent to the model. */
  file_url?: url | null;

  /** The name of the file to be sent to the model. */
  filename?: string;

  /** The content of the file to be sent to the model. */
  file_data?: string;
}

model RankingOptions {
  /** The ranker to use for the file search. */
  ranker?: "auto" | "default-2024-11-15";

  /** The score threshold for the file search, a number between 0 and 1. Numbers closer to 1 will attempt to return only the most relevant results, but may return fewer results. */
  score_threshold?: float32;
}

union Filters {
  ResponsesComparisonFilter,
  ResponsesCompoundFilter,
}

// Tool customization (apply_discriminator): Apply discriminated type base, rename for clarity
/** A text output from the model. */
model ItemContentOutputText extends ItemContent {
  @doc("""
    The type of the output text. Always `output_text`.
    """)
  type: ItemContentType.output_text;

  /** The text output from the model. */
  text: string;

  /** The annotations of the text output. */
  annotations: Annotation[];

  logprobs?: LogProb[];
}

/**
 * Represents token usage details including input tokens, output tokens,
 * a breakdown of output tokens, and the total tokens used.
 */
model ResponseUsage {
  /** The number of input tokens. */
  input_tokens: int32;

  /** A detailed breakdown of the input tokens. */
  input_tokens_details: {
    /**
     * The number of tokens that were retrieved from the cache.
     * [More on prompt caching](/docs/guides/prompt-caching).
     */
    cached_tokens: int32;
  };

  /** The number of output tokens. */
  output_tokens: int32;

  /** A detailed breakdown of the output tokens. */
  output_tokens_details: {
    /** The number of reasoning tokens. */
    reasoning_tokens: int32;
  };

  /** The total number of tokens used. */
  total_tokens: int32;
}

/** An error object returned when the model fails to generate a Response. */
model ResponseError {
  code: ResponseErrorCode;

  /** A human-readable description of the error. */
  message: string;

  /** The parameter that caused the error, if applicable. */
  param: string | null;

  /** The type of the error. */
  type: string;
}

/** The error code for the response. */
union ResponseErrorCode {
  "server_error",
  "rate_limit_exceeded",
  "invalid_prompt",
  "vector_store_timeout",
  "invalid_image",
  "invalid_image_format",
  "invalid_base64_image",
  "invalid_image_url",
  "image_too_large",
  "image_too_small",
  "image_parse_error",
  "image_content_policy_violation",
  "invalid_image_mode",
  "image_file_too_large",
  "unsupported_image_media_type",
  "empty_image_file",
  "failed_to_download_image",
  "image_file_not_found",
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** A tool that runs Python code to help generate a response to a prompt. */
model CodeInterpreterTool extends Tool {
  @doc("""
    The type of the code interpreter tool. Always `code_interpreter`.
    """)
  type: ToolType.code_interpreter;

  /**
   * The code interpreter container. Can be a container ID or an object that
   * specifies uploaded file IDs to make available to your code.
   */
  container: string | CodeInterpreterToolAuto;
}

/**
 * Configuration for a code interpreter container. Optionally specify the IDs
 * of the files to run the code on.
 */
model CodeInterpreterToolAuto extends CodeInterpreterContainerConfiguration {
  @doc("Always `auto`.")
  type: CodeInterpreterContainerConfigurationType.auto;

  /** An optional list of uploaded files to make available to your code. */
  file_ids?: string[];
}

// Shared image generation enums used across ImageGenTool and ImageGenToolCallItemResource

/** The quality of a generated image. */
union ImageGenerationQuality {
  string,
  /** Automatically select the best quality for the model. */
  Auto: "auto",

  /** Low quality. */
  Low: "low",

  /** Medium quality. */
  Medium: "medium",

  /** High quality. */
  High: "high",
}

/** The size of a generated image. */
union ImageGenerationSize {
  string,
  /** Automatically select the best size for the model. */
  Auto: "auto",

  /** A square image with 1024 pixels of both width and height. */
  W1024xH1024: "1024x1024",

  /** An image 1024 pixels wide by 1536 pixels high. */
  W1024xH1536: "1024x1536",

  /** An image 1536 pixels wide by 1024 pixels high. */
  W1536xH1024: "1536x1024",
}

/** The output format for a generated image. */
union ImageGenerationOutputFormat {
  string,
  /** PNG format. */
  png: "png",

  /** WebP format. */
  webp: "webp",

  /** JPEG format. */
  jpeg: "jpeg",
}

/** The background type for a generated image. */
union ImageGenerationBackground {
  string,
  /** Transparent background. */
  transparent: "transparent",

  /** Opaque background. */
  opaque: "opaque",

  /** Automatically select the best background for the image. */
  auto: "auto",
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
@doc("""
  A tool that generates images using a model like `gpt-image-1`.
  """)
model ImageGenTool extends Tool {
  @doc("""
    The type of the image generation tool. Always `image_generation`.
    """)
  type: ToolType.image_generation;

  @doc("""
    The image generation model to use. Default: `gpt-image-1`.
    """)
  `model`?: "gpt-image-1" = "gpt-image-1";

  @doc("""
    The quality of the generated image. One of `low`, `medium`, `high`,
    or `auto`. Default: `auto`.
    """)
  quality?: ImageGenerationQuality = ImageGenerationQuality.Auto;

  @doc("""
    The size of the generated image. One of `1024x1024`, `1024x1536`,
    `1536x1024`, or `auto`. Default: `auto`.
    """)
  size?: ImageGenerationSize = ImageGenerationSize.Auto;

  @doc("""
    The output format of the generated image. One of `png`, `webp`, or
    `jpeg`. Default: `png`.
    """)
  output_format?: ImageGenerationOutputFormat = ImageGenerationOutputFormat.png;

  /** Compression level for the output image. Default: 100. */
  @minValue(0)
  @maxValue(100)
  output_compression?: int32 = 100;

  @doc("""
    Moderation level for the generated image. Default: `auto`.
    """)
  moderation?: "auto" | "low" = "auto";

  @doc("""
    Background type for the generated image. One of `transparent`,
    `opaque`, or `auto`. Default: `auto`.
    """)
  background?: ImageGenerationBackground = ImageGenerationBackground.auto;

  @doc("""
    Control how much effort the model will exert to match the style and features,
    especially facial features, of input images. This parameter is only supported
    for `gpt-image-1`. Supports `high` and `low`. Defaults to `low`.
    """)
  input_fidelity?: "low" | "high" = "low";

  @doc("""
    Optional mask for inpainting. Contains `image_url`
    (string, optional) and `file_id` (string, optional).
    """)
  input_image_mask?: ImageGenToolInputImageMask;

  /** Number of partial images to generate in streaming mode, from 0 (default value) to 3. */
  @minValue(0)
  @maxValue(3)
  partial_images?: int32 = 0;

  /** Whether to generate a new image or edit an existing image. Default: `auto`. */
  action?: ImageGenerationToolAction = ImageGenerationToolAction.Auto;
}

model ImageGenToolInputImageMask {
  image_url?: url;
  file_id?: string;
}

union ImageGenerationToolAction {
  string,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  Generate: "generate",
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  Edit: "edit",
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  Auto: "auto",
}

/** Execute a shell command on the server. */
model LocalShellExecAction {
  @doc("""
    The type of the local shell action. Always `exec`.
    """)
  type: "exec";

  /** The command to run. */
  command: string[];

  /** Optional timeout in milliseconds for the command. */
  timeout_ms?: int32 | null;

  /** Optional working directory to run the command in. */
  working_directory?: string | null;

  /** Environment variables to set for the command. */
  env: Record<string>;

  /** Optional user to run the command as. */
  user?: string | null;
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** A tool that allows the model to execute shell commands in a local environment. */
model LocalShellTool extends Tool {
  @doc("""
    The type of the local shell tool. Always `local_shell`.
    """)
  type: ToolType.local_shell;
}

/** A tool available on an MCP server. */
model MCPListToolsTool {
  /** The name of the tool. */
  name: string;

  /** The description of the tool. */
  description?: string | null;

  /** The JSON schema describing the tool's input. */
  input_schema: unknown;

  /** Additional annotations about the tool. */
  annotations?: unknown | null;
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/**
 * Give the model access to additional tools via remote Model Context Protocol
 * (MCP) servers. [Learn more about MCP](/docs/guides/tools-remote-mcp).
 */
model MCPTool extends Tool {
  @doc("""
    The type of the MCP tool. Always `mcp`.
    """)
  type: ToolType.mcp;

  /** A label for this MCP server, used to identify it in tool calls. */
  server_label: string;

  /**The URL for the MCP server. One of `server_url` or `connector_id` must be
  provided.*/
  server_url?: string;

  /**Identifier for service connectors, like those available in ChatGPT. One of
  `server_url` or `connector_id` must be provided. Learn more about service
  connectors [here](https://platform.openai.com/docs/guides/tools-remote-mcp#connectors).

  Currently supported `connector_id` values are:

  - Dropbox: `connector_dropbox`
  - Gmail: `connector_gmail`
  - Google Calendar: `connector_googlecalendar`
  - Google Drive: `connector_googledrive`
  - Microsoft Teams: `connector_microsoftteams`
  - Outlook Calendar: `connector_outlookcalendar`
  - Outlook Email: `connector_outlookemail`
  - SharePoint: `connector_sharepoint`*/
  connector_id?:
    | "connector_dropbox"
    | "connector_gmail"
    | "connector_googlecalendar"
    | "connector_googledrive"
    | "connector_microsoftteams"
    | "connector_outlookcalendar"
    | "connector_outlookemail"
    | "connector_sharepoint";

  /**An OAuth access token that can be used with a remote MCP server, either
  with a custom MCP server URL or a service connector. Your application
  must handle the OAuth authorization flow and provide the token here.*/
  authorization?: string;

  /** Optional description of the MCP server, used to provide more context. */
  server_description?: string;

  /**Optional HTTP headers to send to the MCP server. Use for authentication
  or other purposes.*/
  headers?: Record<string> | null;

  /** List of allowed tool names or a filter object. */
  allowed_tools?: string[] | MCPToolFilter | null;

  /** Specify which of the MCP server's tools require approval. */
  require_approval?:
    | {
        always?: MCPToolFilter;
        `never`?: MCPToolFilter;
      }
    | "always"
    | "never"
    | null;
}

/** A filter object to specify which tools are allowed. */
@summary("MCP tool filter")
model MCPToolFilter {
  /** List of allowed tool names. */
  @summary("MCP allowed tools")
  tool_names?: string[];

  /**Indicates whether or not a tool modifies data or is read-only. If an
  MCP server is [annotated with `readOnlyHint`](https://modelcontextprotocol.io/specification/2025-06-18/schema#toolannotations-readonlyhint),
  it will match this filter.*/
  read_only?: boolean;
}

/** The top log probability of a token. */
model TopLogProb {
  token: string;
  logprob: float32;
  bytes: int32[];
}

/** The log probability of a token. */
model LogProb {
  token: string;
  logprob: float32;
  bytes: int32[];
  top_logprobs: TopLogProb[];
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model TokenCountsBody {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  `model`?: string | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  input?: string | InputItem[] | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  previous_response_id?: string | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  tools?: Tool[] | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  text?: ResponseTextParam | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  reasoning?: Reasoning | null;

  /** The truncation strategy to use for the model response. - `auto`: If the input to this Response exceeds the model's context window size, the model will truncate the response to fit the context window by dropping items from the beginning of the conversation. - `disabled` (default): If the input size will exceed the context window size for a model, the request will fail with a 400 error. */
  truncation?: TruncationEnum;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  instructions?: string | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  conversation?: ConversationParam | null;

  /** Controls which tool the model should use, if any. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  tool_choice?: ToolChoiceObject | null | ToolChoiceOptions;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  parallel_tool_calls?: boolean | null;
}

/**Configuration options for a text response from the model. Can be plain
text or structured JSON data. Learn more:
- [Text inputs and outputs](https://platform.openai.com/docs/guides/text)
- [Structured Outputs](https://platform.openai.com/docs/guides/structured-outputs)*/
model ResponseTextParam {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  format?: ResponseTextFormatConfiguration;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  verbosity?: Verbosity;
}

/**Constrains the verbosity of the model's response. Lower values will result in
more concise responses, while higher values will result in more verbose responses.
Currently supported values are `low`, `medium`, and `high`.*/
#suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
union Verbosity {
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  "low" | "medium" | "high",
  null,
}

#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
union TruncationEnum {
  string,
  auto: "auto",
  disabled: "disabled",
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
@summary("Token counts")
model TokenCountsResource {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "response.input_tokens" = "response.input_tokens";

  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  input_tokens: integer;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CompactResponseMethodPublicBody {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  `model`: ModelIdsCompaction;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  input?: string | InputItem[] | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  previous_response_id?: string | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  instructions?: string | null;
}

/** Model ID used to generate the response, like `gpt-5` or `o3`. OpenAI offers a wide range of models with different capabilities, performance characteristics, and price points. Refer to the [model guide](https://platform.openai.com/docs/models) to browse and compare available models. */
union ModelIdsCompaction {
  ModelIdsResponses,
  string,
  null,
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
@summary("The compacted response object")
model CompactResource {
  /** The unique identifier for the compacted response. */
  id: string;

  /** The object type. Always `response.compaction`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  object: "response.compaction" = "response.compaction";

  /** The compacted list of output items. This is a list of all user messages, followed by a single compaction item. */
  output: OutputItem[];

  /** Unix timestamp (in seconds) when the compacted conversation was created. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  created_at: utcDateTime;

  /** Token accounting for the compaction pass, including cached, reasoning, and total tokens. */
  usage: ResponseUsage;
}

/** Allows the assistant to create, delete, or update files using unified diffs. */
@summary("Apply patch tool")
model ApplyPatchTool extends Tool {
  /** The type of the tool. Always `apply_patch`. */
  type: ToolType.apply_patch;
}

alias SseResponseOfResponseStreamEvent = {
  @header("Content-Type") contentType: "text/event-stream";
  @body responseBody: ResponseStreamEvent;
};

union DotNetResponseItemCollectionOrder {
  string,
  Ascending: "asc",
  Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model DotNetResponseItemCollectionOptions {
  @path response_id: string;

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
  @query order?: DotNetResponseItemCollectionOrder;

}

@error
model ResponseErrorResponse {
  error: ResponseError;
}

union CodeInterpreterContainerConfigurationType {
  string,
  auto: "auto",
}

@discriminator("type")
model CodeInterpreterContainerConfiguration {
  @doc("The type of the output.")
  type: CodeInterpreterContainerConfigurationType;
}

model DeleteResponseResponse {
  id: string;
  object: "response.deleted";
  deleted: boolean;
}


@access(Access.public)
@usage(Usage.input)
model DotNetGetResponseOptions {
  @path response_id: string;
  @query startingAfter?: int32;
  @query includeObfuscation?: boolean;
  @query includedProperties?: Includable[] | null;
  @query streamingEnabled?: boolean;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union DotNetGlobalToolCallApprovalPolicy {
  string,
  AlwaysRequireApproval: "always",
  NeverRequireApproval: "never",
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetCustomToolCallApprovalPolicy {
  /** A list of tools that always require approval. */
  always?: MCPToolFilter;

  /** A list of tools that never require approval. */
  `never`?: MCPToolFilter;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetToolCallApprovalPolicy {
  global_policy?: DotNetGlobalToolCallApprovalPolicy;
  custom_policy?: DotNetCustomToolCallApprovalPolicy;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetCodeInterpreterToolContainer {
  container_id?: string;
  container?: CodeInterpreterContainerConfiguration;
}
