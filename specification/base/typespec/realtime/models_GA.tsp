/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "../common";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

/**
 * The anchor point from which client secret expiration is calculated.
 */
union RealtimeCreateClientSecretRequestExpiresAfterAnchor_GA {
  string,
  /** Expiration is calculated from the time the client secret is created. */
  created_at: "created_at",
}

/**
 * Configuration for the client secret expiration.
 */
model RealtimeCreateClientSecretRequestExpiresAfter_GA {
  /**
   * The anchor point from which the expiration is calculated.
   */
  anchor?: RealtimeCreateClientSecretRequestExpiresAfterAnchor_GA;

  /**
   * The number of seconds after the anchor point when the client secret expires.
   */
  seconds?: integer;
}

union RealtimeOutputModalities_GA {
  string,
  text: "text",
  audio: "audio"
}

/**
 * Request to create a Realtime client secret with an associated session configuration.
 * The request can specify either a realtime or a transcription session configuration.
 */
@summary("Realtime client secret creation request")
model RealtimeCreateClientSecretRequest_GA {
  @doc("""
    Configuration for the client secret expiration. Expiration refers to the time after which
    a client secret will no longer be valid for creating sessions. The session itself may
    continue after that time once started. A secret can be used to create multiple sessions
    until it expires.
    """)
  @summary("Client secret expiration")
  expires_after?: RealtimeCreateClientSecretRequestExpiresAfter_GA;

  /**
   * Session configuration to use for the client secret. Choose either a realtime
   * session or a transcription session.
   */
  session?: RealtimeSessionCreateRequestGABase_GA;
}

/**
 * The type of session that was created.
 */
union RealtimeSessionCreateRequestGABaseType_GA {
  string,
  /** A real-time conversation session. */
  realtime: "realtime",
  /** A transcription session. */
  transcription: "transcription",
}

/**
 * Base model for a session creation response.
 */
@discriminator("type")
model RealtimeSessionCreateRequestGABase_GA {
  /** The type of session that was created. */
  type: RealtimeSessionCreateRequestGABaseType_GA;
}

/** Realtime session object configuration. */
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
@summary("Realtime session configuration")
model RealtimeSessionCreateRequestGA_GA extends RealtimeSessionCreateRequestGABase_GA {
  /** The type of session to create. Always `realtime` for the Realtime API. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: RealtimeSessionCreateRequestGABaseType_GA.realtime;

  /**The set of modalities the model can respond with. It defaults to `["audio"]`, indicating
  that the model will respond with audio plus a transcript. `["text"]` can be used to make
  the model respond with text only. It is not possible to request both `text` and `audio` at the same time.*/
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  output_modalities?: RealtimeOutputModalities_GA[] = #["audio"];

  /** The Realtime model used for this session. */
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  `model`?:
    | string
    | "gpt-realtime"
    | "gpt-realtime-2025-08-28"
    | "gpt-4o-realtime-preview"
    | "gpt-4o-realtime-preview-2024-10-01"
    | "gpt-4o-realtime-preview-2024-12-17"
    | "gpt-4o-realtime-preview-2025-06-03"
    | "gpt-4o-mini-realtime-preview"
    | "gpt-4o-mini-realtime-preview-2024-12-17"
    | "gpt-realtime-mini"
    | "gpt-realtime-mini-2025-10-06"
    | "gpt-realtime-mini-2025-12-15"
    | "gpt-audio-mini"
    | "gpt-audio-mini-2025-10-06"
    | "gpt-audio-mini-2025-12-15";

  /**The default system instructions (i.e. system message) prepended to model calls. This field allows the client to guide the model on desired responses. The model can be instructed on response content and format, (e.g. "be extremely succinct", "act friendly", "here are examples of good responses") and on audio behavior (e.g. "talk quickly", "inject emotion into your voice", "laugh frequently"). The instructions are not guaranteed to be followed by the model, but they provide guidance to the model on the desired behavior.
  Note that the server sets default instructions which will be used if this field is not set and are visible in the `session.created` event at the start of the session.*/
  instructions?: string;

  /** Configuration for input and output audio. */
  audio?: RealtimeSessionCreateRequestGAAudio_GA;

  /**Additional fields to include in server outputs.
  `item.input_audio_transcription.logprobs`: Include logprobs for input audio transcription.*/
  include?: Includable_GA[];

  /**Realtime API can write session traces to the [Traces Dashboard](/logs?api=traces). Set to null to disable tracing. Once
  tracing is enabled for a session, the configuration cannot be modified.
  `auto` will create a trace for the session with default values for the
  workflow name, group id, and metadata.*/
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  @summary("Tracing Configuration")
  tracing?: "auto" | RealtimeSessionCreateRequestGATracing_GA | null = "auto";

  /** Tools available to the model. */
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  tools?: RealtimeToolBase_GA[];

  /**How the model chooses tools. Provide one of the string modes or force a specific
  function/MCP tool.*/
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  tool_choice?: "none" | "auto" | "required" | ToolChoiceBase_GA = "auto";

  /**Maximum number of output tokens for a single assistant response,
  inclusive of tool calls. Provide an integer between 1 and 4096 to
  limit output tokens, or `inf` for the maximum available tokens for a
  given model. Defaults to `inf`.*/
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  max_output_tokens?: integer;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  truncation?: "auto" | "disabled" | RealtimeTruncationBase_GA;

  // #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  // prompt?: Prompt;
}

/** Realtime transcription session object configuration. */
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
@summary("Realtime transcription session configuration")
model RealtimeTranscriptionSessionCreateRequestGA_GA extends RealtimeSessionCreateRequestGABase_GA {
  /** The type of session to create. Always `transcription` for transcription sessions. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: RealtimeSessionCreateRequestGABaseType_GA.transcription;

  /** Configuration for input and output audio. */
  audio?: RealtimeTranscriptionSessionCreateRequestGAAudio_GA;

  /**Additional fields to include in server outputs.
  `item.input_audio_transcription.logprobs`: Include logprobs for input audio transcription.*/
  include?: Includable_GA[];
}

union Includable_GA {
  string,
  item_input_audio_transcription_logprobs: "item.input_audio_transcription.logprobs"
}

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model RealtimeSessionCreateRequestGAAudio_GA {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  input?: RealtimeSessionCreateRequestGAAudioInput_GA;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  output?: RealtimeSessionCreateRequestGAAudioOutput_GA;
}

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model RealtimeSessionCreateRequestGAAudioInput_GA {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  format?: RealtimeAudioFormats_GA;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  transcription?: AudioTranscription_GA;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  noise_reduction?: RealtimeNoiseReduction_GA;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  turn_detection?: RealtimeTurnDetectionBase_GA | null;
}

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model RealtimeNoiseReduction_GA {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  type?: RealtimeNoiseReductionType_GA;
}

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model RealtimeSessionCreateRequestGAAudioOutput_GA {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  format?: RealtimeAudioFormats_GA;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  voice?: VoiceIdsShared_GA = "alloy";

  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @maxValue(1.5)
  @minValue(0.25)
  speed?: numeric = 1;
}

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model RealtimeTranscriptionSessionCreateRequestGAAudio_GA {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  input?: RealtimeTranscriptionSessionCreateRequestGAAudioInput_GA;
}

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model RealtimeTranscriptionSessionCreateRequestGAAudioInput_GA {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  format?: RealtimeAudioFormats_GA;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  transcription?: AudioTranscription_GA;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  noise_reduction?: RealtimeNoiseReduction_GA;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  turn_detection?: RealtimeTurnDetectionBase_GA | null;
}

/** Type of noise reduction. `near_field` is for close-talking microphones such as headphones, `far_field` is for far-field microphones such as laptop or conference room microphones. */
#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
enum RealtimeNoiseReductionType_GA {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  near_field,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  far_field,
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model AudioTranscription_GA {
  /** The model to use for transcription. Current options are `whisper-1`, `gpt-4o-mini-transcribe`, `gpt-4o-mini-transcribe-2025-12-15`, `gpt-4o-transcribe`, and `gpt-4o-transcribe-diarize`. Use `gpt-4o-transcribe-diarize` when you need diarization with speaker labels. */
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  `model`?:
    | string
    | "whisper-1"
    | "gpt-4o-mini-transcribe"
    | "gpt-4o-mini-transcribe-2025-12-15"
    | "gpt-4o-transcribe"
    | "gpt-4o-transcribe-diarize";

  /**The language of the input audio. Supplying the input language in
  [ISO-639-1](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes) (e.g. `en`) format
  will improve accuracy and latency.*/
  language?: string;

  /**An optional text to guide the model's style or continue a previous audio
  segment.
  For `whisper-1`, the [prompt is a list of keywords](https://platform.openai.com/docs/guides/speech-to-text#prompting).
  For `gpt-4o-transcribe` models (excluding `gpt-4o-transcribe-diarize`), the prompt is a free text string, for example "expect words related to technology".*/
  prompt?: string;
}

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model RealtimeSessionCreateRequestGATracing_GA {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  workflow_name?: string;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  group_id?: string;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/bad-record-type" "Auto-suppressed warnings non-applicable rules during import."
  metadata?: Record<unknown>;
}

union RealtimeToolBaseType_GA {
  string,
  function: "function",
  mcp: "mcp",
}

@discriminator("type")
model RealtimeToolBase_GA {
  type: RealtimeToolBaseType_GA;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
@summary("Function tool")
model RealtimeFunctionTool_GA extends RealtimeToolBase_GA{
  /** The type of the tool, i.e. `function`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: RealtimeToolBaseType_GA.function;

  /** The name of the function. */
  name: string;

  /**The description of the function, including guidance on when and how
  to call it, and guidance about what to tell the user when calling
  (if anything).*/
  description?: string;

  /** Parameters of the function in JSON Schema. */
  parameters?: unknown;
}

/**Give the model access to additional tools via remote Model Context Protocol
(MCP) servers. [Learn more about MCP](https://platform.openai.com/docs/guides/tools-remote-mcp).*/
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
@summary("MCP tool")
model MCPTool_GA extends RealtimeToolBase_GA {
  /** The type of the MCP tool. Always `mcp`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: RealtimeToolBaseType_GA.mcp;

  /** A label for this MCP server, used to identify it in tool calls. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  server_label: string;

  /**The URL for the MCP server. One of `server_url` or `connector_id` must be
  provided.*/
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  server_url?: url;

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
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
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
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  server_description?: string;

  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  headers?: Record<string> | null;
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  allowed_tools?: string[] | MCPToolFilter | null;
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  require_approval?:
    | MCPToolRequireApproval_GA
    | "always"
    | "never"
    | null = "always";
}

/** A filter object to specify which tools are allowed. */
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
@summary("MCP tool filter")
model MCPToolFilter_GA {
  /** List of allowed tool names. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @summary("MCP allowed tools")
  tool_names?: string[];

  /**Indicates whether or not a tool modifies data or is read-only. If an
  MCP server is [annotated with `readOnlyHint`](https://modelcontextprotocol.io/specification/2025-06-18/schema#toolannotations-readonlyhint),
  it will match this filter.*/
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  read_only?: boolean;
}

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model MCPToolRequireApproval_GA {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  always?: MCPToolFilter_GA;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  `never`?: MCPToolFilter_GA;
}

/**Controls which (if any) tool is called by the model.
`none` means the model will not call any tool and instead generates a message.
`auto` means the model can pick between generating a message or calling one or
more tools.
`required` means the model must call one or more tools.*/
#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
@summary("Tool choice mode")
enum ToolChoiceOptions_GA {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  none,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  auto,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  required,
}

union ToolChoiceBaseType_GA {
  string,
  function: "function",
  mcp: "mcp",
}

@discriminator("type")
model ToolChoiceBase_GA {
  type: ToolChoiceBaseType_GA;
}

/** Use this option to force the model to call a specific function. */
@summary("Function tool")
model ToolChoiceFunction_GA extends ToolChoiceBase_GA {
  /** For function calling, the type is always `function`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: ToolChoiceBaseType_GA.function;

  /** The name of the function to call. */
  name: string;
}

/** Use this option to force the model to call a specific tool on a remote MCP server. */
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
@summary("MCP tool")
model ToolChoiceMCP_GA extends ToolChoiceBase_GA {
  /** For MCP tools, the type is always `mcp`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: ToolChoiceBaseType_GA.mcp;

  /** The label of the MCP server to use. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  server_label: string;

  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  name?: string | null;
}

union RealtimeTruncationBaseType_GA {
  string,
  retention_ratio: "retention_ratio",
}

@discriminator("type")
model RealtimeTruncationBase_GA {
  /** The type of session that was created. */
  type: RealtimeTruncationBaseType_GA;
}


/**When the number of tokens in a conversation exceeds the model's input token limit, the conversation be truncated, meaning messages (starting from the oldest) will not be included in the model's context. A 32k context model with 4,096 max output tokens can only include 28,224 tokens in the context before truncation occurs.
Clients can configure truncation behavior to truncate with a lower max token limit, which is an effective way to control token usage and cost.
Truncation will reduce the number of cached tokens on the next turn (busting the cache), since messages are dropped from the beginning of the context. However, clients can also configure truncation to retain messages up to a fraction of the maximum context size, which will reduce the need for future truncations and thus improve the cache rate.
Truncation can be disabled entirely, which means the server will never truncate but would instead return an error if the conversation exceeds the model's input token limit.*/
#suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
@summary("Realtime Truncation Controls")
model RealtimeTruncationRetentionRatio_GA extends RealtimeTruncationBase_GA {
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: RealtimeTruncationBaseType_GA.retention_ratio;

  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @maxValue(1)
  @minValue(0)
  retention_ratio: numeric;

  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  token_limits?: TokenLimits_GA;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model TokenLimits_GA {
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @minValue(0)
  post_instructions?: integer;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
@discriminator("type")
model RealtimeAudioFormats_GA {
  type: RealtimeAudioFormatsType;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model RealtimeAudioFormatsAudioPcm_GA extends RealtimeAudioFormats_GA {
  type: "audio/pcm";
  rate?: 24000;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model RealtimeAudioFormatsAudioPcmu_GA extends RealtimeAudioFormats_GA {
  type: "audio/pcmu";
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model RealtimeAudioFormatsAudioPcma_GA extends RealtimeAudioFormats_GA {
  type: "audio/pcma";
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
union VoiceIdsShared_GA {
  string,
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."

    | "alloy"
    | "ash"
    | "ballad"
    | "coral"
    | "echo"
    | "sage"
    | "shimmer"
    | "verse"
    | "marin"
    | "cedar",
}

model RealtimeCreateClientSecretResponse_GA {
  /** The generated client secret value. */
  value: string;

  /** Expiration timestamp for the client secret, in seconds since epoch. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  expires_at: utcDateTime;

  /** The session configuration for either a realtime or transcription session. */
  @summary("Session configuration")
  session: RealtimeSessionCreateResponseBase_GA;
}

union RealtimeSessionCreateResponseBaseType_GA {
  string,
  realtime: "realtime",
  transcription:"transcription",
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
@discriminator("type")
model RealtimeSessionCreateResponseBase_GA {
  type: RealtimeSessionCreateResponseBaseType_GA;
}

/**A new Realtime session configuration, with an ephemeral key. Default TTL
for keys is one minute.*/
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@extension("x-oaiMeta", #{ name: "The session object", group: "realtime" })
model RealtimeSessionCreateResponseGA_GA extends RealtimeSessionCreateResponseBase_GA {
  /** Ephemeral key returned by the API. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  client_secret: RealtimeSessionCreateResponseGAClientSecret_GA;

  /** The type of session to create. Always `realtime` for the Realtime API. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: RealtimeSessionCreateResponseBaseType_GA.realtime;

  /**The set of modalities the model can respond with. It defaults to `["audio"]`, indicating
  that the model will respond with audio plus a transcript. `["text"]` can be used to make
  the model respond with text only. It is not possible to request both `text` and `audio` at the same time.*/
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  output_modalities?: RealtimeOutputModalities_GA[] = #["audio"];

  /** The Realtime model used for this session. */
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  `model`?:
    | string
    | "gpt-realtime"
    | "gpt-realtime-2025-08-28"
    | "gpt-4o-realtime-preview"
    | "gpt-4o-realtime-preview-2024-10-01"
    | "gpt-4o-realtime-preview-2024-12-17"
    | "gpt-4o-realtime-preview-2025-06-03"
    | "gpt-4o-mini-realtime-preview"
    | "gpt-4o-mini-realtime-preview-2024-12-17"
    | "gpt-realtime-mini"
    | "gpt-realtime-mini-2025-10-06"
    | "gpt-realtime-mini-2025-12-15"
    | "gpt-audio-mini"
    | "gpt-audio-mini-2025-10-06"
    | "gpt-audio-mini-2025-12-15";

  /**The default system instructions (i.e. system message) prepended to model calls. This field allows the client to guide the model on desired responses. The model can be instructed on response content and format, (e.g. "be extremely succinct", "act friendly", "here are examples of good responses") and on audio behavior (e.g. "talk quickly", "inject emotion into your voice", "laugh frequently"). The instructions are not guaranteed to be followed by the model, but they provide guidance to the model on the desired behavior.
  Note that the server sets default instructions which will be used if this field is not set and are visible in the `session.created` event at the start of the session.*/
  instructions?: string;

  /** Configuration for input and output audio. */
  audio?: RealtimeSessionCreateRequestGAAudio_GA; // SWAP: RealtimeSessionCreateResponseGAAudio;

  /**Additional fields to include in server outputs.
  `item.input_audio_transcription.logprobs`: Include logprobs for input audio transcription.*/
  include?: Includable_GA[];

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  tracing?: "auto" | RealtimeSessionCreateRequestGATracing_GA | null; // SWAP: "auto" | RealtimeSessionCreateResponseGATracing | null;

  /** Tools available to the model. */
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  tools?: RealtimeToolBase_GA[];

  /**How the model chooses tools. Provide one of the string modes or force a specific
  function/MCP tool.*/
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  tool_choice?: "none" | "auto" | "required" | ToolChoiceBase_GA; // SWAP: ToolChoiceOptions | ToolChoiceFunction | ToolChoiceMCP = ToolChoiceOptions.auto;

  /**Maximum number of output tokens for a single assistant response,
  inclusive of tool calls. Provide an integer between 1 and 4096 to
  limit output tokens, or `inf` for the maximum available tokens for a
  given model. Defaults to `inf`.*/
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  max_output_tokens?: integer;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  truncation?: "auto" | "disabled" | RealtimeTruncationBase_GA; // SWAP: RealtimeTruncation;
  
  // #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  // prompt?: Prompt;
}

model RealtimeTranscriptionSessionCreateResponseGA_GA extends RealtimeSessionCreateResponseBase_GA {
  /** The type of session. Always `transcription` for transcription sessions. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: RealtimeSessionCreateResponseBaseType_GA.transcription;

  /** Unique identifier for the session that looks like `sess_1234567890abcdef`. */
  id: string;

  /** The object type. Always `realtime.transcription_session`. */
  object: string;

  /** Expiration timestamp for the session, in seconds since epoch. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  expires_at?: utcDateTime;

  /**Additional fields to include in server outputs.
  - `item.input_audio_transcription.logprobs`: Include logprobs for input audio transcription.*/
  include?: Includable_GA[];

  /** Configuration for input audio for the session. */
  audio?: RealtimeTranscriptionSessionCreateRequestGAAudio_GA;// SWAP: RealtimeTranscriptionSessionCreateResponseGAAudio;
}

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model RealtimeSessionCreateResponseGAClientSecret_GA {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  value: string;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @encode(DateTimeKnownEncoding.unixTimestamp, integer)
  expires_at: utcDateTime;
}

union RealtimeTurnDetectionBaseType_GA {
  string,
  server_vad: "server_vad",
  semantic_vad: "semantic_vad",
}

@discriminator("type")
model RealtimeTurnDetectionBase_GA {
  type: RealtimeTurnDetectionBaseType_GA;
}

model RealtimeServerVADTurnDetection_GA extends RealtimeTurnDetectionBase_GA {
  type: RealtimeTurnDetectionBaseType_GA.server_vad;

  @maxValue(1.0)
  @minValue(0.0)
  threshold?: numeric = 0.5;

  // Default is 300 ms.
  @encode(DurationKnownEncoding.milliseconds, integer)
  prefix_padding_ms?: duration;

  // Default is 500 ms.
  @encode(DurationKnownEncoding.milliseconds, integer)
  silence_duration_ms?: duration;

  create_response?: boolean = true;

  interrupt_response?: boolean = true;

  // Max value is 30000, min value is 5000.
  @encode(DurationKnownEncoding.milliseconds, integer)
  idle_timeout_ms?: duration | null;
}

model RealtimeSemanticVADTurnDetection_GA extends RealtimeTurnDetectionBase_GA {
  type: RealtimeTurnDetectionBaseType_GA.semantic_vad;

  eagerness?: RealtimeTimeSemanticVADTurnDetectionEagerness_GA = RealtimeTimeSemanticVADTurnDetectionEagerness_GA.auto;

  create_response?: boolean = true;

  interrupt_response?: boolean = true;
}

union RealtimeTimeSemanticVADTurnDetectionEagerness_GA {
  string,
  low: "low",
  medium: "medium",
  high: "high",
  auto: "auto",
}