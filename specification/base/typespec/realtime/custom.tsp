import "./custom/events.tsp";
import "./custom/items.tsp";
import "./custom/tools.tsp";

using TypeSpec.OpenAPI;

namespace OpenAI;

model RealtimeRequestSession {
  ...RealtimeSessionBase;
  modalities?: RealtimeModality[];
  instructions?: string;
  `model`?:
    | "gpt-4o-realtime-preview"
    | "gpt-4o-realtime-preview-2024-10-01"
    | "gpt-4o-realtime-preview-2024-12-17"
    | "gpt-4o-mini-realtime-preview"
    | "gpt-4o-mini-realtime-preview-2024-12-17";
  voice?: VoiceIdsShared;
  input_audio_format?: RealtimeAudioFormat = RealtimeAudioFormat.pcm16;
  output_audio_format?: RealtimeAudioFormat = RealtimeAudioFormat.pcm16;
  input_audio_transcription?: RealtimeAudioInputTranscriptionSettings | null;
  turn_detection?: RealtimeTurnDetection | null;
  input_audio_noise_reduction?: RealtimeAudioNoiseReduction;
  tools?: RealtimeTool[];
  tool_choice?: RealtimeToolChoice;
  temperature?: float32;
  max_response_output_tokens?: int32 | "inf";
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
  tools: RealtimeTool[];
  tool_choice: RealtimeToolChoice;
  temperature: float32;
  max_response_output_tokens: int32 | "inf" | null;
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
}

model RealtimeAudioInputTranscriptionSettings {
  `model`?: RealtimeAudioInputTranscriptionModel = RealtimeAudioInputTranscriptionModel.whisper_1;
  language?: string;
  prompt?: string;
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
