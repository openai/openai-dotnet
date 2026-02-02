import "./custom/events.tsp";
import "./custom/items.tsp";
import "./custom/tools.tsp";

using TypeSpec.OpenAPI;

namespace OpenAI;

/**
 * Union type for the session type discriminator.
 */
union RealtimeSessionType {
  string,
  /** A real-time conversation session. */
  realtime: "realtime",
  /** A transcription session. */
  transcription: "transcription",
}

/**
 * Base model for session update requests.
 */
@discriminator("type")
model RealtimeRequestSessionBase {
  /** The type of session. */
  type: RealtimeSessionType;
}

/**
 * Realtime session configuration for session.update events.
 */
model RealtimeRequestSession extends RealtimeRequestSessionBase {
  ...RealtimeSessionBase;
  
  /** The type of session. Must be "realtime" for realtime sessions. */
  type: RealtimeSessionType.realtime;
  
  /**
   * The set of modalities the model can respond with. It defaults to ["audio"], indicating
   * that the model will respond with audio plus a transcript. ["text"] can be used to make
   * the model respond with text only. It is not possible to request both "text" and "audio" at the same time.
   */
  output_modalities?: RealtimeModality[];
  
  instructions?: string;
  
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
  
  /** Configuration for input and output audio. */
  audio?: RealtimeSessionAudioConfiguration;
  
  /**
   * Additional fields to include in server outputs.
   * - `item.input_audio_transcription.logprobs`: Include logprobs for input audio transcription.
   */
  include?: ("item.input_audio_transcription.logprobs")[];
  
  /** Configuration options for tracing. Set to null to disable tracing. */
  tracing?: "auto" | RealtimeTracingConfig | null;
  
  tools?: RealtimeTool[];
  tool_choice?: RealtimeToolChoice;
  temperature?: float32;
  // Tool customization: Address spec issue where session.update uses 'max_output_tokens' (not 'max_response_output_tokens')
  max_output_tokens?: int32 | "inf";
}

/**
 * Configuration for input and output audio for a realtime session.
 */
model RealtimeSessionAudioConfiguration {
  /** Configuration for input audio. */
  input?: RealtimeSessionAudioInputConfiguration;
  
  /** Configuration for output audio. */
  output?: RealtimeSessionAudioOutputConfiguration;
}

/**
 * Configuration for input audio in a realtime session.
 */
model RealtimeSessionAudioInputConfiguration {
  /** The format of input audio. GA format with type discriminator and optional rate for PCM. */
  format?: RealtimeAudioFormats;
  
  /** Configuration for input audio transcription. */
  transcription?: RealtimeAudioInputTranscriptionSettings | null;
  
  /** Configuration for input audio noise reduction. */
  noise_reduction?: RealtimeAudioNoiseReduction | null;
  
  /** Configuration for turn detection. */
  turn_detection?: RealtimeTurnDetection | null;
}

/**
 * Configuration for output audio in a realtime session.
 */
model RealtimeSessionAudioOutputConfiguration {
  /** The format of output audio. GA format with type discriminator and optional rate for PCM. */
  format?: RealtimeAudioFormats;
  
  /** The voice the model uses to respond. */
  voice?: VoiceIdsShared;
  
  /** The speed of the model's spoken response. 1.0 is the default. Range: [0.25, 1.5] */
  @minValue(0.25)
  @maxValue(1.5)
  speed?: float32 = 1;
}

/**
 * Transcription session configuration for session.update events.
 */
model RealtimeTranscriptionRequestSession extends RealtimeRequestSessionBase {
  /** The type of session. Must be "transcription" for transcription sessions. */
  type: RealtimeSessionType.transcription;
  
  /** Configuration for input audio. */
  audio?: RealtimeTranscriptionSessionAudioConfiguration;
  
  /**
   * Additional fields to include in server outputs.
   * - `item.input_audio_transcription.logprobs`: Include logprobs for input audio transcription.
   */
  include?: ("item.input_audio_transcription.logprobs")[];
}

/**
 * Configuration for audio in a transcription session.
 */
model RealtimeTranscriptionSessionAudioConfiguration {
  /** Configuration for input audio. */
  input?: RealtimeTranscriptionSessionAudioInputConfiguration;
}

/**
 * Configuration for input audio in a transcription session.
 */
model RealtimeTranscriptionSessionAudioInputConfiguration {
  /** The format of input audio. GA format with type discriminator and optional rate for PCM. */
  format?: RealtimeAudioFormats;
  
  /** Configuration for input audio transcription. */
  transcription?: RealtimeAudioInputTranscriptionSettings | null;
  
  /** Configuration for input audio noise reduction. */
  noise_reduction?: RealtimeAudioNoiseReduction | null;
  
  /** Configuration for turn detection. */
  turn_detection?: RealtimeTurnDetection | null;
}

/**
 * Base model for session responses in session.created and session.updated events.
 * Discriminated by the `type` field.
 */
@discriminator("type")
model RealtimeSessionResponseBase {
  /** The type of session. */
  type: RealtimeSessionType;
}

/**
 * A Realtime session configuration (GA response type).
 * Used in session.created and session.updated events for realtime sessions.
 */
model RealtimeSessionGA extends RealtimeSessionResponseBase {
  /** The type of session. Always `realtime` for the Realtime API. */
  type: RealtimeSessionType.realtime;
  
  /** The object type. Always `realtime.session`. */
  object?: "realtime.session";
  
  /** Unique identifier for the session that looks like `sess_1234567890abcdef`. */
  id?: string;
  
  /** The Realtime model used for this session. */
  `model`?: string;
  
  /**
   * The set of modalities the model can respond with. It defaults to ["audio"], indicating
   * that the model will respond with audio plus a transcript. ["text"] can be used to make
   * the model respond with text only.
   */
  output_modalities?: RealtimeModality[];
  
  /**
   * The default system instructions prepended to model calls.
   */
  instructions?: string;
  
  /** Configuration for input and output audio. */
  audio?: RealtimeSessionAudioConfiguration;
  
  /**
   * Additional fields to include in server outputs.
   * - `item.input_audio_transcription.logprobs`: Include logprobs for input audio transcription.
   */
  include?: ("item.input_audio_transcription.logprobs")[];
  
  /** Configuration options for tracing. Set to null to disable tracing. */
  tracing?: "auto" | RealtimeTracingConfig | null;
  
  /** Tools available to the model. */
  tools?: RealtimeTool[];
  
  /** How the model chooses tools. */
  tool_choice?: RealtimeToolChoice;
  
  /** Sampling temperature for the model. */
  temperature?: float32;
  
  /** Maximum number of output tokens for a single assistant response. */
  max_output_tokens?: int32 | "inf";
  
  /** Timestamp for when the session expires. */
  @encode("unixTimestamp", int32)
  expires_at?: utcDateTime;
}

/**
 * A Realtime transcription session configuration (GA response type).
 * Used in session.created and session.updated events for transcription sessions.
 */
model RealtimeTranscriptionSessionGA extends RealtimeSessionResponseBase {
  /** The type of session. Always `transcription` for transcription sessions. */
  type: RealtimeSessionType.transcription;
  
  /** The object type. Always `realtime.transcription_session`. */
  object?: "realtime.transcription_session";
  
  /** Unique identifier for the session that looks like `sess_1234567890abcdef`. */
  id?: string;
  
  /** Expiration timestamp for the session, in seconds since epoch. */
  @encode("unixTimestamp", int32)
  expires_at?: utcDateTime;
  
  /**
   * Additional fields to include in server outputs.
   * - `item.input_audio_transcription.logprobs`: Include logprobs for input audio transcription.
   */
  include?: ("item.input_audio_transcription.logprobs")[];
  
  /** Configuration for input audio for the session. */
  audio?: RealtimeTranscriptionSessionAudioConfiguration;
}

model RealtimeResponseSession {
  ...RealtimeSessionBase;
  object: "realtime.session";
  id: string;
  `model`: string;
  modalities: RealtimeModality[];
  instructions: string;
  voice: VoiceIdsShared;
  input_audio_format: RealtimeAudioFormat;
  output_audio_format: RealtimeAudioFormat;
  input_audio_transcription: RealtimeAudioInputTranscriptionSettings | null;
  turn_detection: RealtimeTurnDetection;
  input_audio_noise_reduction: RealtimeAudioNoiseReduction;
  /** The speed of the model's spoken response. */
  speed?: float32;
  /** Tracing configuration for the session. */
  tracing?: "auto" | RealtimeTracingConfig | null;
  tools: RealtimeTool[];
  tool_choice: RealtimeToolChoice;
  temperature: float32;
  // Customization: API changed from max_response_output_tokens to max_output_tokens
  max_output_tokens: int32 | "inf" | null;
  /** Timestamp for when the session expires. */
  @encode("unixTimestamp", int32)
  expires_at?: utcDateTime;
}

/**
 * Union type for the audio formats type discriminator.
 */
union RealtimeAudioFormatsType {
  string,
  /** PCM audio format. */
  audio_pcm: "audio/pcm",
  /** G.711 μ-law audio format. */
  audio_pcmu: "audio/pcmu",
  /** G.711 A-law audio format. */
  audio_pcma: "audio/pcma",
}

/**
 * Base model for audio formats with discriminator.
 * This is the GA format for audio configuration.
 */
@discriminator("type")
model RealtimeAudioFormats {
  /** The type of audio format. */
  type: RealtimeAudioFormatsType;
}

/**
 * PCM audio format configuration.
 */
model RealtimeAudioFormatsPcm extends RealtimeAudioFormats {
  /** The type of audio format. Must be "audio/pcm". */
  type: RealtimeAudioFormatsType.audio_pcm;
  /** The sample rate for PCM audio. Defaults to 24000. */
  rate?: 24000;
}

/**
 * G.711 μ-law audio format configuration.
 */
model RealtimeAudioFormatsPcmu extends RealtimeAudioFormats {
  /** The type of audio format. Must be "audio/pcmu". */
  type: RealtimeAudioFormatsType.audio_pcmu;
}

/**
 * G.711 A-law audio format configuration.
 */
model RealtimeAudioFormatsPcma extends RealtimeAudioFormats {
  /** The type of audio format. Must be "audio/pcma". */
  type: RealtimeAudioFormatsType.audio_pcma;
}

union RealtimeAudioFormat {
  string,
  pcm16: "pcm16",
  g711_ulaw: "g711_ulaw",
  g711_alaw: "g711_alaw",
}

union RealtimeAudioInputTranscriptionModel {
  string,
  whisper_1: "whisper-1",
  gpt_4o_transcribe: "gpt-4o-transcribe",
  gpt_4o_mini_transcribe: "gpt-4o-mini-transcribe",
  gpt_4o_mini_transcribe_2025_12_15: "gpt-4o-mini-transcribe-2025-12-15",
  gpt_4o_transcribe_diarize: "gpt-4o-transcribe-diarize",
}

model RealtimeAudioInputTranscriptionSettings {
  `model`?: RealtimeAudioInputTranscriptionModel = RealtimeAudioInputTranscriptionModel.whisper_1;
  language?: string;
  prompt?: string;
}

/** Configuration options for tracing. */
model RealtimeTracingConfig {
  /** The name of the workflow to attach to this trace. */
  workflow_name?: string;
  /** The group id to attach to this trace for filtering and grouping. */
  group_id?: string;
  /** Arbitrary metadata to attach to this trace for filtering. */
  metadata?: unknown;
}

union RealtimeModality {
  string,
  text: "text",
  audio: "audio",
}

union RealtimeTurnDetectionType {
  string,

  /**
   * Indicates that server-side voice activity detection (VAD) should be enabled, allowing the server to determine when
   * add_user_audio commands present ends of speech and should be automatically committed.
   *
   * The API will also detect when the user begins talking, sending a generation_canceled command.
   */
  server_vad: "server_vad",

  semantic_vad: "semantic_vad",
}

@discriminator("type")
model RealtimeTurnDetection {
  type: RealtimeTurnDetectionType;

  /**
   * Whether or not to automatically generate a response when VAD is enabled. true by default.
   */
  create_response?: boolean = true;

  /**
   * Whether or not to automatically interrupt any ongoing response with output to the default conversation (i.e. `conversation` of `auto`) when a VAD start event occurs.
   */
  interrupt_response?: boolean = true;
}

model RealtimeServerVadTurnDetection extends RealtimeTurnDetection {
  type: RealtimeTurnDetectionType.server_vad;

  /**
   * Activation threshold for VAD (0.0 to 1.0), this defaults to 0.5. A higher threshold will require louder audio to activate the model, and thus might perform better in noisy environments.
   */
  threshold?: float32 = 0.5;

  // @encode("milliseconds", int32)
  /**
   * Amount of audio to include before the VAD detected speech (in milliseconds). Defaults to 300ms.
   */
  prefix_padding_ms?: duration; // = 300ms

  // @encode("milliseconds", int32)
  /**
   * Duration of silence to detect speech stop (in milliseconds). Defaults to 500ms. With shorter values the model will respond more quickly, but may jump in on short pauses from the user.
   */
  silence_duration_ms?: duration; // = 500ms

  // @encode("milliseconds", int32)
  /**
   * Timeout in milliseconds for idle detection. If no audio is received for this duration,
   * the server will emit an `input_audio_buffer.timeout_triggered` event.
   */
  idle_timeout_ms?: duration | null;
}

model RealtimeSemanticVadTurnDetection extends RealtimeTurnDetection {
  type: RealtimeTurnDetectionType.semantic_vad;

  /**
   * Used only for `semantic_vad` mode. The eagerness of the model to respond. `low` will wait longer for the user to continue speaking, `high` will respond more quickly. `auto` is the default and is equivalent to `medium`.
   */
  eagerness?: "low" | "medium" | "high" | "auto" = "auto";
}

model RealtimeServerEventRateLimitsUpdatedRateLimitsItem {
  /** The rate limit property name that this item includes information about. */
  name: string;

  /** The maximum configured limit for this rate limit property. */
  limit: int32;

  /** The remaining quota available against the configured limit for this rate limit property. */
  remaining: int32;

  /** The remaining time, in seconds, until this rate limit property is reset. */
  @encode("seconds", float32)
  reset_seconds: duration;
}

union RealtimeAudioNoiseReductionType {
  near_field: "near_field",
  far_field: "far_field",
}

@discriminator("type")
model RealtimeAudioNoiseReduction {
  type: RealtimeAudioNoiseReductionType;
}

model RealtimeAudioNearFieldNoiseReduction extends RealtimeAudioNoiseReduction {
  type: RealtimeAudioNoiseReductionType.near_field;
}

model RealtimeAudioFarFieldNoiseReduction extends RealtimeAudioNoiseReduction {
  type: RealtimeAudioNoiseReductionType.far_field;
}

// MCP Error Types
union RealtimeMCPErrorType {
  string,
  protocol_error: "protocol_error",
  tool_execution_error: "tool_execution_error",
  http_error: "http_error",
}

/** Base model for MCP errors. */
@discriminator("type")
model RealtimeMCPError {
  type: RealtimeMCPErrorType;
}

/** Realtime MCP protocol error */
model RealtimeMCPProtocolError extends RealtimeMCPError {
  type: RealtimeMCPErrorType.protocol_error;
  code: int32;
  message: string;
}

/** Realtime MCP tool execution error */
model RealtimeMCPToolExecutionError extends RealtimeMCPError {
  type: RealtimeMCPErrorType.tool_execution_error;
  message: string;
}

/** Realtime MCP HTTP error */
model RealtimeMCPHTTPError extends RealtimeMCPError {
  type: RealtimeMCPErrorType.http_error;
  code: int32;
  message: string;
}

// /** A tool listed from an MCP server. */
// model MCPListToolsTool {
//   /** The name of the tool. */
//   name: string;

//   /** A description of the tool. */
//   description?: string;

//   /** The input schema for the tool. */
//   input_schema?: unknown;
// }

// /** MCP tool definition for realtime sessions. */
// model MCPTool {
//   /** The type of tool, must be `mcp`. */
//   type: "mcp";

//   /** The label for the MCP server. */
//   server_label: string;

//   /** The URL of the MCP server. */
//   server_url: string;

//   /** Whether the tool requires approval before execution. */
//   require_approval?: "never" | "always" = "never";

//   /** List of allowed tools from this server. */
//   allowed_tools?: string[];

//   /** HTTP headers to include in requests to the MCP server. */
//   headers?: Record<string>;
// }
